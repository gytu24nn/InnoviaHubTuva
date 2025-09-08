import "./header.css"

type MenuProps = {
    isOpen: boolean;
}

const meny = ({isOpen}: MenuProps) => {
    return (
        <nav className={`menu ${isOpen ? "open" : ""}`}>
            <ul>

                <li><i className="fa-solid fa-star menuStar"></i>Admin panel</li>
                <li><i className="fa-solid fa-star menuStar"></i>Mina bokningar</li>
                <li><i className="fa-solid fa-star menuStar"></i>Boka resurs</li>
                <li><i className="fa-solid fa-star menuStar"></i>Logga ut</li>
            </ul>

        </nav>
    )
}

export default meny;