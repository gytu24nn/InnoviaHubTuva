import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { signIn, fetchMe } from '../../Services/authService'
import './SignIn.css'
import { useUser } from '../../Context/UserContext'


const SignIn = () => {
  const navigate = useNavigate();
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const { refreshUser } = useUser();

  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setError(null);
    setLoading(true);

    try {
      const { id, userName, role } = 
      
      await signIn(username.trim(), password);
      const me = await fetchMe();

      await refreshUser();

      if (role?.toLowerCase() === 'admin') {
        navigate('/Admin');
      } else {
        navigate('/Home');
      }
    } catch (err) {
    setError(err instanceof Error ? err.message : 'Ett fel uppstod');
    } finally {
    setLoading(false);
    }
  }

  return (
    <div className="signing-container">
      <div className="signing-box">
        <h1 className="logo">
          <Link className="logo" to={"/"}>
              Innovia Hub
          </Link>
        </h1>

        <h2>Logga in</h2>

        {error && <div className="error-message">{error}</div>}

        <form onSubmit={handleSubmit} className="signing-form">
          <input 
            type="text" 
            placeholder="Användarnamn" 
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            autoComplete='username'
            required 
          />
          <input 
            type="password" 
            placeholder="Lösenord"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
          <button type="submit">Logga in</button>
        </form>
      </div>
       <div className="signup-redirect">
      <p>Har du inget konto? <Link to="/signup">Skapa ett konto</Link></p>
    </div>
    </div>
  )
}

export default SignIn