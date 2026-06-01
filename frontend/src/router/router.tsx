import { Navigate, createBrowserRouter } from 'react-router-dom'
import { AppLayout } from '@/components/layout/AppLayout'
import { ArticlesPage } from '@/features/articles/ArticlesPage'

export const router = createBrowserRouter([
  {
    path: '/',
    element: <AppLayout />,
    children: [
      {
        index: true,
        element: <Navigate to="/articles" replace />,
      },
      {
        path: 'articles',
        element: <ArticlesPage />,
      },
    ],
  },
])
