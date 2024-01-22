using ProductCatalogService.DTO;

namespace ProductCatalogService.Interfaces.Repositorys;

public interface ICategorieRepository
{
    Task<List<Categorie>> GetCategoriesAsync();
    Task<int> AddCategorieAsync(Categorie categorie, bool isAdmin);
    Task<bool> DeleteCategorieAsync(int categorieId, bool isAdmin);
    Task<bool> UpdateAsync(int brandId, Categorie updatedCategorie, bool isAdmin);
}