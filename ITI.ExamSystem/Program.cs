using ITI.ExamSystem.Mapping;
using ITI.ExamSystem.Models;
using ITI.ExamSystem.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;

namespace ITI.ExamSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<OnlineExaminationDBContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("conn1"))
            );

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IStudentRepositary, StudentRepositary>();

            builder.Services.AddAutoMapper(typeof(StuduentProfileAutoMapper));


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            Console.WriteLine("Using connection: " + builder.Configuration.GetConnectionString("conn1"));

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
