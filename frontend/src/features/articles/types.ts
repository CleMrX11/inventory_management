export type ArticleCategory = "FoodItem" | "Merchandise";
export type TakeawayAvailability = "TakeawayOnly" | "OnSiteOnly" | "Both";
export type PackagingLevel = "New" | "Refurbished" | "Unsellable";

export type Article = {
  id: string;
  reference: string;
  name: string;
  category: ArticleCategory;
  priceExcludingTax: number;
};

export type ArticlePayload = {
  reference: string;
  name: string;
  category: ArticleCategory;
  priceExcludingTax: number;
};
