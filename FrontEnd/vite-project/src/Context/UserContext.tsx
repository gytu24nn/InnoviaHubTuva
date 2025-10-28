import { createContext, useContext, useEffect, useState, type ReactNode } from "react";
import { BASE_URL } from "../config";

type User = {
  id: string;
  email: string;
  roles: string[];
  userName: string;
};

type UserContextType = {
  user: User | null;
  loading: boolean;
  isAdmin: boolean;
  isLoggedIn: boolean;
  refreshUser: () => Promise<void>;
};

const UserContext = createContext<UserContextType | undefined>(undefined);

export const UserProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);
  const apiBase = BASE_URL;

  const fetchUser = async () => {
    setLoading(true);
    try {
      const response = await fetch(`${BASE_URL}/api/auth/me`, {
        credentials: "include",
        });

        if (response.ok) {
            const data = await response.json();
            setUser({
                id: data.id,
                email: data.email,
                roles: data.roles,
                userName: data.username,
            });
            } else {
            setUser(null);
            }
    } catch (err) {
      console.error("Fel vid hämtning av användare:", err);
      setUser(null);
    } finally {
      setLoading(false);
    }
};

  useEffect(() => {
    fetchUser();
  }, []);

  return (
    <UserContext.Provider
    value={{
        user,
        loading,
        isAdmin: user?.roles.includes("Admin") ?? false,
        isLoggedIn: !!user,
        refreshUser: fetchUser,
    }}
    >
    {children}
    </UserContext.Provider>
  );
};

export const useUser = () => {
  const context = useContext(UserContext);
  if (!context) {
    throw new Error("useUser måste användas inom UserProvider");
  }
  return context;
};
