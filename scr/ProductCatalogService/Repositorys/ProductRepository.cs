using Microsoft.EntityFrameworkCore;

namespace ProductCatalogService.Repositorys;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext dbContext;
    public ProductRepository(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task<List<Product>> GetProductsAsync()
    {
        return await dbContext.Products
            .Include(p => p.Brand)
            .Include(p => p.Categorie)
            .Include(p => p.Images)
            .ToListAsync() ?? new List<Product>();
    }

    public async Task<List<Product>> GetPagedAsync(int page, int itemsPerPage)
    {
        if (page <= 0 || itemsPerPage <= 0)
        {
            throw new ArgumentException("Page and itemsPerPage should be greater than 0");
        }

        int skipCount = (page - 1) * itemsPerPage;

        return await dbContext.Products
            .OrderBy(p => p.Id)  // Замените на нужное поле для сортировки
            .Skip(skipCount)
            .Take(itemsPerPage)
            .ToListAsync();
    }
    public async Task<List<Product>> GetProductByNameAsync(string name)
    {
        return await dbContext.Products
            .Include(p => p.Brand)
            .Include(p => p.Categorie)
            .Include(p => p.Images)
            .Where(x => x.Name == name)?.ToListAsync() ?? new List<Product>();
    }

    public async Task AddProductAsync(Product product, int idBrand, bool isAdmin)
    {
        if (!isAdmin) return;
        var brand = await dbContext.Brands.FirstOrDefaultAsync(x=> x.Id == idBrand);
        product.Brand = brand;
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();
    }
    public async Task<bool> DeleteProductAsync(int productId, bool isAdmin)
    {
        if (!isAdmin) return false;

        var product = await dbContext.Products
            .Include(p => p.Images) // Включаем связанные изображения
            .SingleOrDefaultAsync(p => p.Id == productId);

        if (product is null) return false;
       
        dbContext.Images.RemoveRange(product.Images);
        dbContext.Products.Remove(product);

        await dbContext.SaveChangesAsync();
        return true;
    }
    public async Task<bool> UpdateAsync(int productId, Product updatedProduct, bool isAdmin)
    {
        if (!isAdmin) return false;
        var product = await dbContext.Products
            .Include(p => p.Brand)
            .Include(p => p.Categorie)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(x => x.Id == productId);
        if (product is null) return false;

        product.Name          = updatedProduct.Name          ??  product.Name         ;
        product.Description   = updatedProduct.Description   ??  product.Description  ;     
        product.Price         = updatedProduct.Price;
        product.StockQuantity = updatedProduct.StockQuantity;

        if (product.Brand is null) await AddBrendAsync(updatedProduct.Brand, isAdmin);
        else
        {
            product.Brand.Name    = updatedProduct.Brand?.Name ?? product.Brand.Name;
            product.Brand.Country = updatedProduct.Brand?.Country ?? product.Brand.Country;
        }

        product.Categorie.Name     = updatedProduct.Categorie?.Name ?? product.Categorie.Name;

        for (int i = 0; i < product.Images.Count; i++)
        {
            var img = product.Images[0];
            var upImg = updatedProduct.Images.FirstOrDefault(x=>x.Id == img.Id);

            img.Name = upImg?.Name ?? img.Name;
        }

        await dbContext.SaveChangesAsync();

        return true;
    }
    


    public async Task<int> AddBrendAsync(Brand brand, bool isAdmin)
    {
        if (!isAdmin) return default;
        dbContext.Brands.Add(brand);
        await dbContext.SaveChangesAsync();
        return brand.Id;
    }

    public async Task AddProductAsync(Product product, bool isAdmin)
    {
        if (!isAdmin) return;
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();
    }

    
}
