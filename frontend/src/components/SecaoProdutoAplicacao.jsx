import { useState } from "react";
import { api } from "../api/client";

// Vermifugação e antipulgas têm exatamente os mesmos campos e o mesmo fluxo de
// CRUD (produto, data de aplicação, data prevista da próxima aplicação), só
// muda o endpoint. Este componente é reaproveitado pelas duas seções da tela
// de Saúde preventiva, cada instância com seu próprio estado de formulário.
const FORM_INICIAL = {
  produto: "",
  dataAplicacao: "",
  dataProximaAplicacao: "",
};

export function SecaoProdutoAplicacao({
  titulo,
  apiPath,
  itens,
  error,
  onSalvo,
  mensagemVazio,
  confirmacaoRemover,
}) {
  const [form, setForm] = useState(FORM_INICIAL);
  const [editingId, setEditingId] = useState(null);
  const [formError, setFormError] = useState("");

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
      produto: form.produto,
      dataAplicacao: form.dataAplicacao,
      dataProximaAplicacao: form.dataProximaAplicacao || null,
    };

    try {
      if (editingId) {
        await api.put(`${apiPath}/${editingId}`, payload);
      } else {
        await api.post(apiPath, payload);
      }

      limparFormulario();
      await onSalvo();
    } catch (submitError) {
      setFormError(submitError.response?.data?.message ?? "Não foi possível salvar o registro.");
    }
  }

  function handleEditar(item) {
    setEditingId(item.id);
    setForm({
      produto: item.produto,
      dataAplicacao: item.dataAplicacao,
      dataProximaAplicacao: item.dataProximaAplicacao ?? "",
    });
    setFormError("");
  }

  async function handleRemover(id) {
    if (!window.confirm(confirmacaoRemover)) {
      return;
    }

    await api.delete(`${apiPath}/${id}`);
    await onSalvo();
  }

  return (
    <div>
      <section className="dashboard-card">
        <h2>{editingId ? `Editar ${titulo.toLowerCase()}` : `Cadastrar ${titulo.toLowerCase()}`}</h2>
        <form onSubmit={handleSubmit} className="form">
          <label>
            Produto
            <input
              type="text"
              value={form.produto}
              onChange={(event) => handleChange("produto", event.target.value)}
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
            Data prevista da próxima aplicação (opcional)
            <input
              type="date"
              value={form.dataProximaAplicacao}
              onChange={(event) => handleChange("dataProximaAplicacao", event.target.value)}
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
        <h2>{titulo} registradas</h2>
        {error ? (
          <p>{error}</p>
        ) : itens === null ? (
          <p>Carregando...</p>
        ) : itens.length === 0 ? (
          <p>{mensagemVazio}</p>
        ) : (
          <ul className="vacinas-list">
            {itens.map((item) => (
              <li key={item.id} className="vacinas-item">
                <div>
                  <strong>{item.produto}</strong> — aplicado em {item.dataAplicacao}
                  {item.dataProximaAplicacao
                    ? ` (próxima aplicação: ${item.dataProximaAplicacao})`
                    : ""}
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
