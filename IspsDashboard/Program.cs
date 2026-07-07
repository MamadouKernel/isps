using System.Globalization;
using IspsDashboard.Data;
using IspsDashboard.Data.Seed;
using IspsDashboard.Hubs;
using IspsDashboard.Infrastructure;
using IspsDashboard.Jobs;
using IspsDashboard.Models.Entities;
using IspsDashboard.Services.Implementations;
using IspsDashboard.Services.Interfaces;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/isps-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateBootstrapLogger();

try
{
    // Licence QuestPDF Community (gratuit < 1 M$ de CA annuel) — définie globalement.
    QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(
            path: "logs/isps-.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30));

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionString))
        throw new InvalidOperationException(
            "Chaîne de connexion 'DefaultConnection' absente. Définissez-la via la variable d'environnement " +
            "ConnectionStrings__DefaultConnection (ou appsettings.{Environment}.json en développement).");

    builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    builder.Services
        .AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        // Le cookie suit le protocole de la requête : fonctionne en HTTP comme en HTTPS.
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("RequireAdmin", p => p.RequireRole(DataSeeder.AdminRole));
        options.AddPolicy("RequireEditor", p => p.RequireRole(DataSeeder.AdminRole, DataSeeder.EditorRole));
    });

    // Persistance des clés DataProtection sur disque (sinon elles changent à chaque redémarrage)
    builder.Services
        .AddDataProtection()
        .SetApplicationName("IspsDashboard")
        .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "App_Data", "keys")));

    builder.Services.AddControllersWithViews(options =>
        {
            // Ne garder que les [Required] explicites (pas le Required implicite des NRT).
            options.ModelMetadataDetailsProviders.Add(new DisableImplicitRequiredProvider());

            // Messages de binding (conversion/valeur manquante) en français.
            var m = options.ModelBindingMessageProvider;
            m.SetValueIsInvalidAccessor(v => $"La valeur « {v} » est invalide.");
            m.SetAttemptedValueIsInvalidAccessor((v, f) => $"La valeur « {v} » est invalide pour le champ « {f} ».");
            m.SetValueMustNotBeNullAccessor(v => "Ce champ est obligatoire.");
            m.SetMissingBindRequiredValueAccessor(f => $"Le champ « {f} » est requis.");
            m.SetMissingKeyOrValueAccessor(() => "Une valeur est requise.");
            m.SetUnknownValueIsInvalidAccessor(f => $"La valeur fournie pour « {f} » est invalide.");
            m.SetValueMustBeANumberAccessor(f => $"Le champ « {f} » doit être un nombre.");
            m.SetNonPropertyValueMustBeANumberAccessor(() => "La valeur doit être un nombre.");
        })
        .AddRazorRuntimeCompilation();

    // Application en français (messages de validation framework localisés).
    var frChain = new[] { new CultureInfo("fr-FR") };
    builder.Services.Configure<RequestLocalizationOptions>(options =>
    {
        options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("fr-FR");
        options.SupportedCultures = frChain;
        options.SupportedUICultures = frChain;
    });
    builder.Services.AddSignalR();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddMemoryCache();

    builder.Services.AddSingleton<IClock, AbidjanClock>();
    builder.Services.AddSingleton<ISecretProtector, DataProtectionSecretProtector>();
    builder.Services.AddScoped<IDashboardService, DashboardService>();
    builder.Services.AddScoped<IExerciseService, ExerciseService>();
    builder.Services.AddScoped<IAuditService, AuditService>();
    builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
    builder.Services.AddScoped<IRazorTemplateRenderer, RazorTemplateRenderer>();
    builder.Services.AddScoped<IIncidentService, IncidentService>();
    builder.Services.AddScoped<IPersonnelService, PersonnelService>();
    builder.Services.AddScoped<IHabilitationService, HabilitationService>();
    builder.Services.AddScoped<IVisitorService, VisitorService>();
    builder.Services.AddScoped<IPatrolService, PatrolService>();
    builder.Services.AddScoped<IMarsecService, MarsecService>();
    builder.Services.AddScoped<INonConformityService, NonConformityService>();
    builder.Services.AddScoped<ITodayBriefService, TodayBriefService>();
    builder.Services.AddScoped<IVesselService, VesselService>();
    builder.Services.AddScoped<ICameraService, CameraService>();
    builder.Services.AddScoped<IContactService, ContactService>();
    builder.Services.AddScoped<IBriefingService, BriefingService>();
    builder.Services.AddScoped<IRexService, RexService>();
    builder.Services.AddScoped<IPdfReportService, PdfReportService>();
    builder.Services.AddScoped<IVehicleAccessService, VehicleAccessService>();
    builder.Services.AddScoped<IDocumentService, DocumentService>();
    builder.Services.AddScoped<IAuditCampaignService, AuditCampaignService>();
    builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
    builder.Services.AddScoped<IAccessPassService, AccessPassService>();
    builder.Services.AddScoped<IAlertService, AlertService>();
    builder.Services.AddSingleton<IQrCodeService, QrCodeService>();
    builder.Services.AddScoped<IBadgePdfService, BadgePdfService>();
    builder.Services.AddSingleton<IExportService, ExportService>();
    builder.Services.AddScoped<IRestrictedZoneService, RestrictedZoneService>();
    builder.Services.AddScoped<ISoftDeleteService, SoftDeleteService>();

    builder.Services.AddQuartz(q =>
    {
        var jobKey = new JobKey("ExerciseAlertJob");
        q.AddJob<ExerciseAlertJob>(opts => opts.WithIdentity(jobKey));
        q.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity("ExerciseAlertJob-trigger")
            .WithCronSchedule("0 0 8 * * ?", x => x.InTimeZone(TimeZoneInfo.FindSystemTimeZoneById(
                OperatingSystem.IsWindows() ? "GMT Standard Time" : "Africa/Abidjan"))));

        var purgeKey = new JobKey("SoftDeletePurgeJob");
        q.AddJob<SoftDeletePurgeJob>(opts => opts.WithIdentity(purgeKey));
        q.AddTrigger(opts => opts
            .ForJob(purgeKey)
            .WithIdentity("SoftDeletePurgeJob-trigger")
            .WithCronSchedule("0 30 3 * * ?", x => x.InTimeZone(TimeZoneInfo.FindSystemTimeZoneById(
                OperatingSystem.IsWindows() ? "GMT Standard Time" : "Africa/Abidjan"))));
    });
    builder.Services.AddQuartzHostedService(opts => opts.WaitForJobsToComplete = true);

    builder.Services.AddHealthChecks()
        .AddSqlServer(connectionString, name: "sql-server", tags: new[] { "db" });

    var app = builder.Build();

    // HTTPS optionnel : par défaut l'application sert en HTTP (prod sur réseau interne /
    // derrière un reverse proxy). Mettre "Hosting:UseHttps": true pour forcer HTTPS + HSTS.
    var useHttps = app.Configuration.GetValue("Hosting:UseHttps", false);

    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        if (useHttps) app.UseHsts();
    }

    app.UseRequestLocalization();
    app.UseSerilogRequestLogging();
    if (useHttps) app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Dashboard}/{action=Index}/{id?}");

    app.MapHub<DashboardHub>("/hubs/dashboard");
    app.MapHealthChecks("/health");

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var db = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var config = services.GetRequiredService<IConfiguration>();
        await DataSeeder.SeedAsync(db, userManager, roleManager, config);
        await SampleDataSeeder.SeedAsync(db, config);
    }

    Log.Information("ISPS Dashboard démarré");
    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Échec critique au démarrage de l'application");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
