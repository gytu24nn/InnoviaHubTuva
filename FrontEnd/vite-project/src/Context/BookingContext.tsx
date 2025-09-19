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
  resourceTypeId: number;
}

// TimeSlot from backend (TimeSlotDTO)
export interface TimeSlot {
  timeSlotsId: number;
  startTime: string; // "HH:mm"
  endTime: string;   // "HH:mm"
  duration: number;
}

export interface Resource {
  resourcesId: number;
  name: string;
  resourceTypeId: number;
  type: string;
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
  resource: Resource[]
}

const BookingContext = createContext<BookingContextType | undefined>(undefined);

export const BookingProvider = ({ children }: { children: ReactNode }) => {
  const [resourceId, setResourceId] = useState<number | null>(null);
  const [date, setDate] = useState<Date | null>(null);
  const [timeSlotId, setTimeSlotId] = useState<number | null>(null);

  const [bookings, setBookings] = useState<Booking[]>([]);
  const [timeSlots, setTimeSlots] = useState<TimeSlot[]>([]);
  const [resource, setResource] = useState<Resource[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);

useEffect(() => {
  const fetchData = async () => {
    try {
      // 🔹 1. Fetch MyBookings (temporarily without token)
      const bookingsRes = await fetch("http://localhost:5099/api/user/public-bookings",{
        credentials: "include",
      });
      if (!bookingsRes.ok) {
        throw new Error(`Failed to fetch bookings: ${bookingsRes.status}`);
      }
      const bookingsData: Booking[] = await bookingsRes.json();
      setBookings(bookingsData);

      const resourcesRes = await fetch("http://localhost:5099/api/Resource/available", {
        credentials: "include",
      });
      if(!resourcesRes.ok) {
        throw new Error(`Failed to fetch resources: ${resourcesRes.status}`);
      }
      const resourcesData: Resource[] = await resourcesRes.json();
      setResource(resourcesData)

      // 🔹 2. Fetch TimeSlots
      const timeSlotsRes = await fetch("http://localhost:5099/api/user/timeslots", {
        credentials: "include",
      });
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
  let isMounted = true; 
  const Newconnection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5099/bookingHub", {withCredentials: true})
    .withAutomaticReconnect()
    .build();

  Newconnection.on("BookingCreated", (booking: Booking) => {
    if(isMounted) {
      setBookings((prev) => [...prev, booking]);
    }
  });

  Newconnection.on("BookingCancelled", (booking: Booking) => {
    if(isMounted) {
      setBookings((prev) => prev.filter((b) => b.bookingId !== booking.bookingId));
    }
  });

  Newconnection.start()
    .then(() => console.log("SignralR connected"))
    .catch((err) => console.error("SignalR error:", err));

  return () => {
    isMounted = false; 
    Newconnection.stop();
  };
}, []);

useEffect(() => {
  if(!date || !resourceId || !connection) return; 

  connection
    .invoke("SendBookingsForResource", resourceId, date)
    .catch(err => console.error("Error invoking sendBookingsForResource:", err));

  const handler = (updatedBookings: Booking[]) => {
    setBookings(updatedBookings);
  }
  connection.on("ReceiveBookingsForResource", handler);

  return () => {
    connection.off("ReceiveBookingsForResource", handler);
  }
}, [date, resourceId, connection]);

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
        resource
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