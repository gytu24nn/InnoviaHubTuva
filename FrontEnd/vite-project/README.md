# InnoviaHub Frontend 
This is the frontend part of this application, built with React, Typescript and Vite. It provides the user interface for booking desks, meeting rooms, VR and AI resources, receiving notification and much more. 

## 🚀 Getting Started:

### Prerequisites: 
- **Node.js** 
- **npm**
- **Visual Studio Code** or **Visual Studio**

### Installation: 
1. **Navigate to the frontend folder:**
    ```bash
    cd frontend
    cd vite-project
    ```

2. **Install dependencies:**
    ```bash
    npm install
    ```

3. Start frontend in the frontend folder: 
    **npm run dev:**  → Runs the app in development mode with HMR
    ```bash
    npm run dev
    ```

   or  **npm run build** → Builds the app for production
    ```bash
    npm run build
    ```

    or **npm run preview** → Locally preview the production build
    ```bash 
    npm run preview
    ```

4. **IoT/Sensor data**

    To see real-time data from the IoT/sensor system, you need to make sure IoT backend is running.

    Link to IoT repository: https://github.com/gytu24nn/innovia-iot-tuva 

    1. **Clone the repo**
        ```bash 
        git clone https://github.com/gytu24nn/innovia-iot-tuva.git
        ```
    2. **Start the database**

        To start the database do this command in deploy folder in the terminal. 

        ```bash
        docker-compose up -d
        ``` 

    3. **Start the different microservices in this order (each command in its own terminal)**
        ```bash 
        cd innovia-iot-tuva\src\DeviceRegistry.Api && dotnet run
        cd innovia-iot-tuva\src\Realtime.Hub && dotnet run 
        cd innovia-iot-tuva\src\Ingest-Gateway && dotnet run
        cd innovia-iot-tuva\src\Edge.Simulator && dotnet run
        ```

    4. **Add a tenant and devices**

        You need to create your own tenant for innovia and your own devices to be able to se them: 

        **First create tenant: Swagger**    
        This is the endpoint for create tenant:                             
        POST http://localhost:5101/api/tenants

        When you create a tenant you need to fill in name: Innovia Hub and slug: innovia

        **Second create devices for tenant: Swagger**

        This is the endpoint for create devices for a tenant:           
        POST http://localhost:5101/api/tenants/<TENANT_ID>/devices

        When your create a device you need to fill in this for exampel: 

        { "model":"Acme CO2-Temp", "serial":"dev-101", "status":"active" }

        **if your want to do add more different models you need to add information in the config.json file in Edge.Simulator.**
    

## 📖 Related Documentation
- **Backend:** [Backend guide](./BackEnd/README.md)
- **Root ReadMe:** [Root guide](../README.md)
