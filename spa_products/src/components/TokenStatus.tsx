import { Clock, Shield, ShieldAlert } from 'lucide-react'
import { useEffect, useState } from 'react'
import { getTokenTimeRemaining, isTokenValid } from '../lib/tokenValidator'
import { useAuthStore } from '../store/authStore'

export default function TokenStatus() {
  const token = useAuthStore((s) => s.token)
  const [timeRemaining, setTimeRemaining] = useState<number>(0)

  useEffect(() => {
    if (!token) {
      setTimeRemaining(0)
      return
    }

    const updateTimeRemaining = () => {
      const remaining = getTokenTimeRemaining(token)
      setTimeRemaining(remaining)
    }

    // Actualizar inmediatamente
    updateTimeRemaining()

    // Actualizar cada minuto
    const interval = setInterval(updateTimeRemaining, 60000)

    return () => clearInterval(interval)
  }, [token])

  if (!token || !isTokenValid(token)) {
    return null
  }

  const formatTime = (seconds: number): string => {
    if (seconds < 60) {
      return `${seconds}s`
    }
    const minutes = Math.floor(seconds / 60)
    const remainingSeconds = seconds % 60
    return `${minutes}m ${remainingSeconds}s`
  }

  const getStatusColor = (seconds: number): string => {
    if (seconds < 300) return '#ef4444' // Rojo si quedan menos de 5 minutos
    if (seconds < 900) return '#f59e0b' // Amarillo si quedan menos de 15 minutos
    return '#10b981' // Verde
  }

  const getStatusIcon = (seconds: number) => {
    if (seconds < 300) return <ShieldAlert size={14} />
    return <Shield size={14} />
  }

  return (
    <div
      style={{
        display: 'flex',
        alignItems: 'center',
        gap: '0.5rem',
        padding: '0.25rem 0.75rem',
        background: 'rgba(255, 255, 255, 0.05)',
        borderRadius: '8px',
        border: '1px solid rgba(255, 255, 255, 0.1)',
        fontSize: '0.8rem',
      }}
    >
      {getStatusIcon(timeRemaining)}
      <Clock size={14} color={getStatusColor(timeRemaining)} />
      <span style={{ color: getStatusColor(timeRemaining) }}>
        {formatTime(timeRemaining)}
      </span>
    </div>
  )
}
