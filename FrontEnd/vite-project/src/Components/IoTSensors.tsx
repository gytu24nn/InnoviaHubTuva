import { useState, useEffect } from "react";
import * as signalR from "@microsoft/signalr";
import "./IotSensors.css";
import "../ErrorAndLoading.css";

const IOT_SERVER_OFFLINE_ERROR = "IOT_SERVER_OFFLINE";

interface Device {
    id: string;
    tenantId: string;
    model: string;
    serial: string;
    status: string;
}

interface Measurment {
    deviceId: string; 
    type: string;
    value: number;
    unit: string;
    timestamp: string; 
}

const IotSensors = () => {
    const [devices, setDevices] = useState<Device[]>([]);
    const [latestMeasurements, setLatestMeasurements] = useState<Record<string, Measurment>>({});
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchIoTSensors = async () => {
            try {
                //Loading state medans sensor info hämtas
                setLoading(true);
                // Hämtar tenant-data (kund/organisation) baserat på en slug (här: "innovia").
                const IotTenant = await fetch("http://localhost:5101/api/tenants/by-slug/innovia");

                // Kontrollerar om hämtning av tenant lyckades.
                if (!IotTenant.ok) {
                    // Om 404, 500 etc.
                    throw new Error(`Kunde inte hitta tenant 'innovia'. Status: ${IotTenant.status}`);
                }
                const tenant = await IotTenant.json();

                // Hämtar alla enheter (devices) som tillhör den aktuella tenant:en. 
                const IotTenantDevices = await fetch(`http://localhost:5101/api/tenants/${tenant.id}/devices`);
                
                // Kontrollerar om hämning av devices lyckades om inte 404 eller 500 bland annat.
                if(!IotTenantDevices.ok)
                {
                    throw new Error(`Kunde inte hämta enheter för tenant ${tenant.id}. Status: ${IotTenantDevices.status}`)
                }
                const data = await IotTenantDevices.json();
                // Här sätts datan man fetchat in i ett usestate/variabel.
                // Konverterar svaret till JSON och sparar i state.
                setDevices(data);
            } catch (err) {
                console.error("Error fetching tenants or tenants devices.", err);

                if(err instanceof TypeError && err.message.includes('Failed to fetch')) {
                    setError(IOT_SERVER_OFFLINE_ERROR);
                } else if (err instanceof Error) {
                    setError(err.message)
                } else {
                    setError("Ett oväntat fel uppstod vid hämtning av sensorer");
                }
            } finally {
                setLoading(false);
            }
        };

        // Kör funktionen som hämtar sensorer.
        fetchIoTSensors();
        
          // Skapar en SignalR-anslutning för att ta emot realtidsdata från sensorerna.
        const connectionIot = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:5103/hub/telemetry")
            .withAutomaticReconnect()
            .build();
        
            // lyssnar på eventet measurmentReceived som skickas från SignalR 
            // varje gång en ny mätning tas emot, uppdateras statet med de senaste mätningarna.
        connectionIot.on("measurementReceived", (m: Measurment) => {
            console.log("Mätning mottagen:", m); 
            setLatestMeasurements(prev => ({ ...prev, [m.deviceId]: m }));
        });

        // Här startas signalR-anslutningen. 
        // När anslutningen är igång anropar vi moetoden jointentant på servern
        // För att ansluta os till rätt tenant och ta emot alla relevanta sensorer.
        connectionIot
            .start()
            .then(() => {
                console.log("Ansluten till SignalR hub IoT");
                connectionIot.invoke("JoinTenant", "innovia");
            })
            .catch(err => console.error("SignalR-fel:", err));
        
         // Den här return-funktionen körs när komponenten tas bort från DOM:en.
        // Den stänger anslutningen till SignalR-hubben för att undvika minnesläckor.
        return () => {
            connectionIot.stop();
        };
    }, []);


    if(loading) return <p className="loading-message">Laddar sensorer...</p>

    return (
        <div className="Sensor-container">
            <h2 className="Sensor-Title">Sensorer:</h2>

            {error ? (
                error === IOT_SERVER_OFFLINE_ERROR ? (
                    <p className="Sensor-list-offline">Kontorets sensorer är offline. Kontakta admin!</p>
                ) : (
                    <p className="error-message">{error}</p>
                )
            ): (
                <ul className="Sensor-list">
                    {devices.map(device => {
                        const measurement = latestMeasurements[device.id]; // Hämtar senaste mätning för den här enheten
                        let displayValue = "-";

                        if(measurement) {
                            if(measurement.unit === "detections" || measurement.unit === "motions")
                            {
                                displayValue = measurement.value > 0.5 ? "Detected" : "Undetected"
                            } else if (measurement.unit === "Waterleak") {
                                displayValue = measurement.value > 0.5 ? "Yes" : "No"
                            }
                            else if (typeof measurement.value === "number") {
                                displayValue = measurement.value.toFixed(2)
                            } 
                            else {
                                displayValue = String(measurement.value)
                            }
                        }


                        return (
                            <li key={device.id} className={`Sensor-list-item ${latestMeasurements[device.id] ? "updated" : ""}`}>
                                <strong className="Sensor-list-item-Title">
                                    {device.model}: 
                                    <span className={device.status.toLowerCase() === "active" ? "Sensor-status-online" : "Sensor-status-offline"}>
                                        ({device.status})
                                    </span>
                                </strong>

                                <div className="Sensor-list-item-measurements">
                                    {measurement ? (
                                        <>
                                            <p className={
                                                displayValue === "Detected" ? "value-green" 
                                                : displayValue === "Undetected" ? "value-red"
                                                : displayValue === "Yes" ? "value-red"
                                                : displayValue === "No" ? "value-green"
                                                : "measurements-info"
                                            }>
                                                {displayValue} <i>{measurement.unit === "detections" || measurement.unit === "Waterleak" || measurement.unit === "motions" ? "" : measurement.unit}</i>
                                            </p> 
                                            
                                        </>
                                    ) : (
                                        "Inga mätningar ännu"
                                    )}
                                </div>
                            </li>
                        );
                    })}
                    {devices.length === 0 && !loading && !error && (
                        <li className="Sensor-list-item">Inga enheter registrerade för denna tenant.</li>
                    )}
                </ul>
            )}

            
        </div>
    )
}

export default IotSensors;