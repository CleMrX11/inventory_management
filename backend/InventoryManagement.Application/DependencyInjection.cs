using InventoryManagement.Application.Articles.UseCases;
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

        return services;
    }
}
