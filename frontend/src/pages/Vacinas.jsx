import { useEffect, useState } from "react";
import { api } from "../api/client";

const VACINA_STATUS_LABEL = {
  SemProximaDose: "Sem próxima dose",
  EmDia: "Em dia",
  Proxima: "Próxima",
  Vencida: "Vencida",
};

const FORM_INICIAL = {
  nome: "",
  dataAplicacao: "",
  dataProximaDose: "",
  antecedenciaLembreteDias: "",
};

function classeDestaque(status) {
  if (status === "Vencida") return "vacinas-item vacinas-item-vencida";
  if (status === "Proxima") return "vacinas-item vacinas-item-proxima";
  return "vacinas-item";
}

export function Vacinas() {
  const [vacinas, setVacinas] = useState(null);
  const [error, setError] = useState("");
  const [form, setForm] = useState(FORM_INICIAL);
  const [editingId, setEditingId] = useState(null);
  const [formError, setFormError] = useState("");

  async function load() {
    try {
      const response = await api.get("/api/vacinas");
      setVacinas(response.data);
    } catch {
      setError("Não foi possível carregar as vacinas.");
    }
  }

  useEffect(() => {
    load();
  }, []);

  function handleChange(campo, valor) {
    setForm((atual) => ({
      ...atual,
      [campo]: valor,
      ...(campo === "dataProximaDose" && !valor ? { antecedenciaLembreteDias: "" } : {}),
    }));
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
      dataAplicacao: form.dataAplicacao,
      dataProximaDose: form.dataProximaDose || null,
      antecedenciaLembreteDias: form.dataProximaDose
        ? Number(form.antecedenciaLembreteDias)
        : null,
    };

    try {
      if (editingId) {
        await api.put(`/api/vacinas/${editingId}`, payload);
      } else {
        await api.post("/api/vacinas", payload);
      }

      limparFormulario();
      await load();
    } catch (submitError) {
      setFormError(submitError.response?.data?.message ?? "Não foi possível salvar a vacina.");
    }
  }

  function handleEditar(vacina) {
    setEditingId(vacina.id);
    setForm({
      nome: vacina.nome,
      dataAplicacao: vacina.dataAplicacao,
      dataProximaDose: vacina.dataProximaDose ?? "",
      antecedenciaLembreteDias: vacina.antecedenciaLembreteDias ?? "",
    });
    setFormError("");
  }

  async function handleRemover(id) {
    if (!window.confirm("Remover esta vacina?")) {
      return;
    }

    await api.delete(`/api/vacinas/${id}`);
    await load();
  }

  if (error) {
    return <p>{error}</p>;
  }

  if (vacinas === null) {
    return <p>Carregando...</p>;
  }

  return (
    <div className="dashboard-page">
      <h1>Vacinas</h1>

      <section className="dashboard-card">
        <h2>{editingId ? "Editar vacina" : "Cadastrar vacina"}</h2>
        <form onSubmit={handleSubmit} className="form">
          <label>
            Nome/tipo da vacina
            <input
              type="text"
              value={form.nome}
              onChange={(event) => handleChange("nome", event.target.value)}
              required
            />
          </label>
          <label>
            Data de aplicação
            <input
              type="date"
              value={form.dataAplicacao}
              onChange={(event) => handleChange("dataAplicacao", event.target.value)}
              required
            />
          </label>
          <label>
            Data prevista da próxima dose (opcional)
            <input
              type="date"
              value={form.dataProximaDose}
              onChange={(event) => handleChange("dataProximaDose", event.target.value)}
            />
          </label>
          {form.dataProximaDose && (
            <label>
              Antecedência do lembrete (dias)
              <input
                type="number"
                value={form.antecedenciaLembreteDias}
                onChange={(event) => handleChange("antecedenciaLembreteDias", event.target.value)}
                required
              />
            </label>
          )}
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
        <h2>Vacinas cadastradas</h2>
        {vacinas.length === 0 ? (
          <p>Nenhuma vacina cadastrada ainda.</p>
        ) : (
          <ul className="vacinas-list">
            {vacinas.map((vacina) => (
              <li key={vacina.id} className={classeDestaque(vacina.status)}>
                <div>
                  <strong>{vacina.nome}</strong> — aplicada em {vacina.dataAplicacao}
                  {vacina.dataProximaDose ? ` (próxima dose: ${vacina.dataProximaDose})` : ""}
                  {" — "}
                  {VACINA_STATUS_LABEL[vacina.status]}
                </div>
                <div className="vacinas-item-acoes">
                  <button type="button" onClick={() => handleEditar(vacina)}>
                    Editar
                  </button>
                  <button type="button" onClick={() => handleRemover(vacina.id)}>
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
