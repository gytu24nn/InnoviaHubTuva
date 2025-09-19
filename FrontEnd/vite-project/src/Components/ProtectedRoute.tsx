import { Link } from "react-router-dom";
import { useUser } from "../Context/UserContext";
import type { JSX } from "react";

type Props = {
  children: JSX.Element;
};

const ProtectedRoute = ({ children }: Props) => {
  const { user, loading, isLoggedIn } = useUser();

  // Vänta tills användardata laddats
  if (loading) {
    return <div className="loading-message">⏳ Kontrollerar inloggning...</div>;
  }

  // Om användaren inte är inloggad → redirect till login
  if (!isLoggedIn || !user) {
    return   (
      <>
        <div className="errorAndLoadingMessage-container">
          <div className="error-message">
            Du måste vara inloggad för att se den här sidan.
            <br />
            <Link to="/" className="login-link">Logga in här.</Link>
          </div>
        </div>
      </>
      
    )    
    
    
  }

  // Annars → släpp igenom
  return children;
};

export default ProtectedRoute;
