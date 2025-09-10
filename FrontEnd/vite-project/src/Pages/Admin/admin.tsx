import { useEffect, useState } from "react";
import "./Admin.css"

const admin = () => {
    const [resources, setResources] = useState<Resource[]>([]);
    const [bookings, setBookings] = useState<Booking[]>([]);
    const [newResource, setNewResource] = useState<{ resourceTypeId: string; name: string }>({ resourceTypeId: "", name: ""});
    const [editResource, setEditResource] = useState({ id: "", resourceTypeId: "", name: "" });
    const [resourceTypes, setResourceTypes] = useState<ResourceType[]>([]);
    const [editingResourceId, setEditingResourceId] = useState<number | null>(null);

    // Typ för en tidslucka
    type TimeSlot = {
      timeSlotsId: number;
      startTime: string; 
      endTime: string;   
    };

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

    const apiBase = "http://localhost:5099/api/admin";

    // Hämta alla resurser
    const fetchResources = async () => {
      const response = await fetch(`${apiBase}/resources`);
      const data = await response.json();
      setResources(data);
    }

    // Hämta alla bokningar
    const fetchBookings = async () => {
      const response = await fetch(`${apiBase}/bookings`);
      const data = await response.json();
      setBookings(data);
    }

    const fetchResourceTypes = async () => {
    const response = await fetch(`${apiBase}/resourcetypes`);
    const data: ResourceType[] = await response.json();
    setResourceTypes(data);
};

    useEffect(() => {
      fetchBookings();
      fetchResources();
      fetchResourceTypes();
    }, []);

    // Skapa en ny resurs
    const createResource = async () => {

          const resourceData = {
        ...newResource,
        resourceTypeId: parseInt(newResource.resourceTypeId, 10),
    };

      await fetch(`${apiBase}/resources`, {
        method: "POST",
        headers:{ "Content-Type": "application/json" },
        body: JSON.stringify(resourceData),
      });

      setNewResource({ resourceTypeId: "", name: ""});
      fetchResources();
    };

  // Uppdatera resurs
    const submitEditResource = async () => {
      await fetch(`${apiBase}/resource/edit/${editResource.id}`, {
        method: "PATCH",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          resourceTypeId: editResource.resourceTypeId,
          name: editResource.name,
        }),
      });
      setEditResource({ id: "", resourceTypeId: "", name: "" });
      fetchResources();
    };

    const deleteResource = async (id: number) => {

      const userConfirmed = window.confirm("Är du säker på att du vill radera denna resurs? Detta går inte att ångra.");

      if (userConfirmed){
      await fetch(`${apiBase}/resource/${id}`, {
        method: "DELETE",
      });
      fetchResources();
      }
    };

return (
  <div id="admin-panel">
    <h1>Admin Panel</h1>

            {/* KNAPP FÖR ATT VISA BOKNINGAR */}
            <button className="GettingStartedButton" onClick={() => setShowBookings(!showBookings)}>
                {showBookings ? "Dölj bokningar" : "Se alla bokningar"}
            </button>

            {/* Visa listan om showBookings är true */}
            {showBookings && (
                <div id="bookings-list">
                    <h2>Alla bokningar</h2>
                    <ul id="booking-ul">
                        {bookings.map((booking) => (
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
                      <ul id="resource-ul">
                          {resources.map((resource) => (
                              <li key={resource.resourceId} className="resource-item">
                                  <span className="resource-details">
                                      <strong>ID:</strong> {resource.resourceId} | <strong>Namn:</strong> {resource.name} |{" "}
                                      <strong>Resurstyp:</strong> {resource.resourceTypeName}
                                  </span>
                                  <div className="resource-actions"> {/* Ny behållare för knapparna */}
                                      <button
                                          className="edit-resource-btn"
                                          onClick={() => setEditingResourceId(resource .resourceId)}
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
                        <option value="">Välj resurstyp</option> {/* Ett standardalternativ */}
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
  </div>
  );
}

export default admin