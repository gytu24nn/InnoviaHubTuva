const API = "http://localhost:5099";

type LoginResponse = {
    id: string;
    userName: string;
    role: string;
};

export async function signIn(username: string, password: string): Promise<LoginResponse> {
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


//get current user through /me endpoint
export async function fetchMe() : Promise<LoginResponse | null> {
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
export async function logout(): Promise<void> {
    await fetch(`${API}/api/auth/logout`, {
        method: 'POST',
        credentials: 'include',
    });
}