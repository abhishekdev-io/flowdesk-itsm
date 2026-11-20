# Stage 1: Build Vite React App
FROM node:22-alpine AS build

WORKDIR /app

# Copy package.json only to let npm resolve native bindings for the container OS
COPY package.json ./
RUN npm install

COPY . .

# In production behind Nginx, API is on the same origin under /api/v1
ENV VITE_API_BASE_URL=/api/v1

RUN npm run build

# Stage 2: Serve static files
FROM nginx:alpine AS runtime

COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
