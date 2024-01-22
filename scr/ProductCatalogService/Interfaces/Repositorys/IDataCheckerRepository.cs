namespace ProductCatalogService.Interfaces.Repositorys;

public interface IDataCheckerRepository
{
    Task<int> GetProductStockCountAsync(int productId);
    Task<bool> ReduceStockAsync(int productId, int quantity);
}
