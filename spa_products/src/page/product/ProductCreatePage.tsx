import { ArrowLeft, Plus } from 'lucide-react'
import toast from 'react-hot-toast'
import { useNavigate } from 'react-router-dom'
import ProductForm from '../../components/ProductForm'
import { handleApiError } from '../../lib/errorHandler'
import { createProduct } from '../../service/productsService'
import type { ProductFormData } from '../../types/product'

export default function ProductCreatePage() {
  const navigate = useNavigate()

  async function onSubmit(form: ProductFormData) {
    try {
      await createProduct({
        ...form,
        status: form.status ?? true,
      })
      toast.success('Producto creado exitosamente')
      navigate('/')
    } catch (error: unknown) {
      toast.error(handleApiError(error, 'Error al crear el producto'))
    }
  }

  return (
    <div style={{ padding: '2rem', maxWidth: '1200px', margin: '0 auto' }}>
      <div className="fade-in-up">
        <div
          style={{
            display: 'flex',
            alignItems: 'center',
            gap: '1rem',
            marginBottom: '2rem',
          }}
        >
          <button
            onClick={() => navigate('/')}
            className="futuristic-button"
            style={{
              display: 'flex',
              alignItems: 'center',
              gap: '0.5rem',
              padding: '0.5rem 1rem',
              fontSize: '0.9rem',
            }}
          >
            <ArrowLeft size={16} />
            Volver
          </button>
        </div>

        <div className="futuristic-container">
          <div
            style={{
              display: 'flex',
              alignItems: 'center',
              gap: '0.5rem',
              marginBottom: '2rem',
            }}
          >
            <Plus size={32} color="#667eea" />
            <h1 className="futuristic-title" style={{ margin: 0 }}>
              Crear Nuevo Producto
            </h1>
          </div>

          <ProductForm onSubmit={onSubmit} submitText="Crear Producto" />
        </div>
      </div>
    </div>
  )
}
