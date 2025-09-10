import { createContext, useContext, useState } from "react";
import type { ReactNode } from "react";

type BookingContextType = {
    resourceId: number | null;
    setResourceId: (id: number) => void;
    date: Date | null;
    setDate: (date: Date) => void;
    timeSlotId: number | null;
    setTimeSlotId: (id: number) => void;
};

const BookingContext = createContext<BookingContextType | undefined>(undefined);

export const BookingProvider = ({ children }: { children: ReactNode }) => {
    const [resourceId, setResourceId] = useState<number | null>(null);
    const [date, setDate] = useState<Date | null>(null);
    const [timeSlotId, setTimeSlotId] = useState<number | null>(null);

  return (
    <BookingContext.Provider value={{ resourceId, setResourceId, date, setDate, timeSlotId, setTimeSlotId }}>
      {children}
    </BookingContext.Provider>
  );
};

export const useBooking = () => {
    const context = useContext(BookingContext);
    if (!context) throw new Error("useBooking must be used inside BookingProvider");
    return context;
};