import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { registerUser, signIn } from '../../Services/authService'
import "../SignIn/Signin.css"

const SignUp = () => {
  const navigate = useNavigate();
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault()
    setError(null);
    
    if (password !== confirmPassword) {
      alert("Lösenorden matchar inte");
      return
    }

    try {
      setLoading(true);
      await registerUser(username.trim(), password);
      await signIn(username.trim(), password);
      navigate('/Home');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Ett fel uppstod');
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="signup-container">
      <div className="signup-box">
        <h1 className="logo">
          <Link className="logo" to={"/"}>
            Innovia Hub
          </Link>
          </h1>
        <h2>Registrera dig</h2>
        {error && <div className="error-message">{error}</div>}
        <form onSubmit={handleSubmit} className="signup-form" noValidate>
          <input 
            type="text" 
            placeholder="Användarnamn" 
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required 
          />
          <input 
            type="password" 
            placeholder="Lösenord"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
          <input 
            type="password" 
            placeholder="Bekräfta Lösenord"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            required
          />
          <button type="submit">Registrera</button>
        </form>
        <p>Har du redan ett konto? <Link to="/">Logga in här</Link></p>
      </div>
    </div>
  )
}

export default SignUp