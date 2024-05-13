using DoctorsAppointmentScheduler.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoctorsAppointmentScheduler.DTO;
using DoctorsScheduler.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoctorsAppointmentScheduler.Data.Services
{
    public interface IPatientService
    {
        Task<bool> RegisterPatient(PatientDTO userDTO);
        public Task<IEnumerable<DoctorSpecialityDTO>> GetAllDoctorsWithSpecialities(int specialtyId);
        Task<Appointment> BookAppointment(AppointmentDTO appointmentDTO);
        Task<IEnumerable<DateTime>> AvailableDates(int selectedDoctorId);
        bool DoesPatientAlreadyExist(string phoneNumber);
        Task<int?> LoginPatient(AuthenticationDTO authorisationDTO);
        Task<IEnumerable<DateTime>> AvailableTimeSlot(TimeSlotRequestDTO date);
        Task<IEnumerable<AppointmentHistoryDTO>> AppointmentHistory(int userId);
        Task<bool> ForgotPassword(ResetPasswordDTO forgot);
        Task<bool> ResetPassword(AuthenticationDTO authorisationDTO);
        Task<Patient?> GetPatientByUsername(string username);
        Task<IEnumerable<AppointmentHistoryDTO>> BookedHistory(int userId);
        Task<EditUserDTO> GetPatient(int id);
        Task<bool> EditPatient(int id, EditUserDTO userDTO);
        bool DoesPatientAlreadyExistId(string phoneNumber, int id);
        Task<IEnumerable<DoctorSpecialityDTO>> AvailableSpecialities();
        bool DoesAppointmentExistForDifferentDoctor(AppointmentDTO appointmentDTO);
        bool DoesExistAppointmentForSameDoctor(AppointmentDTO appointmentDTO);
    }
}
