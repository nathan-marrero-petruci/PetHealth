import { useEffect, useState } from "react";
import { api } from "../api/client";

const FORM_INICIAL = {
  data: "",
  descricao: "",
};

export function Observacoes() {
  const [observacoes, setObservacoes] = useState(null);
  const [error, setError] = useState("");
  const [form, setForm] = useState(FORM_INICIAL);
  const [editingId, setEditingId] = useState(null);
  const [formError, setFormError] = useState("");

  async function load() {
    try {
      const response = await api.get("/api/observacoes");
      setObservacoes(response.data);
    } catch {
      setError("Não foi possível carregar as observações.");
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
      data: form.data,
      descricao: form.descricao,
    };

    try {
      if (editingId) {
        await api.put(`/api/observacoes/${editingId}`, payload);
      } else {
        await api.post("/api/observacoes", payload);
      }

      limparFormulario();
      await load();
    } catch (submitError) {
      setFormError(submitError.response?.data?.message ?? "Não foi possível salvar a observação.");
    }
  }

  function handleEditar(observacao) {
    setEditingId(observacao.id);
    setForm({
      data: observacao.data,
      descricao: observacao.descricao,
    });
    setFormError("");
  }

  async function handleRemover(id) {
    if (!window.confirm("Remover esta observação?")) {
      return;
    }

    await api.delete(`/api/observacoes/${id}`);
    await load();
  }

  if (error) {
    return <p>{error}</p>;
  }

  if (observacoes === null) {
    return <p>Carregando...</p>;
  }

  return (
    <div className="dashboard-page">
      <h1>Observações e sintomas</h1>

      <section className="dashboard-card">
        <h2>{editingId ? "Editar observação" : "Cadastrar observação"}</h2>
        <form onSubmit={handleSubmit} className="form">
          <label>
            Data
            <input
              type="date"
              value={form.data}
              onChange={(event) => handleChange("data", event.target.value)}
              required
            />
          </label>
          <label>
            Descrição
            <textarea
              value={form.descricao}
              onChange={(event) => handleChange("descricao", event.target.value)}
              required
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
        <h2>Observações registradas</h2>
        {observacoes.length === 0 ? (
          <p>Nenhuma observação registrada ainda.</p>
        ) : (
          <ul className="vacinas-list">
            {observacoes.map((observacao) => (
              <li key={observacao.id} className="vacinas-item">
                <div>
                  <strong>{observacao.data}</strong> — {observacao.descricao}
                </div>
                <div className="vacinas-item-acoes">
                  <button type="button" onClick={() => handleEditar(observacao)}>
                    Editar
                  </button>
                  <button type="button" onClick={() => handleRemover(observacao.id)}>
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
