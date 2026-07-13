# PetHealth

Aplicação web para monitoramento de saúde, qualidade de vida e treinamento da Akira (SRD).

Projeto piloto para testar um fluxo de desenvolvimento com agentes de IA (Product Owner,
Developer, QA, UX/CX) via Claude Code. Veja o levantamento de requisitos completo em
[docs/requisitos.md](docs/requisitos.md).

## Stack

- **Backend**: .NET Web API (`backend/Api`) — EF Core + Npgsql, JWT, BCrypt
- **Frontend**: React + Vite (`frontend`) — axios, react-router-dom, chart.js
- **Banco de dados**: Supabase (Postgres)
- **Deploy**: Docker, via VPS própria (Hostinger) com Nginx como reverse proxy

## Rodando localmente

### Backend
```bash
cd backend/Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<sua connection string>"
dotnet user-secrets set "Jwt:Key" "<uma chave secreta>"
dotnet run
```

### Frontend
```bash
cd frontend
cp .env.example .env
npm install
npm run dev
```

### Credenciais do usuário seed

O sistema tem um único usuário (tutor), provisionado via seed fixo na migration
(`InitialCreate`) — não há endpoint de registro. Após rodar `dotnet ef database update`,
use estas credenciais para o primeiro login:

- **Email**: `tutor@pethealth.local`
- **Senha**: `TrocarDepoisDoPrimeiroLogin!`

Troque a senha assim que possível (ainda não há endpoint de troca de senha — ver módulo
de Fundação no backlog).

## Agentes

Os papéis de equipe estão definidos em `.claude/agents/`:
- `product-owner` — levantamento/refinamento de requisitos, backlog, issues
- `developer` — implementação de funcionalidades
- `qa` — validação contra critérios de aceite, testes
- `ux-cx` — experiência de uso e consistência de telas
