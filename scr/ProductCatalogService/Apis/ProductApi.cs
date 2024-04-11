using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace ProductCatalogService.Apis;

public class ProductApi : IApi
{
    const string endpoint = "/products";
    public void Register(WebApplication app)
    {


        app.MapPost($"{endpoint}/create", AddProduct)
            .Accepts<IProduct>("application/json")
            .Produces<IProduct>(StatusCodes.Status201Created)
            .WithName("CreateProduct")
            .WithTags("Create");


        app.MapGet($"{endpoint}/search", GetByName)
           .Produces<List<IProduct>>(StatusCodes.Status200OK)
           .WithName("GetNameProducts")
           .WithTags("Read");


        app.MapGet(endpoint, GetAll)
          .Produces<List<IProduct>>(StatusCodes.Status200OK)
          .WithName("GetAllProducts")
          .WithTags("Read");


        app.MapGet($"{endpoint}/paged", (int page, int itemsPerPage, IProductRepository repository) => GetPaged(page, itemsPerPage, repository))
            .Produces<List<IProduct>>(StatusCodes.Status200OK)
            .WithName("GetPageProducts")
            .WithTags("Read");





        app.MapDelete($"{endpoint}/delete",Delete)
           .WithMetadata(new HttpMethodMetadata(new[] { "DELETE" }))
           .WithTags("Delete");


        app.MapPut($"{endpoint}/update", Update)
            .Accepts<IProduct>("application/json")
            .WithName("UpdateProducts")
            .WithTags("Update");

        #if DEBUG
        app.MapGet("/GreateRandomDatas", async (AppDbContext db) =>
            await TestData.GenerateRandomDatasAsync(db, 30,5,3))
            .WithName("CreateTestData")
            .WithTags("TestData");
#endif

    }

   
    private async Task<IResult> Update(HttpContext httpContext, int idProduct,  [FromBody] Product product, IProductRepository repository)
    {
        bool isAdmin = SharedDomainModels.Extensions.ExtensionAvtorization.CheckAuthenticated(httpContext);
        if (!isAdmin) return Results.StatusCode(StatusCodes.Status403Forbidden);

        bool check = await repository.UpdateAsync(idProduct, product,true);
        if (check) return Results.Ok($"Продукт с ID {idProduct} успешно обновлен");
        else return Results.NotFound();
       
    }   
    private async Task<IResult> Delete(HttpContext httpContext, int id, IProductRepository repository)
    {
        bool isAdmin = SharedDomainModels.Extensions.ExtensionAvtorization.CheckAuthenticated(httpContext);
        if (!isAdmin) return Results.StatusCode(StatusCodes.Status403Forbidden);

        bool checkDelete = await repository.DeleteProductAsync(id, isAdmin);
        if (checkDelete) return Results.Ok($"Продукт с ID {id} успешно удален");
        else return Results.NotFound();
    }
    private async Task<IResult> GetAll(IProductRepository repository) => await repository.GetProductsAsync() is List<Product> products
        ? Results.Ok(products)
        : Results.NotFound();
    private async Task<IResult> GetByName(string name, IProductRepository repository) =>
        await repository.GetProductByNameAsync(name) is List<Product> products
        ? Results.Ok(products)
        : Results.NotFound();

    private async Task<IResult> GetPaged(int page, int itemsPerPage, IProductRepository repository)
    {
      return  await repository.GetPagedAsync(page,itemsPerPage) is List<Product> products
            ? Results.Ok(products)
            : Results.NotFound();
    }


    private async Task<IResult> AddProduct(HttpContext httpContext, Product product, IProductRepository repository)
    {
        bool isAdmin = SharedDomainModels.Extensions.ExtensionAvtorization.CheckAuthenticated(httpContext);
        if (!isAdmin) return Results.StatusCode(StatusCodes.Status403Forbidden);
       

        await repository.AddProductAsync(product, isAdmin);
        //return Results.Ok($"Добавили новый продукт");
        return Results.Created($"/hotels/{product.Id}", product);
    }


  

}
