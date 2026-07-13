# PetHealth — Levantamento de Requisitos

## Visão geral

Aplicação web para monitoramento e controle da saúde e qualidade de vida da Akira (SRD),
com um módulo dedicado a treinamento/adestramento.

## Contexto

- Desenvolvedor único (solo dev), usando Claude Code com agentes de papéis
  (Product Owner, Desenvolvedor, QA, UX/CX) para simular um fluxo de equipe completo.
- Projeto criado do zero como piloto para validar o fluxo de trabalho: refinamento,
  planejamento, desenvolvimento, code review, testes, staging, deploy e retrospectiva.

## Stack definida

- **Backend**: .NET 8 Web API (C#), EF Core + Npgsql, JWT auth, BCrypt, Swashbuckle/OpenAPI
  — mesmo padrão usado em `scgtr` e `barbershop.manager`.
- **Frontend**: React + Vite, axios, react-router-dom, chart.js (para gráficos de evolução
  de peso/saúde) — mesmo padrão usado em `scgtr`.
- **Banco de dados / Auth**: Supabase (Postgres).
- **Deploy**: VPS própria (Hostinger), via Docker. A VPS já roda outros projetos com Docker
  e Nginx como reverse proxy. Será criado um subdomínio dedicado (ex: `pethealth.<dominio>`).
- **CI/CD**: GitHub Actions — build/test automatizado + deploy via SSH para a VPS
  (docker compose up com a nova imagem), cuidando para não conflitar com portas/serviços
  já existentes na VPS.

## Escopo do MVP (completo desde o início)

### Módulo de Saúde
- Registro de peso ao longo do tempo (com gráfico de evolução)
- Histórico de consultas veterinárias
- Controle de vacinas (com datas e lembretes de próxima dose)
- Controle de vermifugação / antipulgas
- Registro de medicações em uso (dosagem, frequência, período)
- Observações/sintomas pontuais (ex: mudanças de comportamento, apetite)

### Módulo de Alimentação
- Registro da dieta/ração utilizada
- Controle de quantidade e horários de alimentação
- Registro de petiscos e itens fora da dieta padrão

### Módulo de Treinamento/Adestramento
- Cadastro de comandos/comportamentos sendo trabalhados
- Registro de progresso por sessão de treino (data, comando, nível de sucesso)
- Histórico de evolução por comando ao longo do tempo

### Módulo de Relatórios
- Dashboard com visão geral (peso atual x histórico, próximas vacinas, resumo de treino)
- Exportação/visualização de histórico por período

## Fora de escopo (por enquanto)
- Múltiplos pets / múltiplos usuários (o app é pensado para a Akira e o tutor)
- Integração com clínicas veterinárias externas
- App mobile nativo (o frontend web deve ser responsivo o suficiente para uso no celular)

## Próximos passos
1. Refinamento: quebrar os módulos acima em histórias/issues com critérios de aceite (DoD)
2. Planejamento: priorizar ordem de entrega (sugestão: Saúde → Alimentação → Treino → Relatórios)
3. Modelagem de dados inicial (entidades: Pet, RegistroPeso, Vacina, Medicacao, Refeicao,
   ComandoTreino, SessaoTreino)
