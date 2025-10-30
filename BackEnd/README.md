# InnoviaHub Backend
This is the backend part of this application, built with .NET Web API and Entity Framework. It Provides the business logic, database handling, and API endpoints for the platform. Admin can manage resources, bookings, and notifications through the backend services. 

## 🚀 Getting Started

### Prerequisites
- **.NET 8 SDK** (or version used in this project)
- **SQL Server**
- **Visual Studio Code** or **Visual Studio**

1. **Navigate to the backend folder:**
```bash
cd BackEnd
```

2. **Restore dependencies:**
```bash
dotnet restore
```

3. **Update the database:**
```bash
dotnet ef database update
```

4. **Create a `.env` file in the backend folder root to enable AI functionality:**

    #### OPEN_AI_KEY:
1. Go to [OpenAI’s API Keys page](https://platform.openai.com/account/api-keys).
(You need an OpenAI account to access this page.)

2. Click on “Create new secret key” and copy the generated key.
    >⚠️ You will only see the key once — make sure to copy and save it securely.

3. Add following line in the `.env` file and replace it with your OPENAI API-key: 
    ```env
    OPENAI_API_KEY="YOURE_OPENAI_API_KEY"
    ```
    >⚠️ Never commit your API key to version control. Keep it safe and only share 
it via environment variables or secret managers.

5. **Run the backend server:**
    ```bash
    dotnet watch --urls=http://localhost:5099/
    ```

**The server will start and automatically open Swagger UI at:**
👉 https://localhost:5001/swagger

## 📚 API Documentation (Swagger)

### Swagger provides an interactive interface where you can: 
- Browse and test all available endpoints
- Log in and perform authorized requests directly in the browser

## 🔑 Test Accounts
**You can log in using the following credentials in Swagger: (your can create your own user account but not a admin)**
- **Admin account:**
    - **Username:** admin
    - **Password:** Admin123!
- **User account:** 
    - **Username:** user
    - **Password:** User123!

## 📦 Available Commands
- dotnet restore → Restores dependencies

- dotnet build → Builds the application

- dotnet run → Runs the backend server

- dotnet ef migrations add <Name> → Creates a new migration

- dotnet ef database update → Applies migrations to the database

## 📖 Related Documentation
- **frontend:** [Frontend guide](./FrontEnd/vite-project/README.md) 
- **Root ReadMe:** [Root guide](../README.md)