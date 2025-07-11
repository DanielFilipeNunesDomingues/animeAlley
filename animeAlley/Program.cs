using animeAlley.Configuration;
using animeAlley.Data;
using animeAlley.Data.Seed;
using animeAlley.Hubs;
using animeAlley.Mapping;
using animeAlley.Models;
using animeAlley.Services;
using animeAlley.Services.Implementations;
using animeAlley.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Define database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configure Identity with proper authentication schemes
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

    options.SignIn.RequireConfirmedEmail = true;
    options.SignIn.RequireConfirmedAccount = true;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;

    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI(); 

// Configure Cookie Authentication (for web pages)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// Configure JWT settings
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtSettings>(jwtSection);
var jwtSettings = jwtSection.Get<JwtSettings>();

if (jwtSettings == null)
{
    throw new InvalidOperationException("JWT settings não configuradas corretamente no appsettings.json");
}

// Configure JWT Authentication (for API)
builder.Services.AddAuthentication()
    .AddJwtBearer("Bearer", options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ClockSkew = TimeSpan.Zero
        };
    });

// Configure authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));

    // API-specific policies that use JWT
    options.AddPolicy("ApiAdminOnly", policy =>
    {
        policy.RequireRole("Admin");
        policy.AuthenticationSchemes.Add("JWT");
    });

    options.AddPolicy("ApiUserOnly", policy =>
    {
        policy.RequireRole("User");
        policy.AuthenticationSchemes.Add("JWT");
    });
});

// Add MVC services
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Configure session
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Increased from 60 seconds
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddDistributedMemoryCache();

// Configure SendGrid
builder.Services.Configure<AuthMessageSenderOptions>(options =>
{
    options.SendGridKey = builder.Configuration["SendGridKey"] ?? throw new InvalidOperationException("SendGridKey não configurado");
    options.FromEmail = builder.Configuration["FromEmail"] ?? throw new InvalidOperationException("FromEmail não configurado");
    options.FromName = builder.Configuration["FromName"] ?? throw new InvalidOperationException("FromName não configurado");
});

// Register services
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<UtilizadorService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddHttpContextAccessor();

// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "animeAlley API",
        Version = "v2",
        Description = "API para gestão de shows, autores, personagens, generos e estúdios"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// SignalR configuration
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
});

// AutoMapper configuration
builder.Services.AddAutoMapper(typeof(ShowsMappingProfile));
builder.Services.AddAutoMapper(typeof(AutoresMappingProfile));
builder.Services.AddAutoMapper(typeof(StudiosMappingProfile));
builder.Services.AddAutoMapper(typeof(PersonagensMappingProfile));
builder.Services.AddAutoMapper(typeof(GenerosMappingProfile));
builder.Services.AddAutoMapper(typeof(EstatisticasMappingProfile));

// Business services
builder.Services.AddScoped<IShowsService, ShowsService>();
builder.Services.AddScoped<IAutorService, AutorService>();
builder.Services.AddScoped<IStudioService, StudioService>();
builder.Services.AddScoped<IPersonagemService, PersonagemService>();
builder.Services.AddScoped<IGeneroService, GeneroService>();
builder.Services.AddScoped<IEstatisticaService, EstatisticaService>();

// Configure JSON serialization
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

var app = builder.Build();

// Seed roles and admin function
async Task SeedRolesAndAdmin(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

    string[] roleNames = { "Admin", "User" };
    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    var adminEmail = "admin@animealley.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        // First create the Utilizador
        var utilizador = new Utilizador
        {
            Nome = "Administrador",
            UserName = adminEmail,
            isAdmin = true,
            Foto = "placeholder.png"
        };

        context.Utilizadores.Add(utilizador);
        await context.SaveChangesAsync();

        // Then create the ApplicationUser
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            UtilizadorId = utilizador.Id,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await userManager.CreateAsync(adminUser, "Admin123!");
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DbInitializer.Initialize(context);
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseItToSeedSqlServer();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Session must come before authentication
app.UseSession();

// Authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Status code pages
app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");

// Map routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.MapHub<ShowsHub>("/showsHub");
app.MapControllers();

// Execute seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedRolesAndAdmin(services);
}

app.Run();