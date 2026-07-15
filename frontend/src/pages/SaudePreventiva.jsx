import { useEffect, useState } from "react";
import { api } from "../api/client";
import { SecaoProdutoAplicacao } from "../components/SecaoProdutoAplicacao";

export function SaudePreventiva() {
  const [vermifugacoes, setVermifugacoes] = useState(null);
  const [vermifugacoesError, setVermifugacoesError] = useState("");
  const [antipulgas, setAntipulgas] = useState(null);
  const [antipulgasError, setAntipulgasError] = useState("");

  async function carregarVermifugacoes() {
    try {
      const response = await api.get("/api/vermifugacoes");
      setVermifugacoes(response.data);
      setVermifugacoesError("");
    } catch {
      setVermifugacoesError("Não foi possível carregar as vermifugações.");
    }
  }

  async function carregarAntipulgas() {
    try {
      const response = await api.get("/api/antipulgas");
      setAntipulgas(response.data);
      setAntipulgasError("");
    } catch {
      setAntipulgasError("Não foi possível carregar os registros de antipulgas.");
    }
  }

  useEffect(() => {
    Promise.allSettled([carregarVermifugacoes(), carregarAntipulgas()]);
  }, []);

  return (
    <div className="dashboard-page">
      <h1>Saúde preventiva</h1>

      <div className="saude-preventiva-grid">
        <SecaoProdutoAplicacao
          titulo="Vermifugação"
          apiPath="/api/vermifugacoes"
          itens={vermifugacoes}
          error={vermifugacoesError}
          onSalvo={carregarVermifugacoes}
          mensagemVazio="Nenhuma vermifugação cadastrada ainda."
          confirmacaoRemover="Remover esta vermifugação?"
        />
        <SecaoProdutoAplicacao
          titulo="Antipulgas"
          apiPath="/api/antipulgas"
          itens={antipulgas}
          error={antipulgasError}
          onSalvo={carregarAntipulgas}
          mensagemVazio="Nenhuma aplicação de antipulgas cadastrada ainda."
          confirmacaoRemover="Remover este registro de antipulgas?"
        />
      </div>
    </div>
  );
}
