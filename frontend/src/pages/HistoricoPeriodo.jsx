import { useState } from "react";
import { api } from "../api/client";

const VACINA_STATUS_LABEL = {
  SemProximaDose: "Sem próxima dose",
  EmDia: "Em dia",
  Proxima: "Próxima",
  Vencida: "Vencida",
};

const MEDICACAO_STATUS_LABEL = {
  EmUso: "Em uso",
  Encerrada: "Encerrada",
};

const NIVEL_SUCESSO_LABEL = {
  NaoRespondeu: "Não respondeu",
  Tentou: "Tentou",
  ConseguiuComAjuda: "Conseguiu com ajuda",
  ConseguiuSozinho: "Conseguiu sozinho",
};

const SECOES = [
  { key: "pesos", endpoint: "/api/pesos" },
  { key: "vacinas", endpoint: "/api/vacinas" },
  { key: "consultas", endpoint: "/api/consultas" },
  { key: "medicacoes", endpoint: "/api/medicacoes" },
  { key: "vermifugacoes", endpoint: "/api/vermifugacoes" },
  { key: "antipulgas", endpoint: "/api/antipulgas" },
  { key: "observacoes", endpoint: "/api/observacoes" },
  { key: "refeicoes", endpoint: "/api/refeicoes" },
  { key: "itensForaDieta", endpoint: "/api/itens-fora-dieta" },
  { key: "sessoes", endpoint: "/api/sessoes-treino" },
];

function Secao({ titulo, error, itens, renderItem }) {
  return (
    <section className="dashboard-card">
      <h2>{titulo}</h2>
      {error ? (
        <p>Não foi possível carregar os dados de {titulo.toLowerCase()}.</p>
      ) : itens.length === 0 ? (
        <p className="text-meta">Nenhum registro no período selecionado.</p>
      ) : (
        <ul>
          {itens.map((item) => (
            <li key={item.id}>{renderItem(item)}</li>
          ))}
        </ul>
      )}
    </section>
  );
}

export function HistoricoPeriodo() {
  const [dataInicio, setDataInicio] = useState("");
  const [dataFim, setDataFim] = useState("");
  const [validationError, setValidationError] = useState("");
  const [loading, setLoading] = useState(false);
  const [resultados, setResultados] = useState(null);
  const [erros, setErros] = useState({});

  async function handleSubmit(event) {
    event.preventDefault();

    if (!dataInicio || !dataFim) {
      setValidationError("Informe a data inicial e a data final.");
      return;
    }

    if (dataFim < dataInicio) {
      setValidationError("Data final não pode ser anterior à data inicial.");
      return;
    }

    setValidationError("");
    setLoading(true);

    const results = await Promise.allSettled(
      SECOES.map((secao) => api.get(secao.endpoint, { params: { dataInicio, dataFim } }))
    );

    const novosResultados = {};
    const novosErros = {};

    results.forEach((result, index) => {
      const chave = SECOES[index].key;

      if (result.status === "fulfilled") {
        novosResultados[chave] = result.value.data;
      } else {
        novosResultados[chave] = [];
        novosErros[chave] = true;
      }
    });

    setResultados(novosResultados);
    setErros(novosErros);
    setLoading(false);
  }

  return (
    <div className="dashboard-page">
      <h1>Histórico por período</h1>

      <form onSubmit={handleSubmit} className="form">
        <label>
          Data inicial
          <input
            type="date"
            value={dataInicio}
            onChange={(event) => setDataInicio(event.target.value)}
          />
        </label>
        <label>
          Data final
          <input
            type="date"
            value={dataFim}
            onChange={(event) => setDataFim(event.target.value)}
          />
        </label>
        <button type="submit">Aplicar filtro</button>
      </form>

      {validationError && <p className="login-error">{validationError}</p>}
      {loading && <p>Carregando...</p>}

      {resultados && !loading && (
        <>
          <Secao
            titulo="Peso"
            error={erros.pesos}
            itens={resultados.pesos}
            renderItem={(item) => `${item.peso} kg — ${item.data}`}
          />

          <Secao
            titulo="Vacinas"
            error={erros.vacinas}
            itens={resultados.vacinas}
            renderItem={(item) =>
              `${item.nome} — aplicada em ${item.dataAplicacao}` +
              (item.dataProximaDose ? ` (próxima dose: ${item.dataProximaDose})` : "") +
              ` — ${VACINA_STATUS_LABEL[item.status]}`
            }
          />

          <Secao
            titulo="Consultas veterinárias"
            error={erros.consultas}
            itens={resultados.consultas}
            renderItem={(item) =>
              `${item.data} — ${item.motivo}` +
              (item.veterinarioClinica ? ` (${item.veterinarioClinica})` : "")
            }
          />

          <Secao
            titulo="Medicações"
            error={erros.medicacoes}
            itens={resultados.medicacoes}
            renderItem={(item) =>
              `${item.nome} — ${item.dosagemValor} ${item.dosagemUnidade}, ${item.vezesPorDia}x ao dia — início em ${item.dataInicio}` +
              (item.dataTermino ? `, término em ${item.dataTermino}` : "") +
              ` — ${MEDICACAO_STATUS_LABEL[item.status]}`
            }
          />

          <Secao
            titulo="Vermifugação"
            error={erros.vermifugacoes}
            itens={resultados.vermifugacoes}
            renderItem={(item) =>
              `${item.produto} — aplicado em ${item.dataAplicacao}` +
              (item.dataProximaAplicacao ? ` (próxima aplicação: ${item.dataProximaAplicacao})` : "")
            }
          />

          <Secao
            titulo="Antipulgas"
            error={erros.antipulgas}
            itens={resultados.antipulgas}
            renderItem={(item) =>
              `${item.produto} — aplicado em ${item.dataAplicacao}` +
              (item.dataProximaAplicacao ? ` (próxima aplicação: ${item.dataProximaAplicacao})` : "")
            }
          />

          <Secao
            titulo="Observações"
            error={erros.observacoes}
            itens={resultados.observacoes}
            renderItem={(item) => `${item.data} — ${item.descricao}`}
          />

          <Secao
            titulo="Refeições"
            error={erros.refeicoes}
            itens={resultados.refeicoes}
            renderItem={(item) =>
              `${new Date(item.dataHora).toLocaleString()} — ${item.quantidadeGramas} g`
            }
          />

          <Secao
            titulo="Itens fora da dieta"
            error={erros.itensForaDieta}
            itens={resultados.itensForaDieta}
            renderItem={(item) =>
              `${new Date(item.dataHora).toLocaleString()} — ${item.descricao}` +
              (item.quantidadeObservacao ? ` (${item.quantidadeObservacao})` : "")
            }
          />

          <Secao
            titulo="Sessões de treino"
            error={erros.sessoes}
            itens={resultados.sessoes}
            renderItem={(item) => (
              <>
                {item.data}
                <ul>
                  {item.comandos.map((comando) => (
                    <li key={comando.comandoTreinoId}>
                      {comando.comandoTreinoNome}: {NIVEL_SUCESSO_LABEL[comando.nivelSucesso]}
                    </li>
                  ))}
                </ul>
              </>
            )}
          />
        </>
      )}
    </div>
  );
}
