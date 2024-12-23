# Movies CRUD Application

[![CI](https://github.com/EricSantos00/movies/actions/workflows/ci.yaml/badge.svg)](https://github.com/EricSantos00/movies/actions/workflows/ci.yaml)

A full-stack application for managing movies and actors, featuring a .NET backend and a React frontend. The application allows users to perform CRUD operations for movies and actors via the backend API. The frontend provides an interface for listing movies/actors and viewing their details.

## Getting Started

### Clone the Repository

```
git clone https://github.com/EricSantos00/movies.git
cd movies
```

### Run with Docker Compose

```
docker-compose up --build
```

This will:

- Start the backend API.
- Launch the React frontend.
- Set up the database.

Access the application:

- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5175

### Running Locally Without Docker

#### 1. Backend
```
cd movies-api
dotnet build
cd MoviesApi
dotnet run
```
The API Swagger will be available at: http://localhost:5175/swagger/index.html

#### 2. Frontend
```
cd movies-app
npm install
npm run dev
```
The React app will run on http://localhost:8080
