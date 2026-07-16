import { useEffect, useState } from "react";
import { api } from "../api/client";
import { formatarDataHoraLocal, localInputParaUtcIso, utcIsoParaLocalInput } from "../utils/dataHora";

const FORM_INICIAL = {
  dataHora: "",
  descricao: "",
  quantidadeObservacao: "",
};

export function Petiscos() {
  const [itens, setItens] = useState(null);
  const [error, setError] = useState("");
  const [form, setForm] = useState(FORM_INICIAL);
  const [editingId, setEditingId] = useState(null);
  const [formError, setFormError] = useState("");

  async function load() {
    try {
      const response = await api.get("/api/itens-fora-dieta");
      setItens(response.data);
    } catch {
      setError("Não foi possível carregar os petiscos.");
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
      dataHora: localInputParaUtcIso(form.dataHora),
      descricao: form.descricao,
      quantidadeObservacao: form.quantidadeObservacao || null,
    };

    try {
      if (editingId) {
        await api.put(`/api/itens-fora-dieta/${editingId}`, payload);
      } else {
        await api.post("/api/itens-fora-dieta", payload);
      }

      limparFormulario();
      await load();
    } catch (submitError) {
      setFormError(submitError.response?.data?.message ?? "Não foi possível salvar o petisco.");
    }
  }

  function handleEditar(item) {
    setEditingId(item.id);
    setForm({
      dataHora: utcIsoParaLocalInput(item.dataHora),
      descricao: item.descricao,
      quantidadeObservacao: item.quantidadeObservacao ?? "",
    });
    setFormError("");
  }

  async function handleRemover(id) {
    if (!window.confirm("Remover este petisco?")) {
      return;
    }

    await api.delete(`/api/itens-fora-dieta/${id}`);
    await load();
  }

  if (error) {
    return <p>{error}</p>;
  }

  if (itens === null) {
    return <p>Carregando...</p>;
  }

  return (
    <div className="dashboard-page">
      <h1>Petiscos / itens fora da dieta</h1>

      <section className="dashboard-card">
        <h2>{editingId ? "Editar petisco" : "Registrar petisco"}</h2>
        <form onSubmit={handleSubmit} className="form">
          <label>
            Data/hora
            <input
              type="datetime-local"
              value={form.dataHora}
              onChange={(event) => handleChange("dataHora", event.target.value)}
              required
            />
          </label>
          <label>
            Descrição
            <input
              type="text"
              value={form.descricao}
              onChange={(event) => handleChange("descricao", event.target.value)}
              required
            />
          </label>
          <label>
            Quantidade/observação
            <input
              type="text"
              value={form.quantidadeObservacao}
              onChange={(event) => handleChange("quantidadeObservacao", event.target.value)}
            />
          </label>
          {formError && <p className="login-error">{formError}</p>}
          <button type="submit">{editingId ? "Salvar" : "Registrar"}</button>
          {editingId && (
            <button type="button" onClick={limparFormulario}>
              Cancelar edição
            </button>
          )}
        </form>
      </section>

      <section className="dashboard-card">
        <h2>Petiscos registrados</h2>
        {itens.length === 0 ? (
          <p className="text-meta">Nenhum petisco registrado ainda.</p>
        ) : (
          <ul className="vacinas-list">
            {itens.map((item) => (
              <li key={item.id} className="vacinas-item">
                <div>
                  <strong>{formatarDataHoraLocal(item.dataHora)}</strong> — {item.descricao}
                  {item.quantidadeObservacao && ` — ${item.quantidadeObservacao}`}
                </div>
                <div className="vacinas-item-acoes">
                  <button type="button" onClick={() => handleEditar(item)}>
                    Editar
                  </button>
                  <button type="button" onClick={() => handleRemover(item.id)}>
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
