import { useBookingContext } from "../../Context/BookingContext";
import { useNavigate } from "react-router-dom";
import { Calendar, momentLocalizer } from "react-big-calendar";
import moment from "moment";
import "react-big-calendar/lib/css/react-big-calendar.css";
import {useSearchParams} from "react-router-dom";
import { useEffect, useState } from "react";
import "./booking.css"
import "../../ErrorAndLoading.css"
import OfficeLayout from "../../Components/OfficeLayout";
import { set } from "date-fns";
import SmartTips from "../../Components/SmartTips";
import "../../Components/SmartTips.css";
import { BASE_URL } from "../../config";

const localizer = momentLocalizer(moment);

const Booking = () => {
  const {
    date,
    setDate,
    timeSlotId,
    setTimeSlotId,
    timeSlots,
    bookings,
    loading,
    error,
    resource,
    resourceId,
    setResourceId
  } = useBookingContext();

  const navigate = useNavigate();

  const [searchParams] = useSearchParams();
  const resourceTypeIdParam = searchParams.get("resourceTypeId");
  const resourceTypeId = resourceTypeIdParam ? Number(resourceTypeIdParam) : null;
  const resourceName = searchParams.get("resourceTypeName");
  const ResourceTypeName = searchParams.get("resourceName");
  const [showMap, setShowMap] = useState(false);
  const [selectedDate, setSelectedDate] = useState<Date | null>(null);
  const [resourceIdLocal, setResourceIdLocal] = useState<number | null>(null);

  const apiBase = `${BASE_URL}/api/booking`

  useEffect(() => {
    if (resource.length > 0 && resourceIdLocal === null) {
      // Sätt resourceIdLocal till första resursen i listan
      setResourceIdLocal(resource[0].resourcesId);
    }
  }, [resource, resourceIdLocal]);

  
    if (loading) {
    return <div className="loading-message">Laddar...</div>;
  }

  if (error) {
    return <div className="error-message">Ett fel uppstod: {error}</div>;
  }

  // Handle calendar click
  const handleDateSelect = (slotInfo: { start: Date }) => {
    // Förhindra klick på förflutna datum
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    const clickedDate = new Date(slotInfo.start);
    clickedDate.setHours(0, 0, 0, 0);

    if (clickedDate < today) {
      return; // förflutna datum blockeras
    }

    setDate(clickedDate);
    setSelectedDate(clickedDate) // spara valt datum
    setTimeSlotId(null); // nollställ vald tid
  };

  function parseTime(timeStr: string, date: Date): Date {
    const [hours, minutes] = timeStr.split(":").map(Number);
    return new Date(date.getFullYear(), date.getMonth(), date.getDate(), hours, minutes)
  }

  const slotsForDay = date && resourceIdLocal
    ? timeSlots.map((slots) => {
      const isBooked = bookings.some((b) => {
        if(b.resourceId !== resourceIdLocal || new Date(b.date).toDateString() !== date.toDateString()) {
          return false;
        }

        const bookingTimeSlot = timeSlots.find(ts => ts.timeSlotsId === b.timeSlotId);

        if(!bookingTimeSlot || !bookingTimeSlot.startTime || !bookingTimeSlot.endTime) {
          console.error("Ogiltig bokningsdata (saknar tid):", b);
          return false; // Hoppa över denna ogiltiga bokning
        }

        const slotStart = parseTime(slots.startTime, date);
        const slotEnd = parseTime(slots.endTime, date);
        const bookingStart = parseTime(bookingTimeSlot.startTime, date);
        const bookingEnd = parseTime(bookingTimeSlot.endTime, date);

        return slotStart < bookingEnd && slotEnd > bookingStart;

      })

    return{...slots, isBooked}
  })
  : [];

  const availableRescources = date 
    ? resource.filter(
      (r) => 
        (!resourceName || r.type === resourceName ) &&

        !bookings.some(
          (b) => 
            new Date(b.date).toLocaleDateString() === date.toLocaleDateString() &&
            b.timeSlotId === timeSlotId &&
            b.resourceId === r.resourcesId
        )
    )
    : resource;

    const selectedResource = resource.find((r) => r.resourcesId === resourceId);
    const selecedType = selectedResource?.type;

  // Group slots by duration
  const groupedSlots = {
    "2h": slotsForDay.filter((s) => s.duration === 2),
    "4h": slotsForDay.filter((s) => s.duration === 4),
    "8h": slotsForDay.filter((s) => s.duration === 8),
  };

  const freeSlotsCount = slotsForDay.filter(s => !s.isBooked).length;
  const isFullyBooked = freeSlotsCount === 0;

  // Confirm booking flow
const handleConfirm = async () => {
  if (!selectedDate || !timeSlotId || !resourceIdLocal) {
    alert("Välj ett datum, en tid och resurs innan du fortsätter.");
    return;
  }

  // Skapa boknings-objekt
  const utcMidnight = new Date(Date.UTC(selectedDate.getFullYear(), selectedDate.getMonth(), selectedDate.getDate()));
  const bookingData = {
    date: utcMidnight.toISOString(),
    resourceId: resourceIdLocal,
    timeSlotId: timeSlotId,
    // Lägg till fler fält om det behövs, t.ex. userId
  };

  console.log("Booking data:", {
  date: utcMidnight.toISOString(),
  resourceId,
  timeSlotId,
});

console.log("Available resources:", availableRescources);


  try {
    const response = await fetch(`${apiBase}`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(bookingData),
      credentials: "include", // om du använder cookies/autentisering
    });

    if (!response.ok) {
      throw new Error("Kunde inte boka tiden.");
    }

    setTimeSlotId(null);
    setResourceIdLocal(null);

    const result = await response.json();
    // Navigera till bekräftelsesida med boknings-id
    navigate(`/BookingConfirmed/${result.bookingId}`);
  } catch (err) {
    alert("Fel vid bokning:");
  }
};

  // Funktion för att hantera stil för varje dag i kalendern
  const dayPropGetter = (day: Date) => {
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    // Gråa ut förflutna datum
    if (day.getTime() < today.getTime()) {
      return {
        className: 'rbc-past-day',
      };
    }
    
    // Markera det valda datumet
    if (date && day.toDateString() === date.toDateString()) {
      return {
        className: 'rbc-selected-day',
      };
    }

    return {};
  };

  // Hitta den nu valda resursen baserat på ID:t i local state
const currentSelectedResource = resource.find(r => r.resourcesId === resourceIdLocal);
// Använd resursens namn, eller en fallback om ingen resurs är vald/hittad
const currentResourceName = currentSelectedResource ? currentSelectedResource.name : 'den valda resursen';

  return (
    <div className="booking-container">
      <h1 className="booking-title">Boka en tid för {resourceName}</h1>

      {/* Calendar */}
      <div className="booking-calender">
        <Calendar
          localizer={localizer}
          selectable
          onSelectSlot={handleDateSelect}
          date={date || new Date()} 
          onNavigate={(newDate) => {
            setDate(newDate); 
            setTimeSlotId(null);
          }}
          views={['month']} 
          style={{ height: 500 }}
          dayPropGetter={dayPropGetter}
          />
      </div>

      {date && !isFullyBooked && (
        <SmartTips 
          dateTips={selectedDate ? new Date(Date.UTC(selectedDate.getFullYear(), selectedDate.getMonth(), selectedDate.getDate())) : undefined}
          resourceTypeTips={ResourceTypeName}
          resourceIdTips={null} // AI ska inte få enskild resurs
        />
      )}

      {date && isFullyBooked && (
        <div className="SmartTipsContainer">
          <div className="AI-Icon-container">
            <div className="AI-icon">
              <i className={`fa-solid fa-robot`}></i>
            </div>
          </div>
            


            <div className="TextContent-smartTips">
              <h3>SmartTips</h3>
              <p className="tip-smartTips">Tyvärr, alla tider är fullbokade för {currentResourceName} just idag. Prova en annan resurs i rullistan eller byt dag 🙈</p>
            </div>
        
        </div>
      )}

      

          {/*välj en resurs*/}
            <label>Välj resurs:</label>
            <div>
              <select 
                value={resourceIdLocal ?? ""}
                onChange={(e) => setResourceIdLocal(Number(e.target.value))}
                className="booking-resource-select"
              >

                <option value="" disabled>
                  -- Välj en resurs -- 
                </option>

                {availableRescources.map((r) => (
                  <option key={r.resourcesId} value={r.resourcesId}>
                    {r.name}
                  </option>
                ))}
              </select>
            </div>

          
        <OfficeLayout/>

      {/* Slots list */}
      {date && (
        <div>
          <h2 className="booking-slots-label">
            Tillgängliga tider för {date.toDateString()}
          </h2>

          {Object.entries(groupedSlots).map(([label, slots]) => (
            <div key={label} className="booking-slots-group">
              <h3 className="booking-slots-label">{label}</h3>
              <div>
                {slots.length === 0 ? (
                  <p className="booking-message">Inga tider</p>
                ) : (
                  slots.map((slot) => (
                    <button
                      key={slot.timeSlotsId}
                      onClick={() => !slot.isBooked && setTimeSlotId(slot.timeSlotsId)}
                      disabled={slot.isBooked}
                      className={`booking-slot-btn${timeSlotId === slot.timeSlotsId ? " selected" : ""} ${slot.isBooked ? " booked" : ""}`}
                    >
                      {slot.startTime} - {slot.endTime}
                      {slot.isBooked && " Bokad"}
                    </button>
                  ))
                )}
              </div>
            </div>
          ))}

          {/* Confirm button */}
          <div className="booking-confirm-container">
            <button
              onClick={handleConfirm}
              disabled={!timeSlotId}
              className="booking-confirm-btn"
            >
              Boka
            </button>
          </div>
          </div>
      )}
    </div>
  );
};

export default Booking;