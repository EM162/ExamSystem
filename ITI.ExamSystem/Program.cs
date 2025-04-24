using ITI.ExamSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ITI.ExamSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // ✅ Read connection string from environment variable
            var connectionString = Environment.GetEnvironmentVariable("EXAM_DB_CONNECTION");

            Console.WriteLine("CONNECTION STRING:");
            Console.WriteLine(connectionString ?? "🚫 CONNECTION STRING IS NULL");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("❌ Environment variable EXAM_DB_CONNECTION is not set.");
            }

            builder.Services.AddDbContext<OnlineExaminationDBContext>(options =>
                options.UseSqlServer(connectionString));

            //        builder.Services.AddDbContext<OnlineExaminationDBContext>(options =>
            //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
