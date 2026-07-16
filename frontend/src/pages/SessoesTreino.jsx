import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { api } from "../api/client";

const NIVEL_SUCESSO_LABEL = {
  NaoRespondeu: "Não respondeu",
  Tentou: "Tentou",
  ConseguiuComAjuda: "Conseguiu com ajuda",
  ConseguiuSozinho: "Conseguiu sozinho",
};

const NIVEL_SUCESSO_OPCOES = Object.keys(NIVEL_SUCESSO_LABEL);

function paraDiaIndex(data) {
  const [ano, mes, dia] = data.split("-").map(Number);
  return Date.UTC(ano, mes - 1, dia) / 86_400_000;
}

// Streak "vivo": só é exibido quando a sessão mais recente foi hoje ou ontem
// (senão estaríamos anunciando uma sequência que já parou há tempos) e quando
// há pelo menos 2 dias consecutivos com sessão (1 dia isolado não é sequência).
function calcularStreak(sessoes) {
  const dias = [...new Set(sessoes.map((sessao) => sessao.data))].sort().reverse();

  if (dias.length === 0) {
    return 0;
  }

  const hoje = new Date();
  const hojeIndex = Date.UTC(hoje.getFullYear(), hoje.getMonth(), hoje.getDate()) / 86_400_000;
  const maisRecenteIndex = paraDiaIndex(dias[0]);

  if (hojeIndex - maisRecenteIndex > 1) {
    return 0;
  }

  let streak = 1;

  for (let i = 1; i < dias.length; i++) {
    if (paraDiaIndex(dias[i - 1]) - paraDiaIndex(dias[i]) === 1) {
      streak++;
    } else {
      break;
    }
  }

  return streak;
}

// Popula o estado de seleção a partir de TODOS os comandos da sessão, mesmo os
// que hoje estão inativos e por isso não têm checkbox renderizado (só
// `comandosAtivos` vira checkbox). Isso é intencional: como o handleSubmit
// reenvia tudo que estiver marcado como `incluido` neste estado, um comando
// inativo presente numa sessão existente continua sendo reenviado no
// PUT mesmo sem aparecer na UI, preservando o histórico já registrado. Não
// "limpe" do estado o que não está em `comandosAtivos` — isso reintroduziria
// perda de dado ao editar sessões antigas.
function comandosParaSelecao(comandos) {
  const selecao = {};

  for (const comando of comandos) {
    selecao[comando.comandoTreinoId] = { incluido: true, nivelSucesso: comando.nivelSucesso };
  }

  return selecao;
}

const FORM_INICIAL = { data: "" };

export function SessoesTreino() {
  const [sessoes, setSessoes] = useState(null);
  const [comandosAtivos, setComandosAtivos] = useState(null);
  const [error, setError] = useState("");
  const [form, setForm] = useState(FORM_INICIAL);
  const [selecaoComandos, setSelecaoComandos] = useState({});
  const [editingId, setEditingId] = useState(null);
  const [formError, setFormError] = useState("");

  async function load() {
    try {
      const [sessoesResponse, comandosResponse] = await Promise.all([
        api.get("/api/sessoes-treino"),
        api.get("/api/comandos-treino"),
      ]);

      setSessoes(sessoesResponse.data);
      setComandosAtivos(comandosResponse.data.filter((comando) => comando.ativo));
    } catch {
      setError("Não foi possível carregar as sessões de treino.");
    }
  }

  useEffect(() => {
    load();
  }, []);

  function alternarComando(comandoId, incluido) {
    setSelecaoComandos((atual) => ({
      ...atual,
      [comandoId]: {
        incluido,
        nivelSucesso: atual[comandoId]?.nivelSucesso ?? NIVEL_SUCESSO_OPCOES[0],
      },
    }));
  }

  function alterarNivelSucesso(comandoId, nivelSucesso) {
    setSelecaoComandos((atual) => ({
      ...atual,
      [comandoId]: { ...atual[comandoId], nivelSucesso },
    }));
  }

  function limparFormulario() {
    setForm(FORM_INICIAL);
    setSelecaoComandos({});
    setEditingId(null);
    setFormError("");
  }

  async function handleSubmit(event) {
    event.preventDefault();
    setFormError("");

    const payload = {
      data: form.data,
      comandos: Object.entries(selecaoComandos)
        .filter(([, selecao]) => selecao.incluido)
        .map(([comandoTreinoId, selecao]) => ({
          comandoTreinoId,
          nivelSucesso: selecao.nivelSucesso,
        })),
    };

    try {
      if (editingId) {
        await api.put(`/api/sessoes-treino/${editingId}`, payload);
      } else {
        await api.post("/api/sessoes-treino", payload);
      }

      limparFormulario();
      await load();
    } catch (submitError) {
      setFormError(
        submitError.response?.data?.message ?? "Não foi possível salvar a sessão de treino.",
      );
    }
  }

  function handleEditar(sessao) {
    setEditingId(sessao.id);
    setForm({ data: sessao.data });
    setSelecaoComandos(comandosParaSelecao(sessao.comandos));
    setFormError("");
  }

  async function handleRemover(id) {
    if (!window.confirm("Remover esta sessão de treino?")) {
      return;
    }

    await api.delete(`/api/sessoes-treino/${id}`);
    await load();
  }

  if (error) {
    return <p>{error}</p>;
  }

  if (sessoes === null || comandosAtivos === null) {
    return <p>Carregando...</p>;
  }

  const streak = calcularStreak(sessoes);

  return (
    <div className="dashboard-page">
      <h1>Sessões de treino</h1>

      {streak >= 2 && (
        <p className="streak-badge">{streak} dias seguidos treinando!</p>
      )}

      <section className="dashboard-card">
        <h2>{editingId ? "Editar sessão" : "Registrar sessão"}</h2>
        {comandosAtivos.length === 0 ? (
          <p className="text-meta">
            Nenhum comando de treino cadastrado ainda. <Link to="/comandos-treino">Cadastre um comando</Link>{" "}
            antes de registrar uma sessão.
          </p>
        ) : (
          <form onSubmit={handleSubmit} className="form">
            <label>
              Data da sessão
              <input
                type="date"
                value={form.data}
                onChange={(event) => setForm({ data: event.target.value })}
                required
              />
            </label>

            <fieldset>
              <legend>Comandos trabalhados</legend>
              {comandosAtivos.map((comando) => {
                const selecao = selecaoComandos[comando.id];

                return (
                  <div key={comando.id}>
                    <label>
                      <input
                        type="checkbox"
                        checked={selecao?.incluido ?? false}
                        onChange={(event) => alternarComando(comando.id, event.target.checked)}
                      />
                      {comando.nome}
                    </label>
                    {selecao?.incluido && (
                      <select
                        value={selecao.nivelSucesso}
                        onChange={(event) => alterarNivelSucesso(comando.id, event.target.value)}
                      >
                        {NIVEL_SUCESSO_OPCOES.map((opcao) => (
                          <option key={opcao} value={opcao}>
                            {NIVEL_SUCESSO_LABEL[opcao]}
                          </option>
                        ))}
                      </select>
                    )}
                  </div>
                );
              })}
            </fieldset>

            {formError && <p className="login-error">{formError}</p>}
            <button type="submit">{editingId ? "Salvar" : "Registrar"}</button>
            {editingId && (
              <button type="button" onClick={limparFormulario}>
                Cancelar edição
              </button>
            )}
          </form>
        )}
      </section>

      <section className="dashboard-card">
        <h2>Histórico de sessões</h2>
        {sessoes.length === 0 ? (
          <p className="text-meta">Nenhuma sessão de treino registrada ainda.</p>
        ) : (
          <ul className="vacinas-list">
            {sessoes.map((sessao) => (
              <li key={sessao.id} className="vacinas-item">
                <div>
                  <strong>{sessao.data}</strong>
                  <ul>
                    {sessao.comandos.map((comando) => (
                      <li key={comando.comandoTreinoId}>
                        {comando.comandoTreinoNome} — {NIVEL_SUCESSO_LABEL[comando.nivelSucesso]}
                      </li>
                    ))}
                  </ul>
                </div>
                <div className="vacinas-item-acoes">
                  <button type="button" onClick={() => handleEditar(sessao)}>
                    Editar
                  </button>
                  <button type="button" onClick={() => handleRemover(sessao.id)}>
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
