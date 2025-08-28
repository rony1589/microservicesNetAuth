import React from 'react'
import ReactDOM from 'react-dom/client'
import '../src/lib/logger' // engancha listeners globales
import App from './app/App'
import './index.css'

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
)
