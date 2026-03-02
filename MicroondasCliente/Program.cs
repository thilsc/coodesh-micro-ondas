var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// 1. Configurar HttpClient para a API
builder.Services.AddHttpClient("MicroondasApi", client =>
{
    // A porta da sua API (ajuste se mudar)
    client.BaseAddress = new Uri("http://localhost:5161/api/v1/"); 
});

// 2. Habilitar Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();

// 3. Adicionar middleware de Session ANTES de Auth
app.UseSession(); 

app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
