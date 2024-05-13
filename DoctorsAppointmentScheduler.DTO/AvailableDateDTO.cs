using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorsAppointmentScheduler.DTO
{
    public class AvailableDateDTO
    {
        public int DoctorId { get; set; }
        public IEnumerable<DateTime> Date { get; set; }
    }
}
