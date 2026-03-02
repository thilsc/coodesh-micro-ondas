using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MicroondasDigital.Middleware;
using MicroondasDigital.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AquecimentoModel>(); 

//Enable Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
                                            options.IdleTimeout = TimeSpan.FromMinutes(30);
                                            options.Cookie.HttpOnly = true;
                                            options.Cookie.IsEssential = true;
                                       });

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };

        // Para debug (remove em produção)
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"JWT Fail: {context.Exception.Message}");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// ========================================
// CRÍTICO: UseAuthentication ANTES de Authorization
// ========================================
app.UseAuthentication();
app.UseAuthorization();
// ========================================

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();
app.UseSession();

//API antes do MVC para evitar conflitos de rotas
app.MapControllerRoute(
    name: "api",
    pattern: "api/v1/{controller}/{action=Index}/{id?}"); 

//MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Microondas}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllers();

app.Run();
