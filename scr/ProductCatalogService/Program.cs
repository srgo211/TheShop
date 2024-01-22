var builder = WebApplication.CreateBuilder(args);

RegisterServices(builder.Services);

var app = builder.Build();

await Configure(app);

var apis = app.Services.GetServices<IApi>();
foreach (var api in apis)
{
    if (api is null) throw new InvalidProgramException("Api not found");
    api.Register(app);
}
app.Run();


async Task Configure(WebApplication app)
{

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseHttpsRedirection();
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