import {
  CheckCircle,
  DollarSign,
  Edit,
  Package,
  Plus,
  Tag,
  Trash2,
  XCircle,
} from 'lucide-react'
import { useEffect, useState } from 'react'
import toast from 'react-hot-toast'
import { Link } from 'react-router-dom'
import ProductForm from '../../components/ProductForm'
import {
  createProduct,
  deleteProduct,
  getProducts,
} from '../../service/productsService'
import type { ProblemDetails } from '../../types/problemDetails'
import type { ProductDto, ProductFormData } from '../../types/product'

export default function ProductsPage() {
  const [products, setProducts] = useState<ProductDto[]>([])
  const [loading, setLoading] = useState(true)
  const [showForm, setShowForm] = useState(false)

  const fetchProducts = async () => {
    try {
      setLoading(true)
      const data = await getProducts()
      setProducts(data)
    } catch (pd: unknown) {
      const error = pd as ProblemDetails
      toast.error(error?.title || error?.detail || 'Error al cargar productos')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    fetchProducts()
  }, [])

  const handleDelete = async (id: string) => {
    try {
      await deleteProduct(id)
      toast.success('Producto eliminado exitosamente')
      fetchProducts()
    } catch (pd: unknown) {
      const error = pd as ProblemDetails
      toast.error(error?.title || error?.detail || 'Error al eliminar producto')
    }
  }

  const onCreate = async (formData: ProductFormData) => {
    try {
      await createProduct(formData)
      toast.success('Producto creado exitosamente')
      setShowForm(false)
      fetchProducts()
    } catch (pd: unknown) {
      const error = pd as ProblemDetails
      toast.error(error?.title || error?.detail || 'Error al crear producto')
    }
  }

  if (loading) {
    return (
      <div className="futuristic-container fade-in-up">
        <div style={{ textAlign: 'center', padding: '2rem' }}>
          <div className="loading-spinner"></div>
          <p>Cargando productos...</p>
        </div>
      </div>
    )
  }

  return (
    <div className="futuristic-container fade-in-up">
      <div
        style={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          marginBottom: '2rem',
        }}
      >
        <h1 className="futuristic-title">
          <Package size={32} style={{ marginRight: '0.5rem' }} />
          Productos
        </h1>
        <button
          onClick={() => setShowForm(true)}
          className="futuristic-button"
          style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}
        >
          <Plus size={20} />
          Nuevo Producto
        </button>
      </div>

      {showForm && (
        <div className="modal-overlay">
          <div className="modal-content">
            <ProductForm
              onSubmit={onCreate}
              submitText="Crear Producto"
              defaultValues={{
                name: '',
                description: '',
                price: 0,
                category: '',
                status: true,
              }}
            />
            <button
              onClick={() => setShowForm(false)}
              className="futuristic-button"
              style={{ marginTop: '1rem', width: '100%' }}
            >
              Cancelar
            </button>
          </div>
        </div>
      )}

      <div className="futuristic-table-container">
        <table className="futuristic-table">
          <thead>
            <tr>
              <th>Nombre</th>
              <th>Descripción</th>
              <th>Precio</th>
              <th>Categoría</th>
              <th>Estado</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {products.map((product) => (
              <tr key={product.id}>
                <td>
                  <div
                    style={{
                      display: 'flex',
                      alignItems: 'center',
                      gap: '0.5rem',
                    }}
                  >
                    <Package size={16} />
                    {product.name}
                  </div>
                </td>
                <td>{product.description}</td>
                <td>
                  <div
                    style={{
                      display: 'flex',
                      alignItems: 'center',
                      gap: '0.5rem',
                    }}
                  >
                    <DollarSign size={16} />${product.price}
                  </div>
                </td>
                <td>
                  <div
                    style={{
                      display: 'flex',
                      alignItems: 'center',
                      gap: '0.5rem',
                    }}
                  >
                    <Tag size={16} />
                    {product.category}
                  </div>
                </td>
                <td>
                  {product.status ? (
                    <div
                      style={{
                        display: 'flex',
                        alignItems: 'center',
                        gap: '0.5rem',
                        color: '#10b981',
                      }}
                    >
                      <CheckCircle size={16} />
                      Activo
                    </div>
                  ) : (
                    <div
                      style={{
                        display: 'flex',
                        alignItems: 'center',
                        gap: '0.5rem',
                        color: '#ef4444',
                      }}
                    >
                      <XCircle size={16} />
                      Inactivo
                    </div>
                  )}
                </td>
                <td>
                  <div style={{ display: 'flex', gap: '0.5rem' }}>
                    <Link
                      to={`/products/${product.id}`}
                      className="futuristic-button-small"
                      style={{
                        display: 'flex',
                        alignItems: 'center',
                        gap: '0.25rem',
                      }}
                    >
                      <Edit size={14} />
                      Editar
                    </Link>
                    <button
                      onClick={() => handleDelete(product.id)}
                      className="futuristic-button-small danger"
                      style={{
                        display: 'flex',
                        alignItems: 'center',
                        gap: '0.25rem',
                      }}
                    >
                      <Trash2 size={14} />
                      Eliminar
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  )
}
