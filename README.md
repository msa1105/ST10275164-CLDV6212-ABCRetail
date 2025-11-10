# 🏬 ABC Retail Management Suite

**A Cloud-Native Retail Management Platform**
Built with **ASP.NET Core MVC** and **.NET Azure Functions**, designed for scalability, maintainability, and seamless retail operations.

<p align="center">
  <a href="https://st10275164webapp.azurewebsites.net" target="_blank">
    <img src="https://img.shields.io/badge/Live_Demo-View_Now-blue?style=for-the-badge&logo=microsoft-azure" alt="Live Demo">
  </a>
  <img src="https://img.shields.io/badge/.NET-8.0-purple?style=for-the-badge&logo=dotnet" alt=".NET 8.0">
  <img src="https://img.shields.io/badge/Hosted_on-Azure-blue?style=for-the-badge&logo=microsoft-azure" alt="Azure">
</p>

---

## 🚀 Cloud-Native Architecture

A **decoupled, Platform-as-a-Service (PaaS)** model that maximizes **scalability**, **security**, and **performance**.

| Component         | Description                                                                                              |
| ----------------- | -------------------------------------------------------------------------------------------------------- |
| **Frontend**      | ASP.NET Core MVC app hosted on **Azure App Service** – handles UI rendering and user interaction.        |
| **Backend**       | Serverless **.NET Azure Functions** – processes business logic and data operations via secure HTTP APIs. |
| **Database**      | **Azure SQL Database** – stores relational data such as Customers, Products, and Orders using EF Core.   |
| **Blob Storage**  | Stores product images and other unstructured assets.                                                     |
| **File Storage**  | Manages business contracts and related files.                                                            |
| **Queue Storage** | Handles background events and auditing asynchronously.                                                   |

> 🧩 The frontend **never directly accesses** the database — all interactions flow securely through the Azure Functions API.

---

## ✨ Key Features

### 👥 Customer Management (CRM)

* Add and manage customers stored in **Azure SQL**
* View all clients with a modern, paginated interface

### 🛒 Product Management (PIM)

* Full **CRUD** operations (Create, Read, Update, Delete)
* Upload product images directly to **Azure Blob Storage**
* Manage and edit existing product listings

### 📦 Order Processing

* Streamlined order creation and management
* Automatically links **Customers** and **Products** in Azure SQL

### 📂 Contract Handling

* Upload and organize business contracts
* Secure storage and access via **Azure File Storage**

### 📬 Event Queue Monitoring

* Real-time insights into **Azure Queue Storage**
* Transparent logging and auditing of background operations

---

## 🧰 Technology Stack

| Layer           | Technology                                               |
| --------------- | -------------------------------------------------------- |
| **Frontend**    | ASP.NET Core MVC 8.0, HTML5, CSS3, Bootstrap             |
| **Backend API** | .NET 8.0 **Azure Functions (Isolated Worker)**           |
| **Hosting**     | Azure App Service (Frontend) • Azure Functions (Backend) |
| **Database**    | Azure SQL Database                                       |
| **ORM**         | Entity Framework Core 8.0                                |
| **Storage**     | Azure Blob (Images), Azure File (Contracts)              |
| **Messaging**   | Azure Queue Storage                                      |

---

## ⚙️ Local Setup & Configuration

### 1️⃣ Backend – `ABCRetail.Functions`

Configure and launch the serverless API before running the frontend.

#### 🔧 Setup Steps:

1. Create Azure services:

   * **Azure SQL Database**
   * **Azure Storage Account**
2. Add connection strings to `local.settings.json`:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "YOUR_AZURE_STORAGE_CONNECTION_STRING",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "SqlConnectionString": "YOUR_AZURE_SQL_DATABASE_CONNECTION_STRING"
  }
}
```

3. Run the project – typically available at:
   👉 `http://localhost:7071`

---

### 2️⃣ Frontend – `ST10275164-CLDV6212-POE`

Connect the MVC web app to your local Function API.

#### 🧩 Update `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "AzureStorage": {
    "ConnectionString": "YOUR_AZURE_STORAGE_CONNECTION_STRING"
  },
  "FunctionApiUrl": "http://localhost:7071/api/",
  "StorageAccountName": "YOUR_STORAGE_ACCOUNT_NAME"
}
```

4. Run the MVC app — it will now connect to your **local backend API**, while all data persists securely in **Azure Cloud Services**.

---

## 🌐 Live Deployment

🚀 **Deployed Frontend:**
🔗 [st10275164webapp.azurewebsites.net](https://st10275164webapp.azurewebsites.net)

☁️ **Architecture:**
Hosted using **Azure App Service** + **Azure Functions** + **Azure SQL** + **Blob/File/Queue Storage**

---

## 🧑‍💻 Author

**Muhammed Saif Alexander**
🎓 Student Number: ST10275164
💡 Bachelor of Computer Science and Application Development
🌍 South Africa

---

## 🪪 License

This project is licensed under the **MIT License** – free to use, modify, and distribute.

---

<p align="center">
  <img src="https://img.shields.io/badge/Built_with-❤_and_.NET_8.0-purple?style=for-the-badge&logo=dotnet">
</p>
