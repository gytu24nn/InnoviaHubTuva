import { useEffect, useState } from "react";
import "../ErrorAndLoading.css";
import "./SensorSummary.css";

interface SensorSummaryProps {
    totalSensors: number;
    activeSensors: number;
    inactiveSensor: number;
}

const SensorSummary = () => {
    const [stats, setStats] = useState<SensorSummaryProps>({
        totalSensors: 0,
        activeSensors: 0,
        inactiveSensor: 0,
    });

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    
    useEffect(() => {
        const fetchSensors = async () => {
            try {
                setLoading(true);

                const tenantRes = await fetch("http://localhost:5101/api/tenants/by-slug/innovia");
                if (!tenantRes.ok) throw new Error("kunde inte hämta tenant");
                const tenant = await tenantRes.json();

                const devicesRes = await fetch(`http://localhost:5101/api/tenants/${tenant.id}/devices`);
                if(!devicesRes.ok) throw new Error("Kunde inte hämta sensorer.");
                const devices = await devicesRes.json();

                // Beräkna statistik
                const activeSensors = devices.filter((d: any) => d.status === "active").length;
                const inactiveSensor = devices.filter((d: any) => d.status !== "active").length;

                setStats({
                    totalSensors: devices.length,
                    activeSensors,
                    inactiveSensor
                });
            } catch (err) {
                console.error(err);
                setError(err instanceof Error ? err.message : "Ett fel uppstod.");
            } finally {
                setLoading(false);
            }
            };

            fetchSensors();
    }, []);

    if(loading) return <p className="loading-message">Laddar sensorstatistik...</p>
    if(error) return <p className="error-message">{error}</p>

    const { totalSensors, activeSensors, inactiveSensor} = stats;

    return (
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
    )
}

export default SensorSummary;