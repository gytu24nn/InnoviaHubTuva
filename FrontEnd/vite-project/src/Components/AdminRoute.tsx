import { useUser } from "../Context/UserContext";
import type { JSX } from "react";

type Props = {
  children: JSX.Element;
};

const AdminRoute = ({ children }: Props) => {
  const { user, loading, isAdmin } = useUser();

  // Vänta tills användardata laddats
  if (loading) {
    return <div className="loading-message">⏳ Kontrollerar admin-behörighet...</div>;
  }

  // Om inte admin → redirecta eller visa fel
  if (!user || !isAdmin) {
    return <div className="error-message">🚫 Åtkomst nekad - du har inte behörighet att se denna sida.</div>;
    }

  // Annars → släpp igenom
  return children;
};

export default AdminRoute;
