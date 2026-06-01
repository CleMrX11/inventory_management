using InventoryManagement.Application.Articles.UseCases;
using InventoryManagement.Application.Stock.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ListArticlesUseCase>();
        services.AddScoped<GetArticleByIdUseCase>();
        services.AddScoped<CreateArticleUseCase>();
        services.AddScoped<UpdateArticleUseCase>();
        services.AddScoped<DeleteArticleUseCase>();
        services.AddScoped<GetStockByArticleIdUseCase>();
        services.AddScoped<CreateStockMovementUseCase>();

        return services;
    }
}
