using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorsAppointmentScheduler.DTO
{
    public class CountDTO
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public int Count { get; set; }
    }
}
