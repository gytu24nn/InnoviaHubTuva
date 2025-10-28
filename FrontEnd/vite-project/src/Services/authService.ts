import { BASE_URL } from "../config";
const API = BASE_URL;

type LoginResponse = {
    id: string;
    userName: string;
    role: string;
};

export async function signIn(username: string, password: string) {
    const response = await fetch(`${API}/api/auth/login`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        credentials: 'include',
        body: JSON.stringify({ username, password }),
    });

    if (!response.ok) {
        if (response.status === 401) {
            throw new Error('Fel användarnamn eller lösenord');
        }
    }

    return response.json();

}

type RegisterResponse = {
    id: string;
    userName: string;
    role: string;
};

export async function registerUser(username: string, password: string, isAdmin = false): Promise<RegisterResponse> {
    const response = await fetch(`${API}/api/auth/register`, {
        method: 'POST',
        headers: { "Content-Type": "application/json" },
        credentials: 'include',
        body: JSON.stringify({ username, password, isAdmin }),
    });

    if (!response.ok) {
        try {
            const data = await response.json();
            const msg = (Array.isArray(data) && data[0]?.description || data?.message) || 'Kunde inte registrera användare';
            throw new Error(msg);
        } catch {
            throw new Error('Kunde inte registrera användare');
        }
    }

    return response.json();
}


//get current user through /me endpoint
export async function fetchMe() {
    const response = await fetch(`${API}/api/auth/me`, {
        credentials: 'include',
    });

    if (response.status === 401) return null;
    if(!response.ok) {
        throw new Error('Kunde inte hämta användarinformation');
    }

    return response.json();
}

//log out, delete cookie
export async function logout(): Promise<boolean> {
    try {
    const res = await fetch(`${API}/api/auth/logout`, {
        method: 'POST',
        credentials: 'include',
    });
    return res.ok;
    } catch {
        return false;
    }
}