# EVCS (Electric Vehicle Charging Station) Management System

## Overview
This project is an Electric Vehicle Charging Station (EVCS) Management System designed to manage and monitor charging stations. It utilizes a microservices-oriented architecture with gRPC for efficient communication between components.

## Technology Stack

### Core Framework
- **.NET 8.0**: The primary framework for all project components.

### Communication
- **gRPC**: Used for high-performance, cross-platform Remote Procedure Calls between the Web App, Console App, and the gRPC Service.
- **SignalR**: Implemented for real-time web functionality (found in WebApp and GrpcService).

### Database & Data Access
- **SQL Server**: The relational database management system.
- **Entity Framework Core 8.0**: The Object-Relational Mapper (ORM) for data access.

### Authentication & Security
- **ASP.NET Core Identity**: Manages user authentication and authorization.
- **JWT (JSON Web Tokens)**: Likely used for secure API authentication (referenced in Services).

## Project Structure

The solution `FA25_PRN232_SE1713_ASM3_SE183033_TriNM.sln` consists of the following projects:

### 1. EVCS.GrpcService.TriNM
- **Type**: ASP.NET Core gRPC Service
- **Role**: The central server handling business logic and data operations. It exposes gRPC endpoints for clients.
- **Key Dependencies**: `Grpc.AspNetCore`, `Microsoft.AspNetCore.SignalR`.

### 2. EVCS.WebApp.TriNM
- **Type**: ASP.NET Core Web Application
- **Role**: A web-based client that interacts with the gRPC service.
- **Key Dependencies**: `Grpc.Net.ClientFactory`, `Google.Protobuf`, `Microsoft.AspNetCore.SignalR.Client`.

### 3. EVCS.ConsoleApp.TriNM
- **Type**: Console Application
- **Role**: A command-line client for administrative or testing purposes.
- **Key Dependencies**: `Grpc.Net.Client`, `Google.Protobuf`.

### 4. EVCS.TriNM.Services
- **Type**: Class Library
- **Role**: Contains the business logic layer.
- **Key Dependencies**: `Microsoft.Extensions.Identity.Core`, `System.IdentityModel.Tokens.Jwt`.

### 5. EVCS.TriNM.Repositories
- **Type**: Class Library
- **Role**: Contains the data access layer and Entity Framework Core context.
- **Key Dependencies**: `Microsoft.EntityFrameworkCore.SqlServer`.

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server

### Running the Application

1.  **Database Setup**:
    - Ensure your connection string in `appsettings.json` (in `EVCS.GrpcService.TriNM`) points to a valid SQL Server instance.
    - Apply migrations if necessary (using EF Core CLI).

2.  **Start the Server**:
    Navigate to `EVCS.GrpcService.TriNM` and run:
    ```bash
    dotnet run
    ```
    The service will typically listen on `http://localhost:5112` (or similar, check launch logs).

3.  **Run the Web Client**:
    Navigate to `EVCS.WebApp.TriNM` and run:
    ```bash
    dotnet run
    ```

4.  **Run the Console Client**:
    Navigate to `EVCS.ConsoleApp.TriNM` and run:
    ```bash
    dotnet run
    ```
