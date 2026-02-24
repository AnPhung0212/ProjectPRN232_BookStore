var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ✅ Thêm dòng này để bật session
builder.Services.AddSession();

// Không bắt buộc, chỉ khi bạn cần TempData
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Cấu hình HTTP client gọi API
builder.Services.AddHttpClient("BookStoreAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ❗️ Quan trọng: phải thêm dòng này trước Authorization
app.UseSession(); // ✅ KHÔNG ĐƯỢC THIẾU

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
