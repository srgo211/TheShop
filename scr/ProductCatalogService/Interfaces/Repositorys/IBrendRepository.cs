using ProductCatalogService.DTO;

namespace ProductCatalogService.Interfaces.Repositorys;

public interface IBrendRepository
{
    Task<List<Brand>> GetBrendsAsync();
    Task<int> AddBrendAsync(Brand brand, bool isAdmin);
    Task<bool> DeleteBrendAsync(int brandId, bool isAdmin);
    Task<bool> UpdateAsync(int brandId, Brand updatedBrand, bool isAdmin);
}