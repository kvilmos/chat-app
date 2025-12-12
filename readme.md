# 💬 .NET 9 Chat Server (Raw WebSockets)

![.NET](https://img.shields.io/badge/.NET-9.0-purple)
![Docker](https://img.shields.io/badge/Docker-DevContainer-blue)

A lightweight Chat Application server demonstrating the use of **native WebSockets** in ASP.NET Core without external libraries like SignalR. The project uses **Entity Framework Core** with **PostgreSQL** for data persistence.

> **Note:** This project is designed for educational purposes to understand the underlying mechanics of WebSocket communication and connection lifecycle management in .NET. It also serves as a refresher project for C# and .NET 9 development.

## ✨ Features

- **Authentication System** (User verification logic using JWT)
- **Real-time Communication**: Native WebSocket implementation.
- **Group Management**: Create groups and manage memberships.
- **Message Broadcasting**: Send real-time updates to connected clients.
- **Message History**: Fetch past messages with paging support.
- **Database Integration**: Full persistence using PostgreSQL.

## 🛠️ Tech Stack

- **Framework**: .NET 9.0
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **Environment**: Docker DevContainers

## 🚀 Getting Started

This project is configured with a **DevContainer** specification for instant setup using VS Code.

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Visual Studio Code](https://code.visualstudio.com/)
- [Dev Containers Extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers) for VS Code

### Installation & Setup

1. **Clone the repository**

   ```bash
   git clone https://github.com/kvilmos/chat-app.git
   ```

2. **Open in VS Code**
   Open the folder in Visual Studio Code.

3. **Reopen in Container**
   When prompted by VS Code (bottom right), click **"Reopen in Container"**.
   _Alternatively, press `F1` and type `>Dev Containers: Reopen in Container`._

   Wait for the Docker container to build (this sets up .NET SDK and the PostgreSQL database automatically).

4. **Apply Migrations**
   Initialize the database inside the container terminal:

   ```bash
   dotnet ef database update
   ```

5. **Run the Server**
   ```bash
   dotnet run
   ```
   The API will be available at `https://localhost:5275`.

## 🔌 API Testing

API endpoints can be tested using the included [Bruno](https://www.usebruno.com) collection files found in the repository.

## 🤝 Acknowledgments

- User management inspired by Patrick God's [JWT Authentication tutorial](https://youtu.be/6EEltKS8AwA?t=2863).
- API server structure and many-to-many relations adopted from the [DotNetManyToManyExample](https://github.com/badsyntax/DotNetManyToManyExample) repository.
- Native WebSocket implementation based on the [.NET Core documentation](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/websockets?view=aspnetcore-9.0).
