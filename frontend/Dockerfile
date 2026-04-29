# Etapa 1: Construcción (Build)
FROM node:18-alpine AS build
WORKDIR /app

# Copiamos solo package.json y package-lock.json para aprovechar la caché de Docker
COPY package*.json ./
RUN npm ci

# Copiamos el resto del código fuente
COPY . .

# Compilamos la aplicación Angular en modo producción
RUN npm run build --configuration=production

# Etapa 2: Servidor Web Nginx
FROM nginx:alpine

# Removemos la configuración por defecto de Nginx y copiamos la nuestra
RUN rm /etc/nginx/conf.d/default.conf
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copiamos los archivos compilados desde la Etapa 1
# La carpeta "browser" es donde el nuevo Angular Builder pone los archivos
COPY --from=build /app/dist/frontend/browser /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
