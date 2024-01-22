namespace ProductCatalogService.Interfaces.Repositorys;

public interface IProductRepository
{

    Task<List<Product>> GetProductsAsync();

    Task<List<Product>> GetPagedAsync(int page, int itemsPerPage);

    Task<List<Product>> GetProductByNameAsync(string name);

    Task AddProductAsync(Product product, int idBrand, bool isAdmin);
    Task AddProductAsync(Product product, bool isAdmin);
    Task<bool> DeleteProductAsync(int productId, bool isAdmin);
    Task<bool> UpdateAsync(int productId, Product updatedProduct, bool isAdmin);
}