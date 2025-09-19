import { useParams, useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { CheckCircle } from "lucide-react"; // ✅ green checkmark icon
import "./BookingConfirmed.css";
// import { useUser } from "../../Context/UserContext";

interface BookingDetails {
  bookingId: number;
  resourceName: string;
  startTime: string;
  endTime: string;
  date: string;
  userName?: string;
}

const BookingConfirmed = () => {
  const { bookingId } = useParams<{ bookingId: string }>();
  const navigate = useNavigate();
  const [booking, setBooking] = useState<BookingDetails | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchBooking = async () => {
      try {
        const res = await fetch(`http://localhost:5099/api/booking/${bookingId}`);
        if (!res.ok) throw new Error("Kunde inte hämta bokningsinformationen.");
        const data: BookingDetails = await res.json();
        setBooking(data);
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    };
    fetchBooking();
  }, [bookingId]);

  if (loading) {
    return <div className="PageLayout">⏳ Laddar bokningsinformation...</div>;
  }

  if (!booking) {
    return <div className="PageLayout">❌ Kunde inte hitta bokningen.</div>;
  }

  return (
    <div className="PageLayout">
      <div className="ConfirmedContainer">
        {/* ✅ Green checkmark */}
        <CheckCircle className="ConfirmedCheck" />

        {/* Headline */}
        <h1 id="ConfirmedHeadline">Bokningsbekräftelse</h1>

        {/* Booking details card */}
        <div className="ConfirmedDetails">
          <p>
            <span className="ConfirmedLabel">Resurs:</span> {booking.resourceName}
          </p>
          <p>
            <span className="ConfirmedLabel">Datum:</span>{" "}
            {new Date(booking.date).toLocaleDateString("sv-SE")}
          </p>
          <p>
            <span className="ConfirmedLabel">Tid:</span>{" "}
            {booking.startTime} - {booking.endTime}
          </p>
          <p>
            <span className="ConfirmedLabel">Bokad av:</span>{" "}
            {booking.userName ?? "Okänd användare"}
          </p>

          <p className="ConfirmedMessage">
            Tack för din bokning! Du kan nu se den i mina bokningar och där kan du avboka om du får förhinder.
          </p>
        </div>

        {/* Back button */}
        <button className="ConfirmedButton" onClick={() => navigate("/Home")}>
          Tillbaka
        </button>
      </div>
    </div>
  );
};

export default BookingConfirmed;