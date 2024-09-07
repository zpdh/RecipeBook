## Introduction

This is a Recipe Book API, a robust backend service built using Domain-Driven Design (DDD) principles, offering a
variety of features to manage and explore culinary recipes.
It includes user registration, JWT authentication, external login via Google, CRUD recipe operations with image upload,
along filtering and search functionalities.
The API integrates with ChatGPT for recipe creating as well as some Azure cloud services such as Blob storages for
images, Service Bus for messaging and more.
It also includes unit and integration tests for most of the features, to assure reliability and quality to the product.


## Technologies

Backend:

- .NET 8.0
- C# 12
- MySQL
- SQL Server
- OpenAPI (Swagger)
- Entity Framework
- Dapper ORM
- FluentMigrator
- FluentValidation
- AutoMapper
- Sqids
- BCrypt

Tests:

- xUnit
- FluentAssertions
- Bogus
- Moq


## How to run the app

### Requirements

- IDE of your choice
- MySQL or SQL Server

### Installation

1. Clone the repository `git clone https://github.com/zpdh/RecipeBook.git`
2. Fills all the fields in `appsettings.Development.json`
3. Execute the API and enjoy!