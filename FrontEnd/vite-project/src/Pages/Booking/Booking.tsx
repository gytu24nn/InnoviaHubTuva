import { useBookingContext } from "../../Context/BookingContext";
import { useNavigate } from "react-router-dom";
import { Calendar, momentLocalizer } from "react-big-calendar";
import moment from "moment";
import "react-big-calendar/lib/css/react-big-calendar.css";

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

  // Handle calendar click
  const handleDateSelect = (slotInfo: { start: Date }) => {
    setDate(slotInfo.start);
    setTimeSlotId(null); // reset timeslot when new date is picked
  };

  // Filter available slots by selected date
  const availableSlots = date
    ? timeSlots.filter(
        (slot) =>
          !bookings.some(
            (b) =>
              new Date(b.date).toDateString() === date.toDateString() &&
              b.timeSlotId === slot.timeSlotsId
          )
      )
    : [];

  // Group slots by duration
  const groupedSlots = {
    "2h": availableSlots.filter((s) => s.duration === 120),
    "4h": availableSlots.filter((s) => s.duration === 240),
    "8h": availableSlots.filter((s) => s.duration === 480),
  };

  // Confirm booking flow
  const handleConfirm = () => {
    if (!date || !timeSlotId) {
      alert("Välj ett datum och en tid innan du fortsätter.");
      return;
    }
    navigate("/BookingConfirmed");
  };

  if (loading) {
    return <div className="p-4 text-center">⏳ Laddar tider...</div>;
  }

  if (error) {
    return <div className="p-4 text-red-500">Fel: {error}</div>;
  }

  return (
    <div className="p-6 max-w-3xl mx-auto">
      <h1 className="text-2xl font-bold mb-4">Boka en tid</h1>

      {/* Calendar */}
      <div className="mb-6">
        <Calendar
          localizer={localizer}
          selectable
          onSelectSlot={handleDateSelect}
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