import { Button } from '@/components/ui/button'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import type { Article } from './types'

type ArticleTableProps = {
  articles: Article[]
  onEdit: (article: Article) => void
  onDelete: (article: Article) => void
}

const currencyFormatter = new Intl.NumberFormat('fr-FR', {
  style: 'currency',
  currency: 'EUR',
})

export function ArticleTable({ articles, onEdit, onDelete }: ArticleTableProps) {
  if (articles.length === 0) {
    return <p className="rounded-md border border-dashed p-6 text-sm text-muted-foreground">No articles yet. Create the first one from the form.</p>
  }

  return (
    <div className="rounded-md border">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Reference</TableHead>
            <TableHead>Name</TableHead>
            <TableHead>Price HT</TableHead>
            <TableHead>Price TTC</TableHead>
            <TableHead className="text-right">Actions</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {articles.map((article) => (
            <TableRow key={article.id}>
              <TableCell className="font-mono">{article.reference}</TableCell>
              <TableCell className="font-medium">{article.name}</TableCell>
              <TableCell>{currencyFormatter.format(article.priceExcludingTax)}</TableCell>
              <TableCell>{currencyFormatter.format(article.priceIncludingTax)}</TableCell>
              <TableCell>
                <div className="flex justify-end gap-2">
                  <Button variant="outline" size="sm" type="button" onClick={() => onEdit(article)}>
                    Edit
                  </Button>
                  <Button variant="destructive" size="sm" type="button" onClick={() => onDelete(article)}>
                    Delete
                  </Button>
                </div>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  )
}
