using Autofac.Core;
using DoctorsScheduler.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
namespace DoctorsScheduler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.Configure<ApiOption>(builder.Configuration.GetSection("Api"));

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<MedicalStaffServices>();
            builder.Services.AddScoped<PatientUserServices>();
            builder.Services.AddScoped<DoctorUseServices>();
            builder.Services.AddScoped<AdminUserServices>();
            builder.Services.AddHttpContextAccessor();
            
           builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
           {
               options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
               options.SlidingExpiration = true;
               options.AccessDeniedPath = "/Home/Index";
               options.LoginPath = "/Home/Index";
           });

            builder.Services.AddMvc(options =>
            {
                options.Filters.Add(new ResponseCacheAttribute
                {
                    NoStore = true,
                    Location = ResponseCacheLocation.None
                });
            });
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
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}