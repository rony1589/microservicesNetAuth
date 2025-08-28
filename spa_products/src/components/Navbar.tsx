import { LogOut, Package, User, UserPlus } from 'lucide-react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuthStore } from '../store/authStore'

export default function Navbar() {
  const user = useAuthStore((s) => s.user)
  const logout = useAuthStore((s) => s.logout)
  const navigate = useNavigate()

  return (
    <nav className="futuristic-navbar">
      <div style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
        <Link
          to="/"
          className="futuristic-link"
          style={{
            display: 'flex',
            alignItems: 'center',
            gap: '0.5rem',
            fontSize: '1.2rem',
            fontWeight: '600',
          }}
        >
          <Package size={24} color="#667eea" />
          ProductHub
        </Link>
      </div>

      <div
        style={{
          marginLeft: 'auto',
          display: 'flex',
          alignItems: 'center',
          gap: '1rem',
        }}
      >
        {user ? (
          <>
            <div
              style={{
                display: 'flex',
                alignItems: 'center',
                gap: '0.5rem',
                padding: '0.5rem 1rem',
                background: 'rgba(255, 255, 255, 0.1)',
                borderRadius: '12px',
                border: '1px solid rgba(255, 255, 255, 0.2)',
              }}
            >
              <User size={16} color="#667eea" />
              <span style={{ color: 'rgba(255, 255, 255, 0.9)' }}>
                {user.name} ({user.role})
              </span>
            </div>
            <button
              onClick={() => {
                logout()
                navigate('/login')
              }}
              className="futuristic-button"
              style={{
                display: 'flex',
                alignItems: 'center',
                gap: '0.5rem',
                padding: '0.5rem 1rem',
                fontSize: '0.9rem',
              }}
            >
              <LogOut size={16} />
              Salir
            </button>
          </>
        ) : (
          <>
            <Link
              to="/register"
              className="futuristic-button"
              style={{
                display: 'flex',
                alignItems: 'center',
                gap: '0.5rem',
                textDecoration: 'none',
                padding: '0.5rem 1rem',
                fontSize: '0.9rem',
                background: 'linear-gradient(135deg, #51cf66 0%, #40c057 100%)',
              }}
            >
              <UserPlus size={16} />
              Registrarse
            </Link>
            <Link
              to="/login"
              className="futuristic-button"
              style={{
                display: 'flex',
                alignItems: 'center',
                gap: '0.5rem',
                textDecoration: 'none',
                padding: '0.5rem 1rem',
                fontSize: '0.9rem',
              }}
            >
              <User size={16} />
              Iniciar Sesión
            </Link>
          </>
        )}
      </div>
    </nav>
  )
}
