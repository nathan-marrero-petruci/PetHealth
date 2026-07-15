import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import { Login } from './pages/Login'
import { PesoChart } from './pages/PesoChart'
import { Vacinas } from './pages/Vacinas'
import { Consultas } from './pages/Consultas'
import { Medicacoes } from './pages/Medicacoes'
import { SaudePreventiva } from './pages/SaudePreventiva'
import { Observacoes } from './pages/Observacoes'
import { DietaPadrao } from './pages/DietaPadrao'
import { Refeicoes } from './pages/Refeicoes'
import { Petiscos } from './pages/Petiscos'
import { ComandosTreino } from './pages/ComandosTreino'
import { SessoesTreino } from './pages/SessoesTreino'
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
          <Route path="/vacinas" element={<Vacinas />} />
          <Route path="/consultas" element={<Consultas />} />
          <Route path="/medicacoes" element={<Medicacoes />} />
          <Route path="/saude-preventiva" element={<SaudePreventiva />} />
          <Route path="/observacoes" element={<Observacoes />} />
          <Route path="/dieta" element={<DietaPadrao />} />
          <Route path="/refeicoes" element={<Refeicoes />} />
          <Route path="/petiscos" element={<Petiscos />} />
          <Route path="/comandos-treino" element={<ComandosTreino />} />
          <Route path="/sessoes-treino" element={<SessoesTreino />} />
          <Route path="/historico" element={<HistoricoPeriodo />} />
        </Route>
        <Route path="*" element={<Navigate to="/dashboard" replace />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App
