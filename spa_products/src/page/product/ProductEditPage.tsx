import { ArrowLeft, Edit3 } from 'lucide-react'
import { useEffect, useState } from 'react'
import toast from 'react-hot-toast'
import { useNavigate, useParams } from 'react-router-dom'
import ProductForm from '../../components/ProductForm'
import { getProduct, updateProduct } from '../../service/productsService'
import type { ProblemDetails } from '../../types/problemDetails'
import type { ProductDto, ProductFormData } from '../../types/product'

export default function ProductEditPage() {
  const { id } = useParams<{ id: string }>()
  const [entity, setEntity] = useState<ProductDto | null>(null)
  const [loading, setLoading] = useState(true)
  const navigate = useNavigate()

  useEffect(() => {
    ;(async () => {
      try {
        if (!id) return
        setLoading(true)
        const data = await getProduct(id)
        setEntity(data)
      } catch (pd: unknown) {
        const error = pd as ProblemDetails
        toast.error(error?.title || 'No se pudo cargar el producto')
      } finally {
        setLoading(false)
      }
    })()
  }, [id])

  async function onSubmit(form: ProductFormData) {
    try {
      if (!id) return
      await updateProduct(id, {
        ...form,
        status: form.status ?? entity?.status ?? true,
      })
      toast.success('Producto actualizado exitosamente')
      navigate('/')
    } catch (pd: unknown) {
      const error = pd as ProblemDetails
      toast.error(error?.title || 'Error al actualizar el producto')
    }
  }

  if (loading) {
    return (
      <div style={{ padding: '2rem', maxWidth: '800px', margin: '0 auto' }}>
        <div
          className="futuristic-container"
          style={{ textAlign: 'center', padding: '3rem' }}
        >
          <div
            style={{ fontSize: '1.2rem', color: 'rgba(255, 255, 255, 0.7)' }}
          >
            Cargando producto...
          </div>
        </div>
      </div>
    )
  }

  if (!entity) {
    return (
      <div style={{ padding: '2rem', maxWidth: '800px', margin: '0 auto' }}>
        <div
          className="futuristic-container"
          style={{ textAlign: 'center', padding: '3rem' }}
        >
          <div style={{ fontSize: '1.2rem', color: '#ff6b6b' }}>
            Producto no encontrado
          </div>
        </div>
      </div>
    )
  }

  return (
    <div style={{ padding: '2rem', maxWidth: '800px', margin: '0 auto' }}>
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
            <Edit3 size={32} color="#667eea" />
            <h1 className="futuristic-title" style={{ margin: 0 }}>
              Editar Producto
            </h1>
          </div>

          <ProductForm
            defaultValues={{
              name: entity.name,
              description: entity.description,
              price: entity.price,
              category: entity.category,
              status: entity.status,
            }}
            onSubmit={onSubmit}
            submitText="Actualizar Producto"
          />
        </div>
      </div>
    </div>
  )
}
