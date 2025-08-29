import { useEffect } from 'react'
import { Toaster } from 'react-hot-toast'
import { RouterProvider } from 'react-router-dom'
import { isTokenValid } from '../lib/tokenValidator'
import { initializeAuth, useAuthStore } from '../store/authStore'
import { router } from './routes'

export default function App() {
  const { token, logout } = useAuthStore()

  useEffect(() => {
    // Inicializar autenticación al cargar la app
    initializeAuth()
  }, [])

  useEffect(() => {
    // Verificar token cada 30 segundos
    const interval = setInterval(() => {
      if (token && !isTokenValid(token)) {
        logout()
      }
    }, 30000)

    return () => clearInterval(interval)
  }, [token, logout])

  return (
    <>
      <RouterProvider router={router} />
      <Toaster
        position="bottom-center"
        toastOptions={{
          duration: 4000,
          style: {
            background: 'rgba(0, 0, 0, 0.9)',
            color: '#fff',
            border: '1px solid rgba(255, 255, 255, 0.1)',
            backdropFilter: 'blur(10px)',
            borderRadius: '12px',
            padding: '16px 20px',
            fontSize: '14px',
            fontWeight: '500',
            boxShadow: '0 8px 32px rgba(0, 0, 0, 0.3)',
          },
          success: {
            iconTheme: {
              primary: '#10b981',
              secondary: '#fff',
            },
          },
          error: {
            iconTheme: {
              primary: '#ef4444',
              secondary: '#fff',
            },
          },
        }}
      />
    </>
  )
}
