import "./header.css"
import {Link, useNavigate} from "react-router-dom"
import { logout } from "../../../Services/authService";
import { useState } from "react";

type MenuProps = {
    isOpen: boolean;
}

const meny = ({isOpen}: MenuProps) => {
    const navigate = useNavigate();
    const [loggingOut, setLoggingOut] = useState(false);

    const handleLogout = async () => {
        if (loggingOut) return; // Prevent multiple clicks
        setLoggingOut(true);
        try {
            await logout();
        }catch {
            console.error("Logout failed");
        } finally {
            localStorage.removeItem("token");
            localStorage.removeItem("Role");
            navigate("/", {replace: true});
            setLoggingOut(false);
        }
        
    }
    return (
        <nav className={`menu ${isOpen ? "open" : ""}`}>
            <ul>
                <li>
                    <Link className="linkMenuOption" to={"/Admin"}>
                       <i className="fa-solid fa-star"></i>Admin panel
                    </Link>
                </li>
                <li>
                    <Link className="linkMenuOption" to={"/MyBookings"}>
                        <i className="fa-solid fa-star"></i>Mina bokningar
                    </Link>
                </li>
                <li>
                    <Link className="linkMenuOption" to={"/Resursvy"}>
                       <i className="fa-solid fa-star"></i>Boka resurs
                    </Link>
                </li>
                <li>
                <button
                    type="button"
                    className="linkMenuOption as-button"
                    onClick={handleLogout}
                    disabled={loggingOut}
                >
                    <i className="fa-solid fa-star"></i>{loggingOut ? "Loggar ut..." : "Logga ut"}
                </button>
                </li>
            </ul>
        </nav>
    )
}

export default meny;