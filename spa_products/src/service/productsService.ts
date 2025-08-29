import { API_CONFIG, getApiUrl } from '../lib/config'
import { isTokenValid } from '../lib/tokenValidator'
import { useAuthStore } from '../store/authStore'
import type {
  CreateProductRequestDto,
  ProductDto,
  UpdateProductRequestDto,
} from '../types/product'

// Función helper para manejar errores de respuesta
const handleResponseError = (response: Response, responseText: string) => {
  // Si es 401 (Unauthorized), hacer logout automático
  if (response.status === 401) {
    useAuthStore.getState().logout()
    throw {
      title: 'Sesión expirada',
      detail: 'Tu sesión ha expirado. Por favor, inicia sesión nuevamente.',
      status: response.status,
    }
  }

  let error
  try {
    error = JSON.parse(responseText)
    // Extract validation errors if they exist
    if (error.extensions?.errors) {
      const validationErrors = Object.entries(error.extensions.errors)
        .map(
          ([field, messages]) =>
            `${field}: ${
              Array.isArray(messages) ? messages.join(', ') : messages
            }`
        )
        .join('; ')
      error.detail = `Validation failed: ${validationErrors}`
    }
  } catch {
    error = {
      title: 'Error de conexión',
      detail: responseText || 'Error desconocido',
      status: response.status,
    }
  }
  throw error
}

// Función para obtener el token del localStorage
const getAuthToken = (): string | null => {
  const token = localStorage.getItem('authToken')

  // Verificar si el token es válido
  if (token && !isTokenValid(token)) {
    // Hacer logout automático
    useAuthStore.getState().logout()
    return null
  }

  return token
}

export const getProducts = async (): Promise<ProductDto[]> => {
  const token = getAuthToken()
  const url = getApiUrl(API_CONFIG.PRODUCTS.LIST)

  const response = await fetch(url, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
  })

  const responseText = await response.text()

  if (!response.ok) {
    handleResponseError(response, responseText)
  }

  let data
  try {
    data = JSON.parse(responseText)
  } catch {
    throw {
      title: 'Error de respuesta',
      detail: 'La respuesta no es un JSON válido',
      status: response.status,
    }
  }

  return data
}

export const getProduct = async (id: string): Promise<ProductDto> => {
  const token = getAuthToken()
  if (!token) {
    throw new Error('No authentication token found')
  }

  const response = await fetch(getApiUrl(API_CONFIG.PRODUCTS.GET(id)), {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
  })

  const responseText = await response.text()

  if (!response.ok) {
    handleResponseError(response, responseText)
  }

  let data
  try {
    data = JSON.parse(responseText)
  } catch {
    throw {
      title: 'Error de respuesta',
      detail: 'La respuesta no es un JSON válido',
      status: response.status,
    }
  }

  return data
}

export const createProduct = async (
  product: CreateProductRequestDto
): Promise<ProductDto> => {
  const token = getAuthToken()
  const response = await fetch(getApiUrl(API_CONFIG.PRODUCTS.CREATE), {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(product),
  })

  const responseText = await response.text()

  if (!response.ok) {
    handleResponseError(response, responseText)
  }

  let data
  try {
    data = JSON.parse(responseText)
  } catch {
    throw {
      title: 'Error de respuesta',
      detail: 'La respuesta no es un JSON válido',
      status: response.status,
    }
  }

  return data
}

export const updateProduct = async (
  id: string,
  product: UpdateProductRequestDto
): Promise<ProductDto> => {
  const token = getAuthToken()

  // Include the id in the request body
  const productWithId = {
    ...product,
    id: id,
  }

  const response = await fetch(getApiUrl(API_CONFIG.PRODUCTS.UPDATE(id)), {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(productWithId),
  })

  const responseText = await response.text()

  if (!response.ok) {
    handleResponseError(response, responseText)
  }

  let data
  try {
    data = JSON.parse(responseText)
  } catch {
    throw {
      title: 'Error de respuesta',
      detail: 'La respuesta no es un JSON válido',
      status: response.status,
    }
  }

  return data
}

export const deleteProduct = async (id: string): Promise<void> => {
  const token = getAuthToken()
  if (!token) {
    throw new Error('No authentication token found')
  }

  const response = await fetch(getApiUrl(API_CONFIG.PRODUCTS.DELETE(id)), {
    method: 'DELETE',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
  })

  if (!response.ok) {
    const responseText = await response.text()
    handleResponseError(response, responseText)
  }
}
