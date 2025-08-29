import type { ProblemDetails } from '../types/problemDetails'

/**
 * Extrae el mensaje de error más relevante de una respuesta de error
 */
export function getErrorMessage(error: unknown): string {
  if (!error) return 'Error desconocido'

  // Si es un objeto ProblemDetails
  if (typeof error === 'object' && error !== null) {
    const problemDetails = error as ProblemDetails

    // Si hay errores de validación específicos, mostrarlos
    if (problemDetails.extensions?.errors) {
      const validationErrors = Object.entries(problemDetails.extensions.errors)
        .map(([field, messages]) => {
          const message = Array.isArray(messages)
            ? messages.join(', ')
            : messages
          return `${field}: ${message}`
        })
        .join('; ')
      return `Errores de validación: ${validationErrors}`
    }

    // Si hay un detail específico, usarlo
    if (
      problemDetails.detail &&
      problemDetails.detail !== 'Validation failed'
    ) {
      return problemDetails.detail
    }

    // Si hay un title, usarlo
    if (problemDetails.title) {
      return problemDetails.title
    }
  }

  // Si es un string, usarlo directamente
  if (typeof error === 'string') {
    return error
  }

  // Si es un Error, usar su mensaje
  if (error instanceof Error) {
    return error.message
  }

  return 'Error desconocido'
}

/**
 * Función para manejar errores de API de forma consistente
 */
export function handleApiError(
  error: unknown,
  defaultMessage: string = 'Error de la aplicación'
): string {
  const message = getErrorMessage(error)
  return message !== 'Error desconocido' ? message : defaultMessage
}
