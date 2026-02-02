FROM node:20-alpine
WORKDIR /app

COPY package*.json ./

RUN npm install --legacy-peer-deps && npm cache clean --force

COPY . .

RUN npm install -g @angular/cli --legacy-peer-deps

EXPOSE 4200 4201 4202
