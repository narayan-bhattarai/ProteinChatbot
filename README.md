Proteomics Expert Chatbot

This project implements a chatbot application that uses .NET 6 as the backend, Angular 19 as the frontend, and Ollama's Llama 3.2:1b model for chatbot functionality. The application is designed for querying proteomics data and interacting with the UniProt database for protein-related information.

Table of Contents
Project Overview
Prerequisites
Installation
Running the Project
Docker Containerization
Ollama Integration
Project Structure
API Endpoints
Key Features
Troubleshooting
Project Overview
This chatbot integrates:

Frontend: Angular 19 for the user interface.
Backend: .NET 6 for APIs and business logic.
LLM: Ollama’s Llama 3.2:1b hosted locally on port 11434.
Prerequisites
Ensure you have the following installed:

Node.js: v16 or later.
Angular CLI: v15 or later.
.NET 6 SDK.
Docker: v20 or later.
Ollama CLI: Installed and configured locally.
Git.
Installation
1. Clone the Repository
bash

git clone https://github.com/your-repo/proteomics-chatbot.git
cd proteomics-chatbot
2. Install Frontend Dependencies
bash

cd client-app
npm install
3. Install Backend Dependencies
bash

cd ../server
dotnet restore
4. Configure Ollama
Ensure the Ollama service is running locally:

bash

ollama serve --port=11434
Running the Project
1. Start the Backend
bash

cd server
dotnet run
2. Start the Frontend
bash

cd client-app
ng serve
3. Access the Application
Navigate to http://localhost:4200 in your browser.

Docker Containerization
Dockerfile for Backend (.NET 6)
Create a file named Dockerfile in the server directory:

dockerfile

# Use the official .NET 6 runtime as base
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

# Use the official .NET 6 SDK for building the app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["server.csproj", "./"]
RUN dotnet restore

# Build the project
COPY . .
RUN dotnet publish -c Release -o /app

# Runtime stage
FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "server.dll"]
Dockerfile for Frontend (Angular)
Create a file named Dockerfile in the client-app directory:

dockerfile

# Use Node.js for building the Angular app
FROM node:16 AS build
WORKDIR /app

# Copy package.json and install dependencies
COPY package*.json ./
RUN npm install

# Copy the rest of the application and build
COPY . .
RUN npm run build --prod

# Use Nginx for serving the Angular app
FROM nginx:alpine
COPY --from=build /app/dist/client-app /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
Docker-Compose for Backend and Frontend
Create a docker-compose.yml file:

yaml

version: '3.8'

services:
  backend:
    build:
      context: ./server
    ports:
      - "5000:5000"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production

  frontend:
    build:
      context: ./client-app
    ports:
      - "4200:80"

  ollama:
    image: ollama/ollama:latest
    ports:
      - "11434:11434"
    volumes:
      - ./ollama:/ollama-data
    command: ["ollama", "serve"]
Ollama Integration
Ensure that Ollama is installed locally and running on port 11434. To install Ollama:

bash

# Install Ollama CLI (Example for Windows)
https://ollama.com/

# Run Ollama service
ollama run llama3.2:1b
Project Structure
bash

proteomics-chatbot/
├── Clientapp/       # Angular frontend
├── WebUI/           # .NET 6 backend
├── docker-compose.yml
└── README.md
API Endpoints
1. Protein by Name
Endpoint: /api/Chat/parse-protein-data-by-proteinName
Method: GET
Description: Fetch proteins based on their names.

2. Protein by UniProt ID
Endpoint: /api/Chat/extract-protein-data-by-uniProtId
Method: GET
Description: Fetch details about a protein using its UniProt ID.

3. Chatbot Query
Endpoint: /api/Chat/query-to-ollama?promptValue=test
Method: POST
Description: Chat with the Llama 3.2:1b model.

Key Features
Chatbot integrated with Ollama LLM.
Frontend-Backend communication via REST APIs.
Querying protein data via UniProt.
Troubleshooting
Angular Build Issues:

Ensure you are using Node.js v16+ and Angular CLI v15+.
Run npm cache clean --force.
Backend Issues:

Check that .NET 6 SDK is installed.
Run dotnet restore to fix missing dependencies.
Ollama Issues:

Verify Ollama is running on port 11434.
Restart the service with ollama serve --port=11434.



The Image of the Chabot Looks like 
![chatbot](https://github.com/user-attachments/assets/9b0afa11-bfa6-4c91-b8cf-ed106ca65d6b)
