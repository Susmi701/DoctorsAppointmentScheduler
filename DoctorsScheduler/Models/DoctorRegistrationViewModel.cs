using DoctorsAppointmentScheduler.DTO;

namespace DoctorsScheduler.Models
{
    public class DoctorRegistrationViewModel
    {
        public MedicalStaffDTO MedicalStaff { get; set; }
        public IEnumerable<DoctorSpecialityDTO>? Specialities { get; set; }
    }
}
