using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using BookStore.BusinessObject.Models; // namespace chứa BookStoreDbContext

using BookStore.Services;
using BookStore.Services.Implement;
using BookStore.Services.Interfaces;
using BookStore.DataAccessObject;
using BookStore.DataAccessObject.IRepository;
using BookStore.DataAccessObject.Repository;
using Microsoft.EntityFrameworkCore;
using BookStore.API.Middleware;
using BookStore.Services.Catalog.MailServices;
using BookStore.BusinessObject.Config;
using static BookStore.BusinessObject.Config.MailSetting;

var builder = WebApplication.CreateBuilder(args);
//Connect VNPay API

// Read JWT configuration from appsettings.json
var configuration = builder.Configuration;
var jwtIssuer = configuration["JwtSettings:Issuer"];
var jwtAudience = configuration["JwtSettings:Audience"];
var jwtKey = configuration["JwtSettings:Key"];

if (string.IsNullOrEmpty(jwtKey))
{
    // Log lỗi hoặc quăng exception nếu thiếu key
    throw new InvalidOperationException("JWT Key is missing from configuration.");
}
// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});
builder.Services.AddHttpContextAccessor();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Dependency Injection
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
// Đăng ký mail service và cấu hình
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.Configure<JwtEmailSettings>(builder.Configuration.GetSection("JwtEmailSettings"));

builder.Services.AddScoped<IRegisterService, RegisterService>();

// Add Controllers
builder.Services.AddControllers();
// Đăng ký DbContext
builder.Services.AddDbContext<BookStoreContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 10,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }));

// Swagger with JWT Authorization
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "BookStore API", Version = "v1" });

    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Nhập token theo định dạng: Bearer {your_token}",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"

    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    // Thêm đoạn này để Swagger đọc được comment XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    option.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Middleware pipeline
// Cho phép chạy Swagger ở mọi môi trường (hoặc xóa if check)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookStore API V1");
    c.RoutePrefix = "swagger"; // Đảm bảo đường dẫn là /swagger
});

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseMiddleware<AuthorizationMiddleware>();

app.UseAuthentication(); // Bắt buộc nếu dùng JWT
app.UseAuthorization();

app.MapControllers();
app.Run();
