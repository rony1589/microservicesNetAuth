export type UserRole = 'Admin' | 'Usuario'

export type UserDto = {
  id: string
  email: string
  name: string
  role: string
  isActive: boolean
  createdAt: string
  updatedAt?: string
}

export type TokenResponseDto = {
  accessToken: string
  user: UserDto
}

export type LoginRequestDto = {
  email: string
  password: string
}

export type RegisterRequestDto = {
  email: string
  name: string
  password: string
  role?: UserRole // Completamente opcional
}
