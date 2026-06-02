import type { Article, ArticlePayload } from './types'

const articlesUrl = '/api/articles'

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

  if (response.status === 204) {
    return undefined as T
  }

  return response.json()
}

export function listArticles(): Promise<Article[]> {
  return request<Article[]>(articlesUrl)
}

export function getArticle(id: string): Promise<Article> {
  return request<Article>(`${articlesUrl}/${id}`)
}

export function createArticle(payload: ArticlePayload): Promise<Article> {
  return request<Article>(articlesUrl, {
    method: 'POST',
    body: JSON.stringify(payload),
  })
}

export function updateArticle(id: string, payload: ArticlePayload): Promise<Article> {
  return request<Article>(`${articlesUrl}/${id}`, {
    method: 'PUT',
    body: JSON.stringify(payload),
  })
}

export function deleteArticle(id: string): Promise<void> {
  return request<void>(`${articlesUrl}/${id}`, {
    method: 'DELETE',
  })
}
