import { useEffect, useState } from "react";
import { useUser } from "../Context/UserContext";


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
            <h2>SmartTips</h2>
            <p>{smartTips}</p>
            

        </div>
    )
}
export default SmartTips;