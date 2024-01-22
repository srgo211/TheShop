
using ProductCatalogService.Interfaces.Models;

namespace ProductCatalogService.Apis;

public class CheckDataApi : IApi
{
    const string endpoint = "/checked";
    public void Register(WebApplication app)
    {
        app.MapGet(endpoint, GetProductStockCount)
            .Produces<List<IProduct>>(StatusCodes.Status200OK)
            .WithName("GetProductStockCount")
            .WithTags("Сheck");

        app.MapGet($"{endpoint}/Reduce", ReduceStock)
            .Produces<List<IProduct>>(StatusCodes.Status200OK)
            .WithName("ReduceStock")
            .WithTags("Сheck");
    }

    private async Task<IResult> GetProductStockCount(int idProduct, IDataCheckerRepository repository)
    {
        int count = await repository.GetProductStockCountAsync(idProduct);

        var data = new { IdProduct = idProduct, Count = count };
        return Results.Json(data);
    }

     private async Task<IResult> ReduceStock(int idProduct, int quantity, IDataCheckerRepository repository)
     {
        bool check = await repository.ReduceStockAsync(idProduct, quantity);
        var data = new { IdProduct = idProduct, Result = check };
        return Results.Json(data);
    }
}
