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
      try {
        const [pesosResponse, petResponse] = await Promise.all([
          api.get("/api/pesos"),
          api.get("/api/pet"),
        ]);
        setPesos(pesosResponse.data);
        setPet(petResponse.data);
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

  if (pesos.length === 0) {
    return <p>Nenhum registro de peso cadastrado ainda.</p>;
  }

  const registrosOrdenados = [...pesos].reverse();
  const labels = registrosOrdenados.map((registro) => registro.data);
  const pesoData = registrosOrdenados.map((registro) => registro.peso);

  // Chart.js só desenha um segmento de linha entre 2 posições no eixo X.
  // Com um único registro, a linha de referência não teria como ser traçada,
  // então adicionamos uma posição extra (sem rótulo, sem dado de peso) só
  // para dar à linha de referência onde se estender.
  const precisaPontoExtra = labels.length === 1;
  const labelsGrafico = precisaPontoExtra ? [...labels, ""] : labels;
  const pesoDataGrafico = precisaPontoExtra ? [...pesoData, null] : pesoData;

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
