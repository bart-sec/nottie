import { useEffect, useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from './assets/vite.svg'
import heroImg from './assets/hero.png'
import './App.css'
import Keycloak from 'keycloak-js'
import { BrowserRouter, Route, Routes } from 'react-router-dom'
import SecurityGate from './SecurityGate'
import Dashboard from './Dashboard'
import { ReactKeycloakProvider } from '@react-keycloak/web'



const keycloak = new Keycloak({
  url: "http://localhost:8080",
  realm: "nottie",
  clientId: "nottie_client"
})


function App() {
  return (
    <ReactKeycloakProvider authClient={keycloak}>
      <BrowserRouter>
        <Routes>
          <Route
            path="/"
            element={
              <SecurityGate>
                <Dashboard />
              </SecurityGate>
            }>

          </Route>
        </Routes>
      </BrowserRouter>
    </ReactKeycloakProvider>
  )

}

export default App
