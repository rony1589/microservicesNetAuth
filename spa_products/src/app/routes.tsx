import { createBrowserRouter, Navigate } from 'react-router-dom'
import Layout from '../components/Layout'
import ProtectedRoute from '../components/ProtectedRoute'
import LoginPage from '../page/login/LoginPage'
import ProductEditPage from '../page/product/ProductEditPage'
import ProductsPage from '../page/product/ProductsPage'
import RegisterPage from '../page/register/RegisterPage'

export const router = createBrowserRouter([
  {
    path: '/login',
    element: <LoginPage />,
  },
  {
    path: '/register',
    element: <RegisterPage />,
  },
  {
    path: '/',
    element: (
      <Layout>
        <ProtectedRoute>
          <ProductsPage />
        </ProtectedRoute>
      </Layout>
    ),
  },
  {
    path: '/products/:id',
    element: (
      <Layout>
        <ProtectedRoute>
          <ProductEditPage />
        </ProtectedRoute>
      </Layout>
    ),
  },
  {
    path: '*',
    element: <Navigate to="/login" replace />,
  },
])
