using Autofac.Core;
using DoctorsAppointmentScheduler.Data.Core;
using DoctorsAppointmentScheduler.Data.Services;
using DoctorsScheduler.Services;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DoctorsAppointmentScheduler.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            


            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<HospitalDBContext>(options => options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection")
                ));

            builder.Services.AddScoped<IDoctorServices,DoctorServices>();
            builder.Services.AddScoped<IAdminServices, AdminServices>();
            builder.Services.AddScoped<IMedicalStaffService,MedicalStaffService>();
            builder.Services.AddScoped<IPatientService, PatientServices>();
            builder.Services.AddHostedService<MyBackgroundServices>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}