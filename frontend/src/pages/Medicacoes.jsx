import { useEffect, useState } from "react";
import { api } from "../api/client";

const FORM_INICIAL = {
  nome: "",
  dosagemValor: "",
  dosagemUnidade: "",
  vezesPorDia: "",
  intervaloHoras: "",
  dataInicio: "",
  dataTermino: "",
};

export function Medicacoes() {
  const [medicacoes, setMedicacoes] = useState(null);
  const [error, setError] = useState("");
  const [form, setForm] = useState(FORM_INICIAL);
  const [editingId, setEditingId] = useState(null);
  const [formError, setFormError] = useState("");

  async function load() {
    try {
      const response = await api.get("/api/medicacoes");
      setMedicacoes(response.data);
    } catch {
      setError("Não foi possível carregar as medicações.");
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
      dosagemValor: Number(form.dosagemValor),
      dosagemUnidade: form.dosagemUnidade,
      vezesPorDia: Number(form.vezesPorDia),
      intervaloHoras: Number(form.intervaloHoras),
      dataInicio: form.dataInicio,
      dataTermino: form.dataTermino || null,
    };

    try {
      if (editingId) {
        await api.put(`/api/medicacoes/${editingId}`, payload);
      } else {
        await api.post("/api/medicacoes", payload);
      }

      limparFormulario();
      await load();
    } catch (submitError) {
      setFormError(submitError.response?.data?.message ?? "Não foi possível salvar a medicação.");
    }
  }

  function handleEditar(medicacao) {
    setEditingId(medicacao.id);
    setForm({
      nome: medicacao.nome,
      dosagemValor: medicacao.dosagemValor,
      dosagemUnidade: medicacao.dosagemUnidade,
      vezesPorDia: medicacao.vezesPorDia,
      intervaloHoras: medicacao.intervaloHoras,
      dataInicio: medicacao.dataInicio,
      dataTermino: medicacao.dataTermino ?? "",
    });
    setFormError("");
  }

  async function handleRemover(id) {
    if (!window.confirm("Remover esta medicação?")) {
      return;
    }

    await api.delete(`/api/medicacoes/${id}`);
    await load();
  }

  if (error) {
    return <p>{error}</p>;
  }

  if (medicacoes === null) {
    return <p>Carregando...</p>;
  }

  const emUso = medicacoes.filter((medicacao) => medicacao.status === "EmUso");
  const encerradas = medicacoes.filter((medicacao) => medicacao.status === "Encerrada");

  return (
    <div className="dashboard-page">
      <h1>Medicações</h1>

      <section className="dashboard-card">
        <h2>{editingId ? "Editar medicação" : "Cadastrar medicação"}</h2>
        <form onSubmit={handleSubmit} className="login-form">
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
            Dosagem (valor)
            <input
              type="number"
              step="0.01"
              min="0.01"
              max="999.99"
              value={form.dosagemValor}
              onChange={(event) => handleChange("dosagemValor", event.target.value)}
              required
            />
          </label>
          <label>
            Dosagem (unidade)
            <input
              type="text"
              placeholder="ex: mg, ml, comprimido"
              value={form.dosagemUnidade}
              onChange={(event) => handleChange("dosagemUnidade", event.target.value)}
              required
            />
          </label>
          <label>
            Vezes por dia
            <input
              type="number"
              min="1"
              value={form.vezesPorDia}
              onChange={(event) => handleChange("vezesPorDia", event.target.value)}
              required
            />
          </label>
          <label>
            Intervalo entre doses (horas)
            <input
              type="number"
              min="1"
              value={form.intervaloHoras}
              onChange={(event) => handleChange("intervaloHoras", event.target.value)}
              required
            />
          </label>
          <label>
            Data de início
            <input
              type="date"
              value={form.dataInicio}
              onChange={(event) => handleChange("dataInicio", event.target.value)}
              required
            />
          </label>
          <label>
            Data de término (opcional)
            <input
              type="date"
              value={form.dataTermino}
              onChange={(event) => handleChange("dataTermino", event.target.value)}
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
        <h2>Em uso</h2>
        {emUso.length === 0 ? (
          <p>Nenhuma medicação em uso.</p>
        ) : (
          <ul className="vacinas-list">
            {emUso.map((medicacao) => (
              <li key={medicacao.id} className="vacinas-item">
                <div>
                  <strong>{medicacao.nome}</strong> — {medicacao.dosagemValor}{" "}
                  {medicacao.dosagemUnidade}, {medicacao.vezesPorDia}x ao dia (a cada{" "}
                  {medicacao.intervaloHoras}h) — desde {medicacao.dataInicio}
                </div>
                <div className="vacinas-item-acoes">
                  <button type="button" onClick={() => handleEditar(medicacao)}>
                    Editar
                  </button>
                  <button type="button" onClick={() => handleRemover(medicacao.id)}>
                    Remover
                  </button>
                </div>
              </li>
            ))}
          </ul>
        )}
      </section>

      <section className="dashboard-card">
        <h2>Encerradas</h2>
        {encerradas.length === 0 ? (
          <p>Nenhuma medicação encerrada.</p>
        ) : (
          <ul className="vacinas-list">
            {encerradas.map((medicacao) => (
              <li key={medicacao.id} className="vacinas-item vacinas-item-encerrada">
                <div>
                  <strong>{medicacao.nome}</strong> — {medicacao.dosagemValor}{" "}
                  {medicacao.dosagemUnidade}, {medicacao.vezesPorDia}x ao dia (a cada{" "}
                  {medicacao.intervaloHoras}h) — de {medicacao.dataInicio} até{" "}
                  {medicacao.dataTermino} (Encerrada)
                </div>
                <div className="vacinas-item-acoes">
                  <button type="button" onClick={() => handleEditar(medicacao)}>
                    Editar
                  </button>
                  <button type="button" onClick={() => handleRemover(medicacao.id)}>
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
