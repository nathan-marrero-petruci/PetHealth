import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import { Login } from './pages/Login'
import { PesoChart } from './pages/PesoChart'
import { ProtectedRoute } from './components/ProtectedRoute'
import './App.css'

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route
          path="/peso"
          element={
            <ProtectedRoute>
              <PesoChart />
            </ProtectedRoute>
          }
        />
        <Route path="*" element={<Navigate to="/peso" replace />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App
