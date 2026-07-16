import { useEffect, useRef, useState } from "react";
import { api } from "../api/client";

const FORM_INICIAL = {
  nomeMarca: "",
  quantidadeDiariaGramas: "",
  quantidadePorRefeicaoGramas: "",
};

export function DietaPadrao() {
  const [carregando, setCarregando] = useState(true);
  const [error, setError] = useState("");
  const [form, setForm] = useState(FORM_INICIAL);
  const [formError, setFormError] = useState("");
  const [sucesso, setSucesso] = useState("");
  const sucessoTimeoutRef = useRef(null);

  useEffect(() => {
    async function load() {
      try {
        const response = await api.get("/api/dieta");
        setForm({
          nomeMarca: response.data.nomeMarca,
          quantidadeDiariaGramas: response.data.quantidadeDiariaGramas,
          quantidadePorRefeicaoGramas: response.data.quantidadePorRefeicaoGramas,
        });
      } catch (loadError) {
        if (loadError.response?.status !== 404) {
          setError("Não foi possível carregar a dieta.");
        }
      } finally {
        setCarregando(false);
      }
    }

    load();

    return () => clearTimeout(sucessoTimeoutRef.current);
  }, []);

  function handleChange(campo, valor) {
    setForm((atual) => ({ ...atual, [campo]: valor }));
  }

  async function handleSubmit(event) {
    event.preventDefault();
    setFormError("");
    setSucesso("");

    const payload = {
      nomeMarca: form.nomeMarca,
      quantidadeDiariaGramas: Number(form.quantidadeDiariaGramas),
      quantidadePorRefeicaoGramas: Number(form.quantidadePorRefeicaoGramas),
    };

    try {
      const response = await api.put("/api/dieta", payload);
      setForm({
        nomeMarca: response.data.nomeMarca,
        quantidadeDiariaGramas: response.data.quantidadeDiariaGramas,
        quantidadePorRefeicaoGramas: response.data.quantidadePorRefeicaoGramas,
      });
      setSucesso("Dieta salva com sucesso.");
      sucessoTimeoutRef.current = setTimeout(() => setSucesso(""), 4000);
    } catch (submitError) {
      setFormError(submitError.response?.data?.message ?? "Não foi possível salvar a dieta.");
    }
  }

  if (error) {
    return <p>{error}</p>;
  }

  if (carregando) {
    return <p className="text-meta">Carregando...</p>;
  }

  return (
    <div className="dashboard-page">
      <h1>Dieta padrão</h1>

      <section className="dashboard-card">
        <form onSubmit={handleSubmit} className="form">
          <label>
            Nome/marca da ração
            <input
              type="text"
              value={form.nomeMarca}
              onChange={(event) => handleChange("nomeMarca", event.target.value)}
              required
            />
          </label>
          <label>
            Quantidade diária recomendada (g)
            <input
              type="number"
              step="0.01"
              min="0.01"
              max="9999.99"
              value={form.quantidadeDiariaGramas}
              onChange={(event) => handleChange("quantidadeDiariaGramas", event.target.value)}
              required
            />
          </label>
          <label>
            Quantidade por refeição (g)
            <input
              type="number"
              step="0.01"
              min="0.01"
              max="9999.99"
              value={form.quantidadePorRefeicaoGramas}
              onChange={(event) => handleChange("quantidadePorRefeicaoGramas", event.target.value)}
              required
            />
          </label>
          {formError && <p className="login-error">{formError}</p>}
          {sucesso && <p className="form-success">{sucesso}</p>}
          <button type="submit">Salvar</button>
        </form>
      </section>
    </div>
  );
}
