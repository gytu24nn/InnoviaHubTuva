import { useState, useEffect } from "react";
import * as signalR from "@microsoft/signalr";

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
        // Kanske kolla om användaren är inloggad beror på vad den placeras.

        const fetchIoTSensors = async () => {
            try {
                //Loading state medans sensor info hämtas
                setLoading(true);
                const IotTenant = await fetch("http://localhost:5101/api/tenants/by-slug/innovia");

                // KONTROLLERA OM HÄMTNING AV TENANT LYCKADES
                if (!IotTenant.ok) {
                    // Om 404, 500 etc.
                    throw new Error(`Kunde inte hitta tenant 'innovia'. Status: ${IotTenant.status}`);
                }
                const tenant = await IotTenant.json();

                const IotTenantDevices = await fetch(`http://localhost:5101/api/tenants/${tenant.id}/devices`);
                
                if(!IotTenantDevices.ok)
                {
                    throw new Error(`Kunde inte hämta enheter för tenant ${tenant.id}. Status: ${IotTenantDevices.status}`)
                }

                const data = await IotTenantDevices.json();
                setDevices(data);
            } catch (err) {
                console.error("Error fetching tenants or tenants devices.", err);
                setError(err instanceof Error ? err.message : "Ett oväntat fel uppstod vid hämtning av sensorer.");
            } finally {
                setLoading(false);
            }
        };

        fetchIoTSensors();
        
        // Här ansluter jag till signalR för att ta emot mätvärden från simulatorn.
        const connectionIot = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:5103/hub/telemetry")
            .withAutomaticReconnect()
            .build();
        
        connectionIot.on("measurementReceived", (m: Measurment) => {
            console.log("Mätning mottagen:", m); 
            setLatestMeasurements(prev => ({ ...prev, [m.deviceId]: m }));
        });

        connectionIot
            .start()
            .then(() => {
                console.log("Ansluten till SignalR hub IoT");
                connectionIot.invoke("JoinTenant", "innovia");
            })
            .catch(err => console.error("SignalR-fel:", err));
        
        return () => {
            connectionIot.stop();
        };
    }, []);


    if(loading) return <p>Laddar sensorer...</p>
    if (error) return <p>Fel: {error}</p>;

    return (
        <div>
            <h2>Sensorer:</h2>
            <ul>
                {devices.map(device => {
                     const measurement = latestMeasurements[device.id];
                    return (
                        <li key={device.id}>
                            <strong>{device.serial}</strong> ({device.model}) - {device.status}
                            <div>
                                {measurement ? (
                                    <>
                                        {measurement.type}: {measurement.value.toFixed(2)} {measurement.unit} 
                                        ({new Date(measurement.timestamp).toLocaleTimeString()})
                                    </>
                                ) : (
                                    "Inga mätningar ännu"
                                )}
                            </div>
                        </li>
                    );
                })}
            </ul>
        </div>
    )
}

export default IotSensors;