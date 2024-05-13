namespace DoctorsScheduler.Models
{
    public class TimeSlotRequestDTO
    {
        public int DoctorId { get; set; }
        public DateTime Date { get; set; }
        public IEnumerable<DateTime> TimeSlots { get; set; }
    }
}
