import { useEffect, useState } from "react";
import "../ErrorAndLoading.css";
import "./SensorSummary.css";
import { Inspect } from "lucide-react";
import IotSensors from "./IoTSensors";

const IOT_SERVER_OFFLINE_ERROR = "IOT_SERVER_OFFLINE";

// Definierar typen för statistiken vi ska visa
interface SensorSummaryProps {
    totalSensors: number;
    activeSensors: number;
    inactiveSensor: number;
}

const SensorSummary = () => {
    // State för att lagra statistik om sensorer
    const [stats, setStats] = useState<SensorSummaryProps>({
        totalSensors: 0,
        activeSensors: 0,
        inactiveSensor: 0,
    });

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    
    useEffect(() => {
        // Funktion som hämtar data om sensorer och beräknar statistik
        const fetchSensors = async () => {
            try {
                setLoading(true);

                // Hämtar tenant-information baserat på slug "innovia"
                const tenantRes = await fetch("http://localhost:5101/api/tenants/by-slug/innovia");
                if (!tenantRes.ok) throw new Error("kunde inte hämta tenant");
                const tenant = await tenantRes.json();

                // Hämtar alla enheter som tillhör den här tenant:en
                const devicesRes = await fetch(`http://localhost:5101/api/tenants/${tenant.id}/devices`);
                if(!devicesRes.ok) throw new Error("Kunde inte hämta sensorer.");
                const devices = await devicesRes.json();

                // Beräknar statistik:
                // activeSensors = antal enheter där status är "active"
                // inactiveSensor = antal enheter där status inte är "active"
                const activeSensors = devices.filter((d: any) => d.status === "active").length;
                const inactiveSensor = devices.filter((d: any) => d.status !== "active").length;

                // Sparar statistiken i state
                setStats({
                    totalSensors: devices.length,
                    activeSensors,
                    inactiveSensor
                });
            } catch (err) {
                console.error(err);

                if(err instanceof TypeError && err.message.includes('Failed to fetch')) {
                    setError(IOT_SERVER_OFFLINE_ERROR);
                } else if (err instanceof Error) {
                    setError(err.message)
                } else {
                    setError(err instanceof Error ? err.message : "Ett oväntat fel uppstod vid hämtning av sensorer.");
                }
            } finally {
                setLoading(false);
            }
            };

            // Kör funktionen som hämtar och beräknar statistik
            fetchSensors();
    }, []);  // Tom array = körs endast en gång när komponenten mountas

    if(loading) return <p className="loading-message">Laddar sensorstatistik...</p>

    const { totalSensors, activeSensors, inactiveSensor} = stats;

    // Renderar statistik som kort (cards)
    return (

        <div className="SensorSummary">
            {error ? (
                error === IOT_SERVER_OFFLINE_ERROR ? (
                    <p className="Sensor-list-offline">Kontorets sensorer är offline.</p>
                ) : (
                    <p className="error-message">{error}</p>
                )
            ): (
                <div className="SensorSummary">
                <h2 className="SummaryTitle">Sensor översikt</h2>

                <div className="SummaryCards">
                    <div className="SummaryCard">
                        <i className="fa-solid fa-microchip"></i>
                        <h3>{totalSensors}</h3>
                        <p>Totalt antal sensorer</p>
                    </div>

                    <div className="SummaryCard Active">
                        <i className="fa-solid fa-signal"></i>
                        <h3>{activeSensors}</h3>
                        <p>Aktiva sensorer</p>
                    </div>

                    <div className="SummaryCard inactive">
                        <i className="fa-solid fa-power-off"></i>
                        <h3>{inactiveSensor}</h3>
                        <p>Inaktiva sensorer</p>
                    </div>
                </div>
            </div>
            )}
        </div>
        
        
    )
}

export default SensorSummary;