using DoctorsAppointmentScheduler.Data.Entities;
using DoctorsAppointmentScheduler.DTO;

namespace DoctorsAppointmentScheduler.Data.Services
{
    public interface IMedicalStaffService
    {
        Task<bool> ForgotPassword(ResetPasswordDTO forgot);
        Task<MedicalStaff> GetByUsername(string username);
        Task<Patient> GetPatient(int userId);
        Task<IEnumerable<AppointmentHistoryDTO>> GetTotalAppointmentsForDoctorOverAPeriod(int doctorId, DateTime startDate, DateTime endDate);
        Task<MedicalStaff> Login(AuthenticationDTO authorisationDTO);
        Task<bool> ResetPassword(ResetPasswordDTO authorisationDTO);
        Task UpdateExpiredAppointments();
    }
}