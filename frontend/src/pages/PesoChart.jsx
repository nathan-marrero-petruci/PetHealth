import { useEffect, useState } from "react";
import { Line } from "react-chartjs-2";
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Tooltip,
  Legend,
} from "chart.js";
import { api } from "../api/client";

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, Tooltip, Legend);

export function PesoChart() {
  const [pesos, setPesos] = useState(null);
  const [pet, setPet] = useState(null);
  const [error, setError] = useState("");

  useEffect(() => {
    async function load() {
      let petAtual = null;

      try {
        const petResponse = await api.get("/api/pet");
        petAtual = petResponse.data;
      } catch (petError) {
        if (petError.response?.status !== 404) {
          setError("Não foi possível carregar os dados de peso.");
          return;
        }
      }

      setPet(petAtual);

      // Sem pet cadastrado, /api/pesos responde 400 ("cadastre o pet antes
      // de..."), então nem faz sentido chamá-lo.
      if (!petAtual) {
        setPesos([]);
        return;
      }

      try {
        const pesosResponse = await api.get("/api/pesos");
        setPesos(pesosResponse.data);
      } catch {
        setError("Não foi possível carregar os dados de peso.");
      }
    }

    load();
  }, []);

  if (error) {
    return <p>{error}</p>;
  }

  if (pesos === null) {
    return <p>Carregando...</p>;
  }

  if (pesos.length === 0 && !pet) {
    return <p className="text-meta">Nenhum registro de peso cadastrado ainda.</p>;
  }

  const registrosOrdenados = [...pesos].reverse();
  const labels = registrosOrdenados.map((registro) => registro.data);
  const pesoData = registrosOrdenados.map((registro) => registro.peso);

  // Chart.js só desenha um segmento de linha entre 2 posições no eixo X.
  // Com menos de 2 registros, a linha de referência não teria como ser
  // traçada, então completamos com posições extras (sem rótulo, sem dado de
  // peso) só para dar à linha de referência onde se estender.
  const pontosExtras = Math.max(0, 2 - labels.length);
  const labelsGrafico = pontosExtras > 0 ? [...labels, ...Array(pontosExtras).fill("")] : labels;
  const pesoDataGrafico =
    pontosExtras > 0 ? [...pesoData, ...Array(pontosExtras).fill(null)] : pesoData;

  const data = {
    labels: labelsGrafico,
    datasets: [
      {
        label: "Peso (kg)",
        data: pesoDataGrafico,
        borderColor: "#4f46e5",
        backgroundColor: "#4f46e5",
      },
      ...(pet
        ? [
            {
              label: "Peso de referência (kg)",
              data: labelsGrafico.map(() => pet.pesoReferencia),
              borderColor: "#f59e0b",
              borderDash: [6, 6],
              pointRadius: 0,
            },
          ]
        : []),
    ],
  };

  return (
    <div className="peso-chart-page">
      <h1>Evolução de peso</h1>
      <Line data={data} />
    </div>
  );
}
