using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorsAppointmentScheduler.DTO
{
    public class MedicalStaffLoginDTO
    {
        public int MedicalStaffId { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsFirstLogin { get; set; }
    }
}
