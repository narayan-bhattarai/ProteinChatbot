Proteomics Expert Chatbot
This project implements a Proteomics Expert Chatbot designed to provide expert-level insights into
proteomics.
It uses a combination of a backend developed in .NET 6, a frontend built with Angular 19, and a
locally hosted
large language model (LLM) via Ollama for robust functionality.
Running Ollama Locally
1. Download and install Ollama's LLM engine from their official site.
2. Configure the environment variables or paths for the LLM binary.
3. Start the Ollama server:
 ```
 ollama run llama3.2:1b
 ```
4. Verify that the LLM server is running on the expected port (default: 11434) on http://localhost:11434.
Angular Frontend Setup
1. Navigate to the frontend directory:
 ```bash
 cd frontend
 ```
2. Install dependencies:
 ```bash
 npm install
 ```
3. Start the development server:
 ```bash
 ng serve
 ```
4. Access the application at `http://localhost:4200`.
.NET Backend Setup
1. Navigate to the backend directory:
 ```bash
 cd backend
 ```
2. Restore dependencies:
 ```bash
 dotnet restore
 ```
3. Build the project:
 ```bash
 dotnet build
 ```
4. Run the server:
 ```bash
 dotnet run
 ```
5. Access the backend API at `https://localhost:5001/api`.
Dockerizing the Application
### Dockerizing Angular Frontend
1. Build the Docker image:
 ```bash
 docker build -t proteomics-chatbot-frontend -f frontend/Dockerfile .
 ```
2. Run the Docker container:
 ```bash
 docker run -d -p 80:80 proteomics-chatbot-frontend
 ```
### Dockerizing .NET Backend
1. Build the Docker image:
 ```bash
 docker build -t proteomics-chatbot-backend -f backend/Dockerfile .
 ```
2. Run the Docker container:
 ```bash
 docker run -d -p 5001:5001 proteomics-chatbot-backend
 ```
The application will be accessible at `http://localhost`.
Conclusion
This project combines advanced technologies to provide a seamless user experience for exploring
proteomics data.
It leverages the power of Ollama's LLM, Angular for the frontend, .NET for the backend, and Docker
for easy deployment.
