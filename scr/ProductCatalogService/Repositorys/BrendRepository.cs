namespace ProductCatalogService.Repositorys;

public class BrendRepository : IBrendRepository
{
    private readonly AppDbContext dbContext;
    public BrendRepository(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    /// <summary>Получить данные </summary>
    public async Task<List<Brand>> GetBrendsAsync() => await dbContext.Brands.ToListAsync() ?? new();

    public async Task<int> AddBrendAsync(Brand brand, bool isAdmin)
    {
        if (!isAdmin) return 0;
        dbContext.Brands.Add(brand);
        await dbContext.SaveChangesAsync();
        return brand.Id;
    }

    public async Task<bool> DeleteBrendAsync(int brandId, bool isAdmin)
    {
        if (!isAdmin) return false;
        var data = await dbContext.Brands.FirstOrDefaultAsync(x=>x.Id == brandId);
        
        if(data is null) return false;
        dbContext.Brands.Remove(data);
        await dbContext.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> UpdateAsync(int brandId, Brand updatedBrand, bool isAdmin)
    {
        if (!isAdmin) return false;
        var data = await dbContext.Brands.FirstOrDefaultAsync(x => x.Id == brandId);
        if (data is null) return false;

        data.Name    = updatedBrand.Name;
        data.Country = updatedBrand.Country;

        await dbContext.SaveChangesAsync();
        return true;
    }
  

}