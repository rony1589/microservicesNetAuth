import { getApiUrl } from '../lib/config'
import type {
  LoginRequestDto,
  RegisterRequestDto,
  TokenResponseDto,
} from '../types/user'

export const login = async (
  credentials: LoginRequestDto
): Promise<TokenResponseDto> => {
  try {
    const response = await fetch(getApiUrl('/users/login'), {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Accept: 'application/json',
      },
      body: JSON.stringify(credentials),
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
  } catch (error) {
    throw error
  }
}

export const register = async (
  userData: RegisterRequestDto
): Promise<TokenResponseDto> => {
  try {
    const response = await fetch(getApiUrl('/users/register'), {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Accept: 'application/json',
      },
      body: JSON.stringify(userData),
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
  } catch (error) {
    throw error
  }
}
