using InventoryManagement.Api.Requests;
using InventoryManagement.Application.Articles.Commands;
using InventoryManagement.Application.Articles.UseCases;
using InventoryManagement.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Controllers;

[ApiController]
[Route("api/articles")]
public sealed class ArticlesController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(ListArticlesUseCase useCase, CancellationToken cancellationToken)
    {
        var articles = await useCase.ExecuteAsync(cancellationToken);
        return Ok(articles);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, GetArticleByIdUseCase useCase, CancellationToken cancellationToken)
    {
        try
        {
            var article = await useCase.ExecuteAsync(id, cancellationToken);
            return Ok(article);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateArticleRequest request, CreateArticleUseCase useCase, CancellationToken cancellationToken)
    {
        try
        {
            var article = await useCase.ExecuteAsync(
                new CreateArticleCommand(
                    request.Reference,
                    request.Name,
                    request.Category,
                    request.PriceExcludingTax,
                    request.ExpirationDate,
                    request.TakeawayAvailability,
                    request.PackagingLevel),
                cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = article.Id }, article);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { ex.Message });
        }
        catch (ConflictException ex)
        {
            return Conflict(new { ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateArticleRequest request, UpdateArticleUseCase useCase, CancellationToken cancellationToken)
    {
        try
        {
            var article = await useCase.ExecuteAsync(
                new UpdateArticleCommand(
                    id,
                    request.Reference,
                    request.Name,
                    request.Category,
                    request.PriceExcludingTax,
                    request.ExpirationDate,
                    request.TakeawayAvailability,
                    request.PackagingLevel),
                cancellationToken);

            return Ok(article);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { ex.Message });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
        catch (ConflictException ex)
        {
            return Conflict(new { ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, DeleteArticleUseCase useCase, CancellationToken cancellationToken)
    {
        try
        {
            await useCase.ExecuteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
        catch (ConflictException ex)
        {
            return Conflict(new { ex.Message });
        }
    }

}
