import { useEffect, useRef, useState } from "react";
import "./NotificationsListener.css"
import * as signalR from "@microsoft/signalr";
import { BASE_URL } from "../config";

type Notif = { title: string; message: string; sentAt?: string; id: string };

export default function NotificationListener() {
    const [items, setItems] = useState<Notif[]>([]);
    const connRef = useRef<signalR.HubConnection | null>(null);

    const apiBase = BASE_URL;

    useEffect(() => {
        const conn = new signalR.HubConnectionBuilder()
            .withUrl(`${apiBase}/NotificationHub`, { withCredentials: true })
            .withAutomaticReconnect()
            .build();

    conn.on("ReceiveNotification", (n: { title: string; message: string; sentAt?: string }) => {
        const id = crypto.randomUUID() ?? String(Date.now());
        const notif: Notif = { ...n, id };
        setItems((prev) => [notif, ...prev]);

        setTimeout(() => {
            setItems((prev) => prev.filter((x) => x.id !== id));
        }, 10000);
    }); 

    const bookingConn = new signalR.HubConnectionBuilder()
        .withUrl(`${apiBase}/BookingHub`, { withCredentials: true })
        .withAutomaticReconnect()
        .build();
    
    bookingConn.on("BookingCreated", (booking) => {
        const id = crypto.randomUUID();
        const notif: Notif = {
            id,
            title: "Ny bokning",
            message: `${booking.resourceName} bokades ${new Date(booking.date).toLocaleDateString("sv-SE")} (${booking.startTime} - ${booking.endTime})`
        };
        setItems((prev) => [notif, ...prev]);
        setTimeout(() => {
            setItems((prev) => prev.filter((x) => x.id !== id));
        }, 10000);
    });

    bookingConn.on("BookingCancelled", (booking) => {
        const id = crypto.randomUUID();
        const notif: Notif = {
            id,
            title: "Avbokning",
            message: `${booking.resourceName} avbokades ${new Date(booking.date).toLocaleDateString("sv-SE")} (${booking.startTime} - ${booking.endTime})`
        };
        setItems((prev) => [notif, ...prev]);
        setTimeout(() => {
            setItems((prev) => prev.filter((x) => x.id !== id));
        }, 10000);
    });

    bookingConn.start().catch((e) => console.error("BookingHub connection error:", e));
    conn.start().catch((e) => console.error("SignalR connection error:", e));
    connRef.current = conn;

    return () => {
        bookingConn.stop();
        conn.stop(); };
    }, []);

    if (items.length === 0) return null;

    return (
        <div className="toast-container">
            {items.map((n) => (
            <div key={n.id} className={`toast info`} role="status" aria-live="polite">
                <strong>{n.title}</strong>
                <div>{n.message}</div>
                {n.sentAt && <small>{new Date(n.sentAt).toLocaleString("sv-SE")}</small>}
            </div>
            ))}
        </div>
    );
}