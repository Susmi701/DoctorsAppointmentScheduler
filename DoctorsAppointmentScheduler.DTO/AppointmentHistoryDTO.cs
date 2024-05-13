namespace DoctorsAppointmentScheduler.DTO
{
    public class AppointmentHistoryDTO
    {
        public int AppointmentId { get; set; }
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
        public DateTime Created_Date { get; set; }
        public DateTime Selected_Date { get; set; }
        public DateTime TimeSlot { get; set; }
        public BookingStatus Status { get; set; }

    }
}