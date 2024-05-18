using Funq;
using UserSyncingApp;
using UserSyncingApp.App_Data;
using UserSyncingApp.ServiceInterface;

[assembly: HostingStartup(typeof(AppHost))]

namespace UserSyncingApp;

public class AppHost : AppHostBase, IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices(services => {
            services.AddDbContext<AppDbContext>();
            services.AddHttpClient<IUserService, UserService>();
            services.AddScoped<IUserService, UserService>();
        });

    public AppHost() : base("UserSyncingApp", typeof(UserService).Assembly) {}

    public override void Configure(Container container)
    {
        // Configure ServiceStack only IOC, Config & Plugins
        SetConfig(new HostConfig {
            UseSameSiteCookies = true,
        });
    }
}
