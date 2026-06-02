import type { PackagingLevel, TakeawayAvailability } from '@/features/articles/types'

export type StockMovementType = 'receive' | 'remove' | 'adjust'

export type Stock = {
  articleId: string
  currentQuantity: number
  sellableQuantity: number
  lots: StockLot[]
  movements: StockMovement[]
}

export type StockLot = {
  id: string
  articleId: string
  currentQuantity: number
  sellableQuantity: number
  expirationDate?: string | null
  takeawayAvailability?: TakeawayAvailability | null
  packagingLevel?: PackagingLevel | null
  priceIncludingTax: number
}

export type StockMovementPayload = {
  type: StockMovementType
  quantity: number
  reason: string
  expirationDate?: string | null
  takeawayAvailability?: TakeawayAvailability | null
  packagingLevel?: PackagingLevel | null
}

export type StockMovement = {
  id: string
  stockItemId: string
  articleId: string
  type: StockMovementType
  quantity: number
  quantityBefore: number
  quantityAfter: number
  reason: string
  occurredAt: string
  expirationDate?: string | null
  takeawayAvailability?: TakeawayAvailability | null
  packagingLevel?: PackagingLevel | null
}
