# ABC Retail Management Suite


Welcome to the **ABC Retail Management Suite**, a comprehensive, cloud-native web application designed to streamline retail operations. Built with a robust, scalable architecture using **ASP.NET Core MVC** and **Microsoft Azure**, this solution offers powerful tools for managing every aspect of your retail business, from customer relationships to product inventory and sales orders.

<p align="center">
  <a href="https://st10275164.azurewebsites.net" target="_blank">
    <img src="https://img.shields.io/badge/Live-Demo-blue.svg?style=for-the-badge&logo=microsoft-azure" alt="Live Demo">
  </a>
</p>

---

## ✨ Key Features

This application is packed with features to enhance productivity and provide deep insights into your business operations.

### 🗂️ **Dynamic Data Management**

-   **Customer Relationship Management (CRM):** Easily add new customers and view a comprehensive list of all your clients. The system is designed for quick access and efficient management of customer information.
-   **Product Information Management (PIM):** Maintain a detailed catalog of your products. Add new items, update existing ones, and keep your inventory organized and easily accessible.
-   **Order Processing:** A streamlined interface for creating and viewing customer orders. Link products to customers seamlessly and keep track of all sales transactions.
-   **Contract Handling:** Upload, store, and manage important business contracts. All documents are securely stored in the cloud for easy retrieval.

### ☁️ **Cloud-Powered Architecture**

-   **Azure Table Storage:** Serves as the primary database for storing structured NoSQL data like customer, product, and order information, ensuring high availability and scalability.
-   **Azure Blob Storage & File Storage:** Provides secure and scalable storage for all your business documents, including product images and signed contracts.
-   **Azure Queue Storage:** Decouples application components and enables asynchronous communication. Every major action—like creating a new customer or uploading a contract—generates a message, ensuring reliable and traceable event handling.

### 🛠️ **Advanced System Diagnostics**

-   **Cloud Connection Testing:** An integrated diagnostic tool to verify the connection status of all underlying Azure services, ensuring your application is always running smoothly.
-   **Event Queue Monitoring:** A dedicated view to monitor messages in the Azure Queue. This provides transparency into the system's background processes and helps in debugging and auditing.

---

## 🚀 Getting Started

To get this project up and running on your local machine, you'll need to have the following prerequisites installed:

-   [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
-   [Visual Studio 2022](https://visualstudio.microsoft.com/) or another compatible IDE
-   An active **Microsoft Azure subscription**

### **Configuration**

1.  **Clone the repository:**
    ```bash
    git clone [https://github.com/ST10275164/ST10275164-CLDV6212-ABCRetail.git](https://github.com/ST10275164/ST10275164-CLDV6212-ABCRetail.git)
    ```
2.  **Set up Azure services:**
    -   Create an Azure Storage Account.
    -   Within the storage account, create the necessary **tables** (`Customers`, `Products`, `Orders`), **blob containers**, **file shares**, and **queues** (`customer-events`, `contract-events`, etc.).
3.  **Configure connection strings:**
    -   In the `appsettings.json` file, update the connection strings for your Azure Storage services. It is highly recommended to use `secrets.json` for local development to keep your credentials secure.

    ```json
    {
      "ConnectionStrings": {
        "AzureStorage": "DefaultEndpointsProtocol=https;AccountName=your_account_name;AccountKey=your_account_key;EndpointSuffix=core.windows.net"
      },
      "StorageAccountName": "your_account_name"
    }
    ```

4.  **Run the application:**
    -   Open the solution in Visual Studio and press `F5` to build and run the project.

---

## 🔧 Technology Stack

This project is built on a modern, robust technology stack, ensuring high performance, scalability, and maintainability.

-   **Backend:** C#, ASP.NET Core MVC
-   **Frontend:** HTML5, CSS3, JavaScript, Bootstrap
-   **Cloud Platform:** Microsoft Azure
    -   **Storage:** Azure Table Storage, Azure Blob Storage, Azure File Storage
    -   **Messaging:** Azure Queue Storage
-   **Frameworks & Libraries:**
    -   Entity Framework Core (for potential future relational database integration)
    -   Azure.Data.Tables
    -   Azure.Storage.Blobs
    -   Azure.Storage.Files.Shares
    -   Azure.Storage.Queues


---

## 🤝 Contribution

This project was developed as a comprehensive portfolio piece to showcase skills in cloud-native application development. While it is not actively seeking contributions, feedback and suggestions are always welcome.

---

## 📜 License

This project is open-source and available for personal and educational use. Please refer to the project's license for more details.
