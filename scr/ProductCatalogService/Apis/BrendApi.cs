using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace ProductCatalogService.Apis;

public class BrendApi : IApi
{
    const string endpoint = "/brends";
    public void Register(WebApplication app)
    {
        
        app.MapGet(endpoint, GetBrends)
            .Produces<List<IBrand>>(StatusCodes.Status200OK)
            .WithName("GetAllBrands")
            .WithTags("Read");


        app.MapPost($"{endpoint}/create", AddBrend)
        .Accepts<IBrand>("application/json")
        .Produces<IBrand>(StatusCodes.Status201Created)
        .WithName("CreateBrand")
        .WithTags("Create");


         app.MapPut($"{endpoint}/update", Update)
             .Accepts<IBrand>("application/json")
             .WithName("UpdateBrand")
             .WithTags("Update");

         app.MapDelete($"{endpoint}/delete", (HttpContext httpContext, int id,  IBrendRepository repository) => Delete(httpContext, id,  repository))
             .WithMetadata(new HttpMethodMetadata(new[] { "DELETE" }))
             .WithName("DeleteBrand")
             .WithTags("Delete");



    }


    private async Task<IResult> GetBrends(IBrendRepository repository) => await repository.GetBrendsAsync() is List<Brand> products
        ? Results.Ok(products)
        : Results.NotFound();

    private async Task<IResult> Delete(HttpContext httpContext, int id, IBrendRepository repository)
    {
        bool isAdmin = SharedDomainModels.Extensions.ExtensionAvtorization.CheckAuthenticated(httpContext);
        if (!isAdmin) return Results.StatusCode(StatusCodes.Status403Forbidden);

        bool checkDelete = await repository.DeleteBrendAsync(id, isAdmin);
        if (checkDelete) return Results.Ok($"Бренд с ID {id} успешно удален");
        else return Results.NotFound();
    }

    private async Task<IResult> Update(HttpContext httpContext, int idBrend, [FromBody] Brand brend, IBrendRepository repository)
    {
        bool isAdmin = SharedDomainModels.Extensions.ExtensionAvtorization.CheckAuthenticated(httpContext);
        if (!isAdmin) return Results.StatusCode(StatusCodes.Status403Forbidden);

        bool check = await repository.UpdateAsync(idBrend, brend, true);
        if (check) return Results.Ok($"Продукт с ID {idBrend} успешно обновлен");
        else return Results.NotFound();

    }

    private async Task<IResult> AddBrend(HttpContext httpContext, [FromBody] Brand brend, IBrendRepository repository)
    {
        bool isAdmin = SharedDomainModels.Extensions.ExtensionAvtorization.CheckAuthenticated(httpContext);
        if (!isAdmin) return Results.StatusCode(StatusCodes.Status403Forbidden);
        await repository.AddBrendAsync(brend, isAdmin);
        //return Results.Ok($"Добавили новый бренд с ID {brend.BrandId}");
        return Results.Created($"{endpoint}/{brend.Id}", brend);
    }

  
}
