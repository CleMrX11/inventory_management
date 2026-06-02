import { Navigate, createBrowserRouter } from 'react-router-dom'
import { AppLayout } from '@/components/layout/AppLayout'
import { ArticlesPage } from '@/features/articles/ArticlesPage'
import { StockMovementsPage } from '@/features/stock/StockMovementsPage'
import { StockPage } from '@/features/stock/StockPage'

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
      {
        path: 'stock',
        element: <StockPage />,
      },
      {
        path: 'stock/:articleId/movements',
        element: <StockMovementsPage />,
      },
    ],
  },
])
