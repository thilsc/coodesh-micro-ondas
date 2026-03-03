var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<MicroondasCliente.Security.AuthHeaderHandler>();

// 1. Configurar HttpClient para a API
builder.Services.AddHttpClient("MicroondasApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5161/api/v1/MicroondasApi/");
})
.AddHttpMessageHandler<MicroondasCliente.Security.AuthHeaderHandler>();

// 2. Habilitar Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Inicializar ApiResponseHelper
MicroondasCliente.Models.ApiResponseHelper.Initialize(app.Services.GetRequiredService<IHttpClientFactory>());

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
