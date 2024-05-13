using DoctorsAppointmentScheduler.DTO;

namespace DoctorsAppointmentScheduler.Data.Services
{
    public interface IDoctorServices
    {
        Task<IEnumerable<AppointmentHistoryDTO>> GetRequest(int id);
        Task<IEnumerable<AppointmentHistoryDTO>> GetTotalAppointmentsForDoctor(int doctorId);
        Task<bool> MarkLeave(int doctorId, DateTime leaveDate);
        Task<bool> UpdateBookingStatus(int appointmentId);
        Task<MedicalStaffDTO> GetDoctor(int id);
        Task<bool> EditDoctor(int id,MedicalStaffDTO medicalStaffDTO);
        Task<IEnumerable<CountDTO>> GetPatientDetails(int id);
        Task<IEnumerable<DateTime>> GetMarkedLeave(int doctorId);
    }
}