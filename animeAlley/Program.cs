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

// ========================================
// CONFIGURAÇÃO DO BANCO DE DADOS
// ========================================

/// <summary>
/// Configuração da string de conexão com o banco de dados SQL Server
/// </summary>
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

/// <summary>
/// Configuração do contexto do Entity Framework com SQL Server
/// </summary>
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

/// <summary>
/// Adiciona filtros de exceção para páginas de desenvolvimento do banco de dados
/// </summary>
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ========================================
// CONFIGURAÇÃO DO IDENTITY
// ========================================

/// <summary>
/// Configuração do ASP.NET Identity para gerenciamento de usuários e autenticação
/// </summary>
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    // Configurações de usuário
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

    // Configurações de login
    options.SignIn.RequireConfirmedEmail = true;
    options.SignIn.RequireConfirmedAccount = true;

    // Configurações de bloqueio de conta
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;

    // Configurações de senha
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

// ========================================
// CONFIGURAÇÃO DE AUTENTICAÇÃO
// ========================================

/// <summary>
/// Configuração dos cookies de autenticação para as páginas web
/// </summary>
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

/// <summary>
/// Configuração das definições JWT a partir do appsettings.json
/// </summary>
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtSettings>(jwtSection);
var jwtSettings = jwtSection.Get<JwtSettings>();

if (jwtSettings == null)
{
    throw new InvalidOperationException("JWT settings não configuradas corretamente no appsettings.json");
}

/// <summary>
/// Configuração da autenticação JWT para APIs
/// </summary>
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

// ========================================
// CONFIGURAÇÃO DE AUTORIZAÇÃO
// ========================================

/// <summary>
/// Configuração das políticas de autorização para diferentes tipos de usuários
/// </summary>
builder.Services.AddAuthorization(options =>
{
    // Políticas para páginas web (usando cookies)
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));

    // Políticas específicas para APIs (usando JWT)
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

// ========================================
// CONFIGURAÇÃO MVC E RAZOR PAGES
// ========================================

/// <summary>
/// Adiciona suporte para Controllers com Views e Razor Pages
/// </summary>
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// ========================================
// CONFIGURAÇÃO DE SESSÃO
// ========================================

/// <summary>
/// Configuração de sessões para armazenar dados temporários do usuário
/// </summary>
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddDistributedMemoryCache();

// ========================================
// CONFIGURAÇÃO DE EMAIL (SENDGRID)
// ========================================

/// <summary>
/// Configuração do SendGrid para envio de emails
/// </summary>
builder.Services.Configure<AuthMessageSenderOptions>(options =>
{
    options.SendGridKey = builder.Configuration["SendGridKey"]
        ?? throw new InvalidOperationException("SendGridKey não configurado");
    options.FromEmail = builder.Configuration["FromEmail"]
        ?? throw new InvalidOperationException("FromEmail não configurado");
    options.FromName = builder.Configuration["FromName"]
        ?? throw new InvalidOperationException("FromName não configurado");
});

// ========================================
// REGISTRO DE SERVIÇOS
// ========================================

/// <summary>
/// Registro dos serviços de negócio e infraestrutura
/// </summary>
// Serviços de infraestrutura
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<UtilizadorService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddHttpContextAccessor();

// ========================================
// CONFIGURAÇÃO DO SWAGGER
// ========================================

/// <summary>
/// Configuração do Swagger para documentação da API
/// </summary>
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "animeAlley API",
        Version = "v2",
        Description = "API para gestão de shows, autores, personagens, generos e estúdios"
    });

    // Configuração de segurança JWT no Swagger
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

    // Inclusão de documentação XML
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// ========================================
// CONFIGURAÇÃO DO SIGNALR
// ========================================

/// <summary>
/// Configuração do SignalR para comunicação em tempo real
/// </summary>
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
});

// ========================================
// CONFIGURAÇÃO DO AUTOMAPPER
// ========================================

/// <summary>
/// Configuração do AutoMapper para mapeamento de objetos
/// </summary>
builder.Services.AddAutoMapper(typeof(ShowsMappingProfile));
builder.Services.AddAutoMapper(typeof(AutoresMappingProfile));
builder.Services.AddAutoMapper(typeof(StudiosMappingProfile));
builder.Services.AddAutoMapper(typeof(PersonagensMappingProfile));
builder.Services.AddAutoMapper(typeof(GenerosMappingProfile));
builder.Services.AddAutoMapper(typeof(EstatisticasMappingProfile));

// ========================================
// SERVIÇOS DE NEGÓCIO
// ========================================

/// <summary>
/// Registro dos serviços de negócio específicos da aplicação
/// </summary>
builder.Services.AddScoped<IShowsService, ShowsService>();
builder.Services.AddScoped<IAutorService, AutorService>();
builder.Services.AddScoped<IStudioService, StudioService>();
builder.Services.AddScoped<IPersonagemService, PersonagemService>();
builder.Services.AddScoped<IGeneroService, GeneroService>();
builder.Services.AddScoped<IEstatisticaService, EstatisticaService>();

// ========================================
// CONFIGURAÇÃO JSON
// ========================================

/// <summary>
/// Configuração da serialização JSON para evitar referências circulares
/// </summary>
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

var app = builder.Build();

// ========================================
// FUNÇÃO DE INICIALIZAÇÃO DE DADOS
// ========================================

/// <summary>
/// Função responsável por criar roles padrão e usuário administrador
/// </summary>
/// <param name="serviceProvider">Provedor de serviços do DI container</param>
async Task SeedRolesAndAdmin(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

    // Criação dos roles padrão
    string[] roleNames = { "Admin", "User" };
    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Criação do usuário administrador padrão
    var adminEmail = "admin@animealley.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        // Primeiro cria o Utilizador
        var utilizador = new Utilizador
        {
            Nome = "Administrador",
            UserName = adminEmail,
            isAdmin = true,
            Foto = "placeholder.png"
        };

        context.Utilizadores.Add(utilizador);
        await context.SaveChangesAsync(); // Gera o ID

        // Depois cria o ApplicationUser com o ID gerado
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            UtilizadorId = utilizador.Id,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var result = await userManager.CreateAsync(adminUser, "Admin123!");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
        else
        {
            // Se falhar, remove o Utilizador criado
            context.Utilizadores.Remove(utilizador);
            await context.SaveChangesAsync();

            // Log dos erros para debug
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"Error creating admin user: {error.Description}");
            }
        }
    }
}

// ========================================
// INICIALIZAÇÃO DO BANCO DE DADOS
// ========================================

/// <summary>
/// Inicializa o banco de dados com dados padrão
/// </summary>
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DbInitializer.Initialize(context);
}

// ========================================
// CONFIGURAÇÃO DO PIPELINE HTTP
// ========================================

/// <summary>
/// Configuração do pipeline de requisições HTTP
/// </summary>
if (app.Environment.IsDevelopment())
{
    // Configurações para ambiente de desenvolvimento
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseItToSeedSqlServer();
    app.UseExceptionHandler("/Home/ErrorDev");
}
else
{
    // Configurações para ambiente de produção
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Middlewares básicos
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Importante: Session deve vir antes da autenticação
app.UseSession();

// Autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

// Páginas de erro personalizadas
app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");

// ========================================
// MAPEAMENTO DE ROTAS
// ========================================

/// <summary>
/// Configuração das rotas da aplicação
/// </summary>
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.MapHub<ShowsHub>("/showsHub");
app.MapControllers();

// ========================================
// EXECUÇÃO DA INICIALIZAÇÃO
// ========================================

/// <summary>
/// Executa a inicialização de dados (roles e admin)
/// </summary>
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedRolesAndAdmin(services);
}

// Inicia a aplicação
app.Run();