/**
 * Función para decodificar un token JWT sin verificar la firma
 * Solo para extraer información del payload
 */
function decodeJWT(token: string): any {
  try {
    const base64Url = token.split('.')[1]
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/')
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    )
    return JSON.parse(jsonPayload)
  } catch (error) {
    return null
  }
}

/**
 * Verifica si un token JWT ha expirado
 */
export function isTokenExpired(token: string): boolean {
  if (!token) return true

  const payload = decodeJWT(token)
  if (!payload) return true

  // Verificar si tiene campo exp (expiration time)
  if (!payload.exp) return false // Si no tiene exp, asumimos que no expira

  const currentTime = Math.floor(Date.now() / 1000)
  return payload.exp < currentTime
}

/**
 * Obtiene la información del usuario del token JWT
 */
export function getUserFromToken(token: string): any {
  if (!token) return null

  const payload = decodeJWT(token)
  if (!payload) return null

  return payload
}

/**
 * Verifica si el token es válido (existe y no ha expirado)
 */
export function isTokenValid(token: string | null): boolean {
  if (!token) return false
  return !isTokenExpired(token)
}

/**
 * Obtiene el tiempo restante del token en segundos
 */
export function getTokenTimeRemaining(token: string): number {
  if (!token) return 0

  const payload = decodeJWT(token)
  if (!payload || !payload.exp) return 0

  const currentTime = Math.floor(Date.now() / 1000)
  return Math.max(0, payload.exp - currentTime)
}
