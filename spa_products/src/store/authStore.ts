import { create } from 'zustand'
import { getUserFromToken, isTokenValid } from '../lib/tokenValidator'
import type { TokenResponseDto, UserDto } from '../types/user'

interface AuthState {
  user: UserDto | null
  token: string | null
  isAuthenticated: boolean
  login: (response: TokenResponseDto) => void
  logout: () => void
  initialize: () => void
}

export const useAuthStore = create<AuthState>((set, get) => ({
  user: null,
  token: null,
  isAuthenticated: false,

  login: (response: TokenResponseDto) => {
    // Guardar token en localStorage
    localStorage.setItem('authToken', response.accessToken)

    set({
      user: response.user,
      token: response.accessToken,
      isAuthenticated: true,
    })
  },

  logout: () => {
    // Remover token del localStorage
    localStorage.removeItem('authToken')

    set({
      user: null,
      token: null,
      isAuthenticated: false,
    })
  },

  initialize: () => {
    const token = localStorage.getItem('authToken')
    if (token && isTokenValid(token)) {
      // Solo actualizar si no está ya autenticado
      const currentState = get()
      if (!currentState.isAuthenticated) {
        // Intentar obtener información del usuario del token
        const userInfo = getUserFromToken(token)
        set({
          token,
          user: userInfo
            ? {
                id: userInfo.sub || userInfo.id,
                name: userInfo.name,
                email: userInfo.email,
                role: userInfo.role || 'Usuario',
                isActive: true,
                createdAt: new Date().toISOString(),
                updatedAt: new Date().toISOString(),
              }
            : null,
          isAuthenticated: true,
        })
      }
    } else if (token && !isTokenValid(token)) {
      // Token expirado, limpiar
      localStorage.removeItem('authToken')
      set({
        user: null,
        token: null,
        isAuthenticated: false,
      })
    }
  },
}))

// Función para inicializar el estado desde localStorage
export const initializeAuth = () => {
  useAuthStore.getState().initialize()
}
