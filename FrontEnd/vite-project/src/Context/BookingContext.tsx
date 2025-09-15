import { createContext, useContext, useState, useEffect } from "react";
import type { ReactNode } from "react";
import * as signalR from "@microsoft/signalr";

// Booking from backend (BookingDTO)
export interface Booking {
  bookingId: number;
  date: string; // ISO date string
  userId: string;
  resourceId: number;
  resourceName: string;
  timeSlotId: number;
  startTime: string;
  endTime: string;
  userEmail?: string;
}

// TimeSlot from backend (TimeSlotDTO)
export interface TimeSlot {
  timeSlotsId: number;
  startTime: string; // "HH:mm"
  endTime: string;   // "HH:mm"
  duration: number;
}

interface BookingContextType {
  resourceId: number | null;
  setResourceId: (id: number | null) => void;
  date: Date | null;
  setDate: (date: Date | null) => void;
  timeSlotId: number | null;
  setTimeSlotId: (id: number | null) => void;

  bookings: Booking[];
  timeSlots: TimeSlot[];
  loading: boolean;
  error: string | null;
}

const BookingContext = createContext<BookingContextType | undefined>(undefined);

export const BookingProvider = ({ children }: { children: ReactNode }) => {
  const [resourceId, setResourceId] = useState<number | null>(null);
  const [date, setDate] = useState<Date | null>(null);
  const [timeSlotId, setTimeSlotId] = useState<number | null>(null);

  const [bookings, setBookings] = useState<Booking[]>([]);
  const [timeSlots, setTimeSlots] = useState<TimeSlot[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

useEffect(() => {
  const fetchData = async () => {
    try {
      // 🔹 1. Fetch MyBookings (temporarily without token)
      const bookingsRes = await fetch("http://localhost:5099/api/user/public-bookings");
      if (!bookingsRes.ok) {
        throw new Error(`Failed to fetch bookings: ${bookingsRes.status}`);
      }
      const bookingsData: Booking[] = await bookingsRes.json();
      setBookings(bookingsData);

      // 🔹 2. Fetch TimeSlots
      const timeSlotsRes = await fetch("http://localhost:5099/api/user/timeslots");
      if (!timeSlotsRes.ok) {
        throw new Error(`Failed to fetch timeslots: ${timeSlotsRes.status}`);
      }
      const timeSlotsData: TimeSlot[] = await timeSlotsRes.json();
      setTimeSlots(timeSlotsData);

      setError(null);
    } catch (err) {
      console.error("Error fetching bookings or timeslots", err);
      setError(err instanceof Error ? err.message : "Unexpected error");
    } finally {
      setLoading(false);
    }
  };

  fetchData();

  // 🔴 Real-time updates with SignalR
  const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5099/bookinghub")
    .withAutomaticReconnect()
    .build();

  connection.on("BookingCreated", (booking: Booking) => {
    setBookings((prev) => [...prev, booking]);
  });

  connection.on("BookingCancelled", (booking: Booking) => {
    setBookings((prev) => prev.filter((b) => b.bookingId !== booking.bookingId));
  });

  connection.start().catch((err) => console.error("SignalR error:", err));

  return () => {
    connection.stop();
  };
}, []);

  return (
    <BookingContext.Provider
      value={{
        resourceId,
        setResourceId,
        date,
        setDate,
        timeSlotId,
        setTimeSlotId,
        bookings,
        timeSlots,
        loading,
        error,
      }}
    >
      {children}
    </BookingContext.Provider>
  );
};

export const useBookingContext = () => {
  const context = useContext(BookingContext);
  if (!context) throw new Error("useBookingContext must be used inside BookingProvider");
  return context;
};