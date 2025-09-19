import { useState } from "react";
import KontorsLayout from "../../public/img/Kontorslayout.png"
import "../Pages/Booking/booking.css"

const OfficeLayout = () => {
    const [showMap, setShowMap] = useState(false);


    return(
        <>
            <button className="booking-map-btn" onClick={() => setShowMap(true)}>
                Visa kontorslayout
            </button>

          {/*Karta över kontorslayout*/}
          {showMap && (
            <div className="booking-map-modal-overlay" onClick={() => setShowMap(false)}>
              <div className="booking-map-modal" onClick={e => e.stopPropagation()}>
                <button className="booking-close-map-btn" onClick={() => setShowMap(false)}>stäng</button>
                <img src={KontorsLayout} alt="Kontorslayout" className="booking-office-map-img" />
              </div>              
            </div>
          )}
        
        </>
    )
}

export default OfficeLayout;