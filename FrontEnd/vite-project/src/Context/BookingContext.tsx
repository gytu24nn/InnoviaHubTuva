import { createContext, useContext, useState, useEffect } from "react";
import type { ReactNode } from "react";

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
  endTime: string; // "HH:mm"
  duration: number;
}

interface BookingContextType {
  // Selection state (for booking flow)
  resourceId: number | null;
  setResourceId: (id: number | null) => void;
  date: Date | null;
  setDate: (date: Date | null) => void;
  timeSlotId: number | null;
  setTimeSlotId: (id: number | null) => void;

  // Data state (fetched from backend)
  bookings: Booking[];
  setBookings: React.Dispatch<React.SetStateAction<Booking[]>>;
  timeSlots: TimeSlot[];
  setTimeSlots: React.Dispatch<React.SetStateAction<TimeSlot[]>>;
}

// Context
const BookingContext = createContext<BookingContextType | undefined>(undefined);

export const BookingProvider = ({ children }: { children: ReactNode }) => {
  // Selection
  const [resourceId, setResourceId] = useState<number | null>(null);
  const [date, setDate] = useState<Date | null>(null);
  const [timeSlotId, setTimeSlotId] = useState<number | null>(null);

  // Data
  const [bookings, setBookings] = useState<Booking[]>([]);
  const [timeSlots, setTimeSlots] = useState<TimeSlot[]>([]);

  // Fetch from backend when component mounts
  useEffect(() => {
    const fetchData = async () => {
      try {
        // Fetch user's own bookings
        const bookingsRes = await fetch("/api/Booking/mybookings", {
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`, // adjust if your token is stored differently
          },
        });
        if (bookingsRes.ok) {
          const data: Booking[] = await bookingsRes.json();
          setBookings(data);
        }

        // Fetch available time slots
        const slotsRes = await fetch("/api/Admin/timeslots", {
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`, // remove if endpoint is public
          },
        });
        if (slotsRes.ok) {
          const data: TimeSlot[] = await slotsRes.json();
          setTimeSlots(data);
        }
      } catch (error) {
        console.error(" Failed to fetch booking data:", error);
      }
    };

    fetchData();
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
        setBookings,
        timeSlots,
        setTimeSlots,
      }}
    >
      {children}
    </BookingContext.Provider>
  );
};

// Custom Hook
export const useBookingContext = () => {
  const context = useContext(BookingContext);
  if (!context) {
    throw new Error("useBookingContext must be used inside BookingProvider");
  }
  return context;
};