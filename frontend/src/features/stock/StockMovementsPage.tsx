import { useCallback, useEffect, useState } from 'react'
import { ArrowLeft, RefreshCw } from 'lucide-react'
import { Link, useParams } from 'react-router-dom'
import { Alert, AlertDescription } from '@/components/ui/alert'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { getArticle } from '@/features/articles/api'
import type { Article, PackagingLevel, TakeawayAvailability } from '@/features/articles/types'
import { cn } from '@/lib/utils'
import { getArticleStock } from './api'
import type { Stock, StockMovementType } from './types'

const movementLabels: Record<StockMovementType, string> = {
  receive: 'Receive',
  remove: 'Remove',
  adjust: 'Adjust',
}

const takeawayLabels: Record<TakeawayAvailability, string> = {
  TakeawayOnly: 'Takeaway only',
  OnSiteOnly: 'On site only',
  Both: 'Both',
}

const packagingLabels: Record<PackagingLevel, string> = {
  New: 'New',
  Refurbished: 'Refurbished',
  Unsellable: 'Unsellable',
}

const dateFormatter = new Intl.DateTimeFormat('fr-FR', {
  dateStyle: 'short',
  timeStyle: 'short',
})

export function StockMovementsPage() {
  const { articleId } = useParams<{ articleId: string }>()
  const [article, setArticle] = useState<Article | null>(null)
  const [stock, setStock] = useState<Stock | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const refreshPage = useCallback(async () => {
    if (!articleId) {
      setError('Article id is missing.')
      setIsLoading(false)
      return
    }

    setIsLoading(true)
    setError(null)

    try {
      const [loadedArticle, loadedStock] = await Promise.all([
        getArticle(articleId),
        getArticleStock(articleId),
      ])

      setArticle(loadedArticle)
      setStock(loadedStock)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Unable to load article movements')
    } finally {
      setIsLoading(false)
    }
  }, [articleId])

  useEffect(() => {
    let ignore = false

    async function load() {
      if (!articleId) {
        setError('Article id is missing.')
        setIsLoading(false)
        return
      }

      setIsLoading(true)
      setError(null)

      try {
        const [loadedArticle, loadedStock] = await Promise.all([
          getArticle(articleId),
          getArticleStock(articleId),
        ])

        if (!ignore) {
          setArticle(loadedArticle)
          setStock(loadedStock)
        }
      } catch (err) {
        if (!ignore) {
          setError(err instanceof Error ? err.message : 'Unable to load article movements')
        }
      } finally {
        if (!ignore) {
          setIsLoading(false)
        }
      }
    }

    load()

    return () => {
      ignore = true
    }
  }, [articleId])

  const movements = stock?.movements ?? []

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
        <div className="space-y-2">
          <Button asChild variant="ghost" size="sm" className="-ml-3 w-fit">
            <Link to="/stock">
              <ArrowLeft className="h-4 w-4" aria-hidden="true" />
              Stock
            </Link>
          </Button>
          <div className="space-y-1">
            <h1 className="text-3xl font-semibold tracking-tight">{article?.name ?? 'Article movements'}</h1>
            <p className="text-muted-foreground">
              {article ? <span className="font-mono">{article.reference}</span> : 'Movement history and sellable quantity.'}
            </p>
          </div>
        </div>
        <Button type="button" variant="outline" onClick={refreshPage} disabled={isLoading}>
          <RefreshCw className={cn('h-4 w-4', isLoading && 'animate-spin')} aria-hidden="true" />
          Refresh
        </Button>
      </div>

      {error && (
        <Alert variant="destructive">
          <AlertDescription>{error}</AlertDescription>
        </Alert>
      )}

      <div className="grid gap-4 sm:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>Current stock</CardTitle>
            <CardDescription>Units currently tracked for this article.</CardDescription>
          </CardHeader>
          <CardContent>
            <p className="text-4xl font-semibold tabular-nums">{stock?.currentQuantity ?? 0}</p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Can be sold</CardTitle>
            <CardDescription>Units available for sale from the current stock.</CardDescription>
          </CardHeader>
          <CardContent>
            <p className="text-4xl font-semibold tabular-nums">{stock?.sellableQuantity ?? 0}</p>
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Movements</CardTitle>
          <CardDescription>{movements.length} movement{movements.length > 1 ? 's' : ''} recorded for this article.</CardDescription>
        </CardHeader>
        <CardContent>
          {isLoading ? (
            <p className="rounded-md border border-dashed p-6 text-sm text-muted-foreground">Loading movements...</p>
          ) : movements.length === 0 ? (
            <p className="rounded-md border border-dashed p-6 text-sm text-muted-foreground">No movements recorded for this article.</p>
          ) : (
            <div className="rounded-md border">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Date</TableHead>
                    <TableHead>Lot</TableHead>
                    <TableHead>Type</TableHead>
                    <TableHead className="text-right">Quantity</TableHead>
                    <TableHead className="text-right">Before</TableHead>
                    <TableHead className="text-right">After</TableHead>
                    <TableHead>Reason</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {movements.map((movement) => (
                    <TableRow key={movement.id}>
                      <TableCell className="whitespace-nowrap">{dateFormatter.format(new Date(movement.occurredAt))}</TableCell>
                      <TableCell>
                        {article?.category === 'FoodItem'
                          ? `${movement.expirationDate ?? '-'} · ${movement.takeawayAvailability ? takeawayLabels[movement.takeawayAvailability] : '-'}`
                          : movement.packagingLevel ? packagingLabels[movement.packagingLevel] : '-'}
                      </TableCell>
                      <TableCell>{movementLabels[movement.type]}</TableCell>
                      <TableCell className="text-right tabular-nums">{movement.quantity}</TableCell>
                      <TableCell className="text-right tabular-nums">{movement.quantityBefore}</TableCell>
                      <TableCell className="text-right tabular-nums">{movement.quantityAfter}</TableCell>
                      <TableCell className="min-w-48">{movement.reason}</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  )
}
