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
    console.log('Intentando login con URL:', getApiUrl('/users/login'))
    console.log('Email:', credentials.email) // Solo log del email, no la contraseña

    const response = await fetch(getApiUrl('/users/login'), {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Accept: 'application/json',
      },
      body: JSON.stringify(credentials),
    })

    console.log('Response status:', response.status)

    const responseText = await response.text()
    console.log('Response received')

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

    console.log('Login successful for user:', data.user?.email)
    return data
  } catch (error) {
    console.error('Login error:', error)
    throw error
  }
}

export const register = async (
  userData: RegisterRequestDto
): Promise<TokenResponseDto> => {
  try {
    console.log('Intentando registro con URL:', getApiUrl('/users/register'))
    console.log('Email:', userData.email) // Solo log del email
    console.log('Payload completo:', JSON.stringify(userData, null, 2))

    const response = await fetch(getApiUrl('/users/register'), {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Accept: 'application/json',
      },
      body: JSON.stringify(userData),
    })

    console.log('Response status:', response.status)

    const responseText = await response.text()
    console.log('Response text:', responseText)

    if (!response.ok) {
      let error
      try {
        error = JSON.parse(responseText)
        console.log('Parsed error:', error)
      } catch {
        error = {
          title: 'Error de conexión',
          detail: responseText || 'Error desconocido',
          status: response.status,
        }
        console.log('Fallback error:', error)
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

    console.log('Register successful for user:', data.user?.email)
    return data
  } catch (error) {
    console.error('Register error:', error)
    throw error
  }
}
