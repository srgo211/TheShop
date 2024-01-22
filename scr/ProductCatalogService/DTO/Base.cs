using ProductCatalogService.Interfaces.Models;

namespace ProductCatalogService.DTO;

public class Base : IBase
{
    public int Id { get; set; }
    public string Name { get; set; }
}