using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorsAppointmentScheduler.Data.Entities
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(30)]
        public string UserName { get; set; }        
        public string Password { get; set; }
        [MaxLength(50)]
        public string FirstName { get; set; }
        [MaxLength(30)]
        public string? MiddleName { get; set; }
        [MaxLength(30)]
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
        [MaxLength(70)]
        public string Address { get; set; }
        [MaxLength(15)]
        public string PhoneNumber { get; set; }
        [MaxLength(30)]
        public string? Email { get; set; }
        public ICollection<Appointment>? Appointments { get; set; }
        

    }
}
