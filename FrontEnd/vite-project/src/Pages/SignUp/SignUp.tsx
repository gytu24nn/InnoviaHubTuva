import { useState } from 'react'
import { Link } from 'react-router-dom'

const SignUp = () => {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    
    if (password !== confirmPassword) {
      alert("Fel lösenord!")
      return
    }

    console.log("Registrerad med:", { email, password }
    // Här kan du lägga till logik för att skicka data till backend
    )
  }

  return (
    <div className="signup-container">
      <div className="signup-box">
        <h2>Registrera dig</h2>
        <form onSubmit={handleSubmit} className="signup-form">
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
          <input 
            type="password" 
            placeholder="Bekräfta Lösenord"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            required
          />
          <button type="submit">Registrera</button>
        </form>
        <p>Har du redan ett konto? <Link to="/signin">Logga in här</Link></p>
      </div>
    </div>
  )
}

export default SignUp