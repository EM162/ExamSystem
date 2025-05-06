using ITI.ExamSystem.Mapping;
using ITI.ExamSystem.Models;
using ITI.ExamSystem.Repository;
using ITI.ExamSystem.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace ITI.ExamSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //✅ Read connection string from environment variable
            var connectionString = Environment.GetEnvironmentVariable("EXAM_DB_CONNECTION");

            Console.WriteLine("CONNECTION STRING:");
            Console.WriteLine(connectionString ?? "🚫 CONNECTION STRING IS NULL");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("❌ Environment variable EXAM_DB_CONNECTION is not set.");
            }

            ///TEMPERORARRY
            //var myconnectionString = Environment.GetEnvironmentVariable("EXAM_DB_CONNECTION") ??
            //           builder.Configuration.GetConnectionString("DefaultConnection");

            //if (myconnectionString == null)
            //    throw new InvalidOperationException("No connection string found for EXAM_DB_CONNECTION or DefaultConnection.");

            var keysPath = Path.Combine(Directory.GetCurrentDirectory(), "DataProtectionKeys");
            Directory.CreateDirectory(keysPath);

            builder.Services.AddDbContext<OnlineExaminationDBContext>(options =>
                options.UseSqlServer(connectionString));

            //Idenity Services
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            }).AddEntityFrameworkStores<OnlineExaminationDBContext>()
            .AddDefaultTokenProviders();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = null;
                //Email Service required
                options.SignIn.RequireConfirmedEmail = true;
            });
            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<SpAdmin_Repo>();

            builder.Services.AddScoped<IStudentRepositary, StudentRepositary>();

            builder.Services.AddAutoMapper(typeof(StuduentProfileAutoMapper));
            builder.Services.AddScoped<IInstructorRepository, InstructorRepository>();
            //        builder.Services.AddDbContext<OnlineExaminationDBContext>(options =>
            //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
            .SetApplicationName("ExamSystem");

            builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
                opt.TokenLifespan = TimeSpan.FromHours(2));

            builder.Services.AddTransient<IEmailSender, EmailSender>();



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

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}