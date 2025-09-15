import { useBookingContext } from "../../Context/BookingContext";
import { useNavigate } from "react-router-dom";
import { Calendar, momentLocalizer } from "react-big-calendar";
import moment from "moment";
import "react-big-calendar/lib/css/react-big-calendar.css";
import {useSearchParams} from "react-router-dom";

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
  } = useBookingContext();

  const navigate = useNavigate();

  const [searchParams] = useSearchParams();
  const resourceId = searchParams.get("resourceId");
  const resourceName = searchParams.get("resourceName");

  // Handle calendar click
  const handleDateSelect = (slotInfo: { start: Date }) => {
    console.log("SlotInfo från kalender:", slotInfo);
    setDate(slotInfo.start);
    setTimeSlotId(null); // reset timeslot when new date is picked
  };

  // Filter available slots by selected date
  const availableSlots = date
    ? timeSlots.filter(
        (slot) =>
          !bookings.some(
            (b) =>
              new Date(b.date).toLocaleDateString() === date.toLocaleDateString() &&
              b.timeSlotId === slot.timeSlotsId &&
              String(b.resourceId) === String(resourceId)
          )
      )
    : [];

    console.log("Available slots:", availableSlots);

  // Group slots by duration
  const groupedSlots = {
    "2h": availableSlots.filter((s) => s.duration === 2),
    "4h": availableSlots.filter((s) => s.duration === 4),
    "8h": availableSlots.filter((s) => s.duration === 8),
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

  if (loading) {
    return <div className="p-4 text-center">⏳ Laddar tider...</div>;
  }

  if (error) {
    return <div className="p-4 text-red-500">Fel: {error}</div>;
  }

  return (
    <div className="p-6 max-w-3xl mx-auto">
      <h1 className="text-2xl font-bold mb-4">Boka en tid för {resourceName}</h1>

      {/* Calendar */}
      <div className="mb-6">
        <Calendar
          localizer={localizer}
          selectable 
          onSelectSlot={handleDateSelect}
            onDrillDown={date => {
            setDate(date);
            setTimeSlotId(null);
            console.log("DrillDown date:", date);
          }}
          defaultDate={new Date()}   // show today by default
          views={["month"]}
          style={{ height: 500 }}
          />
      </div>

      {/* Slots list */}
      {date && (
        <div className="mt-4">
          <h2 className="text-lg font-semibold mb-2">
            Tillgängliga tider för {date.toDateString()}
          </h2>

          {Object.entries(groupedSlots).map(([label, slots]) => (
            <div key={label} className="mb-4">
              <h3 className="font-medium mb-1">{label}</h3>
              <div className="flex flex-wrap gap-2">
                {slots.length === 0 ? (
                  <p className="text-sm text-gray-500">Inga tider</p>
                ) : (
                  slots.map((slot) => (
                    <button
                      key={slot.timeSlotsId}
                      onClick={() => setTimeSlotId(slot.timeSlotsId)}
                      className={`px-4 py-2 rounded-lg border transition ${
                        timeSlotId === slot.timeSlotsId
                          ? "bg-blue-600 text-white"
                          : "bg-gray-100 hover:bg-gray-200"
                      }`}
                    >
                      {slot.startTime} - {slot.endTime}
                    </button>
                  ))
                )}
              </div>
            </div>
          ))}

          {/* Confirm button */}
          <div className="mt-6">
            <button
              onClick={handleConfirm}
              disabled={!timeSlotId}
              className={`w-full py-3 rounded-lg text-white font-bold ${
                timeSlotId
                  ? "bg-green-600 hover:bg-green-700"
                  : "bg-gray-400 cursor-not-allowed"
              }`}
            >
              Klar (Boka)
            </button>
          </div>
        </div>
      )}
    </div>
  );
};

export default Booking;