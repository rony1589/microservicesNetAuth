import { zodResolver } from '@hookform/resolvers/zod'
import { ArrowRight, Lock, Mail, Sparkles } from 'lucide-react'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { useNavigate } from 'react-router-dom'
import { z } from 'zod'
import { login } from '../../service/authService'
import { useAuthStore } from '../../store/authStore'
import type { ProblemDetails } from '../../types/problemDetails'

const schema = z.object({
  email: z.string().email('Email inválido'),
  password: z.string().min(1, 'La contraseña es requerida'),
})

type FormData = z.infer<typeof schema>

export default function LoginPage() {
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<FormData>({ resolver: zodResolver(schema) })
  const doLogin = useAuthStore((s) => s.login)
  const navigate = useNavigate()

  const onSubmit = async (form: FormData) => {
    try {
      const res = await login(form)
      doLogin(res)
      toast.success('Inicio de sesión exitoso')
      navigate('/')
    } catch (pd: unknown) {
      const error = pd as ProblemDetails
      console.error('Error completo:', error)

      // Mostrar el mensaje de error más específico disponible
      const errorMessage =
        error?.detail || error?.title || 'Error en el inicio de sesión'
      toast.error(errorMessage)
    }
  }

  return (
    <div
      style={{
        minHeight: '100vh',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        padding: '2rem',
      }}
    >
      <div
        className="futuristic-container fade-in-up"
        style={{ maxWidth: '500px', width: '100%' }}
      >
        {/* Header con icono */}
        <div style={{ textAlign: 'center', marginBottom: '2rem' }}>
          <div
            style={{
              display: 'inline-flex',
              alignItems: 'center',
              gap: '0.5rem',
              marginBottom: '1rem',
            }}
          >
            <Sparkles size={32} color="#667eea" />
            <span
              style={{
                fontSize: '1.5rem',
                fontWeight: '700',
                background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                WebkitBackgroundClip: 'text',
                WebkitTextFillColor: 'transparent',
                backgroundClip: 'text',
              }}
            >
              ProductHub
            </span>
          </div>
          <h1 className="futuristic-title">Iniciar Sesión</h1>
          <p className="futuristic-subtitle">
            Accede a tu cuenta para gestionar productos
          </p>
        </div>

        {/* Formulario */}
        <form
          onSubmit={handleSubmit(onSubmit)}
          style={{
            display: 'flex',
            flexDirection: 'column',
            gap: '1.5rem',
            width: '100%',
            maxWidth: '400px',
            margin: '0 auto',
          }}
        >
          <div>
            <div style={{ position: 'relative', marginBottom: '0.5rem' }}>
              <Mail
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
                {...register('email')}
                type="email"
                placeholder="tu@empresa.com"
                className="futuristic-input"
                style={{
                  paddingLeft: '44px',
                  width: '100%',
                  boxSizing: 'border-box',
                }}
              />
            </div>
            {errors.email && (
              <div className="error-message">{errors.email.message}</div>
            )}
          </div>

          <div>
            <div style={{ position: 'relative', marginBottom: '0.5rem' }}>
              <Lock
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
                {...register('password')}
                type="password"
                placeholder="••••••••"
                className="futuristic-input"
                style={{
                  paddingLeft: '44px',
                  width: '100%',
                  boxSizing: 'border-box',
                }}
              />
            </div>
            {errors.password && (
              <div className="error-message">{errors.password.message}</div>
            )}
          </div>

          <button
            type="submit"
            disabled={isSubmitting}
            className="futuristic-button"
            style={{
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              gap: '0.5rem',
              marginTop: '1rem',
              width: '100%',
            }}
          >
            {isSubmitting ? (
              'Iniciando sesión...'
            ) : (
              <>
                Iniciar Sesión
                <ArrowRight size={20} />
              </>
            )}
          </button>

          <div style={{ textAlign: 'center', marginTop: '1.5rem' }}>
            <span style={{ color: 'rgba(255, 255, 255, 0.7)' }}>
              ¿No tienes una cuenta?{' '}
            </span>
            <button
              type="button"
              onClick={() => navigate('/register')}
              className="futuristic-link"
              style={{
                background: 'none',
                border: 'none',
                cursor: 'pointer',
                fontSize: 'inherit',
              }}
            >
              Registrarse
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}
