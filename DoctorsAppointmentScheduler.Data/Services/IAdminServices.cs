using DoctorsAppointmentScheduler.Data.Entities;
using DoctorsAppointmentScheduler.DTO;

namespace DoctorsAppointmentScheduler.Data.Services
{
    public interface IAdminServices
    {
        Task<AuthenticationDTO> CreateDoctor(MedicalStaffDTO medicalStaffDTO);
        Task<bool> DeleteDoctor(int id);
        bool DoesDoctorAlreadyExist(string phoneNumber);
        Task<IEnumerable<AppointmentHistoryDTO>> GetAppointmentsWithStatus(DateTime startDate, DateTime endDate);
         Task<IEnumerable<AppointmentHistoryDTO>> GetAllAppointments();
        Task<IEnumerable<CountDTO>> GetDoctorsByPatientCount();
        Task<IEnumerable<CountDTO>> GetSpecialitiesByPatientCount();
        Task<IEnumerable<DoctorSpecialityDTO>> GetAllSpeciality();
        Task<IEnumerable<DoctorSpecialityDTO>> GetAllDoctorsWithSpecialities();
        Task<IEnumerable<CountDTO>> GetAllPatients();
        Task<bool> EditDoctorForAdmin(int id, MedicalStaffDTO medicalStaffDTO);
    }
}