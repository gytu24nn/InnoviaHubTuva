import { use, useEffect, useRef, useState } from "react";
import * as signalR from "@microsoft/signalr";

type Notif = { title: string; message: string; sentAt?: string; id: string };

export default function NotificationListener() {
    const [items, setItems] = useState<Notif[]>([]);
    const connRef = useRef<signalR.HubConnection | null>(null);

    useEffect(() => {
        const conn = new signalR.HubConnectionBuilder()
            .withUrl("/NotificationHub", { withCredentials: true })
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

    conn.start().catch((e) => console.error("SignalR connection error:", e));
    connRef.current = conn;

    return () => {conn.stop(); };
    }, []);

    if (items.length === 0) return null;

    return (
        <div style={containerStyle}>
            {items.map((n) => (
                <div key={n.id} style={toastStyle} role="status" aria-live="polite">
                    <strong style={{ display: "block", marginBottom: 4 }}>{n.title}</strong>
                    <div>{n.message}</div>
                    {n.sentAt && (
                        <small style={{ opacity: 0.7 }}>
                        {new Date(n.sentAt).toLocaleString("sv-SE")}
                        </small>
                    )}
                </div>
            ))}
        </div>
    );
}

const containerStyle: React.CSSProperties = {
    position: "fixed",
    right: 16,
    top: 16,
    display: "flex",
    flexDirection: "column",
    gap: 8,
    zIndex: 9999,
};

const toastStyle: React.CSSProperties = {
    background: "white",
    color: "black",
    padding: "10px 12px",
    borderRadius: 8,
    boxShadow: "0 8px 24px rgba(0,0,0,0.12)",
    minWidth: 260,
    maxWidth: 360
};