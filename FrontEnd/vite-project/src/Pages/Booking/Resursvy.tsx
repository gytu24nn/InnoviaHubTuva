import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import "./Resursvy.css"
import MyBookingsSummary from "../MyBookings/MyBookingsSummary";
import "../../ErrorAndLoading.css"

type ResourceType = {
    resourceTypeId: number;
    name: string;
}

const Resursvy = () => {
    const apiBase = "http://localhost:5099/api/Resource";
    const [resources, setResources] = useState<ResourceType[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    //fetch resources
    const fetchResources = async () => {
        try {
            const response = await fetch(`${apiBase}/resourceTypes`);
            if (!response.ok) {
                throw new Error("Något gick fel vid hämtning av resurser.")
            }
            const data = await response.json();
            setResources(data);
        } catch (err) {
            setError((err as Error).message);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchResources();
    }, []);


  return (
    <div className="PageLayout">
        <div className="ResourceContainer">
            <div className="Info-text">
                <h1 id="ResourceHeadLine">Välj en resurs</h1>
                <p id="ResourceInfoText">Välj en resurs du vill boka</p>
            </div>
            
            <div className="errorAndLoadingMessage-container">
                {loading && <p className="loading-message">Laddar resurser...</p>}
                {error && <p className="error-message">{error}</p>} 
            </div>
            
            <div id="ResourceInfo">
                {!loading && !error && resources.length === 0 && (
                    <p id="no-resources">Inga resurser tillgängliga</p>
                )}

                {!loading && resources.length > 0 && (
                    <ul className="ResourceList">
                        {resources.map((r) => (
                            <li key={r.resourceTypeId} className="ResourceItem">
                                <Link to={"`/Booking?resourceId=${r.resourceId}`"} className="ResourceItem-Link">
                                    <i className="fa-solid fa-star"></i>
                                    {r.name}
                                </Link>
                            </li>
                        ))}
                    </ul>
                )}
            </div>

            
            
            
        </div> 
            <div className="MyBookingsSummary">
                <MyBookingsSummary />
            </div>
    </div>
    
  )
}

export default Resursvy;