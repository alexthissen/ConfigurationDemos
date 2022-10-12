namespace HostStartup
{
    public class Startup
    {
        // Configuring dependency injection
        public void ConfigureServices(IServiceCollection services, IHostEnvironment env) 
        {
            if (env.IsProduction()) { }
        }
        public void ConfigureProductionServices(IServiceCollection services) { }

        // Configuring application and middleware
        public void Configure(IApplicationBuilder app, IHostEnvironment env) 
        {
            if (env.IsProduction()) { }
        }
        public void ConfigureProduction(IApplicationBuilder app, IHostEnvironment env) 
        { 
        }
    }
}
