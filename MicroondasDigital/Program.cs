var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Enable Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
                                            options.IdleTimeout = TimeSpan.FromMinutes(30);
                                            options.Cookie.HttpOnly = true;
                                            options.Cookie.IsEssential = true;
                                       });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Microondas}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
