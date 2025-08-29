import { AlertTriangle, X } from 'lucide-react'

interface ConfirmModalProps {
  isOpen: boolean
  onClose: () => void
  onConfirm: () => void
  title: string
  message: string
  confirmText?: string
  cancelText?: string
  type?: 'danger' | 'warning' | 'info'
}

export default function ConfirmModal({
  isOpen,
  onClose,
  onConfirm,
  title,
  message,
  confirmText = 'Confirmar',
  cancelText = 'Cancelar',
  type = 'danger',
}: ConfirmModalProps) {
  if (!isOpen) return null

  const getTypeStyles = () => {
    switch (type) {
      case 'danger':
        return {
          iconColor: '#ef4444',
          buttonClass: 'futuristic-button danger',
        }
      case 'warning':
        return {
          iconColor: '#f59e0b',
          buttonClass: 'futuristic-button warning',
        }
      default:
        return {
          iconColor: '#3b82f6',
          buttonClass: 'futuristic-button',
        }
    }
  }

  const { iconColor, buttonClass } = getTypeStyles()

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        <div
          style={{
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
            marginBottom: '1.5rem',
          }}
        >
          <div
            style={{
              display: 'flex',
              alignItems: 'center',
              gap: '0.75rem',
            }}
          >
            <AlertTriangle size={24} color={iconColor} />
            <h2
              style={{
                margin: 0,
                fontSize: '1.25rem',
                fontWeight: '600',
                color: 'rgba(255, 255, 255, 0.9)',
              }}
            >
              {title}
            </h2>
          </div>
          <button
            onClick={onClose}
            style={{
              background: 'none',
              border: 'none',
              color: 'rgba(255, 255, 255, 0.6)',
              cursor: 'pointer',
              padding: '0.25rem',
              borderRadius: '4px',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
            }}
            onMouseEnter={(e) => {
              e.currentTarget.style.color = 'rgba(255, 255, 255, 0.9)'
            }}
            onMouseLeave={(e) => {
              e.currentTarget.style.color = 'rgba(255, 255, 255, 0.6)'
            }}
          >
            <X size={20} />
          </button>
        </div>

        <p
          style={{
            margin: '0 0 2rem 0',
            color: 'rgba(255, 255, 255, 0.8)',
            lineHeight: '1.5',
            fontSize: '1rem',
          }}
        >
          {message}
        </p>

        <div
          style={{
            display: 'flex',
            gap: '1rem',
            justifyContent: 'flex-end',
          }}
        >
          <button
            onClick={onClose}
            className="futuristic-button"
            style={{
              background: 'rgba(255, 255, 255, 0.1)',
              border: '1px solid rgba(255, 255, 255, 0.2)',
            }}
          >
            {cancelText}
          </button>
          <button onClick={onConfirm} className={buttonClass}>
            {confirmText}
          </button>
        </div>
      </div>
    </div>
  )
}
