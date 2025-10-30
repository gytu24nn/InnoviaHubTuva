# InnoviaHub
InnoviaHub is a company where you can become a member. This is its digital platform designed to bring people, ideas and resources together. 

On the platform, members can book and rent desks, meeting rooms, VR, and AI resource. You will receive notification in the application when a booking is made, and if you are an admin, you can add more resources, manage time slots, send notifications to all members or individuals, and much more. 

## 🌐 Deployed Application
You can test the fully functional version of the application here: (it will only deployed this month for a assignment)

**Link:** https://innvoiafrontend-bfhzfqenfdh5b7d5.swedencentral-01.azurewebsites.net/ 

## 🔑 Test Accounts
**You can sign in with the following credentials on the deployed application: (You can create your own user account but not a admin account)**
- **Admin account:**
    - **Username:** admin
    - **Password:** Admin123!
- **User account:** 
    - **Username:** user
    - **Password:** User123!

## Deployment Strategy
The code is managed using the following git branches: 

- **Main:** This is the primary branch where code is first pushed for review and testning.

- **Deploy-Innovia:** Code form this branch is automatically deployed to azure. 

## 📂 Project Structure

frontend: → React application (UI, user interactions)
```bash
cd
frontend
```
```bash
cd
vite-project      
```

backend: → .NET Web API (business logic, database handling)
```bash
cd
BackEnd
```

## 🚀 Getting Started
### Prerequisites:
- **Node.js** 
- **Vite** = (for the frontend)
- **.NET** = (for the backend)
- **Visual studio code or any moden code editor**
- **A modern web browser** 

### Qick start: 
1. Clone the repository: 
```bash
git clone https://github.com/hallerstrom/InnoviaHub.git
cd InnoviaHub
```
2. Navigate to the desired part of the project: 

**frontend:** [Frontend guide](./FrontEnd/vite-project/README.md) 
```bash 
cd frontend
cd vite-project
npm install
npm run dev
```

**Backend:** [Backend guide](./BackEnd/README.md)
```bash
cd BackEnd
dotnet restore
dotnet run
```

## 🛠️ Tech Stack
- **Frontend:** React, TypeScript, Vite, CSS/styling, SignalR, IoT-sensors
- **Backend:** .NET Web API, C#, Entity Framework, cookies, SignalR
- **IoT-backend:** IoT-sensors
(Link to [IoT forked repository](https://github.com/gytu24nn/innovia-iot-tuva))
- **Database:** SQL deployed to Azure 


## 👥 Team
InnoviaHub is developed by:

**Group:** Hub4

**Group members:**
- Tuva Gyllensten
- Christoffer Kjellgren
- Linus Hallerström
- Villiam Damström

## ✨ Individual Core Development (Tuva Gyllensten)
The following functionality was developed and implemented individually by me, **Tuva Gyllensten**:

- **SmartTips:** AI service where the user gets suggestions for optimal booking times. This function is added on the booking page where the user enters a time for booking. This function can help the user book a time when its less crouded if they want to.

- **IoT-sensorer:** I added IoT-sensorer thats fetched in from its own backend repository. I added a menu option where both admin and users can go in and see the sensors measurement value and on home page i added a summary of the sensor where the users can see if they are inactive or active and how many sensors the company have. See the forked [IoT-sensor](https://github.com/gytu24nn/innovia-iot-tuva) for details.

## 📖 Related Documentation
- **Backend Readme:** [Backend guide](./BackEnd/README.md)
- **Frontend Readme:** [Frontend guide](./FrontEnd/vite-project/README.md)