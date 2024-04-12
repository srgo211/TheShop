using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);






// Create a logger early on
var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
logger.LogInformation("����� ���������� ProductCatalogService");

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
    logger.LogInformation("������������ ����������");
    // Applying middleware
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();

    // ������������� ��������� �������������� � �����������
    app.UseAuthentication();
    app.UseAuthorization();

    // Register API routes
    var apis = app.Services.GetServices<IApi>();
    foreach (var api in apis)
    {
        if (api is null) throw new InvalidProgramException("Api not found");
        api.Register(app);
    }

    IConfiguration configuration = app.Configuration;

    // ��������� �������� URL � ����� �� ������������
    string url = configuration.GetValue<string>("AppSettings:Url") ?? "http://localhost";
    int port   = configuration.GetValue<int?>("AppSettings:Port") ?? 5001;

    string baseUrl = $"{url}:{port}";
    logger.LogInformation($"������ ���������� ��:{baseUrl}");

    

    app.Urls.Add(baseUrl);
}






void RegisterServices(IServiceCollection services)
{
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(opt =>
    {
        opt.SwaggerDoc("v1", new OpenApiInfo { Title = "������� �������", Version = "v1" });

        // ���������� ����� ������������ ��� ����������� ����� Bearer Token
        opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        opt.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                },
                new List<string>()
            }
        });

        opt.DocumentFilter<TagsOrderFilter>();
    });

    
    // ��������� �������������� JWT
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var secretKey = jwtSettings["SecretKey"]; // �������� SecretKey �� ������������
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"]
            };
        });

    services.AddAuthorization();

    // �������������� �������
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
        OpenApiTag teg1 = new OpenApiTag() { Name = "Create", Description = "�������� ������" };
        OpenApiTag teg2 = new OpenApiTag() { Name = "Read", Description = "��������� ������" };
        OpenApiTag teg3 = new OpenApiTag() { Name = "Update", Description = "���������� ������" };
        OpenApiTag teg4 = new OpenApiTag() { Name = "Delete", Description = "�������� ������" };
        OpenApiTag teg5 = new OpenApiTag() { Name = "�heck", Description = "�������� ������" };

        swaggerDoc.Tags.Add(teg1);
        swaggerDoc.Tags.Add(teg2);
        swaggerDoc.Tags.Add(teg3);
        swaggerDoc.Tags.Add(teg4);
    }


}