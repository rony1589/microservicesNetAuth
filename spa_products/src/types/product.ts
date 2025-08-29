export type ProductDto = {
  id: string
  name: string
  description: string
  price: number
  category: string
  status: boolean
  createdAt: string
  updatedAt?: string
}

export type ProductFormData = {
  name: string
  description: string
  price: number
  category: string
  status?: boolean
}

export type CreateProductRequestDto = {
  name: string
  description: string
  price: number
  category: string
  status?: boolean
}

export type UpdateProductRequestDto = {
  name: string
  description: string
  price: number
  category: string
  status?: boolean
}
