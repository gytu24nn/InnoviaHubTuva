import {useState, useEffect} from 'react'
// import { IMyBooking } from "./TempInterface";
import "../../ErrorAndLoading.css"
import "./MyBookings.css"

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
      const response = await fetch(`${apiBase}/mybookings`, {
        method: 'GET',
        credentials: 'include',
      });
      
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
    <div className='myBookingsContainer'>
      <h2>Mina bokningar</h2>

      <div className="errorAndLoadingMessage-container">
        {loading && <p className='loading-message'>Laddar bokningar...</p>}
        {error && <p className='error-message'>{error}</p>}

        {!loading && !error && bookings.length === 0 && (
          <p className="my-bookings-empty">Du har inga bokningar ännu</p>
        )}
      </div>
      

      

      {!loading && bookings.length > 0 && (
        <ul className='myBookingsList'>
          {bookings.map((b) => (
            <li key={b.bookingId}>
              <span className='bookingResource'>{b.resourceName}</span>
              <span className='bookingDatetime'>
                {new Date(b.date).toLocaleDateString("sv-SE")} ({b.startTime} - {b.endTime})
              </span>
            </li>
          ))}
        </ul>
      )}

    </div>
  )
}

export default MyBookings