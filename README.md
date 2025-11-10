ABC Retail Management Suite

Welcome to the ABC Retail Management Suite, a comprehensive, cloud-native web application designed to streamline retail operations. Built with a robust, scalable architecture using an ASP.NET Core MVC frontend and a serverless .NET Azure Functions backend.

<p align="center">
<a href="https://www.google.com/search?q=https://st10275164webapp.azurewebsites.net" target="_blank">
<img src="https://www.google.com/search?q=https://img.shields.io/badge/Live_Demo-st10275164webapp.azurewebsites.net-blue.svg%3Fstyle%3Dfor-the-badge%26logo%3Dmicrosoft-azure" alt="Live Demo">
</a>
</p>

🚀 Cloud-Native Architecture

This solution is built using a decoupled, Platform-as-a-Service (PaaS) model to ensure scalability, maintainability, and efficient resource use.

Frontend: A user-facing ASP.NET Core MVC application hosted on Azure App Service. This service is responsible for rendering all UI and handling user input.

Backend: A serverless .NET Azure Functions API that exposes all business logic and data operations via HTTP endpoints.

Data Storage: The backend API communicates with a suite of specialized Azure storage services:

Azure SQL Database: The primary database for all structured, relational data (Customers, Products, Orders), accessed via Entity Framework Core.

Azure Blob Storage: Used to store unstructured data, such as product images.

Azure File Storage: Used to store business documents like contracts.

Azure Queue Storage: Used for asynchronous event messaging and auditing.

This decoupled design means the frontend web application has no direct access to the database; it communicates exclusively with the secure Azure Functions API.

✨ Key Features

This application provides a full suite of tools for managing retail operations:

Customer Management (CRM): Easily add new customers and view a comprehensive list of all clients, stored in Azure SQL.

Product Management (PIM): A full CRUD (Create, Read, Update, Delete) module for products.

Upload product images directly to Azure Blob Storage.

Edit and delete existing product listings.

Order Processing: A streamlined interface for creating and viewing customer orders, linking Customers and Products from the Azure SQL database.

Contract Handling: Upload, store, and manage important business contracts. All documents are securely stored in Azure File Storage for easy retrieval.

Event Queue Monitoring: A dedicated view to monitor messages in Azure Queue Storage. This provides transparency into the system's background processes and helps in debugging and auditing.

🔧 Technology Stack

This project is built on a modern, robust technology stack:

Component

Technology

Frontend

C#, ASP.NET Core 8.0 MVC, HTML5, CSS, Bootstrap

Backend API

.NET 8.0 Azure Functions (Isolated Worker)

Hosting

Azure App Service (Frontend), Azure Functions (Backend)

Database

Azure SQL Database

Data Access

Entity Framework Core 8.0

File Storage

Azure Blob Storage (for product images)



Azure File Storage (for contracts)

Messaging

Azure Queue Storage

🛠️ Local Setup & Configuration

To run this project locally, you will need to set up both solutions and configure their respective settings.

1. ABCRetail.Functions (Backend API)

This project must be configured and running first.

Set up the required Azure services:

An Azure SQL Database (and run the migrations from the project).

An Azure Storage Account (note the connection string).

In the local.settings.json file (create one if it doesn't exist), add your connection strings:

{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "YOUR_AZURE_STORAGE_CONNECTION_STRING",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "SqlConnectionString": "YOUR_AZURE_SQL_DATABASE_CONNECTION_STRING"
  }
}


Run the ABCRetail.Functions project. It will typically start on http://localhost:7071. Note this URL.

2. ST10275164-CLDV6212-POE (Frontend MVC App)

Open the appsettings.json file in this project.

Update the FunctionApiUrl to point to your locally running function app. Don't forget the /api/ at the end.

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


Run the ST10275164-CLDV6212-POE project. It will now communicate with your local backend API, which in turn communicates with your cloud-based Azure services.
