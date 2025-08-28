import { create } from 'zustand'
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
    if (token) {
      // Solo actualizar si no está ya autenticado
      const currentState = get()
      if (!currentState.isAuthenticated) {
        set({
          token,
          isAuthenticated: true,
          // El usuario se puede obtener del token JWT si es necesario
        })
      }
    }
  },
}))

// Función para inicializar el estado desde localStorage
export const initializeAuth = () => {
  useAuthStore.getState().initialize()
}
