using InventoryManagement.Application.Abstractions;
using InventoryManagement.Application.Articles.Commands;
using InventoryManagement.Application.Common;
using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Application.Articles.UseCases;

public sealed class CreateArticleUseCase(IArticleRepository articles, IUnitOfWork unitOfWork)
{
    public async Task<ArticleDto> ExecuteAsync(CreateArticleCommand command, CancellationToken cancellationToken)
    {
        var reference = new Ean13Reference(command.Reference);
        if (await articles.ExistsByReferenceAsync(reference, excludedArticleId: null, cancellationToken))
        {
            throw new ConflictException("An article with this reference already exists.");
        }

        var article = Article.Create(
            reference,
            command.Name,
            new Money(command.PriceExcludingTax),
            new Money(command.PriceIncludingTax));

        articles.Add(article);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ArticleMapper.ToDto(article);
    }
}
