using DoctorsAppointmentScheduler.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorsAppointmentScheduler.Data.Entities
{

    public class Appointment
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Doctor")]
        public int DoctorId { get; set; }
        public MedicalStaff Doctor { get; set; }
        [ForeignKey("Patient")]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
        public DateTime Created_Date { get; set; }        
        public DateTime TimeSlot { get;set; }        
        public BookingStatus Status { get; set; }
    }
}
