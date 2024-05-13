namespace DoctorsAppointmentScheduler.DTO
{
    public class AppointmentDTO
    {
        public int DoctorId { get; set; }       
        public int PatientId { get; set; }
        public DateTime date { get; set; }        
        public DateTime Timeslot { get; set; }
                
    }
}
