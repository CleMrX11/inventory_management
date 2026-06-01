import { useEffect, useState } from 'react'
import { Alert, AlertDescription } from '@/components/ui/alert'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { ArticleForm } from './ArticleForm'
import { ArticleTable } from './ArticleTable'
import { createArticle, deleteArticle, listArticles, updateArticle } from './api'
import type { Article, ArticlePayload } from './types'

export function ArticlesPage() {
  const [articles, setArticles] = useState<Article[]>([])
  const [selectedArticle, setSelectedArticle] = useState<Article | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [isSaving, setIsSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [formVersion, setFormVersion] = useState(0)

  async function refreshArticles() {
    setIsLoading(true)
    setError(null)

    try {
      setArticles(await listArticles())
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Unable to load articles')
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    let ignore = false

    listArticles()
      .then((data) => {
        if (!ignore) {
          setArticles(data)
        }
      })
      .catch((err) => {
        if (!ignore) {
          setError(err instanceof Error ? err.message : 'Unable to load articles')
        }
      })
      .finally(() => {
        if (!ignore) {
          setIsLoading(false)
        }
      })

    return () => {
      ignore = true
    }
  }, [])

  async function handleSubmit(payload: ArticlePayload) {
    if (payload.priceIncludingTax < payload.priceExcludingTax) {
      setError('Price TTC must be greater than or equal to price HT.')
      return
    }

    setIsSaving(true)
    setError(null)

    try {
      if (selectedArticle) {
        await updateArticle(selectedArticle.id, payload)
      } else {
        await createArticle(payload)
      }

      setSelectedArticle(null)
      setFormVersion((version) => version + 1)
      await refreshArticles()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Unable to save article')
    } finally {
      setIsSaving(false)
    }
  }

  async function handleDelete(article: Article) {
    if (!window.confirm(`Delete ${article.name}?`)) {
      return
    }

    setError(null)

    try {
      await deleteArticle(article.id)
      if (selectedArticle?.id === article.id) {
        setSelectedArticle(null)
      }
      await refreshArticles()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Unable to delete article')
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
        <div className="space-y-1">
          <h1 className="text-3xl font-semibold tracking-tight">Articles</h1>
          <p className="text-muted-foreground">Manage articles identified by a unique EAN-13 reference.</p>
        </div>
        <Button type="button" variant="outline" onClick={refreshArticles} disabled={isLoading}>
          Refresh
        </Button>
      </div>

      {error && (
        <Alert variant="destructive">
          <AlertDescription>{error}</AlertDescription>
        </Alert>
      )}

      <div className="grid gap-6 lg:grid-cols-[360px_minmax(0,1fr)]">
        <ArticleForm
          key={`${selectedArticle?.id ?? 'new'}-${formVersion}`}
          article={selectedArticle}
          onSubmit={handleSubmit}
          onCancel={() => setSelectedArticle(null)}
          isSaving={isSaving}
        />

        <Card>
          <CardHeader>
            <CardTitle>Article catalog</CardTitle>
            <CardDescription>{articles.length} article{articles.length > 1 ? 's' : ''} in inventory.</CardDescription>
          </CardHeader>
          <CardContent>
            {isLoading ? (
              <p className="rounded-md border border-dashed p-6 text-sm text-muted-foreground">Loading articles...</p>
            ) : (
              <ArticleTable articles={articles} onEdit={setSelectedArticle} onDelete={handleDelete} />
            )}
          </CardContent>
        </Card>
      </div>
    </div>
  )
}
