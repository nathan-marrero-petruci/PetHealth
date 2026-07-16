import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { api } from "../api/client";

const NIVEL_SUCESSO_LABEL = {
  NaoRespondeu: "Não respondeu",
  Tentou: "Tentou",
  ConseguiuComAjuda: "Conseguiu com ajuda",
  ConseguiuSozinho: "Conseguiu sozinho",
};

const VACINA_STATUS_LABEL = {
  Vencida: "Vencida",
  Proxima: "Próxima",
};

export function Dashboard() {
  const [pet, setPet] = useState(null);
  const [pesos, setPesos] = useState(null);
  const [pesosError, setPesosError] = useState(false);
  const [vacinas, setVacinas] = useState(null);
  const [vacinasError, setVacinasError] = useState(false);
  const [sessoes, setSessoes] = useState(null);
  const [sessoesError, setSessoesError] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    async function load() {
      try {
        const petResponse = await api.get("/api/pet").catch((petError) => {
          if (petError.response?.status === 404) {
            return { data: null };
          }

          throw petError;
        });

        setPet(petResponse.data);

        // Sem pet cadastrado, os endpoints abaixo respondem 400 ("cadastre o pet
        // antes de..."), então nem faz sentido chamá-los.
        if (petResponse.data === null) {
          setPesos([]);
          setVacinas([]);
          setSessoes([]);
          return;
        }

        const [pesosResult, vacinasResult, sessoesResult] = await Promise.allSettled([
          api.get("/api/pesos"),
          api.get("/api/vacinas"),
          api.get("/api/sessoes-treino"),
        ]);

        if (pesosResult.status === "fulfilled") {
          setPesos(pesosResult.value.data);
        } else {
          setPesosError(true);
        }

        if (vacinasResult.status === "fulfilled") {
          setVacinas(vacinasResult.value.data);
        } else {
          setVacinasError(true);
        }

        if (sessoesResult.status === "fulfilled") {
          setSessoes(sessoesResult.value.data);
        } else {
          setSessoesError(true);
        }
      } catch {
        setError("Não foi possível carregar os dados do dashboard.");
      }
    }

    load();
  }, []);

  if (error) {
    return <p>{error}</p>;
  }

  const pesosCarregando = pesos === null && !pesosError;
  const vacinasCarregando = vacinas === null && !vacinasError;
  const sessoesCarregando = sessoes === null && !sessoesError;

  if (pesosCarregando || vacinasCarregando || sessoesCarregando) {
    return <p>Carregando...</p>;
  }

  const pesoAtual = pesos && pesos.length > 0 ? pesos[0] : null;
  const vacinasAlerta = vacinas
    ? vacinas.filter((vacina) => vacina.status === "Proxima" || vacina.status === "Vencida")
    : [];
  const ultimaSessao = sessoes && sessoes.length > 0 ? sessoes[0] : null;

  return (
    <div className="dashboard-page">
      <h1>Dashboard</h1>

      {pet && (
        <section className="dashboard-card dashboard-pet-identidade">
          {pet.fotoUrl && <img src={pet.fotoUrl} alt={pet.nome} className="dashboard-pet-foto" />}
          <h2>{pet.nome}</h2>
          <Link to="/perfil-pet">Editar perfil</Link>
        </section>
      )}

      <section className="dashboard-card">
        <h2>Peso</h2>
        {pesosError ? (
          <p>Não foi possível carregar os dados de peso.</p>
        ) : pesoAtual ? (
          <p>
            Peso atual: {pesoAtual.peso} kg ({pesoAtual.data})
            {pet ? ` — peso de referência: ${pet.pesoReferencia} kg` : ""}
          </p>
        ) : (
          <p className="text-meta">Nenhum registro de peso cadastrado ainda.</p>
        )}
        <Link to="/peso">Ver evolução de peso</Link>
      </section>

      <section className="dashboard-card">
        <h2>Vacinas</h2>
        {vacinasError ? (
          <p>Não foi possível carregar os dados de vacinas.</p>
        ) : vacinasAlerta.length === 0 ? (
          <p className="text-meta">Nenhuma vacina próxima ou vencida no momento.</p>
        ) : (
          <ul className="dashboard-vacinas-list">
            {vacinasAlerta.map((vacina) => (
              <li
                key={vacina.id}
                className={
                  vacina.status === "Vencida"
                    ? "dashboard-banner dashboard-banner-vencida"
                    : "dashboard-banner dashboard-banner-proxima"
                }
              >
                {vacina.nome} — {VACINA_STATUS_LABEL[vacina.status]}
                {vacina.dataProximaDose ? ` (${vacina.dataProximaDose})` : ""}
              </li>
            ))}
          </ul>
        )}
      </section>

      <section className="dashboard-card">
        <h2>Treino</h2>
        {sessoesError ? (
          <p>Não foi possível carregar os dados de treino.</p>
        ) : ultimaSessao ? (
          <div>
            <p>Última sessão: {ultimaSessao.data}</p>
            <ul>
              {ultimaSessao.comandos.map((comando) => (
                <li key={comando.comandoTreinoId}>
                  {comando.comandoTreinoNome}: {NIVEL_SUCESSO_LABEL[comando.nivelSucesso]}
                </li>
              ))}
            </ul>
          </div>
        ) : (
          <p className="text-meta">Nenhuma sessão de treino registrada ainda.</p>
        )}
      </section>
    </div>
  );
}
