export type Article = {
  id: string
  reference: string
  name: string
  priceExcludingTax: number
  priceIncludingTax: number
}

export type ArticlePayload = {
  reference: string
  name: string
  priceExcludingTax: number
  priceIncludingTax: number
}
