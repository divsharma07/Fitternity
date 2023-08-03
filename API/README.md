# How to Run this .NET Core Application

This Readme provides instructions on how to run this .NET Core application. Before proceeding, ensure that you have installed the required tools and dependencies mentioned below:

## Prerequisites

1. [**.NET Core SDK**](https://dotnet.microsoft.com/download) - Make sure you have the latest .NET Core SDK installed on your machine.

## Getting Started

1. **Clone the Repository**: Start by cloning this repository to your local machine.
```
git clone https://github.com/divsharma07/Fitternity
cd Fitternity
```
2. **Build the Application**: Use the .NET Core CLI to build the application.
```
dotnet build
```

3. **Apply Database Migrations** (if applicable): If the application uses a database, apply the migrations to set up the database schema.

```
dotnet ef database update
```

4. **Run the Application**: Now, you are ready to run the application.

```
dotnet watch run
```
This command will start the application, and you should see the application running on a local development server. By default, it will listen on `http://localhost:5000` (or `https://localhost:5001` for HTTPS).


