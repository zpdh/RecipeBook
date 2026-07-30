# Recipe Book

A modern recipe management platform built with **.NET 8**, **Domain-Driven Design (DDD)**, and cloud-native integrations. Recipe Book provides secure authentication, recipe management with image uploads, AI-assisted recipe generation, and scalable infrastructure powered by Azure services.

## Overview

Recipe Book is designed as a production-ready backend API that demonstrates clean architecture and DDD principles. The solution is structured into multiple layers, separating business rules, application logic, infrastructure concerns, and API endpoints.

Users can:

- Register and authenticate using JWT
- Sign in using Google OAuth
- Create, update, delete, and browse recipes
- Upload recipe images
- Search and filter recipes
- Generate recipes using AI
- Interact with cloud-based services for storage and messaging

The project also includes extensive unit and integration testing to ensure reliability and maintainability.

---

## Technology Stack

### Backend

- .NET 8
- C# 12
- ASP.NET Core Web API
- Entity Framework Core
- Dapper
- FluentMigrator
- FluentValidation
- AutoMapper
- Sqids
- BCrypt
- OpenAPI / Swagger

### Databases

- MySQL
- SQL Server

### Cloud Services

- Azure Blob Storage
- Azure Service Bus
- OpenAI / ChatGPT Integration

### Authentication

- JWT Authentication
- Google OAuth Login

### Testing

- xUnit
- FluentAssertions
- Bogus
- Moq

---

## Features

### Authentication & Authorization

- User registration
- Secure login
- JWT authentication
- Google OAuth integration
- Password hashing with BCrypt

### Recipe Management

- Create recipes
- Update recipes
- Delete recipes
- View recipes
- Upload recipe images
- Recipe ownership validation

### Search & Filtering

- Search by recipe name
- Filter recipes
- Pagination support
- Optimized querying with Dapper

### AI-Powered Recipe Creation

Generate recipes automatically using ChatGPT integration.

Example use cases:

- Generate recipes from ingredients
- Create recipe variations
- Generate cooking instructions
- Discover new meal ideas

### Cloud Storage

Images are stored using Azure Blob Storage, providing:

- Scalability
- Reliability
- Cost-effective storage
- Secure file management

### Messaging

Azure Service Bus enables:

- Event-driven architecture
- Decoupled communication
- Asynchronous processing
- Improved scalability

---

## API Documentation

Swagger/OpenAPI is enabled for interactive API exploration.

After running the application, access:

```text
http://localhost:<port>/swagger
```

---

## Getting Started

### Prerequisites

- .NET 8 SDK
- Visual Studio, Rider, or VS Code
- MySQL or SQL Server
- Azure Storage Account (optional)
- Azure Service Bus (optional)
- OpenAI API Key (optional)
- Docker (optional)

---

## Installation

### Clone the Repository

```bash
git clone https://github.com/zpdh/RecipeBook.git
cd RecipeBook
```

---

## Configuration

Update:

```text
appsettings.Development.json
```

with your own values:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""
  },

  "Jwt": {
    "Secret": "",
    "ExpirationMinutes": 0
  },

  "Google": {
    "ClientId": "",
    "ClientSecret": ""
  },

  "Azure": {
    "BlobStorage": "",
    "ServiceBus": ""
  },

  "OpenAI": {
    "ApiKey": ""
  }
}
```

---

## Running the Application

### Using Visual Studio

1. Open:

```text
RecipeBook.sln
```

2. Set:

```text
RecipeBook.API
```

as startup project.

3. Run the application.

---

### Using .NET CLI

Restore packages:

```bash
dotnet restore
```

Build:

```bash
dotnet build
```

Run:

```bash
dotnet run --project src/Backend/RecipeBook.API
```

---

## Running with Docker

Build image:

```bash
docker build -t recipebook-api .
```

Run container:

```bash
docker run -p 8080:8080 recipebook-api
```

Access the API:

```text
http://localhost:8080
```

Swagger:

```text
http://localhost:8080/swagger
```

---

## Testing

Run all tests:

```bash
dotnet test
```
