import { useEffect, useState } from "react";
import "./Admin.css"
import "../../ErrorAndLoading.css"

const admin = () => {
    const [resources, setResources] = useState<Resource[]>([]);
    const [bookings, setBookings] = useState<Booking[]>([]);
    const [newResource, setNewResource] = useState<{ resourceTypeId: string; name: string }>({ resourceTypeId: "", name: ""});
    const [editResource, setEditResource] = useState({ id: "", resourceTypeId: "", name: "" });
    const [resourceTypes, setResourceTypes] = useState<ResourceType[]>([]);
    const [selectedResourceType, setSelectedResourceType] = useState<number | null>(null);
    const [selectedDate, setSelectedDate] = useState<string | null>(null);
    const [isAdmin, setIsAdmin] = useState<boolean | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const [ntTitle, setNtTitle] = useState("");
    const [ntMessage, setNtMessage] = useState("");
    const [ntUserId, setNtUserId] = useState("");
    const [ntStatus, setNtStatus] = useState("Info");
    const [ntLoading, setNtLoading] = useState(false);
    const [showNotificationForm, setShowNotificationForm] = useState(false);

    // Typ för en bokning
    type Booking = {
      bookingId: number;
      date: string;
      userId: string;
      resourceId: number;
      resourceName: string;
      timeSlotId: number;
      startTime: string; 
      endTime: string;
      userEmail:string;
    //   userName:string; 
    };

    // Typ för en resurs
    type Resource = {
      resourceId: number; 
      resourceTypeId: number;
      name: string;
      resourceTypeName: string;
    };

    // Typ för en resurstyp
    type ResourceType = {
      resourceTypeId: number;
      name: string;
    };

    const [showBookings, setShowBookings] = useState(false);
    const [showResources, setShowResources] = useState(false);
    const [showAddResourceForm, setShowAddResourceForm] = useState(false);

    const apiBase = "/api"; 
    const adminApiBase = `${apiBase}/admin`;
    const authApiBase = `${apiBase}/auth`;

    // Funktion för att hämta användarens behörighet från servern
    const checkUserRole = async () => {
      setLoading(true);
      setError("");
      try {
        const response = await fetch(`${authApiBase}/me`, {
          credentials: 'include'
        });
        
        if (response.ok) {
          const user = await response.json();
          setIsAdmin(user.roles.includes("Admin"));
        } else {
          setIsAdmin(false); 
        }
      } catch (err) {
        console.error("Fel vid behörighetskontroll:", err);
        setError("Kunde inte verifiera behörighet.");
        setIsAdmin(false);
      } finally {
        setLoading(false);
      }
    };

    // Hämta alla resurser
    const fetchResources = async () => {
        try {
            const response = await fetch(`${adminApiBase}/resources`, { credentials: 'include' });
            if (!response.ok) { throw new Error('Kunde inte hämta resurser.'); }
            const data = await response.json();
            setResources(data);
        } catch (err) {
            console.error("Kunde inte hämta resurser:", err);
            setError("Kunde inte hämta resurser.");
        }
    }

    // Hämta alla bokningar
    const fetchBookings = async () => {
        try {
            const response = await fetch(`${adminApiBase}/bookings`, { credentials: 'include' });
            if (!response.ok) { throw new Error('Kunde inte hämta bokningar.'); }
            const data = await response.json();
            setBookings(data);
        } catch (err) {
            console.error("Kunde inte hämta bokningar:", err);
            setError("Kunde inte hämta bokningar.");
        }
    }

    const fetchResourceTypes = async () => {
        try {
            const response = await fetch(`${apiBase}/resource/resourcetypes`, { credentials: 'include' });
            if (!response.ok) { throw new Error('Kunde inte hämta resurstyper.'); }
            const data: ResourceType[] = await response.json();
            setResourceTypes(data);
        } catch (err) {
            console.error("Kunde inte hämta resurstyper:", err);
            setError("Kunde inte hämta resurstyper.");
        }
    };
    
    // Anrop till API-slutpunkter vid komponentens start
    useEffect(() => {
        checkUserRole();
    }, []);

    useEffect(() => {
      if (isAdmin) {
          fetchBookings();
          fetchResources();
          fetchResourceTypes();
      }
    }, [isAdmin]);

    // Skicka notis
    const sendNotification = async (toUserId?: string, quickMessage?: string) => {
        setNtStatus("");
        const title = ntTitle.trim() || "Meddelande från admin";
        const message = (quickMessage ?? ntMessage).trim();
        const userId = (toUserId ?? ntUserId).trim();

        if (!message) {
            setNtStatus("Meddelande kan inte vara tomt.");
            return;
        }

        setNtLoading(true);
        try {
            const response = await fetch(`${adminApiBase}/notify`, {
                method: "POST",
                credentials: 'include',
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ title, message, userId: userId || null })
            });
            if (!response.ok) {
                throw new Error("Kunde inte skicka notis.");
            }

            setNtStatus(`skickat ${userId ? `till ${userId}` : "till alla"}`);
            setNtTitle("");
            setNtMessage("");
            if (!toUserId)  setNtUserId("");
            } catch (e: any) {
                setNtStatus(e?.message ?? "Något gick fel");
            } finally {
                setNtLoading(false);
        }
    };
    

    // Skapa en ny resurs
    const createResource = async () => {
        const resourceData = {
            ...newResource,
            resourceTypeId: parseInt(newResource.resourceTypeId, 10),
        };
        try {
            await fetch(`${adminApiBase}/resources`, {
                method: "POST",
                headers:{ "Content-Type": "application/json" },
                body: JSON.stringify(resourceData),
                credentials: 'include'
            });
            setNewResource({ resourceTypeId: "", name: ""});
            fetchResources();
        } catch (err) {
            console.error("Kunde inte skapa resurs:", err);
            setError("Kunde inte skapa resurs.");
        }
    };

    // Uppdatera resurs
    const submitEditResource = async () => {
        try {
            await fetch(`${adminApiBase}/resource/edit/${editResource.id}`, {
                method: "PATCH",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    resourceTypeId: editResource.resourceTypeId,
                    name: editResource.name,
                }),
                credentials: 'include'
            });
            setEditResource({ id: "", resourceTypeId: "", name: "" });
            fetchResources();
        } catch (err) {
            console.error("Kunde inte uppdatera resurs:", err);
            setError("Kunde inte uppdatera resurs.");
        }
    };

    const deleteResource = async (id: number) => {
        const userConfirmed = window.confirm("Är du säker på att du vill radera denna resurs? Detta går inte att ångra.");
        if (userConfirmed){
            try {
                await fetch(`${adminApiBase}/resource/${id}`, {
                    method: "DELETE",
                    credentials: 'include'
                });
                fetchResources();
            } catch (err) {
                console.error("Kunde inte radera resurs:", err);
                setError("Kunde inte radera resurs.");
            }
        }
    };

    // Funktion för att filtrera resurser
    const filteredResources = resources.filter(resource => {
        return selectedResourceType === null || resource.resourceTypeId === selectedResourceType;
    })

    // Funktion för att filtrera bokningar baserat på datum
    const filteredBookings = bookings.filter(booking => {
        if (!selectedDate) {
            return true;
        }
        return booking.date.split('T')[0] === selectedDate;
    });

    // Visa laddningsstatus medan vi väntar på behörighetskontrollen
    if (loading) {
        return (
            <div id="admin-panel">
                <div className="errorAndLoadingMessage-container">
                    <p className="loading-message">Laddar...</p>
                </div>
            </div>
        );
    }

    // Om användaren inte är admin, returnera ett felmeddelande
    if (!isAdmin) {
        return (
            <div id="admin-panel">
                <div className="errorAndLoadingMessage-container">
                    <p className="error-message">Åtkomst nekad - du har inte behörighet att se denna sida.</p>
                </div>
            </div>
        );
    }

    return (
        <div id="admin-panel">
            <h1>Admin Panel</h1>

            <div className="errorAndLoadingMessage-container">
                {error && <p className='error-message'>{error}</p>}
            </div>

            {/* KNAPP FÖR ATT VISA BOKNINGAR */}
            <button className="GettingStartedButton" onClick={() => setShowBookings(!showBookings)}>
                {showBookings ? "Dölj bokningar" : "Se alla bokningar"}
            </button>

            {/* Visa listan om showBookings är true */}
            {showBookings && (
                <div id="bookings-list">
                    <h2>Alla bokningar</h2>
                    <div className="filter-buttons">
                        <input
                            type="date"
                            value={selectedDate || ''}
                            onChange={(e) => setSelectedDate(e.target.value)}
                        />
                        <button onClick={() => setSelectedDate(null)}>
                            Visa alla
                        </button>
                    </div>
                    <ul id="booking-ul">
                        {filteredBookings.map((booking) => (
                            <li key={booking.bookingId} className="booking-item">
                                <strong>Boknings-ID:</strong> {booking.bookingId} | <strong>Resurs:</strong> {booking.resourceName} |{" "}
                                <strong>Datum:</strong> {new Date(booking.date).toLocaleDateString()} |{" "}
                                <strong>Tid:</strong> {booking.startTime} | <strong>Slut:</strong> {booking.endTime} |
                                <strong>Bokad av:</strong> {booking.userEmail} 
                            </li>
                        ))}
                    </ul>
                </div>
            )}

            {/* KNAPP FÖR ATT VISA RESURSER */}
            <button className="GettingStartedButton" onClick={() => setShowResources(!showResources)}>
                {showResources ? "Dölj resurser" : "Se alla resurser"}
            </button>

            {/* Visa resurslistan om showResources är true */}
            {showResources && (
                <div id="resources-list">
                    <h2>Befintliga resurser</h2>
                    {/* KNAPPAR FÖR FILTRERING */}
                    <div className="filter-buttons">
                        <button
                            onClick={() => setSelectedResourceType(null)}
                            className={selectedResourceType === null ? 'active' : ''}
                        >
                            Visa alla
                        </button>
                        {resourceTypes.map((type) => (
                            <button
                                key={type.resourceTypeId}
                                onClick={() => setSelectedResourceType(type.resourceTypeId)}
                                className={selectedResourceType === type.resourceTypeId ? 'active' : ''}
                            >
                                {type.name}
                            </button>
                        ))}
                    </div>
                    <ul id="resource-ul">
                        {filteredResources.map((resource) => (
                            <li key={resource.resourceId} className="resource-item">
                                <span className="resource-details">
                                    <strong>ID:</strong> {resource.resourceId} | <strong>Namn:</strong> {resource.name} |{" "}
                                    <strong>Resurstyp:</strong> {resource.resourceTypeName}
                                </span>
                                <div className="resource-actions">
                                    <button
                                        className="edit-resource-btn"
                                        onClick={() => setEditResource({
                                            id: resource.resourceId.toString(),
                                            resourceTypeId: resource.resourceTypeId.toString(),
                                            name: resource.name
                                        })}
                                    >
                                        ✏️
                                    </button>
                                    <button
                                        className="delete-resource-btn"
                                        onClick={() => deleteResource(resource.resourceId)}
                                    >
                                        Radera
                                    </button>
                                </div>
                            </li>
                        ))}
                    </ul>
                </div>
            )}

            {/* NY KNAPP FÖR ATT VISA FORMULÄR FÖR ATT LÄGGA TILL RESURS */}
            <button className="GettingStartedButton" onClick={() => setShowAddResourceForm(!showAddResourceForm)}>
                {showAddResourceForm ? "Dölj formulär" : "Lägg till ny resurs"}
            </button>

            {/* VISA FORMULÄRET FÖR ATT LÄGGA TILL RESURS */}
            {showAddResourceForm && (
                <div id="create-resource-form">
                    <h2>Skapa ny resurs</h2>
                    <select
                        value={newResource.resourceTypeId}
                        onChange={(e) => setNewResource({ ...newResource, resourceTypeId: e.target.value })}
                        id="new-resource-type-id"
                    >
                        <option value="">Välj resurstyp</option>
                        {resourceTypes.map((type) => (
                            <option key={type.resourceTypeId} value={type.resourceTypeId}>
                                {type.name}
                            </option>
                        ))}
                    </select>
                    <input
                        type="text"
                        placeholder="Namn på resurs"
                        value={newResource.name}
                        onChange={(e) => setNewResource({ ...newResource, name: e.target.value })}
                        id="new-resource-name"
                    />
                    <button onClick={createResource} id="create-resource-btn">
                        Lägg till resurs
                    </button>
                </div>
            )}

            {/* KNAPP FÖR ATT VISA FORMULÄR FÖR ATT SKICKA NOTIS */}
            <button className="GettingStartedButton" onClick={() => setShowNotificationForm(!showNotificationForm)}>
                {showNotificationForm ? "Dölj notisformulär" : "Skicka notis"}
            </button>

            {/* FORMULÄR FÖR ATT REDIGERA RESURS */}
            {editResource.id && (
                <div className="modal">
                    <div className="modal-content">
                        <h2>Redigera resurs</h2>
                        <select
                            value={editResource.resourceTypeId}
                            onChange={(e) => setEditResource({ ...editResource, resourceTypeId: e.target.value })}
                        >
                            {resourceTypes.map((type) => (
                                <option key={type.resourceTypeId} value={type.resourceTypeId}>
                                    {type.name}
                                </option>
                            ))}
                        </select>
                        <input
                            type="text"
                            value={editResource.name}
                            onChange={(e) => setEditResource({ ...editResource, name: e.target.value })}
                        />
                        <button onClick={submitEditResource}>Spara ändringar</button>
                        <button onClick={() => setEditResource({ id: "", resourceTypeId: "", name: "" })}>Avbryt</button>
                    </div>
                </div>
            )}

            {/* FORMULÄR FÖR ATT SKICKA NOTIS */}
            {showNotificationForm && (
            <section className="notification-panel">
                <h2>Skicka notis</h2>
                <div className="notification-form">
                    <input
                        type="text"
                        placeholder="Titel"
                        value={ntTitle}
                        onChange={(e) => setNtTitle(e.target.value)}
                    />
                    <textarea
                        placeholder="Meddelande"
                        rows={3}
                        value={ntMessage}
                        onChange={(e) => setNtMessage(e.target.value)}
                    />
                    <input
                        type="text"
                        placeholder="Användar-ID (lämna tomt för alla)"
                        value={ntUserId}
                        onChange={(e) => setNtUserId(e.target.value)}
                    />
                    <button className="GettingStartedButton" onClick={() => sendNotification()} disabled={ntLoading}>
                        {ntLoading ? "Skickar..." : "Skicka notis"}
                    </button>
                    {ntStatus && <p className="status-message">{ntStatus}</p>}
                </div>
            </section>
            )}
        
            {/* NY KNAPP FÖR ATT VISA FORMULÄR FÖR ATT LÄGGA TILL RESURS */}
            {/* Behövs implementeras i front/backend */}
            <button className="GettingStartedButton" >
                 Lägg till ny resurstyp (kommer snart)
            </button>
        </div>
    );
}

export default admin;