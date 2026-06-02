using InventoryManagement.Api.Requests;
using InventoryManagement.Application.Common;
using InventoryManagement.Application.Stock.Commands;
using InventoryManagement.Application.Stock.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Controllers;

[ApiController]
[Route("api/articles/{articleId:guid}/stock")]
public sealed class ArticleStockController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(Guid articleId, GetStockByArticleIdUseCase useCase, CancellationToken cancellationToken)
    {
        try
        {
            var stock = await useCase.ExecuteAsync(articleId, cancellationToken);
            return Ok(stock);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
    }

    [HttpPost("movements")]
    public async Task<IActionResult> CreateMovement(
        Guid articleId,
        CreateStockMovementRequest request,
        CreateStockMovementUseCase useCase,
        CancellationToken cancellationToken)
    {
        try
        {
            var movement = await useCase.ExecuteAsync(
                new CreateStockMovementCommand(
                    articleId,
                    request.Type,
                    request.Quantity,
                    request.Reason,
                    request.ExpirationDate,
                    request.TakeawayAvailability,
                    request.PackagingLevel),
                cancellationToken);

            return Ok(movement);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ex.Message });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
    }
}
