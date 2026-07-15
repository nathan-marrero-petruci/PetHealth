// Estrutura de navegação do menu principal, usada pelo AppLayout.
// Cada entrada representa uma página já implementada, associada ao módulo a
// que pertence (ver docs/backlog.md — módulos Saúde, Alimentação, Treinamento
// e Relatórios). Issues futuras (SAU-09 em diante) devem apenas acrescentar um
// novo objeto a este array com o `modulo`, `label` e `rota` da tela nova; o
// AppLayout agrupa e renderiza os itens automaticamente por módulo.
export const NAVEGACAO = [
  { modulo: "Saúde", label: "Peso", rota: "/peso" },
  { modulo: "Saúde", label: "Vacinas", rota: "/vacinas" },
  { modulo: "Saúde", label: "Consultas", rota: "/consultas" },
  { modulo: "Saúde", label: "Medicações", rota: "/medicacoes" },
  { modulo: "Saúde", label: "Saúde preventiva", rota: "/saude-preventiva" },
  { modulo: "Saúde", label: "Observações", rota: "/observacoes" },
  { modulo: "Alimentação", label: "Dieta padrão", rota: "/dieta" },
  { modulo: "Alimentação", label: "Refeições", rota: "/refeicoes" },
  { modulo: "Relatórios", label: "Dashboard", rota: "/dashboard" },
  { modulo: "Relatórios", label: "Histórico por período", rota: "/historico" },
];
