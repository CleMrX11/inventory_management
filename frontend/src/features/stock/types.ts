export type StockMovementType = 'receive' | 'remove' | 'adjust'

export type Stock = {
  articleId: string
  currentQuantity: number
  sellableQuantity: number
  movements: StockMovement[]
}

export type StockMovementPayload = {
  type: StockMovementType
  quantity: number
  reason: string
}

export type StockMovement = {
  id: string
  articleId: string
  type: StockMovementType
  quantity: number
  quantityBefore: number
  quantityAfter: number
  reason: string
  occurredAt: string
}
