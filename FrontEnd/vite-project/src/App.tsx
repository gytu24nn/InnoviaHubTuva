import { useState } from 'react'
import './App.css'
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";

import SignIn from './Pages/SignIn/SignIn';
import SignUp from './Pages/SignUp/SignUp';
import Layout from './Pages/Layout/Layout';
import Admin from './Pages/Admin/admin';
import MyBookings from './Pages/MyBookings/MyBookings';
import Home from './Pages/Home/Home';
import Booking from './Pages/Booking/Booking';
import Resursvy from './Pages/Booking/Resursvy';
import BookingConfirmed from './Pages/Booking/BookingConfirmed';

import { BookingProvider } from "./Context/BookingContext";

function App() {
  return (
    <BookingProvider>
      <Router>
        <Routes>
          <Route path="/" element={<SignIn />} />
          <Route path="/SignUp" element={<SignUp />} />

          {/* All routes with Layout wrapper */}
          <Route element={<Layout />}>
            <Route path='/Admin' element={<Admin />} />
            <Route path='/MyBookings' element={<MyBookings />} />
            <Route path='/Resursvy' element={<Resursvy />} />
            <Route path='/Home' element={<Home />} />
            <Route path='/Booking' element={<Booking />} />

            {/* ✅ new BookingConfirmed route */}
            <Route path='/BookingConfirmed/:bookingId' element={<BookingConfirmed />} />
          </Route>
        </Routes>
      </Router>
    </BookingProvider>
  );
}

export default App;