import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import "../Booking/Resursvy.css"
import "../../ErrorAndLoading.css"

type BookingType = {
    bookingId: number;
    resourceName: string;
    date: string;
    startTime: string;
    endTime: string;
}

const MyBookingsSummary = () => {
    const bookingApi = "http://localhost:5099/api/Booking";

    const [bookings, setBookings] = useState<BookingType[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

     //fetch my bookings
  const fetchMyBookings = async () => {
    try {
      const response = await fetch(`${bookingApi}/mybookings`, {
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
    }, []);

    return (
        <div className="BookingSummaryContainer">
            <h3><i className="fa-solid fa-calendar"></i>Mina bokningar</h3>
            
            <div className="errorAndLoadingMessage-container">
              {loading && <p className='loading-message'>Laddar bokningar...</p>}
              {error && <p className='error-message'>{error}</p>}

            </div>
            
            {!loading && !error && (
              <Link to={"/MyBookings"}>
                <p>Du har {bookings.length} bokning{bookings.length !== 1 ? "ar" : ""}</p>
              </Link>
                
            )}
        </div>
    )
}

export default MyBookingsSummary;
