# Deploy — PetHealth na VPS (Hostinger)

O deploy é automático via GitHub Actions (`.github/workflows/deploy.yml`), disparado
após o CI passar em `master`. O workflow builda as imagens Docker, publica no GitHub
Container Registry (GHCR) e conecta via SSH na VPS para atualizar os containers.

## Passo a passo (fazer uma vez, direto na sua VPS/GitHub — não compartilhe segredos aqui no chat)

1. **Na VPS**, clonar o repositório num diretório dedicado, ex:
   ```bash
   git clone https://github.com/nathan-marrero-petruci/PetHealth.git /opt/pethealth
   cd /opt/pethealth
   cp .env.example .env   # preencher com os valores reais (connection string do Supabase, Jwt:Key, etc.)
   ```

2. **Tornar os pacotes do GHCR públicos** (mais simples) ou configurar `docker login ghcr.io`
   na VPS com um token que tenha escopo `read:packages`. Caminho mais simples: depois do
   primeiro push das imagens, ir em GitHub → seu perfil → Packages → `pethealth-api` /
   `pethealth-web` → Package settings → Change visibility → Public.

3. **Criar os secrets do repositório** em
   `Settings → Secrets and variables → Actions → New repository secret`:
   - `VPS_HOST` — IP ou domínio da VPS
   - `VPS_USER` — usuário SSH usado para deploy
   - `VPS_SSH_KEY` — chave privada SSH (gere um par dedicado para deploy, não reutilize
     sua chave pessoal; adicione a pública em `~/.ssh/authorized_keys` da VPS)
   - `VPS_DEPLOY_PATH` — caminho do repositório na VPS (ex: `/opt/pethealth`)

4. **Configurar o Nginx existente** na VPS para o novo subdomínio, apontando para as
   portas expostas pelo `docker-compose.prod.yml` (api em `8091`, frontend em `8092`):
   ```nginx
   server {
       listen 80;
       server_name pethealth.seudominio.com;

       location /api/ {
           proxy_pass http://127.0.0.1:8091/;
           proxy_set_header Host $host;
           proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
       }

       location / {
           proxy_pass http://127.0.0.1:8092/;
           proxy_set_header Host $host;
       }
   }
   ```
   Depois rodar `certbot` (ou o processo que já usam para os outros projetos) para HTTPS.
   Ajuste as portas `8091`/`8092` em `docker-compose.prod.yml` se já estiverem em uso por
   outro projeto na mesma VPS.

5. Criar o registro DNS do subdomínio `pethealth` apontando para o IP da VPS.

Depois disso, todo merge em `master` que passar no CI dispara o deploy automaticamente.
