FROM node:20-alpine
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .

RUN npm install -g @angular/cli
EXPOSE 4200 4201
