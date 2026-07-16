import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { api } from "../api/client";

const FORM_INICIAL = {
  nome: "",
  descricao: "",
};

export function ComandosTreino() {
  const [comandos, setComandos] = useState(null);
  const [error, setError] = useState("");
  const [form, setForm] = useState(FORM_INICIAL);
  const [editingId, setEditingId] = useState(null);
  const [formError, setFormError] = useState("");

  async function load() {
    try {
      const response = await api.get("/api/comandos-treino");
      setComandos(response.data);
    } catch {
      setError("Não foi possível carregar os comandos de treino.");
    }
  }

  useEffect(() => {
    load();
  }, []);

  function handleChange(campo, valor) {
    setForm((atual) => ({ ...atual, [campo]: valor }));
  }

  function limparFormulario() {
    setForm(FORM_INICIAL);
    setEditingId(null);
    setFormError("");
  }

  async function handleSubmit(event) {
    event.preventDefault();
    setFormError("");

    const payload = {
      nome: form.nome,
      descricao: form.descricao || null,
    };

    try {
      if (editingId) {
        await api.put(`/api/comandos-treino/${editingId}`, payload);
      } else {
        await api.post("/api/comandos-treino", payload);
      }

      limparFormulario();
      await load();
    } catch (submitError) {
      setFormError(
        submitError.response?.data?.message ?? "Não foi possível salvar o comando de treino.",
      );
    }
  }

  function handleEditar(comando) {
    setEditingId(comando.id);
    setForm({
      nome: comando.nome,
      descricao: comando.descricao ?? "",
    });
    setFormError("");
  }

  async function handleRemover(id) {
    if (!window.confirm("Remover este comando de treino?")) {
      return;
    }

    await api.delete(`/api/comandos-treino/${id}`);
    await load();
  }

  if (error) {
    return <p>{error}</p>;
  }

  if (comandos === null) {
    return <p className="text-meta">Carregando...</p>;
  }

  const comandosAtivos = comandos.filter((comando) => comando.ativo);

  return (
    <div className="dashboard-page">
      <h1>Comandos de treino</h1>

      <section className="dashboard-card">
        <h2>{editingId ? "Editar comando" : "Cadastrar comando"}</h2>
        <form onSubmit={handleSubmit} className="form">
          <label>
            Nome
            <input
              type="text"
              value={form.nome}
              onChange={(event) => handleChange("nome", event.target.value)}
              required
            />
          </label>
          <label>
            Descrição (opcional)
            <textarea
              value={form.descricao}
              onChange={(event) => handleChange("descricao", event.target.value)}
            />
          </label>
          {formError && <p className="login-error">{formError}</p>}
          <button type="submit">{editingId ? "Salvar" : "Cadastrar"}</button>
          {editingId && (
            <button type="button" onClick={limparFormulario}>
              Cancelar edição
            </button>
          )}
        </form>
      </section>

      <section className="dashboard-card">
        <h2>Comandos cadastrados</h2>
        {comandosAtivos.length === 0 ? (
          <p className="text-meta">Nenhum comando de treino cadastrado ainda.</p>
        ) : (
          <ul className="vacinas-list">
            {comandosAtivos.map((comando) => (
              <li key={comando.id} className="vacinas-item">
                <div>
                  <strong>{comando.nome}</strong>
                  {comando.descricao ? ` — ${comando.descricao}` : ""}
                </div>
                <div className="vacinas-item-acoes">
                  <Link
                    to={`/comandos-treino/${comando.id}/evolucao`}
                    state={{ nome: comando.nome }}
                  >
                    Ver evolução
                  </Link>
                  <button type="button" onClick={() => handleEditar(comando)}>
                    Editar
                  </button>
                  <button type="button" onClick={() => handleRemover(comando.id)}>
                    Remover
                  </button>
                </div>
              </li>
            ))}
          </ul>
        )}
      </section>
    </div>
  );
}
