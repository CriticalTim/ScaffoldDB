using ScaffoldDB.Components;
using ScaffoldDB.Services;

namespace ScaffoldDB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            //builder.Services.AddSingleton<DatabaseSchemaService>();
            builder.Services.AddSingleton<RuntimeScaffoldService>();
            //builder.Services.AddScoped<DynamicTableService>();
            builder.Services.AddScoped<DynamicTableServiceFactory>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();


            //string appDataLocal = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            //string tempDirectory = Path.Combine(appDataLocal, "Migrations");
            //if (!Directory.Exists(tempDirectory))
            //{
            //    Directory.CreateDirectory(tempDirectory);
            //}

            //// Register cleanup logic on application shutdown
            //var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
            //lifetime.ApplicationStopping.Register(() =>
            //{
            //    if (Directory.Exists(tempDirectory))
            //    {
            //        foreach (var file in Directory.GetFiles(tempDirectory))
            //        {
            //            File.Delete(file);
            //        }

            //        foreach (var subDir in Directory.GetDirectories(tempDirectory))
            //        {
            //            Directory.Delete(subDir, true);
            //        }
            //    }
            //});

            app.Run();
        }
    }
}
