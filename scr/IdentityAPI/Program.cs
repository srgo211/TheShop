using IdentityAPI.Interfaces.Repositorys;
using IdentityAPI.Repositorys;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SharedDomainModels;
using SharedInterfaces;
using System.Text;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Configure JWT authentication
IConfigurationSection jwtSettings = builder.Configuration.GetSection("Jwt");
string secretKey = jwtSettings["SecretKey"];
string issuey = jwtSettings["Issuer"];

// Add services to the container.
builder.Services.AddControllers(); // This enables the use of controllers.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));


builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IJwtTokenService, JwtTokenService>(provider => new JwtTokenService(secretKey, issuey)); 


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Управление пользователями", Version = "v1" });

    // Adding JWT authentication scheme to Swagger
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
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

builder.Services.AddAuthorization();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Каталог товаров v1"));
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Ensure Authentication is used
app.UseAuthorization();

app.MapControllers();


// Получение значений URL и порта из конфигурации
string url = builder.Configuration.GetValue<string>("AppSettings:Url") ?? "http://localhost";
int port = builder.Configuration.GetValue<int?>("AppSettings:Port") ?? 5001;

string baseUrl = $"{url}:{port}";

app.Run(baseUrl);
