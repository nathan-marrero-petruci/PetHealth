import { useEffect, useState } from "react";
import { api } from "../api/client";

const FORM_INICIAL = {
  data: "",
  motivo: "",
  veterinarioClinica: "",
  observacoes: "",
};

export function Consultas() {
  const [consultas, setConsultas] = useState(null);
  const [error, setError] = useState("");
  const [form, setForm] = useState(FORM_INICIAL);
  const [editingId, setEditingId] = useState(null);
  const [formError, setFormError] = useState("");

  async function load() {
    try {
      const response = await api.get("/api/consultas");
      setConsultas(response.data);
    } catch {
      setError("Não foi possível carregar as consultas.");
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
      motivo: form.motivo,
      veterinarioClinica: form.veterinarioClinica || null,
      observacoes: form.observacoes || null,
    };

    try {
      if (editingId) {
        await api.put(`/api/consultas/${editingId}`, payload);
      } else {
        await api.post("/api/consultas", payload);
      }

      limparFormulario();
      await load();
    } catch (submitError) {
      setFormError(submitError.response?.data?.message ?? "Não foi possível salvar a consulta.");
    }
  }

  function handleEditar(consulta) {
    setEditingId(consulta.id);
    setForm({
      data: consulta.data,
      motivo: consulta.motivo,
      veterinarioClinica: consulta.veterinarioClinica ?? "",
      observacoes: consulta.observacoes ?? "",
    });
    setFormError("");
  }

  async function handleRemover(id) {
    if (!window.confirm("Remover esta consulta?")) {
      return;
    }

    await api.delete(`/api/consultas/${id}`);
    await load();
  }

  if (error) {
    return <p>{error}</p>;
  }

  if (consultas === null) {
    return <p>Carregando...</p>;
  }

  return (
    <div className="dashboard-page">
      <h1>Consultas veterinárias</h1>

      <section className="dashboard-card">
        <h2>{editingId ? "Editar consulta" : "Cadastrar consulta"}</h2>
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
            Motivo/descrição
            <input
              type="text"
              value={form.motivo}
              onChange={(event) => handleChange("motivo", event.target.value)}
              required
            />
          </label>
          <label>
            Veterinário/clínica (opcional)
            <input
              type="text"
              value={form.veterinarioClinica}
              onChange={(event) => handleChange("veterinarioClinica", event.target.value)}
            />
          </label>
          <label>
            Observações/diagnóstico (opcional)
            <textarea
              value={form.observacoes}
              onChange={(event) => handleChange("observacoes", event.target.value)}
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
        <h2>Consultas registradas</h2>
        {consultas.length === 0 ? (
          <p>Nenhuma consulta registrada ainda.</p>
        ) : (
          <ul className="vacinas-list">
            {consultas.map((consulta) => (
              <li key={consulta.id} className="vacinas-item">
                <div>
                  <strong>{consulta.data}</strong> — {consulta.motivo}
                  {consulta.veterinarioClinica ? ` (${consulta.veterinarioClinica})` : ""}
                  {consulta.observacoes ? ` — ${consulta.observacoes}` : ""}
                </div>
                <div className="vacinas-item-acoes">
                  <button type="button" onClick={() => handleEditar(consulta)}>
                    Editar
                  </button>
                  <button type="button" onClick={() => handleRemover(consulta.id)}>
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
