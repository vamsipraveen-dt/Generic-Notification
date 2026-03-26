using GenericNotification.Core.Domain.Models;
using GenericNotification.Core.Domain.Repositories;
using GenericNotification.Core.Domain.Services;
using GenericNotification.Core.Persistence.Repositories;
using GenericNotification.Core.Services;
using GenericNotification.Core.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;



var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Info("Init main");
try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    ConfigureConfiguration(builder);

    await ConfigureServices(builder);

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Host.UseNLog();
    var app = builder.Build();

    logger.Info("Application build successful");

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    await app.RunAsync();

}
catch (Exception ex)
{
    // NLog: catch setup errors
    logger.Error(ex, ex.Message);
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}


static void ConfigureConfiguration(WebApplicationBuilder builder)
{
    builder.Configuration
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
        .AddEnvironmentVariables();
}


async Task ConfigureServices(WebApplicationBuilder builder)
{

    var isDevelopment = builder.Environment.IsDevelopment();

    string dBConnectionString = string.Empty;
    string raConnectionString = string.Empty;

    var isVaultActive = builder.Configuration.GetValue<bool>("isVaultActive");

    if (!isVaultActive || isDevelopment)
    {
        dBConnectionString = builder.Configuration.GetConnectionString("DBConnectionString")
    ?? throw new InvalidOperationException("DBConnectionString is missing");

        raConnectionString = builder.Configuration.GetConnectionString("RAConnectionString")
            ?? throw new InvalidOperationException("RAConnectionString is missing");
    }
    else
    {
        var vaultAddress = builder.Configuration["Vault:Address"];
        var vaultToken = builder.Configuration["Vault:Token"];
        var secretPath = builder.Configuration["Vault:SecretPath"];
        var mountPath = builder.Configuration["Vault:MountPath"];

        // Initialize Vault client
        var authMethod = new TokenAuthMethodInfo(vaultToken);
        var vaultClientSettings = new VaultClientSettings(vaultAddress, authMethod);
        var vaultClient = new VaultClient(vaultClientSettings);

        // Fetch secret data from Vault
        var secret = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: secretPath, mountPoint: mountPath);
        var data = secret.Data.Data;

        dBConnectionString = data["DBConnectionString"]?.ToString()
            ?? throw new InvalidOperationException("DBConnectionString missing in Vault");

        raConnectionString = data["RAConnectionString"]?.ToString()
            ?? throw new InvalidOperationException("RAConnectionString missing in Vault");
    }

    if (string.IsNullOrWhiteSpace(dBConnectionString))
        throw new InvalidOperationException("DBConnectionString missing from Vault");
    else
        logger.Info($"DB Connection String :: {dBConnectionString} ");

    if (string.IsNullOrWhiteSpace(raConnectionString))
        throw new InvalidOperationException("RAConnectionString missing from Vault");
    else
        logger.Info($"RA Connection String :: {raConnectionString} ");

    builder.Services.AddDbContextPool<GenericNotificationContext>(options => options.UseNpgsql(dBConnectionString));

    builder.Services.AddDbContextPool<Ra02Context>(options => options.UseNpgsql(raConnectionString));

    builder.Services.AddHttpClient();

    builder.Services.AddScoped<ISubscriberRepository, SubscriberRepository>();
    builder.Services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

    builder.Services.AddScoped<IPushNotificationClient, PushNotificationClient>();
    builder.Services.AddScoped<IGlobalConfiguration, GlobalConfiguration>();

    builder.Services.AddScoped<IEmailSender, EmailSender>();
    builder.Services.AddScoped<IEmailService, EmailService>();
    builder.Services.AddScoped<ISmsService, SmsService>();
    builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
    builder.Services.AddScoped<IPushNotificationService, PushNotificationService>();
}