using DoctorsAppointmentScheduler.DTO;

namespace DoctorsScheduler.Models
{
    public class GetAppointmentsViewModel
    {
      public  int ID { get; set; }
        public IEnumerable<AppointmentHistoryDTO> appointmentHistoryForDoctors { get; set; }
    }
}
