using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorsAppointmentScheduler.Data.Entities
{
    public class StaffSchedule
    {
        [Key] 
        public int Id { get; set; }
        [ForeignKey("Doctor")]
        public int DoctorId { get; set; }
        public MedicalStaff Doctor { get; set; }
        public DateTime Leavedate { get; set; }
        
    }
}
