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

        article.ChangeReference(reference);
        article.Update(
            command.Name,
            new Money(command.PriceExcludingTax),
            new Money(command.PriceIncludingTax));

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ArticleMapper.ToDto(article);
    }
}
