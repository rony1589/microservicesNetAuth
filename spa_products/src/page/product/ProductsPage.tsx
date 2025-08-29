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
import ConfirmModal from '../../components/ConfirmModal'
import { handleApiError } from '../../lib/errorHandler'
import { deleteProduct, getProducts } from '../../service/productsService'
import type { ProductDto } from '../../types/product'

export default function ProductsPage() {
  const [products, setProducts] = useState<ProductDto[]>([])
  const [loading, setLoading] = useState(true)
  const [deleteModal, setDeleteModal] = useState<{
    isOpen: boolean
    product: ProductDto | null
  }>({
    isOpen: false,
    product: null,
  })

  const fetchProducts = async () => {
    try {
      setLoading(true)
      const data = await getProducts()
      setProducts(data)
    } catch (error: unknown) {
      toast.error(handleApiError(error, 'Error al cargar productos'))
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    fetchProducts()
  }, [])

  const handleDeleteClick = (product: ProductDto) => {
    setDeleteModal({
      isOpen: true,
      product,
    })
  }

  const handleDeleteConfirm = async () => {
    if (!deleteModal.product) return

    try {
      await deleteProduct(deleteModal.product.id)
      toast.success('Producto eliminado exitosamente')
      setDeleteModal({ isOpen: false, product: null })
      fetchProducts()
    } catch (error: unknown) {
      toast.error(handleApiError(error, 'Error al eliminar producto'))
    }
  }

  const handleDeleteCancel = () => {
    setDeleteModal({ isOpen: false, product: null })
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
        <Link to="/products/create">
          <button
            className="futuristic-button"
            style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}
          >
            <Plus size={20} />
            Nuevo Producto
          </button>
        </Link>
      </div>

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
                      onClick={() => handleDeleteClick(product)}
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

      <ConfirmModal
        isOpen={deleteModal.isOpen}
        onConfirm={handleDeleteConfirm}
        onClose={handleDeleteCancel}
        title="Eliminar Producto"
        message={`¿Estás seguro de que quieres eliminar el producto "${deleteModal.product?.name}"? Esta acción no se puede deshacer.`}
        confirmText="Eliminar"
        cancelText="Cancelar"
        type="danger"
      />
    </div>
  )
}
