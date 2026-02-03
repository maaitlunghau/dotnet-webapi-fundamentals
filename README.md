<div align="center">

# 🚀 ASP.NET Core Web API Fundamentals

**Learning-focused projects to master backend development with ASP.NET Core**

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API-512BD4?logo=dotnet)](https://docs.microsoft.com/aspnet/core)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp&logoColor=white)](https://docs.microsoft.com/dotnet/csharp/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

</div>

---

## 📚 About This Repository

This repository contains a progressive series of ASP.NET Core Web API projects, designed to build strong backend fundamentals through hands-on practice. Each project focuses on specific concepts and real-world scenarios.

## 🛠️ Tech Stack

<div align="left">

| Technology | Purpose |
|------------|----------|
| ![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white) | Core Framework |
| ![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white) | Programming Language |
| ![MySQL](https://img.shields.io/badge/MySQL-4479A1?style=for-the-badge&logo=mysql&logoColor=white) | Database |
| ![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white) | Database |
| ![Docker](https://img.shields.io/badge/OrbStack-0DB7ED?style=for-the-badge&logo=docker&logoColor=white) | Containerization |
| ![Azure](https://img.shields.io/badge/Azure%20Data%20Studio-0078D4?style=for-the-badge&logo=microsoftazure&logoColor=white) | Database Management |

</div>

## 📂 Project Structure

```
dotnet-webapi-fundamentals/
├── 01_web-api_demo/          # Basic Web API setup
│   ├── Controllers/          # API Controllers
│   ├── Data/                 # DbContext
│   ├── Models/               # Domain Models
│   ├── Migrations/           # EF Core Migrations
│   └── docs/                 # Architecture diagrams
│
├── 02_one-to-many/           # One-to-Many relationship demo
│   ├── backend/              # Web API Project
│   │   ├── Controller/       # CategoryController, ProductController
│   │   ├── Data/             # DataContext with relationships
│   │   ├── DTOs/             # Data Transfer Objects
│   │   ├── Migrations/       # Database migrations
│   │   └── Program.cs        # API configuration + Swagger
│   │
│   ├── frontend/             # MVC Project
│   │   ├── Controllers/      # CategoryController, ProductController
│   │   ├── Views/
│   │   │   ├── Category/     # CRUD views for Category
│   │   │   ├── Product/      # CRUD views for Product
│   │   │   └── Shared/       # Layout, validation scripts
│   │   └── wwwroot/          # Static files (CSS, JS)
│   │
│   ├── LModels/              # Shared Class Library
│   │   └── Domain/           # Category.cs, Product.cs
│   │
│   └── docs/                 # Project diagrams
│
├── 03_*/                     # Coming soon...
└── README.md
```

---

## 📚 Projects

### 01. Product CRUD API

**Description:** Implement complete RESTful Product API demonstrating fundamental CRUD operations and database integration.

**Key Features:**
- RESTful CRUD endpoints (GET, POST, PUT, DELETE)
- Entity Framework Core integration
- SQL Server database with migrations
- Model validation & error handling
- Swagger UI documentation
- Async/await pattern

**Tech Stack:** ASP.NET Core Web API, EF Core, SQL Server, Swagger

<details>
<summary>📊 Architecture Diagram</summary>

![Architecture](01_web-api_demo/docs/architecture-diagram.png)

</details>

<details>
<summary>🔄 Sequence Diagram</summary>

![Sequence Flow](01_web-api_demo/docs/sequence-diagram.png)

</details>

---

### 02. One-to-Many Relationship API

**Description:** Implement complete Category-Product management system demonstrating One-to-Many relationship pattern.

**Key Features:**
- One-to-Many relationship: Category (1) → Products (Many)
- Full CRUD operations for both entities
- DTO pattern for clean API contracts
- Foreign key validation & safe navigation
- EF Core with SQL Server & Migrations
- Swagger UI with auto-redirect
- Cascade delete restriction (Restrict behavior)

**Tech Stack:** ASP.NET Core Web API, EF Core, SQL Server, Swagger

<details>
<summary>🏗️ Project Structure</summary>

![Project Structure](02_one-to-many/docs/demo2-project-structure.png)

</details>

<details>
<summary>🔄 CRUD Flow Sequence Diagram</summary>

![CRUD Product Flow](02_one-to-many/docs/crud-product-flow-sequence-diagram.png)

</details>

---

## 🎯 Learning Path

### Completed
- [x] **01_web-api_demo** - Initial Web API setup
- [x] **02_one-to-many** - One-to-Many relationship with EF Core

### In Progress
- [ ] **03_*** - TBD

### Planned Topics
- Entity Framework Core & Database Integration
- Authentication & Authorization (JWT)
- Repository Pattern & Clean Architecture
- File Upload/Download
- Pagination, Filtering & Sorting
- API Versioning
- Caching Strategies
- Error Handling & Logging
- Unit & Integration Testing
- Docker & Deployment

## 🚀 Getting Started

### Prerequisites
```bash
# Check .NET version
dotnet --version  # Should be 8.0 or higher
```

### Running a Project
```bash
# Navigate to project folder
cd 01_web-api_demo

# Restore dependencies
dotnet restore

# Run the project
dotnet run
```

## 📖 Resources

- [Official ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [.NET API Guidelines](https://github.com/microsoft/api-guidelines)
- [RESTful API Best Practices](https://restfulapi.net/)

## 📝 Notes

This is a personal learning repository. Each project builds upon previous concepts, creating a comprehensive understanding of ASP.NET Core Web API development.

---

<div align="center">

**Built with ❤️ for learning and growth**

</div>
