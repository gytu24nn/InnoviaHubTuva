import { useState } from "react";
import Menu from "./menu";
import "./Header.css"


const Header = () => {
    const [isOpen, setIsOpen] = useState(false);
    // const [isLoggedIn] = useState(true); // testa true or false tills vidare
    return (
        
        <header className="header">
            <h1 className="logo">Innovia Hub</h1>
            <i 
                className="fa-solid fa-bars menu-icon"
                onClick={() => setIsOpen(!isOpen)}
            ></i>
            {isOpen && <div className="menu-overlay" onClick={() => setIsOpen(false)} />}
            <Menu isOpen={isOpen}/>
        </header>
            
        
    )
}

export default Header;