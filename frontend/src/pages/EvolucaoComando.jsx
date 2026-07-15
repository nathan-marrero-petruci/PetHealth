import { useEffect, useState } from "react";
import { Link, useLocation, useParams } from "react-router-dom";
import { api } from "../api/client";

const NIVEL_SUCESSO_LABEL = {
  NaoRespondeu: "Não respondeu",
  Tentou: "Tentou",
  ConseguiuComAjuda: "Conseguiu com ajuda",
  ConseguiuSozinho: "Conseguiu sozinho",
};

export function EvolucaoComando() {
  const { id } = useParams();
  const location = useLocation();
  const [nomeComando, setNomeComando] = useState(location.state?.nome ?? null);
  const [evolucao, setEvolucao] = useState(null);
  const [error, setError] = useState("");

  useEffect(() => {
    async function load() {
      try {
        const requests = [api.get(`/api/comandos-treino/${id}/evolucao`)];

        if (!location.state?.nome) {
          requests.push(api.get("/api/comandos-treino"));
        }

        const [evolucaoResponse, comandosResponse] = await Promise.all(requests);

        setEvolucao(evolucaoResponse.data);

        if (comandosResponse) {
          const comando = comandosResponse.data.find((item) => item.id === id);
          setNomeComando(comando?.nome ?? "");
        }
      } catch {
        setError("Não foi possível carregar a evolução deste comando.");
      }
    }

    load();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [id]);

  if (error) {
    return <p>{error}</p>;
  }

  if (evolucao === null) {
    return <p>Carregando...</p>;
  }

  const primeiraConquistaIndex = evolucao.findIndex(
    (item) => item.nivelSucesso === "ConseguiuSozinho",
  );

  return (
    <div className="dashboard-page">
      <h1>Evolução — {nomeComando}</h1>
      <p>
        <Link to="/comandos-treino">Voltar para comandos de treino</Link>
      </p>

      <section className="dashboard-card">
        {evolucao.length === 0 ? (
          <p>Nenhuma sessão registrada ainda para este comando.</p>
        ) : (
          <ul className="vacinas-list">
            {evolucao.map((item, index) => (
              <li key={`${item.data}-${index}`} className="vacinas-item">
                <div>
                  <strong>{item.data}</strong> — {NIVEL_SUCESSO_LABEL[item.nivelSucesso]}
                </div>
                {index === primeiraConquistaIndex && (
                  <span className="evolucao-primeira-conquista">
                    🎉 Primeira vez que conseguiu sozinho!
                  </span>
                )}
              </li>
            ))}
          </ul>
        )}
      </section>
    </div>
  );
}
