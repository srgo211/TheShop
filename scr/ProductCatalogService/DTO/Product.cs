namespace ProductCatalogService.DTO;

public class Product : Base, IProduct
{
    public string Description { get; set; }
   
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    
    public int BrandId { get; set; }
    public Brand? Brand { get; set; }

    public int CategorieId { get; set; }
    public Categorie? Categorie { get; set; }
   
    public List<Image>? Images { get; set; }
}
