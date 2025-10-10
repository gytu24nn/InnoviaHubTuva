import { useEffect, useState } from "react";
import { useUser } from "../Context/UserContext";
import "./SmartTips.css";

interface SmartTipsProps {
    resourceTypeTips?: string | null, 
    timeSlotIdTips?: number | null,
    dateTips?: Date | null,
    resourceIdTips?: number | null 
}

const SmartTips = ({resourceTypeTips, timeSlotIdTips, dateTips, resourceIdTips}: SmartTipsProps) => {
    const [smartTips, SetSmartTips] = useState<string>("Laddar tips...");
    const { isLoggedIn, loading: userLoading } = useUser();
    const [error, setError] = useState("");
    const [loading, SetLoading] = useState(true);
    const [robotWave, setrobotWave] = useState(false);
    

    useEffect(() => { 
        if(!resourceTypeTips) return;
         
        const fetchSmartTips = async () => {
            try {
                const params = new URLSearchParams({
                    ResourceType: resourceTypeTips,
                    ...(dateTips ? { date: dateTips.toISOString().split("T")[0]} : {}),
                    ...(resourceIdTips ? {resourceId: resourceIdTips?.toString()} : {}),
                    ...(timeSlotIdTips ? {timeSlotId: timeSlotIdTips?.toString()} : {})
                });

                const response = await fetch(`http://localhost:5099/api/SmartTips?${params.toString()}`, {
                    method: "GET",
                    credentials: "include",
                });
                const data = await response.json();
                SetSmartTips(data.tip);

                setrobotWave(true);
                setTimeout(() => setrobotWave(false), 800)

            } catch (err) {
                setError((err as Error).message);
            } finally {
                SetLoading(false);
            }
        }

        if(!userLoading && isLoggedIn) {
            fetchSmartTips();
        }
    }, [userLoading, isLoggedIn, resourceIdTips, dateTips, timeSlotIdTips, resourceTypeTips]);

    return (
        <div className="SmartTipsContainer">
            <div className="AI-icon">
                <i className={`fa-solid fa-robot ${robotWave ? "animate" : ""} `}></i>
            </div>

            <div className="TextContent-smartTips">
                <h3 className="Header-smartTips">SmartTips</h3>
                <p className="tip-smartTips">{smartTips}</p>
            </div>
            
        </div>
    )
}
export default SmartTips;