import {useState, useEffect} from 'react'
// import { IMyBooking } from "./TempInterface";
import "../../ErrorAndLoading.css"
import "./MyBookings.css"
import OfficeLayout from '../../Components/OfficeLayout'
import "../Booking/booking.css"
import { BASE_URL } from '../../config'

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
  const apiBase = `${BASE_URL}/api/Booking`;;
  const [bookings, setBookings] = useState<Booking[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [showMap, setShowMap] = useState(false);

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

  // avboka en bokning
  const cancelBooking = async (bookingId: number) => {
    if (!window.confirm("Är du säker på att du vill avboka denna bokning?")) {
      return;
    }

    try {
      const response = await fetch(`${apiBase}/${bookingId}`, {
        method: 'DELETE',
        credentials: 'include',
      });

      if (!response.ok) {
        throw new Error("Något gick fel vid avbokning.");
      }

      // ta bort bokningen ur state så UI uppdateras direkt
      setBookings(prev => prev.filter(b => b.bookingId !== bookingId));

    } catch (err) {
      alert((err as Error).message);
    }
  };

  const getBookingClass = (resourceName: string) => {
    const nameLower = resourceName.toLocaleLowerCase();

    if (nameLower.includes('skrivbord')) {
        return 'booking-skrivbord';
    }
    if (nameLower.includes('mötesrum')) {
        return 'booking-motesrum';
    }
    if (nameLower.includes('vr')) {
        return 'booking-vr';
    }
    if (nameLower.includes('ai')) {
        return 'booking-ai';
    }
    return '';
  }
 
  return (
    <div className='myBookingsContainer'>
      <h2>Mina bokningar</h2>
      <div className='OfficeLayout'>
        <OfficeLayout/>
      </div>
      

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
            <li key={b.bookingId} className={`booking-item ${getBookingClass(b.resourceName)}`}>
              <span className='bookingResource'>{b.resourceName}</span>
              <span className='bookingDatetime'>
                {new Date(b.date).toLocaleDateString("sv-SE")} ({b.startTime} - {b.endTime})
              </span>

              <button
              className='cancelBtn'
              onClick={() => cancelBooking(b.bookingId)}
              >
                Avboka
              </button>
            </li>
          ))}
        </ul>
      )}

    </div>
  )
}

export default MyBookings