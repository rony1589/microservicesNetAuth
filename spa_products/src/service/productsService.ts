import { API_CONFIG, getApiUrl } from '../lib/config'
import type {
  CreateProductRequestDto,
  ProductDto,
  UpdateProductRequestDto,
} from '../types/product'

// Función para obtener el token del localStorage
const getAuthToken = (): string | null => {
  return localStorage.getItem('authToken')
}

export const getProducts = async (): Promise<ProductDto[]> => {
  const token = getAuthToken()
  if (!token) {
    throw new Error('No authentication token found')
  }

  const url = getApiUrl(API_CONFIG.PRODUCTS.LIST)
  console.log('Fetching products from URL:', url)
  console.log('Token exists:', !!token)

  const response = await fetch(url, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
  })

  const responseText = await response.text()
  console.log('Products response status:', response.status)
  console.log('Products response text length:', responseText.length)

  if (!response.ok) {
    console.error('Products error response:', responseText)
    let error
    try {
      error = JSON.parse(responseText)
    } catch {
      error = {
        title: 'Error de conexión',
        detail: responseText || 'Error desconocido',
        status: response.status,
      }
    }
    throw error
  }

  let data
  try {
    data = JSON.parse(responseText)
    console.log('Products data received:', data)
  } catch {
    console.error('Failed to parse products response:', responseText)
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
    let error
    try {
      error = JSON.parse(responseText)
    } catch {
      error = {
        title: 'Error de conexión',
        detail: responseText || 'Error desconocido',
        status: response.status,
      }
    }
    throw error
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
  if (!token) {
    throw new Error('No authentication token found')
  }

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
    let error
    try {
      error = JSON.parse(responseText)
    } catch {
      error = {
        title: 'Error de conexión',
        detail: responseText || 'Error desconocido',
        status: response.status,
      }
    }
    throw error
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
  if (!token) {
    throw new Error('No authentication token found')
  }

  const response = await fetch(getApiUrl(API_CONFIG.PRODUCTS.UPDATE(id)), {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(product),
  })

  const responseText = await response.text()

  if (!response.ok) {
    let error
    try {
      error = JSON.parse(responseText)
    } catch {
      error = {
        title: 'Error de conexión',
        detail: responseText || 'Error desconocido',
        status: response.status,
      }
    }
    throw error
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
    let error
    try {
      error = JSON.parse(responseText)
    } catch {
      error = {
        title: 'Error de conexión',
        detail: responseText || 'Error desconocido',
        status: response.status,
      }
    }
    throw error
  }
}
