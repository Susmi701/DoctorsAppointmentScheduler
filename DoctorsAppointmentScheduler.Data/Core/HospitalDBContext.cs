using DoctorsAppointmentScheduler.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorsAppointmentScheduler.Data.Core
{
    public class HospitalDBContext : DbContext
    {
        public HospitalDBContext(DbContextOptions<HospitalDBContext> options) : base(options)
        {

        }


        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalStaff> MedicalStaffs { get; set; }
        public DbSet<Patient> Patients { get; set; }

        public DbSet<StaffSchedule> Schedules { get; set; }
        public DbSet<Speciality> Specialities { get; set; }
        //public DbSet<TimeSlot> TimeSlots { get; set; }

    }
}
