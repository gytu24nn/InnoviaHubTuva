import { useEffect, useState } from "react";
import { Calendar, dateFnsLocalizer } from "react-big-calendar";
import "react-big-calendar/lib/css/react-big-calendar.css";
import { parse, startOfWeek, getDay, format } from "date-fns";
import { enUS } from "date-fns/locale";
import { useBookingContext } from "../../Context/BookingContext";

const locales = {
  "en-US": enUS,
};

const localizer = dateFnsLocalizer({
  format,
  parse,
  startOfWeek: () => startOfWeek(new Date(), { weekStartsOn: 1 }),
  getDay,
  locales,
});

interface Event {
  title: string;
  start: Date;
  end: Date;
}

const Booking = () => {
  const { bookings } = useBookingContext();
  const [events, setEvents] = useState<Event[]>([]);

  useEffect(() => {
    // Convert bookings from context into calendar events
    const mapped = bookings.map((b) => ({
      title: `${b.resourceName} (${b.startTime}-${b.endTime})`,
      start: new Date(b.date),
      end: new Date(b.date),
    }));
    setEvents(mapped);
  }, [bookings]);

  return (
    <div style={{ height: "80vh", padding: "20px" }}>
      <h2>Booking Calendar</h2>
      <Calendar
        localizer={localizer}
        events={events}
        startAccessor="start"
        endAccessor="end"
        style={{ height: 600 }}
      />
    </div>
  );
};

export default Booking;