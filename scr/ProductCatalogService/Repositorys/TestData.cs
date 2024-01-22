namespace ProductCatalogService.Repositorys;

public class TestData
{
    static Random random = new Random();

    public static async Task GenerateRandomDatasAsync(AppDbContext db, int countProduct, int countBrand, int countCategori)
    {
        List<Brand> brandList = GenerateRandomBrands(countBrand);
        List<Categorie> categorieList = GenerateRandomCategories(countCategori);

        List<Product> productList = GenerateRandomProducts(countProduct, countBrand, countCategori);

        await db.Brands.AddRangeAsync(brandList);
        await db.Categories.AddRangeAsync(categorieList);
        await db.Products.AddRangeAsync(productList);

        await db.SaveChangesAsync();
    }



    static List<Product> GenerateRandomProducts(int countProduct, int countBrand, int countCategori)
    {
        List<Product> products = new List<Product>();
       

        for (int i = 0; i < countProduct; i++)
        {
            int id = i + 1;
            Product product = new Product
            {

                Name = $"Продукт {id}",
                Description = $"Описание {id}",
                Price = (decimal)random.NextDouble() * 10_000,      // Пример рандомной цены от 0 до 1000
                StockQuantity = random.Next(1, 100),                // Пример рандомного количества на складе от 1 до 100
                BrandId = random.Next(1, countBrand + 1),           // Пример рандомного BrandId от 1 до 5
                CategorieId = random.Next(1, countCategori + 1),        // Пример рандомного CategorieId от 1 до 3
                Images = GenerateRandomImage(id, random.Next(1, 4))
            };

            // Вам нужно иметь уже созданные объекты Brand и Categorie для установки связей
            //product.Brand     = new Brand { Id = product.BrandId, Name = $"Бренд - {product.BrandId}", Country = $"Country{product.BrandId}" };
            //product.Categorie = new Categorie { Id = product.CategorieId, Name = $"Category{product.CategorieId}" };

            products.Add(product);
        }

        return products;
    }
    static List<Brand> GenerateRandomBrands(int count)
    {
        List<Brand> brands = new List<Brand>();        

        for (int i = 1; i <= count; i++)
        {

            Brand brand = new Brand
            {
               
                Name = $"Бренд {i}",
                Country = $"Страна"
            };

            brands.Add(brand);
        }

        return brands;
    }
    static List<Categorie> GenerateRandomCategories(int count)
    {
        List<Categorie> categories = new List<Categorie>();        

        for (int i = 1; i <= count; i++)
        {
            Categorie category = new Categorie
            {
                
                Name = $"Категория {i}"
            };

            categories.Add(category);
        }

        return categories;
    }
    static List<Image> GenerateRandomImage(int id, int count)
    {
        List<Image> datas = new List<Image>();
        for (int i = 1; i <= count; i++)
        {
            Image data = new Image
            {
                Name = $"Путь до фото\\{i}.png"
            };

            datas.Add(data);
        }

        return datas;
    }
}
