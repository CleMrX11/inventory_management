import type { FormEvent } from 'react'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import type { Article, ArticlePayload } from './types'

type ArticleFormProps = {
  article: Article | null
  onSubmit: (payload: ArticlePayload) => Promise<void>
  onCancel: () => void
  isSaving: boolean
}

export function ArticleForm({ article, onSubmit, onCancel, isSaving }: ArticleFormProps) {
  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    const formData = new FormData(event.currentTarget)

    await onSubmit({
      reference: String(formData.get('reference') ?? '').trim(),
      name: String(formData.get('name') ?? '').trim(),
      priceExcludingTax: Number(formData.get('priceExcludingTax')),
      priceIncludingTax: Number(formData.get('priceIncludingTax')),
    })
  }

  return (
    <Card>
      <form onSubmit={handleSubmit}>
        <CardHeader className="flex-row items-center justify-between space-y-0">
          <CardTitle>{article ? 'Edit article' : 'Add article'}</CardTitle>
          {article && (
            <Button variant="ghost" type="button" onClick={onCancel}>
              Cancel
            </Button>
          )}
        </CardHeader>

        <CardContent className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="reference">Reference EAN-13</Label>
            <Input
              id="reference"
              name="reference"
              defaultValue={article?.reference ?? ''}
              inputMode="numeric"
              minLength={13}
              maxLength={13}
              pattern="[0-9]{13}"
              required
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="name">Name</Label>
            <Input id="name" name="name" defaultValue={article?.name ?? ''} required />
          </div>

          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-1">
            <div className="space-y-2">
              <Label htmlFor="priceExcludingTax">Price HT</Label>
              <Input
                id="priceExcludingTax"
                name="priceExcludingTax"
                type="number"
                defaultValue={article?.priceExcludingTax ?? ''}
                min="0"
                step="0.01"
                required
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="priceIncludingTax">Price TTC</Label>
              <Input
                id="priceIncludingTax"
                name="priceIncludingTax"
                type="number"
                defaultValue={article?.priceIncludingTax ?? ''}
                min="0"
                step="0.01"
                required
              />
            </div>
          </div>
        </CardContent>

        <CardFooter>
          <Button className="w-full" type="submit" disabled={isSaving}>
            {isSaving ? 'Saving...' : article ? 'Save changes' : 'Create article'}
          </Button>
        </CardFooter>
      </form>
    </Card>
  )
}
