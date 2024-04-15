using Microsoft.EntityFrameworkCore;
using SharedDomainModels;

namespace IdentityAPI.Repositorys;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) => Database.EnsureCreated();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure the User entity
        modelBuilder.Entity<User>(entity =>
        {
          
            entity.HasKey(u => u.Guid);
            //entity.Property(u => u.Guid).ValueGeneratedOnAdd().HasDefaultValueSql("uuid_generate_v4()");

            entity.HasIndex(u => u.UserId).IsUnique();
            entity.Property(u => u.UserName).HasMaxLength(50);
            entity.Property(u => u.Email).HasMaxLength(50);
        });
    }
}