namespace ProductCatalogService.Repositorys;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Categorie> Categories => Set<Categorie>();
    public DbSet<Image> Images => Set<Image>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)=> Database.EnsureCreated();
   

}
