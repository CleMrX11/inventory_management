import { useState, type FormEvent } from "react";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import type { Article, ArticleCategory, ArticlePayload } from "./types";

type ArticleFormProps = {
  article: Article | null;
  onSubmit: (payload: ArticlePayload) => Promise<void>;
  onCancel: () => void;
  isSaving: boolean;
};

export function ArticleForm({
  article,
  onSubmit,
  onCancel,
  isSaving,
}: ArticleFormProps) {
  const initialCategory =
    article?.category === "FoodItem" ? "FoodItem" : "Merchandise";
  const [category, setCategory] = useState<ArticleCategory>(initialCategory);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const formData = new FormData(event.currentTarget);
    await onSubmit({
      reference: String(formData.get("reference") ?? "").trim(),
      name: String(formData.get("name") ?? "").trim(),
      category,
      priceExcludingTax: Number(formData.get("priceExcludingTax")),
      expirationDate:
        category === "FoodItem"
          ? String(formData.get("expirationDate") ?? "")
          : null,
      takeawayAvailability:
        category === "FoodItem"
          ? (String(
              formData.get("takeawayAvailability") ?? "",
            ) as ArticlePayload["takeawayAvailability"])
          : null,
      packagingLevel:
        category === "Merchandise"
          ? (String(
              formData.get("packagingLevel") ?? "",
            ) as ArticlePayload["packagingLevel"])
          : null,
    });
  }

  return (
    <Card>
      <form onSubmit={handleSubmit}>
        <CardHeader className="flex-row items-center justify-between space-y-0">
          <CardTitle>{article ? "Edit article" : "Add article"}</CardTitle>
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
              defaultValue={article?.reference ?? ""}
              inputMode="numeric"
              minLength={13}
              maxLength={13}
              pattern="[0-9]{13}"
              required
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="name">Name</Label>
            <Input
              id="name"
              name="name"
              defaultValue={article?.name ?? ""}
              required
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="category">Category</Label>
            <select
              id="category"
              name="category"
              value={category}
              onChange={(event) =>
                setCategory(event.target.value as ArticleCategory)
              }
              className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1 text-base shadow-sm transition-colors focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50 md:text-sm"
              required
            >
              <option value="FoodItem">Food item</option>
              <option value="Merchandise">Merchandise</option>
            </select>
          </div>

          {category === "FoodItem" && (
            <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-1">
              <div className="space-y-2">
                <Label htmlFor="expirationDate">Expiration date</Label>
                <Input
                  id="expirationDate"
                  name="expirationDate"
                  type="date"
                  defaultValue={article?.expirationDate ?? ""}
                  required
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="takeawayAvailability">
                  Takeaway availability
                </Label>
                <select
                  id="takeawayAvailability"
                  name="takeawayAvailability"
                  defaultValue={article?.takeawayAvailability ?? "Both"}
                  className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1 text-base shadow-sm transition-colors focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50 md:text-sm"
                  required
                >
                  <option value="TakeawayOnly">Takeaway only</option>
                  <option value="OnSiteOnly">On site only</option>
                  <option value="Both">Both</option>
                </select>
              </div>
            </div>
          )}

          {category === "Merchandise" && (
            <div className="space-y-2">
              <Label htmlFor="packagingLevel">Packaging level</Label>
              <select
                id="packagingLevel"
                name="packagingLevel"
                defaultValue={article?.packagingLevel ?? "New"}
                className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1 text-base shadow-sm transition-colors focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50 md:text-sm"
                required
              >
                <option value="New">New</option>
                <option value="Refurbished">Refurbished</option>
                <option value="Unsellable">Unsellable</option>
              </select>
            </div>
          )}

          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-1">
            <div className="space-y-2">
              <Label htmlFor="priceExcludingTax">Price HT</Label>
              <Input
                id="priceExcludingTax"
                name="priceExcludingTax"
                type="number"
                defaultValue={article?.priceExcludingTax ?? ""}
                min="0"
                step="0.01"
                required
              />
            </div>
          </div>
        </CardContent>

        <CardFooter>
          <Button className="w-full" type="submit" disabled={isSaving}>
            {isSaving
              ? "Saving..."
              : article
                ? "Save changes"
                : "Create article"}
          </Button>
        </CardFooter>
      </form>
    </Card>
  );
}
