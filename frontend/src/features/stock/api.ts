import type { Stock, StockMovement, StockMovementPayload } from './types'

async function request<T>(url: string, options?: RequestInit): Promise<T> {
  const response = await fetch(url, {
    headers: {
      'Content-Type': 'application/json',
      ...options?.headers,
    },
    ...options,
  })

  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: 'Request failed' }))
    throw new Error(error.message ?? 'Request failed')
  }

  return response.json()
}

export function getArticleStock(articleId: string): Promise<Stock> {
  return request<Stock>(`/api/articles/${articleId}/stock`)
}

export function createStockMovement(articleId: string, payload: StockMovementPayload): Promise<StockMovement> {
  return request<StockMovement>(`/api/articles/${articleId}/stock/movements`, {
    method: 'POST',
    body: JSON.stringify(payload),
  })
}
