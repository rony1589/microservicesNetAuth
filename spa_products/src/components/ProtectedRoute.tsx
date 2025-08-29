import type { ReactElement } from 'react'
import { useEffect } from 'react'
import { Navigate } from 'react-router-dom'
import { isTokenValid } from '../lib/tokenValidator'
import { useAuthStore } from '../store/authStore'

export default function ProtectedRoute({
  children,
}: {
  children: ReactElement
}) {
  const { token, logout, isAuthenticated } = useAuthStore()

  useEffect(() => {
    // Verificar si el token es válido en cada renderizado
    if (token && !isTokenValid(token)) {
      logout()
    }
  }, [token, logout])

  // Si no hay token o no está autenticado, redirigir a login
  if (!token || !isAuthenticated) {
    return <Navigate to="/login" replace />
  }

  return children
}
