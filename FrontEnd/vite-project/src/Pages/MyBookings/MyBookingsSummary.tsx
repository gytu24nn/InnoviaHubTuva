import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import "../Booking/Resursvy.css"

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

    const fetchMyBookings = async () => {
        try {
            const response = await fetch(`${bookingApi}/mybookings`, {
                headers: {
                    "Authorization": `Bearer ${localStorage.getItem("token")}` // om du använder token
                }
            });
            if (!response.ok) throw new Error("Något gick fel vid hämtning av bokningar.");
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

            {loading && <p>Laddar bokningar</p>}
            {error && <p>{error}</p>}

            {!loading && !error && (
                <>
                    <p>Du har {bookings.length} bokning{bookings.length !== 1 ? "ar" : ""}</p>
                    <Link to={"/MyBookings"}>
                    
                    </Link>
                </>
            )}
        </div>
    )
}

export default MyBookingsSummary;
