WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Create a logger early on
var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
logger.LogInformation("Старт приложения ProductCatalogService");

RegisterServices(builder.Services);
WebApplication app = builder.Build();
ConfigureApplication(app);
app.Run();


async Task Configure(WebApplication app)
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
}


void ConfigureApplication(WebApplication app)
{
    logger.LogInformation("Конфигурация приложения");
    // Applying middleware
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();

    // Register API routes
    var apis = app.Services.GetServices<IApi>();
    foreach (var api in apis)
    {
        if (api is null) throw new InvalidProgramException("Api not found");
        api.Register(app);
    }

    IConfiguration configuration = app.Configuration;

    // Получение значений URL и порта из конфигурации
    string url = configuration.GetValue<string>("AppSettings:Url") ?? "http://localhost";
    int port   = configuration.GetValue<int?>("AppSettings:Port") ?? 5001;

    string baseUrl = $"{url}:{port}";
    logger.LogInformation($"Запуск приложения на:{baseUrl}");
    app.Urls.Add(baseUrl);
}




void RegisterServices(IServiceCollection services)
{
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(opt =>
    {
        opt.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Каталог товаров", Version = "v1" });
        opt.DocumentFilter<TagsOrderFilter>();
    });


    services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

    services.AddTransient<IApi, ProductApi>();
    services.AddTransient<IApi, BrendApi>();
    services.AddTransient<IApi, CategorieApi>();
    services.AddTransient<IApi, CheckDataApi>();
    services.AddScoped<IProductRepository, ProductRepository>();
    services.AddScoped<IBrendRepository, BrendRepository>();
    services.AddScoped<ICategorieRepository, CategorieRepository>();
    services.AddScoped<IDataCheckerRepository, DataCheckerRepository>();
}


public class TagsOrderFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        OpenApiTag teg1 = new OpenApiTag() { Name = "Create", Description = "Создание данных" };
        OpenApiTag teg2 = new OpenApiTag() { Name = "Read", Description = "Получение данных" };
        OpenApiTag teg3 = new OpenApiTag() { Name = "Update", Description = "Обновление данных" };
        OpenApiTag teg4 = new OpenApiTag() { Name = "Delete", Description = "Удаление данных" };
        OpenApiTag teg5 = new OpenApiTag() { Name = "Сheck", Description = "Проверка данных" };

        swaggerDoc.Tags.Add(teg1);
        swaggerDoc.Tags.Add(teg2);
        swaggerDoc.Tags.Add(teg3);
        swaggerDoc.Tags.Add(teg4);
    }


}