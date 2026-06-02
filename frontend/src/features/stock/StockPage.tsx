import { useEffect, useMemo, useState, type FormEvent } from 'react'
import { Check, Minus, PackageSearch, Plus, RefreshCw, SlidersHorizontal } from 'lucide-react'
import { useNavigate } from 'react-router-dom'
import { Alert, AlertDescription } from '@/components/ui/alert'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { listArticles } from '@/features/articles/api'
import type { Article } from '@/features/articles/types'
import { cn } from '@/lib/utils'
import { createStockMovement, getArticleStock } from './api'
import type { Stock, StockMovement, StockMovementPayload, StockMovementType } from './types'

const movementLabels: Record<StockMovementType, string> = {
  receive: 'Receive',
  remove: 'Remove',
  adjust: 'Adjust',
}

const movementIcons = {
  receive: Plus,
  remove: Minus,
  adjust: SlidersHorizontal,
}

const dateFormatter = new Intl.DateTimeFormat('fr-FR', {
  dateStyle: 'short',
  timeStyle: 'short',
})

export function StockPage() {
  const navigate = useNavigate()
  const [articles, setArticles] = useState<Article[]>([])
  const [stockByArticleId, setStockByArticleId] = useState<Record<string, Stock>>({})
  const [selectedArticleId, setSelectedArticleId] = useState<string | null>(null)
  const [movementType, setMovementType] = useState<StockMovementType>('receive')
  const [lastMovement, setLastMovement] = useState<StockMovement | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [isSaving, setIsSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const selectedArticle = useMemo(
    () => articles.find((article) => article.id === selectedArticleId) ?? null,
    [articles, selectedArticleId],
  )
  const selectedStock = selectedArticleId ? stockByArticleId[selectedArticleId] : null
  const SubmitIcon = movementIcons[movementType]

  async function refreshStock(articleId: string) {
    const stock = await getArticleStock(articleId)
    setStockByArticleId((current) => ({
      ...current,
      [articleId]: stock,
    }))
    return stock
  }

  async function refreshPage() {
    setIsLoading(true)
    setError(null)

    try {
      const loadedArticles = await listArticles()
      const stocks = await Promise.all(loadedArticles.map((article) => getArticleStock(article.id)))
      const stockMap = Object.fromEntries(stocks.map((stock) => [stock.articleId, stock]))

      setArticles(loadedArticles)
      setStockByArticleId(stockMap)
      setSelectedArticleId((current) => {
        if (current && loadedArticles.some((article) => article.id === current)) {
          return current
        }

        return loadedArticles[0]?.id ?? null
      })
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Unable to load stock')
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    let ignore = false

    async function load() {
      setIsLoading(true)

      try {
        const loadedArticles = await listArticles()
        const stocks = await Promise.all(loadedArticles.map((article) => getArticleStock(article.id)))
        const stockMap = Object.fromEntries(stocks.map((stock) => [stock.articleId, stock]))

        if (!ignore) {
          setArticles(loadedArticles)
          setStockByArticleId(stockMap)
          setSelectedArticleId(loadedArticles[0]?.id ?? null)
        }
      } catch (err) {
        if (!ignore) {
          setError(err instanceof Error ? err.message : 'Unable to load stock')
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
  }, [])

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    const form = event.currentTarget

    if (!selectedArticleId) {
      setError('Select an article before creating a movement.')
      return
    }

    const formData = new FormData(form)
    const payload: StockMovementPayload = {
      type: movementType,
      quantity: Number(formData.get('quantity')),
      reason: String(formData.get('reason') ?? '').trim(),
    }

    setIsSaving(true)
    setError(null)

    try {
      const movement = await createStockMovement(selectedArticleId, payload)
      setLastMovement(movement)
      await refreshStock(selectedArticleId)
      form.reset()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Unable to create stock movement')
    } finally {
      setIsSaving(false)
    }
  }

  function handleSelectArticle(articleId: string) {
    setSelectedArticleId(articleId)
    setLastMovement(null)
    setError(null)
    navigate(`/stock/${articleId}/movements`)
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
        <div className="space-y-1">
          <h1 className="text-3xl font-semibold tracking-tight">Stock</h1>
          <p className="text-muted-foreground">Track article quantities through receive, remove, and adjust movements.</p>
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

      <div className="grid gap-6 lg:grid-cols-[minmax(0,1fr)_380px]">
        <Card>
          <CardHeader>
            <CardTitle>Article stock</CardTitle>
            <CardDescription>{articles.length} article{articles.length > 1 ? 's' : ''} available for stock tracking.</CardDescription>
          </CardHeader>
          <CardContent>
            {isLoading ? (
              <p className="rounded-md border border-dashed p-6 text-sm text-muted-foreground">Loading stock...</p>
            ) : articles.length === 0 ? (
              <p className="rounded-md border border-dashed p-6 text-sm text-muted-foreground">No articles available.</p>
            ) : (
              <div className="rounded-md border">
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>Reference</TableHead>
                      <TableHead>Name</TableHead>
                      <TableHead className="text-right">Current quantity</TableHead>
                      <TableHead className="text-right">Sellable</TableHead>
                      <TableHead className="text-right">Action</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {articles.map((article) => {
                      const stock = stockByArticleId[article.id]
                      const isSelected = article.id === selectedArticleId

                      return (
                        <TableRow
                          key={article.id}
                          className={cn('cursor-pointer', isSelected && 'bg-muted/50')}
                          onClick={() => handleSelectArticle(article.id)}
                        >
                          <TableCell className="font-mono">{article.reference}</TableCell>
                          <TableCell className="font-medium">{article.name}</TableCell>
                          <TableCell className="text-right text-lg font-semibold tabular-nums">
                            {stock?.currentQuantity ?? 0}
                          </TableCell>
                          <TableCell className="text-right text-lg font-semibold tabular-nums">
                            {stock?.sellableQuantity ?? 0}
                          </TableCell>
                          <TableCell>
                            <div className="flex justify-end">
                              <Button
                                type="button"
                                size="sm"
                                variant={isSelected ? 'secondary' : 'outline'}
                                onClick={() => handleSelectArticle(article.id)}
                              >
                                {isSelected ? (
                                  <Check className="h-4 w-4" aria-hidden="true" />
                                ) : (
                                  <PackageSearch className="h-4 w-4" aria-hidden="true" />
                                )}
                                {isSelected ? 'Selected' : 'Open'}
                              </Button>
                            </div>
                          </TableCell>
                        </TableRow>
                      )
                    })}
                  </TableBody>
                </Table>
              </div>
            )}
          </CardContent>
        </Card>

        <div className="space-y-6">
          <Card>
            <CardHeader>
              <CardTitle>Movement</CardTitle>
              <CardDescription>
                {selectedArticle ? `${selectedArticle.name} has ${selectedStock?.currentQuantity ?? 0} units.` : 'No article selected.'}
              </CardDescription>
            </CardHeader>
            <CardContent>
              <form className="space-y-4" onSubmit={handleSubmit}>
                <div className="space-y-2">
                  <Label htmlFor="movementType">Type</Label>
                  <select
                    id="movementType"
                    value={movementType}
                    onChange={(event) => setMovementType(event.target.value as StockMovementType)}
                    className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1 text-base shadow-sm transition-colors focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50 md:text-sm"
                    disabled={!selectedArticle || isSaving}
                  >
                    <option value="receive">Receive</option>
                    <option value="remove">Remove</option>
                    <option value="adjust">Adjust</option>
                  </select>
                </div>

                <div className="space-y-2">
                  <Label htmlFor="quantity">{movementType === 'adjust' ? 'Quantity after adjustment' : 'Quantity'}</Label>
                  <Input
                    id="quantity"
                    name="quantity"
                    type="number"
                    min={movementType === 'adjust' ? 0 : 1}
                    step={1}
                    required
                    disabled={!selectedArticle || isSaving}
                  />
                </div>

                <div className="space-y-2">
                  <Label htmlFor="reason">Reason</Label>
                  <Input id="reason" name="reason" required disabled={!selectedArticle || isSaving} />
                </div>

                <Button className="w-full" type="submit" disabled={!selectedArticle || isSaving}>
                  <SubmitIcon className="h-4 w-4" aria-hidden="true" />
                  {isSaving ? 'Saving...' : movementLabels[movementType]}
                </Button>
              </form>
            </CardContent>
          </Card>

          {lastMovement && (
            <Card>
              <CardHeader>
                <CardTitle>Latest movement</CardTitle>
                <CardDescription>{dateFormatter.format(new Date(lastMovement.occurredAt))}</CardDescription>
              </CardHeader>
              <CardContent>
                <dl className="grid grid-cols-2 gap-3 text-sm">
                  <div className="rounded-md border p-3">
                    <dt className="text-muted-foreground">Type</dt>
                    <dd className="font-medium">{movementLabels[lastMovement.type]}</dd>
                  </div>
                  <div className="rounded-md border p-3">
                    <dt className="text-muted-foreground">Quantity</dt>
                    <dd className="font-medium tabular-nums">{lastMovement.quantity}</dd>
                  </div>
                  <div className="rounded-md border p-3">
                    <dt className="text-muted-foreground">Before</dt>
                    <dd className="font-medium tabular-nums">{lastMovement.quantityBefore}</dd>
                  </div>
                  <div className="rounded-md border p-3">
                    <dt className="text-muted-foreground">After</dt>
                    <dd className="font-medium tabular-nums">{lastMovement.quantityAfter}</dd>
                  </div>
                  <div className="col-span-2 rounded-md border p-3">
                    <dt className="text-muted-foreground">Reason</dt>
                    <dd className="font-medium">{lastMovement.reason}</dd>
                  </div>
                </dl>
              </CardContent>
            </Card>
          )}
        </div>
      </div>
    </div>
  )
}
