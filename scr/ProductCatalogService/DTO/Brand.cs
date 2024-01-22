using ProductCatalogService.Interfaces.Models;

namespace ProductCatalogService.DTO;

public class Brand : Base, IBrand
{
   public string Country { get; set; }
}