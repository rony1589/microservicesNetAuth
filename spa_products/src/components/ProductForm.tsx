import { zodResolver } from '@hookform/resolvers/zod'
import { CheckSquare, DollarSign, FileText, Package, Tag } from 'lucide-react'
import { useForm } from 'react-hook-form'
import { z } from 'zod'
import type { ProductFormData } from '../types/product'

const schema = z.object({
  name: z.string().min(2),
  description: z.string().min(2),
  price: z.number().min(0),
  category: z.string().min(2),
  status: z.boolean().optional(), // para edición
})

type FormData = z.infer<typeof schema>

export default function ProductForm({
  defaultValues,
  onSubmit,
  submitText = 'Guardar',
}: {
  defaultValues?: Partial<ProductFormData>
  onSubmit: (data: ProductFormData) => void | Promise<void>
  submitText?: string
}) {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormData>({
    resolver: zodResolver(schema),
    defaultValues: defaultValues as FormData,
  })

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="futuristic-form">
      <div>
        <div style={{ position: 'relative', marginBottom: '0.5rem' }}>
          <Package
            size={20}
            style={{
              position: 'absolute',
              left: '12px',
              top: '50%',
              transform: 'translateY(-50%)',
              color: 'rgba(255, 255, 255, 0.6)',
            }}
          />
          <input
            {...register('name')}
            placeholder="Nombre del producto"
            className="futuristic-input"
            style={{ paddingLeft: '44px' }}
          />
        </div>
        {errors.name && (
          <div className="error-message">{errors.name.message}</div>
        )}
      </div>

      <div>
        <div style={{ position: 'relative', marginBottom: '0.5rem' }}>
          <FileText
            size={20}
            style={{
              position: 'absolute',
              left: '12px',
              top: '50%',
              transform: 'translateY(-50%)',
              color: 'rgba(255, 255, 255, 0.6)',
            }}
          />
          <textarea
            {...register('description')}
            placeholder="Descripción del producto"
            className="futuristic-input"
            style={{
              paddingLeft: '44px',
              minHeight: '100px',
              resize: 'vertical',
            }}
          />
        </div>
        {errors.description && (
          <div className="error-message">{errors.description.message}</div>
        )}
      </div>

      <div>
        <div style={{ position: 'relative', marginBottom: '0.5rem' }}>
          <DollarSign
            size={20}
            style={{
              position: 'absolute',
              left: '12px',
              top: '50%',
              transform: 'translateY(-50%)',
              color: 'rgba(255, 255, 255, 0.6)',
            }}
          />
          <input
            type="number"
            step="0.01"
            placeholder="0.00"
            {...register('price', { valueAsNumber: true })}
            className="futuristic-input"
            style={{ paddingLeft: '44px' }}
          />
        </div>
        {errors.price && (
          <div className="error-message">{errors.price.message}</div>
        )}
      </div>

      <div>
        <div style={{ position: 'relative', marginBottom: '0.5rem' }}>
          <Tag
            size={20}
            style={{
              position: 'absolute',
              left: '12px',
              top: '50%',
              transform: 'translateY(-50%)',
              color: 'rgba(255, 255, 255, 0.6)',
            }}
          />
          <input
            {...register('category')}
            placeholder="Categoría"
            className="futuristic-input"
            style={{ paddingLeft: '44px' }}
          />
        </div>
        {errors.category && (
          <div className="error-message">{errors.category.message}</div>
        )}
      </div>

      {defaultValues?.status !== undefined && (
        <div
          style={{
            display: 'flex',
            alignItems: 'center',
            gap: '0.5rem',
            padding: '0.75rem',
            background: 'rgba(255, 255, 255, 0.05)',
            borderRadius: '12px',
            border: '1px solid rgba(255, 255, 255, 0.1)',
          }}
        >
          <CheckSquare size={20} color="#667eea" />
          <label
            style={{
              display: 'flex',
              alignItems: 'center',
              gap: '0.5rem',
              cursor: 'pointer',
              color: 'rgba(255, 255, 255, 0.9)',
            }}
          >
            <input
              type="checkbox"
              {...register('status')}
              style={{
                width: '18px',
                height: '18px',
                accentColor: '#667eea',
              }}
            />
            Producto activo
          </label>
        </div>
      )}

      <button
        type="submit"
        className="futuristic-button"
        style={{ marginTop: '1rem' }}
      >
        {submitText}
      </button>
    </form>
  )
}
