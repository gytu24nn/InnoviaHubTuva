import { useState } from "react"
import Header from "./Header/Header";
import { Outlet } from "react-router-dom";
import "./Footer&Layout.css";
import Footer from "./Footer";

const Layout = () => {
    const [menuOpen, setMenuOpen] = useState(false);

  return (
    <div className="layout-container">
        <header>
            <Header />
        </header>

        <main>
            <Outlet />
        </main>

        <footer>
            <Footer />
        </footer>
    </div>
  )
}

export default Layout