// For live backend:
// export const BASE_URL = "innoviahubtuva-debdbqgjg9bjaafh.swedencentral-01.azurewebsites.net";

// For development:
// export const BASE_URL = "http://localhost:5099";

export const BASE_URL =
    window.location.hostname === "localhost" || window.location.hostname === "127.0.0.1"
    ? "http://localhost:5099"
    : "innoviahubtuva-debdbqgjg9bjaafh.swedencentral-01.azurewebsites.net";