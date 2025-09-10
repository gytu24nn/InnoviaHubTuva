import {useState, useEffect} from 'react'
// import { IMyBooking } from "./TempInterface";

type Booking = {
  bookingId: number;
  date: string;
  userId: string;
  resourceId: number;
  resourceName: string;
  timeSlotId: number;
  startTime: string;
  endTime: string;
};

const MyBookings = () => {
  const apiBase = "http://localhost:5099/api/Booking";
  const [bookings, setBookings] = useState<Booking[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  //fetch my bookings
  const fetchMyBookings = async () => {
    try {
      const response = await fetch(`${apiBase}/mybookings`);
      if (!response.ok) {
        throw new Error("Något gick fel vid hämtning av bokningar.");
      }
      const data = await response.json();
      setBookings(data);

    } catch (err) {
      setError((err as Error).message);

    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchMyBookings();
  }, [])

  return (
    <div>
      <h2>Mina bokningar</h2>

      {loading && <p>Laddar bokningar...</p>}
      {error && <p>{error}</p>}

      {!loading && !error && bookings.length === 0 && (
        <p>Du har inga bokningar ännu</p>
      )}

      {!loading && bookings.length > 0 && (
        <ul>
          {bookings.map((b) => (
            <li key={b.bookingId}>
              {b.resourceName} – {new Date(b.date).toLocaleDateString("sv-SE")}{" "}
              ({b.startTime} - {b.endTime})
            </li>
          ))}
        </ul>
      )}

    </div>
  )
}

export default MyBookings