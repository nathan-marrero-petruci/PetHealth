import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import { Login } from './pages/Login'
import { PesoChart } from './pages/PesoChart'
import { Dashboard } from './pages/Dashboard'
import { HistoricoPeriodo } from './pages/HistoricoPeriodo'
import { ProtectedRoute } from './components/ProtectedRoute'
import { AppLayout } from './components/AppLayout'
import './App.css'

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route
          element={
            <ProtectedRoute>
              <AppLayout />
            </ProtectedRoute>
          }
        >
          <Route path="/dashboard" element={<Dashboard />} />
          <Route path="/peso" element={<PesoChart />} />
          <Route path="/historico" element={<HistoricoPeriodo />} />
        </Route>
        <Route path="*" element={<Navigate to="/dashboard" replace />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App
