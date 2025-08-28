# ProductHub - Gestión de Productos

Una aplicación web moderna para la gestión de productos con diseño futurista y funcionalidades completas de autenticación.

## 🚀 Características

- **Diseño Futurista**: Interfaz moderna con efectos glassmorphism, gradientes y animaciones
- **Autenticación Completa**: Login y registro de usuarios
- **Gestión de Productos**: CRUD completo para productos
- **Responsive**: Diseño adaptable a diferentes dispositivos
- **TypeScript**: Tipado completo para mayor seguridad
- **Validación**: Formularios validados con Zod

## 🛠️ Tecnologías

- **Frontend**: React 19 + TypeScript + Vite
- **UI**: CSS personalizado con efectos futuristas
- **Iconos**: Lucide React
- **Formularios**: React Hook Form + Zod
- **Estado**: Zustand
- **HTTP**: Axios
- **Notificaciones**: React Hot Toast
- **Enrutamiento**: React Router DOM

## 📋 Requisitos Previos

- Node.js 18+
- pnpm (recomendado) o npm
- API Gateway configurado en puerto 5000

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

- `GET /api/products` - Listar productos
- `GET /api/products/:id` - Obtener producto
- `POST /api/products` - Crear producto
- `PUT /api/products/:id` - Actualizar producto
- `DELETE /api/products/:id` - Eliminar producto

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

**Error Response:**

```json
{
  "title": "Error Title",
  "status": 400,
  "detail": "Error description"
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

## 📱 Funcionalidades

### Autenticación

- ✅ Login con email y contraseña
- ✅ Registro de nuevos usuarios
- ✅ Validación de formularios
- ✅ Manejo de errores
- ✅ Persistencia de sesión

### Gestión de Productos

- ✅ Listar productos con tabla futurista
- ✅ Crear nuevos productos
- ✅ Editar productos existentes
- ✅ Eliminar productos
- ✅ Estados visuales (activo/inactivo)
- ✅ Validación de formularios

### Navegación

- ✅ Navbar responsive
- ✅ Rutas protegidas
- ✅ Redirección automática
- ✅ Breadcrumbs visuales

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
- Manejo seguro de tokens JWT
- Protección de rutas
- Sanitización de datos

## 🐛 Troubleshooting

### Error: "UnableToFindDownstreamRouteError"

Este error indica que las rutas del gateway no están configuradas correctamente.

**Soluciones:**

1. **Verificar la URL del gateway:**

   ```env
   VITE_API_BASE_URL=http://localhost:5000
   ```

2. **Verificar que el gateway esté corriendo:**

   ```bash
   curl http://localhost:5000/health
   ```

3. **Verificar las rutas del gateway:**

   - Asegúrate de que las rutas `/users/login` y `/users/register` estén configuradas
   - Verifica que las rutas `/api/products/*` estén configuradas
   - Confirma que el gateway esté enrutando correctamente a los microservicios

4. **Verificar CORS:**

   Asegúrate de que el gateway permita peticiones desde `http://localhost:5173`

### Error: "Failed to match Route configuration"

Este error sugiere un problema de configuración de rutas en el gateway.

**Soluciones:**

1. **Verificar la configuración de rutas en el gateway**
2. **Asegurar que los endpoints estén habilitados**
3. **Verificar que el middleware de autenticación esté configurado correctamente**

## 📦 Estructura del Proyecto

```
src/
├── app/                 # Configuración de la app
├── components/          # Componentes reutilizables
├── lib/                 # Utilidades y configuración
│   ├── api.ts          # Configuración de Axios
│   └── config.ts       # Configuración de rutas API
├── page/                # Páginas de la aplicación
│   ├── login/          # Página de login
│   ├── register/       # Página de registro
│   └── product/        # Páginas de productos
├── service/            # Servicios de API
├── store/              # Estado global (Zustand)
├── types/              # Tipos TypeScript
└── index.css           # Estilos globales
```

## 🎯 Próximas Mejoras

- [ ] Dashboard con estadísticas
- [ ] Filtros y búsqueda de productos
- [ ] Exportación de datos
- [ ] Temas personalizables
- [ ] Modo oscuro/claro
- [ ] Notificaciones push
- [ ] PWA (Progressive Web App)

## 🤝 Contribución

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo `LICENSE` para más detalles.

## 📞 Soporte

Si tienes alguna pregunta o problema, por favor abre un issue en el repositorio.
