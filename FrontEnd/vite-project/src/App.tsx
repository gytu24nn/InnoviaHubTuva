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
import ErrorBoundaryFallback from './Components/errorbounday';
import { ErrorBoundary } from "react-error-boundary";
import { BookingProvider } from "./Context/BookingContext";
import { UserProvider, useUser } from './Context/UserContext'; 
import ProtectedRoute from "./Components/ProtectedRoute";
import AdminRoute from './Components/AdminRoute';

const AppContent = () => {
    const { loading: userLoading } = useUser();

    if (userLoading) {
        return <div className="loading-message">Laddar användardata...</div>;
    }

    return (
        <Routes>
            <Route path="/" element={<SignIn />} />
            <Route path="/SignUp" element={<SignUp />} />

            <Route element={<Layout />}>
                <Route path='/Admin' element={<AdminRoute><Admin /></AdminRoute>} />
                <Route path='/MyBookings' element={<ProtectedRoute><MyBookings /></ProtectedRoute>} />
                <Route path='/Resursvy' element={<ProtectedRoute><Resursvy /></ProtectedRoute>} />
                <Route path='/Home' element={<ProtectedRoute><Home /></ProtectedRoute>} />
                <Route path='/Booking' element={<ProtectedRoute><Booking /></ProtectedRoute>} />
                <Route path='/BookingConfirmed/:bookingId' element={<ProtectedRoute><BookingConfirmed /></ProtectedRoute>} />
            </Route>
        </Routes>
    );
};

function App() {
  return (
    <UserProvider>
      <BookingProvider>
        <ErrorBoundary FallbackComponent={ErrorBoundaryFallback}>
          <Router>
             <AppContent />
          </Router>
        </ErrorBoundary>
      </BookingProvider>
    </UserProvider>
  );
}

export default App;