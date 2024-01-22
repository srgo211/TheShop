namespace ProductCatalogService.Repositorys;

public class CategorieRepository : ICategorieRepository
{
    private readonly AppDbContext dbContext;

    public CategorieRepository(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<List<Categorie>> GetCategoriesAsync()=> await dbContext.Categories.ToListAsync() ?? new();

    public async Task<int> AddCategorieAsync(Categorie categorie, bool isAdmin)
    {
        if (!isAdmin) return 0;
        dbContext.Categories.Add(categorie);
        await dbContext.SaveChangesAsync();
        return categorie.Id;
    }

    public async Task<bool> DeleteCategorieAsync(int categorieId, bool isAdmin)
    {
        if (!isAdmin) return false;
        var data = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == categorieId);

        if (data is null) return false;
        dbContext.Categories.Remove(data);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAsync(int brandId, Categorie updatedCategorie, bool isAdmin)
    {
        if (!isAdmin) return false;
        var data = await dbContext.Brands.FirstOrDefaultAsync(x => x.Id == brandId);
        if (data is null) return false;

        data.Name = updatedCategorie.Name;

        await dbContext.SaveChangesAsync();
        return true;
    }
}