import axios from 'axios'
import { useAuthStore } from '../store/authStore'
import type { ProblemDetails } from '../types/problemDetails'
import { API_CONFIG } from './config'

const api = axios.create({
  baseURL: API_CONFIG.BASE_URL,
  timeout: API_CONFIG.TIMEOUT,
  headers: API_CONFIG.HEADERS,
})

// Interceptor para agregar token de autenticación
api.interceptors.request.use((config) => {
  const token = useAuthStore.getState().token
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// Interceptor para manejar respuestas y errores
api.interceptors.response.use(
  (response) => response,
  (error) => {
    // Normaliza a ProblemDetails para UI
    const pd: ProblemDetails = error?.response?.data ?? {
      title: 'Error de conexión',
      status: error.response?.status || 0,
      detail: error.message || 'No se pudo conectar con el servidor',
    }

    // 401 → desloguear y redirigir
    if (error?.response?.status === 401) {
      useAuthStore.getState().logout()
    }

    return Promise.reject(pd)
  }
)

export default api
