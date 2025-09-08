import { useState } from 'react'
import { Link } from 'react-router-dom'

const SignIn = () => {

  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    console.log("Inloggning:", { email, password })
    // Hantera inloggning här
  }

  return (
    <div className="signing-container">
      <div className="signing-box">
        <h2>Logga in</h2>
        <form onSubmit={handleSubmit} className="signing-form">
          <input 
            type="email" 
            placeholder="E-post" 
            value={email}
            onChange={(e) => setEmail(e.target.value)}
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