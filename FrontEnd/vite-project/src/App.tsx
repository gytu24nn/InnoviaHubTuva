import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'
import { BrowserRouter as Router, Routes, Route, Link } from "react-router-dom";

import SignIn from './Pages/SignIn/SignIn';
import SignUp from './Pages/SignUp/SignUp';
import Layout from './Pages/Layout/Layout';
import Admin from './Pages/Admin/admin';
import MyBookings from './Pages/MyBookings/MyBookings';
import Home from './Pages/Home/Home';
import Booking from './Pages/Booking/Booking';

function App() {
  const [count, setCount] = useState(0)

  return (
    <>
      <Router>
        <Routes>
          <Route path='/' element={<SignIn />}/>
          <Route path='/SignUp' element={<SignUp />} />

          <Route element={<Layout />}>
            <Route path='/Admin' element={<Admin />} />
            <Route path='/MyBookings' element={<MyBookings />} />
            <Route path='/Home' element={<Home />} />
            <Route path='Booking' element={<Booking />} />
          </Route>
          
        </Routes>
      </Router>
    </>
  )
}

export default App
