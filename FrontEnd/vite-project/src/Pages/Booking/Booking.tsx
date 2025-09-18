import { useBookingContext } from "../../Context/BookingContext";
import { useNavigate } from "react-router-dom";
import { Calendar, momentLocalizer } from "react-big-calendar";
import moment from "moment";
import "react-big-calendar/lib/css/react-big-calendar.css";
import {useSearchParams} from "react-router-dom";
import KontorsLayout from "../../../public/img/Kontorslayout.png"
import { useEffect, useState } from "react";
import "./booking.css"
import "../../ErrorAndLoading.css"

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

  const resourceColors: Record<string, string> = {
    "Mötesrum": "#B5E7AE",
    "Skrivbord": "#FFEE99",
    "VR": "#A6CBF5",
    "AI": "#D6A8F2"
  }

  const navigate = useNavigate();

  const [searchParams] = useSearchParams();
  const resourceTypeIdParam = searchParams.get("resourceTypeId");
  const resourceTypeId = resourceTypeIdParam ? Number(resourceTypeIdParam) : null;
  const resourceName = searchParams.get("resourceName");
  const [showMap, setShowMap] = useState(false);
  
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

    setDate(clickedDate); // spara valt datum
    setTimeSlotId(null); // nollställ vald tid
  };

  function parseTime(timeStr: string, date: Date): Date {
    const [hours, minutes] = timeStr.split(":").map(Number);
    return new Date(date.getFullYear(), date.getMonth(), date.getDate(), hours, minutes)
  }

  const slotsForDay = date && resourceId 
    ? timeSlots.map((slots) => {
      const isBooked = bookings.some((b) => {
        if(b.resourceId !== resourceId || new Date(b.date).toDateString() !== date.toDateString()) {
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

  // Confirm booking flow
const handleConfirm = async () => {
  if (!date || !timeSlotId || !resourceId) {
    alert("Välj ett datum, en tid och resurs innan du fortsätter.");
    return;
  }

  // Skapa boknings-objekt
  const bookingData = {
    date: date.toISOString(),
    resourceId: Number(resourceId),
    timeSlotId: timeSlotId,
    // Lägg till fler fält om det behövs, t.ex. userId
  };

  try {
    const response = await fetch("http://localhost:5099/api/booking", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(bookingData),
      credentials: "include", // om du använder cookies/autentisering
    });

    if (!response.ok) {
      throw new Error("Kunde inte boka tiden.");
    }

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

  if (loading) {
    return <div className="loading-message">⏳ Laddar tider...</div>;
  }

  if (error) {
    return <div className="error-message">Fel: {error}</div>;
  }

  useEffect(() => {
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  setDate(today);
  setTimeSlotId(null);
  }, []);

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

          {/*välj en resurs*/}
            <label>Välj resurs:</label>
            <div>
              <select 
                value={resourceId ?? ""}
                onChange={(e) => setResourceId(Number(e.target.value))}
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
              {/*Visa färgprick för vald resurs*/}
              {resourceId && (
                <span
                  className="resource-color-dot"
                  style={{
                    backgroundColor: selecedType ? resourceColors[selecedType] : "#ccc"
                  }} 
              /> 
              )}
            </div>

          <button className="booking-map-btn" onClick={() => setShowMap(true)}>
            Visa kontorslayout
          </button>

          {/*Karta över kontorslayout*/}
          {showMap && (
            <div className="booking-map-modal-overlay" onClick={() => setShowMap(false)}>
              <div className="booking-map-modal" onClick={e => e.stopPropagation()}>
                <button className="booking-close-map-btn" onClick={() => setShowMap(false)}>stäng</button>
                <img src={KontorsLayout} alt="Kontorslayout" className="booking-office-map-img" />
              </div>              
            </div>
          )}

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