using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration.UserSecrets;
using SecureConfiguration;
using System.Reflection;

// Generated from MSBuild project property
//[assembly: UserSecretsId("ConfigurationWorkerSecrets")]

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        // Code below is added automatically by CreateDefaultBuilder
        //config.AddUserSecrets(appAssembly, optional: true, reloadOnChange: reloadOnChange);
                    
        var partialConfig = config.Build();
        IConfigurationSection section = partialConfig.GetSection("KeyVault");
                    
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

            SecretClient secretClient = new(
                section.GetValue<Uri>("VaultUri"),
                credential);
            config.AddAzureKeyVault(secretClient, 
                new AzureKeyVaultConfigurationOptions()
                {
                    Manager = new KeyVaultSecretManager(),
                    ReloadInterval = TimeSpan.FromMinutes(1)
                });
        }
    })
    .ConfigureServices((context, services) =>
    {
        var options = context.Configuration.GetSection(nameof(Worker)).Get<WorkerOptions>();
        services
            .AddOptions<WorkerOptions>()
            .Bind<WorkerOptions>(context.Configuration.GetSection(nameof(Worker)));

        services.AddHostedService<Worker>();
    })
    .Build();
            
host.Run();