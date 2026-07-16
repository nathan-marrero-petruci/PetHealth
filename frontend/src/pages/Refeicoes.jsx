import { useEffect, useState } from "react";
import { api } from "../api/client";
import { formatarDataHoraLocal, localInputParaUtcIso, utcIsoParaLocalInput } from "../utils/dataHora";

const FORM_INICIAL = {
  dataHora: "",
  quantidadeGramas: "",
};

export function Refeicoes() {
  const [refeicoes, setRefeicoes] = useState(null);
  const [error, setError] = useState("");
  const [form, setForm] = useState(FORM_INICIAL);
  const [editingId, setEditingId] = useState(null);
  const [formError, setFormError] = useState("");

  async function load() {
    try {
      const response = await api.get("/api/refeicoes");
      setRefeicoes(response.data);
    } catch {
      setError("Não foi possível carregar as refeições.");
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
      quantidadeGramas: Number(form.quantidadeGramas),
    };

    try {
      if (editingId) {
        await api.put(`/api/refeicoes/${editingId}`, payload);
      } else {
        await api.post("/api/refeicoes", payload);
      }

      limparFormulario();
      await load();
    } catch (submitError) {
      setFormError(submitError.response?.data?.message ?? "Não foi possível salvar a refeição.");
    }
  }

  function handleEditar(refeicao) {
    setEditingId(refeicao.id);
    setForm({
      dataHora: utcIsoParaLocalInput(refeicao.dataHora),
      quantidadeGramas: refeicao.quantidadeGramas,
    });
    setFormError("");
  }

  async function handleRemover(id) {
    if (!window.confirm("Remover esta refeição?")) {
      return;
    }

    await api.delete(`/api/refeicoes/${id}`);
    await load();
  }

  if (error) {
    return <p>{error}</p>;
  }

  if (refeicoes === null) {
    return <p>Carregando...</p>;
  }

  return (
    <div className="dashboard-page">
      <h1>Refeições diárias</h1>

      <section className="dashboard-card">
        <h2>{editingId ? "Editar refeição" : "Registrar refeição"}</h2>
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
            Quantidade servida (g)
            <input
              type="number"
              step="0.01"
              min="0.01"
              max="9999.99"
              value={form.quantidadeGramas}
              onChange={(event) => handleChange("quantidadeGramas", event.target.value)}
              required
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
        <h2>Refeições registradas</h2>
        {refeicoes.length === 0 ? (
          <p>Nenhuma refeição registrada ainda.</p>
        ) : (
          <ul className="vacinas-list">
            {refeicoes.map((refeicao) => (
              <li key={refeicao.id} className="vacinas-item">
                <div>
                  <strong>{formatarDataHoraLocal(refeicao.dataHora)}</strong> —{" "}
                  {refeicao.quantidadeGramas} g
                  {refeicao.diferencaGramas === null
                    ? " — sem dieta padrão cadastrada para comparar"
                    : ` — diferença de ${refeicao.diferencaGramas} g em relação à dieta padrão`}
                </div>
                <div className="vacinas-item-acoes">
                  <button type="button" onClick={() => handleEditar(refeicao)}>
                    Editar
                  </button>
                  <button type="button" onClick={() => handleRemover(refeicao.id)}>
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
