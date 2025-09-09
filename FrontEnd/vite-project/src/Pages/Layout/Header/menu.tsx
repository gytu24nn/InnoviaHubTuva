import "./header.css"
import {Link, useNavigate} from "react-router-dom"

type MenuProps = {
    isOpen: boolean;
}

const meny = ({isOpen}: MenuProps) => {
    const navigate = useNavigate();

    const handleLogout = () => {
        localStorage.removeItem("token");
        localStorage.removeItem("Role");
        navigate("/");
    }
    return (
        <nav className={`menu ${isOpen ? "open" : ""}`}>
            <ul>
                <li>
                    <Link className="linkMenuOption" to={"/Admin"}>
                        Admin panel
                    </Link>
                </li>
                <li>
                    <Link className="linkMenuOption" to={"/MyBookings"}>
                        Mina bokningar
                    </Link>
                </li>
                <li>
                    <Link className="linkMenuOption" to={"/Booking"}>
                        Boka resurs
                    </Link>
                </li>
                <li onClick={handleLogout}>
                    Logga ut
                </li>
            </ul>
        </nav>
    )
}

export default meny;