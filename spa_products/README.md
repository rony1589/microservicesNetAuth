# ProductHub - Gestión de Productos

Una aplicación web moderna para la gestión de productos con diseño futurista, sistema de autenticación robusto y funcionalidades completas de CRUD.

## 🚀 Características

- **Diseño Futurista**: Interfaz moderna con efectos glassmorphism, gradientes y animaciones
- **Autenticación Robusta**: Sistema completo de login, registro y gestión de tokens JWT
- **Validación de Tokens**: Verificación automática de expiración y renovación
- **Gestión de Productos**: CRUD completo para productos con validación
- **Manejo de Errores**: Sistema centralizado de manejo de errores con mensajes descriptivos
- **Responsive**: Diseño adaptable a diferentes dispositivos
- **TypeScript**: Tipado completo para mayor seguridad
- **Validación**: Formularios validados con Zod y validación de backend

## 🛠️ Tecnologías

- **Frontend**: React 19 + TypeScript + Vite
- **UI**: CSS personalizado con efectos futuristas
- **Iconos**: Lucide React
- **Formularios**: React Hook Form + Zod
- **Estado**: Zustand
- **HTTP**: Fetch API con interceptores personalizados
- **Notificaciones**: React Hot Toast
- **Enrutamiento**: React Router DOM
- **Autenticación**: JWT con validación automática

## 📋 Requisitos Previos

- Node.js 18+
- pnpm (recomendado) o npm
- API Gateway configurado en puerto 5000
- Microservicios de autenticación y productos funcionando

## 🔧 Instalación

1. **Clonar el repositorio**

   ```bash
   git clone <repository-url>
   cd spa_products
   ```

2. **Instalar dependencias**

   ```bash
   pnpm install
   ```

3. **Configurar variables de entorno**
   Crear un archivo `.env` en la raíz del proyecto:

   ```env
   VITE_API_BASE_URL=http://localhost:5000
   ```

   **Ejemplos de URLs comunes:**

   - `http://localhost:5000` (puerto por defecto del gateway)
   - `http://localhost:3000`
   - `https://tu-gateway.com`

4. **Ejecutar en desarrollo**

   ```bash
   pnpm run dev
   ```

## 🔐 Sistema de Autenticación

### Características del Sistema

- **Persistencia de Token**: Almacenamiento seguro en localStorage
- **Validación Automática**: Verificación de expiración de tokens JWT
- **Logout Automático**: Cierre de sesión automático cuando el token expira
- **Protección de Rutas**: Acceso restringido a usuarios autenticados
- **Indicador Visual**: Muestra el tiempo restante del token
- **Manejo de Errores 401**: Logout automático en respuestas no autorizadas

### Flujo de Autenticación

1. **Login/Registro**: Usuario se autentica y recibe token JWT
2. **Persistencia**: Token se guarda en localStorage
3. **Validación Periódica**: Se verifica la validez del token cada 30 segundos
4. **Protección**: Todas las rutas de productos requieren autenticación
5. **Expiración**: Logout automático cuando el token expira

### Componentes de Seguridad

- **TokenValidator**: Validación y decodificación de tokens JWT
- **ProtectedRoute**: Protección de rutas con verificación de token
- **TokenStatus**: Indicador visual del tiempo restante del token
- **ErrorHandler**: Manejo centralizado de errores de autenticación

## 🗄️ Estructura de la Base de Datos

### Tabla Users

```sql
CREATE TABLE users (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    email text NOT NULL UNIQUE,
    name text NOT NULL,
    password_hash text NOT NULL,
    password_salt text NOT NULL,
    role text NOT NULL,
    is_active boolean NOT NULL DEFAULT true,
    created_at timestamp with time zone NOT NULL DEFAULT now(),
    updated_at timestamp with time zone
);
```

### Tabla Products

```sql
CREATE TABLE products (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    name text NOT NULL,
    description text NOT NULL,
    price numeric(18,2) NOT NULL,
    category text NOT NULL,
    status boolean NOT NULL DEFAULT true,
    created_at timestamp with time zone NOT NULL DEFAULT now(),
    updated_at timestamp with time zone
);
```

## 🔌 Endpoints de la API (Gateway)

### Autenticación (No requieren token)

- `POST /users/login` - Iniciar sesión
- `POST /users/register` - Registrar usuario

### Productos (Requieren token)

- `GET /products/all` - Listar productos
- `GET /products/:id` - Obtener producto específico
- `POST /products/create` - Crear producto
- `PUT /products/:id` - Actualizar producto
- `DELETE /products/:id` - Eliminar producto

## 🔧 Configuración del Backend

### Estructura de Respuesta Esperada

**Login/Register Response:**

```json
{
  "accessToken": "jwt-token-here",
  "user": {
    "id": "uuid",
    "email": "user@example.com",
    "name": "User Name",
    "role": "user",
    "isActive": true,
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-01T00:00:00Z"
  }
}
```

**Error Response (ProblemDetails):**

```json
{
  "type": "https://errors.yourdomain.com/common/validation-failed",
  "title": "Validation failed",
  "status": 400,
  "detail": "Validation failed",
  "instance": {
    "value": "/api/products/update/123",
    "hasValue": true
  },
  "extensions": {
    "errorCode": "VALIDATION_FAILED",
    "correlationId": "3ba597d2-a8bd-41d6-b936-8a53dc4f4cf2",
    "traceId": "0HNF6J8PV6B02:00000001",
    "errors": {
      "id": ["'Id' no debería estar vacío."],
      "name": ["El nombre es requerido"]
    }
  }
}
```

### Headers Requeridos

- `Content-Type: application/json`
- `Authorization: Bearer <token>` (solo para rutas de productos)

## 🎨 Características del Diseño

- **Fondo**: Gradiente oscuro con efectos de partículas
- **Glassmorphism**: Contenedores translúcidos con blur
- **Animaciones**: Efectos de entrada y hover
- **Iconos**: Lucide React para consistencia visual
- **Colores**: Paleta de azules, púrpuras y acentos
- **Notificaciones**: Toast centrados en la parte inferior
- **Indicadores de Estado**: Colores dinámicos para el estado del token

## 📱 Funcionalidades

### Autenticación

- ✅ Login con email y contraseña
- ✅ Registro de nuevos usuarios
- ✅ Validación de formularios con Zod
- ✅ Manejo de errores centralizado
- ✅ Persistencia de sesión con localStorage
- ✅ Validación automática de tokens JWT
- ✅ Logout automático en expiración
- ✅ Indicador visual del tiempo restante del token
- ✅ Protección de rutas con redirección automática

### Gestión de Productos

- ✅ Listar productos con tabla futurista
- ✅ Crear nuevos productos con validación
- ✅ Editar productos existentes
- ✅ Eliminar productos con confirmación
- ✅ Estados visuales (activo/inactivo)
- ✅ Validación de formularios en frontend y backend
- ✅ Manejo de errores de validación específicos
- ✅ Inclusión automática del ID en actualizaciones

### Navegación

- ✅ Navbar responsive con información del usuario
- ✅ Rutas protegidas con verificación de token
- ✅ Redirección automática a login
- ✅ Breadcrumbs visuales
- ✅ Botón de logout con redirección

### Manejo de Errores

- ✅ Sistema centralizado de manejo de errores
- ✅ Extracción de errores de validación específicos
- ✅ Mensajes de error descriptivos en toasts
- ✅ Manejo automático de errores 401 (Unauthorized)
- ✅ Logout automático en tokens inválidos

## 🚀 Scripts Disponibles

```bash
# Desarrollo
pnpm run dev

# Construcción para producción
pnpm run build

# Vista previa de producción
pnpm run preview

# Linting
pnpm run lint
```

## 🔒 Seguridad

- Validación de formularios en frontend y backend
- Manejo seguro de tokens JWT con verificación de expiración
- Protección de rutas con verificación automática
- Sanitización de datos
- Logout automático en tokens expirados
- Manejo seguro de errores sin exponer información sensible

## 🏗️ Arquitectura del Proyecto

### Estructura de Archivos

```
src/
├── app/                    # Configuración de la app
│   ├── App.tsx            # Componente principal con verificación de token
│   └── routes.tsx         # Configuración de rutas protegidas
├── components/             # Componentes reutilizables
│   ├── Navbar.tsx         # Barra de navegación con TokenStatus
│   ├── ProductForm.tsx    # Formulario de productos con validación
│   ├── ProtectedRoute.tsx # Protección de rutas con verificación de token
│   └── TokenStatus.tsx    # Indicador visual del estado del token
├── lib/                   # Utilidades y configuración
│   ├── api.ts            # Configuración de interceptores HTTP
│   ├── config.ts         # Configuración de rutas API
│   ├── errorHandler.ts   # Sistema centralizado de manejo de errores
│   ├── logger.ts         # Sistema de logging estructurado
│   └── tokenValidator.ts # Validación y manejo de tokens JWT
├── page/                  # Páginas de la aplicación
│   ├── login/            # Página de login
│   ├── register/         # Página de registro
│   └── product/          # Páginas de productos (CRUD)
├── service/              # Servicios de API
│   ├── authService.ts    # Servicios de autenticación
│   └── productsService.ts # Servicios de productos con manejo de errores
├── store/                # Estado global (Zustand)
│   └── authStore.ts      # Store de autenticación con persistencia
├── types/                # Tipos TypeScript
│   ├── product.ts        # Tipos de productos
│   ├── user.ts           # Tipos de usuario
│   └── problemDetails.ts # Tipos de errores
└── index.css             # Estilos globales
```

### Flujo de Datos

1. **Autenticación**: Login/Register → Token JWT → localStorage
2. **Validación**: Verificación periódica → Logout automático si expira
3. **Protección**: ProtectedRoute → Verificación de token → Redirección
4. **API Calls**: Interceptores → Manejo de errores → Toast notifications
5. **Estado**: Zustand store → Persistencia → Sincronización

## 🐛 Troubleshooting

### Error: "UnableToFindDownstreamRouteError"

Este error indica que las rutas del gateway no están configuradas correctamente.

**Soluciones:**

1. **Verificar la URL del gateway:**

   ```env
   VITE_API_BASE_URL=http://localhost:5000
   ```

2. **Verificar las rutas del gateway:**

   - Asegúrate de que las rutas `/users/login` y `/users/register` estén configuradas
   - Verifica que las rutas `/products/*` estén configuradas correctamente
   - Confirma que el gateway esté enrutando correctamente a los microservicios

### Error: "Sesión expirada"

Este error indica que el token JWT ha expirado.

**Soluciones:**

1. **Reiniciar sesión**: El sistema hará logout automático y redirigirá a login
2. **Verificar configuración del backend**: Asegurar que los tokens JWT tengan tiempo de expiración adecuado
3. **Verificar sincronización de tiempo**: Asegurar que el servidor y cliente tengan hora sincronizada

### Error: "Validation failed"

Este error indica problemas de validación en el backend.

**Soluciones:**

1. **Revisar datos enviados**: Verificar que todos los campos requeridos estén presentes
2. **Verificar formato de datos**: Asegurar que los tipos de datos sean correctos
3. **Revisar logs del backend**: Verificar los errores específicos de validación

### Error: "Failed to match Route configuration"

Este error sugiere un problema de configuración de rutas en el gateway.

**Soluciones:**

1. **Verificar la configuración de rutas en el gateway**
2. **Asegurar que los endpoints estén habilitados**
3. **Verificar que el middleware de autenticación esté configurado correctamente**

## 📄 Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo `LICENSE` para más detalles.
