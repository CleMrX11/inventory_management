using InventoryManagement.Application.Abstractions;
using InventoryManagement.Application.Articles.Commands;
using InventoryManagement.Application.Common;
using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Application.Articles.UseCases;

public sealed class UpdateArticleUseCase(IArticleRepository articles, IUnitOfWork unitOfWork)
{
    public async Task<ArticleDto> ExecuteAsync(UpdateArticleCommand command, CancellationToken cancellationToken)
    {
        var id = new ArticleId(command.Id);
        var article = await articles.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Article not found.");

        var reference = new Ean13Reference(command.Reference);
        if (await articles.ExistsByReferenceAsync(reference, id, cancellationToken))
        {
            throw new ConflictException("An article with this reference already exists.");
        }

        var category = ArticleCategoryParser.Parse(command.Category);
        var priceExcludingTax = new Money(command.PriceExcludingTax);
        var expirationDate = ArticleSpecificityParser.ParseExpirationDate(command.ExpirationDate);
        var takeawayAvailability = ArticleSpecificityParser.ParseTakeawayAvailability(command.TakeawayAvailability);
        var packagingLevel = ArticleSpecificityParser.ParsePackagingLevel(command.PackagingLevel);

        var updatedArticle = Article.Create(
            id,
            reference,
            command.Name,
            category,
            priceExcludingTax,
            expirationDate,
            takeawayAvailability,
            packagingLevel);

        articles.Replace(article, updatedArticle);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ArticleMapper.ToDto(updatedArticle);
    }
}
