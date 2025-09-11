import { useState } from 'react'
import './App.css'
import { BrowserRouter as Router, Routes, Route, Link } from "react-router-dom";

import SignIn from './Pages/SignIn/SignIn';
import SignUp from './Pages/SignUp/SignUp';
import Layout from './Pages/Layout/Layout';
import Admin from './Pages/Admin/admin';
import MyBookings from './Pages/MyBookings/MyBookings';
import Home from './Pages/Home/Home';
import Booking from './Pages/Booking/Booking';

import { BookingProvider } from "./Context/BookingContext";

import Resursvy from './Pages/Booking/Resursvy';

function App() {
  return (
    <BookingProvider>
      <Router>
        <Routes>
          <Route path="/" element={<SignIn />} />
          <Route path="/SignUp" element={<SignUp />} />
          <Route element={<Layout />}>

            <Route path='/Admin' element={<Admin />} />
            <Route path='/MyBookings' element={<MyBookings />} />
            <Route path='/Resursvy' element={<Resursvy />} />
            <Route path='/Home' element={<Home />} />
            <Route path='/Booking' element={<Booking />} />

          </Route>
        </Routes>
      </Router>
    </BookingProvider>
  );
}

export default App;