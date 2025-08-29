// Configuración de la API
export const API_CONFIG = {
  // URL base del gateway (usando proxy de Vite en desarrollo)
  BASE_URL: import.meta.env.DEV ? '' : import.meta.env.VITE_API_BASE_URL,

  // Rutas de autenticación (no requieren token)
  AUTH: {
    LOGIN: '/users/login',
    REGISTER: '/users/register',
  },

  // Rutas de productos (ajustadas según los endpoints de la imagen)
  PRODUCTS: {
    LIST: '/products/all',
    GET: (id: string) => `/products/${id}`,
    CREATE: '/products/create',
    UPDATE: (id: string) => `/products/update/${id}`,
    DELETE: (id: string) => `/products/delete/${id}`,
  },

  // Timeout para las peticiones
  TIMEOUT: 15000,

  // Headers por defecto
  HEADERS: {
    'Content-Type': 'application/json',
  },
}

// Función para obtener la URL completa
export const getApiUrl = (endpoint: string): string => {
  const baseUrl = API_CONFIG.BASE_URL || ''
  const fullUrl = `${baseUrl}${endpoint}`
  return fullUrl
}
