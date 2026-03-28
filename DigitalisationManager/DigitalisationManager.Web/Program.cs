namespace DigitalisationManager.Web
{
    using DigitalisationManager.Data;
    using DigitalisationManager.Data.Models.Identity;
    using DigitalisationManager.Services.Core;
    using DigitalisationManager.Services.Core.Contracts;
    using DigitalisationManager.Services.Core.Options;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<DigitalisationManagerDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                ConfigureIdentityOptions(options);
            })
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<DigitalisationManagerDbContext>();
           
            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            //My services
            builder.Services.AddScoped<IFundService, FundService>();
            builder.Services.AddScoped<IItemService, ItemService>();
            builder.Services.AddScoped<IFileStorageService, FileStorageService>();
            builder.Services.AddScoped<FileStorageOptions>();
            builder.Services.AddScoped<IDigitalFileService, DigitalFileService>();
          
            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }

        private static void ConfigureIdentityOptions(IdentityOptions options)
        {
            // Sign-in options
            options.SignIn.RequireConfirmedAccount = false;

            // Password options
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 3;
            options.Password.RequiredUniqueChars = 1;

            // Lockout options
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 50;

            // User options
            options.User.RequireUniqueEmail = false;
        }
    }
}
