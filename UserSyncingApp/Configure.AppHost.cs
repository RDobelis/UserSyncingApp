using Funq;
using Microsoft.EntityFrameworkCore;
using UserSyncingApp.Data;
using UserSyncingApp.ServiceInterface;
using UserSyncingApp.ServiceStackServices;

[assembly: HostingStartup(typeof(AppHost))]

namespace UserSyncingApp
{
    public class AppHost : AppHostBase, IHostingStartup
    {
        public AppHost() : base("UserSyncingApp", typeof(UserSyncService).Assembly) { }

        public void Configure(IWebHostBuilder builder) => builder
            .ConfigureServices((context, services) =>
            {
                var configuration = context.Configuration;
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
                services.AddHttpClient<IUserService, UserService>();
                services.AddScoped<IUserService, UserService>();
            });

        public override void Configure(Container container)
        {
            SetConfig(new HostConfig
            {
                UseSameSiteCookies = true,
            });
        }
    }
}