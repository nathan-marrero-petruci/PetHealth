# Build stage
FROM node:22-alpine AS build
WORKDIR /src

COPY frontend/package*.json frontend/
RUN cd frontend && npm ci

COPY frontend/. frontend/
RUN cd frontend && npm run build

# Runtime stage
FROM nginx:alpine
COPY --from=build /src/frontend/dist /usr/share/nginx/html
COPY frontend/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
