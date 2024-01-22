using ProductCatalogService.DTO;

namespace ProductCatalogService.Interfaces.Models;

public interface IProduct : IBase
{
    string Description { get; set; }
    decimal Price { get; set; }
    int StockQuantity { get; set; }

    int BrandId { get; set; }
    Brand Brand { get; set; }

    int CategorieId { get; set; }
    Categorie Categorie { get; set; }

    List<Image> Images { get; set; }
}
