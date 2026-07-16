import { useEffect, useRef, useState } from "react";
import { api } from "../api/client";

const FORM_INICIAL = {
  nome: "",
  raca: "",
  porte: "Pequeno",
  dataNascimento: "",
  sexo: "Macho",
  fotoUrl: "",
  pesoReferencia: "",
};

const hoje = new Date();
const HOJE = `${hoje.getFullYear()}-${String(hoje.getMonth() + 1).padStart(2, "0")}-${String(hoje.getDate()).padStart(2, "0")}`;

export function PerfilPet() {
  const [carregando, setCarregando] = useState(true);
  const [error, setError] = useState("");
  const [form, setForm] = useState(FORM_INICIAL);
  const [formError, setFormError] = useState("");
  const [sucesso, setSucesso] = useState("");
  const sucessoTimeoutRef = useRef(null);

  useEffect(() => {
    async function load() {
      try {
        const response = await api.get("/api/pet");
        setForm({
          nome: response.data.nome,
          raca: response.data.raca ?? "",
          porte: response.data.porte,
          dataNascimento: response.data.dataNascimento,
          sexo: response.data.sexo,
          fotoUrl: response.data.fotoUrl ?? "",
          pesoReferencia: response.data.pesoReferencia,
        });
      } catch (loadError) {
        if (loadError.response?.status !== 404) {
          setError("Não foi possível carregar o perfil do pet.");
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
      nome: form.nome,
      raca: form.raca || null,
      porte: form.porte,
      dataNascimento: form.dataNascimento,
      sexo: form.sexo,
      fotoUrl: form.fotoUrl || null,
      pesoReferencia: Number(form.pesoReferencia),
    };

    try {
      const response = await api.put("/api/pet", payload);
      setForm({
        nome: response.data.nome,
        raca: response.data.raca ?? "",
        porte: response.data.porte,
        dataNascimento: response.data.dataNascimento,
        sexo: response.data.sexo,
        fotoUrl: response.data.fotoUrl ?? "",
        pesoReferencia: response.data.pesoReferencia,
      });
      setSucesso("Perfil do pet salvo com sucesso.");
      sucessoTimeoutRef.current = setTimeout(() => setSucesso(""), 4000);
    } catch (submitError) {
      setFormError(submitError.response?.data?.message ?? "Não foi possível salvar o perfil do pet.");
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
      <h1>Perfil do pet</h1>

      <section className="dashboard-card">
        <form onSubmit={handleSubmit} className="form">
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
            Raça (opcional)
            <input
              type="text"
              value={form.raca}
              onChange={(event) => handleChange("raca", event.target.value)}
            />
          </label>
          <label>
            Porte
            <select
              value={form.porte}
              onChange={(event) => handleChange("porte", event.target.value)}
              required
            >
              <option value="Pequeno">Pequeno</option>
              <option value="Medio">Médio</option>
              <option value="Grande">Grande</option>
            </select>
          </label>
          <label>
            Data de nascimento
            <input
              type="date"
              value={form.dataNascimento}
              max={HOJE}
              onChange={(event) => handleChange("dataNascimento", event.target.value)}
              required
            />
          </label>
          <label>
            Sexo
            <select
              value={form.sexo}
              onChange={(event) => handleChange("sexo", event.target.value)}
              required
            >
              <option value="Macho">Macho</option>
              <option value="Femea">Fêmea</option>
            </select>
          </label>
          <label>
            Foto (URL, opcional)
            <input
              type="text"
              value={form.fotoUrl}
              onChange={(event) => handleChange("fotoUrl", event.target.value)}
            />
          </label>
          <label>
            Peso de referência (kg)
            <input
              type="number"
              step="0.01"
              min="0.01"
              value={form.pesoReferencia}
              onChange={(event) => handleChange("pesoReferencia", event.target.value)}
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
