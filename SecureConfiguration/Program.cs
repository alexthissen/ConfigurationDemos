using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration.UserSecrets;
using SecureConfiguration;
using System.Reflection;

// Generated from MSBuild project property
//[assembly: UserSecretsId("ConfigurationWorkerSecrets")]

var builder = Host.CreateApplicationBuilder(args);

// Code below is added automatically by CreateDefaultBuilder
//config.AddUserSecrets(appAssembly, optional: true, reloadOnChange: reloadOnChange);
                    
IConfigurationSection section = builder.Configuration.GetSection("KeyVault");                    
if (section.Exists() && !String.IsNullOrEmpty(section["VaultUri"]))
{
    var credential =
        // For managed identities use:
        // new DefaultAzureCredential();

        // Code below doesn't work because of multiple parametrized constructors
        //section.Get<ClientSecretCredential>(options => { options.BindNonPublicProperties = true; });

        new ClientSecretCredential(
            section["TenantId"],
            section["ClientId"],
            section["ClientSecret"]);

    SecretClient secretClient = new(section.GetValue<Uri>("VaultUri"), credential);
    builder.Configuration.AddAzureKeyVault(secretClient, 
        new AzureKeyVaultConfigurationOptions()
        {
            Manager = new KeyVaultSecretManager(),
            ReloadInterval = TimeSpan.FromMinutes(1)
        });
}

var options = builder.Configuration.GetSection(nameof(Worker)).Get<WorkerOptions>();
builder.Services
    .AddOptions<WorkerOptions>()
    .Bind<WorkerOptions>(builder.Configuration.GetSection(nameof(Worker)));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();