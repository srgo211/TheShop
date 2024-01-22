using Microsoft.EntityFrameworkCore;

namespace ProductCatalogService.Repositorys;

public class DataCheckerRepository : IDataCheckerRepository
{
    private readonly AppDbContext db;  
    public DataCheckerRepository(AppDbContext db)
    {
        this.db = db;
    }
    public async Task<int> GetProductStockCountAsync(int productId)
    {
        var data = await db.Products.FirstOrDefaultAsync(x=>x.Id== productId);
        if(data is null) return 0;
        return data.StockQuantity;
    }

    public async Task<bool> ReduceStockAsync(int productId, int quantity)
    {
        var data = await db.Products.FirstOrDefaultAsync(x => x.Id == productId);
        if (data is null) return false;
        var quantityBd = data.StockQuantity;

        if(quantityBd < quantity) return false;

        data.StockQuantity = quantityBd - quantity;

        await db.SaveChangesAsync();

        return true;

    }
}
