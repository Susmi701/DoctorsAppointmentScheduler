using DoctorsAppointmentScheduler.Data.Core;
using DoctorsAppointmentScheduler.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DoctorsAppointmentScheduler.DTO;

namespace DoctorsAppointmentScheduler.Data.Services
{
    public class MedicalStaffService : IMedicalStaffService
    {

        private readonly HospitalDBContext _hospitalDBContext;
        public MedicalStaffService(HospitalDBContext hospitalDBContext)
        {
            _hospitalDBContext = hospitalDBContext;
        }
        public async Task<MedicalStaff> Login(AuthenticationDTO authorisationDTO)
        {
            var medicalStaff = await _hospitalDBContext.MedicalStaffs
                             .FirstOrDefaultAsync(m => m.UserName == authorisationDTO.Username && m.IsActive);
            if (medicalStaff != null)
            {
                if (medicalStaff.Password == Helper.HashPassword(authorisationDTO.Password))
                    return medicalStaff;
            }
            return null;
        }
        public async Task<bool> ResetPassword(ResetPasswordDTO authorisationDTO)
        {
            var medicalStaff = await GetByUsername(authorisationDTO.Username);

            if (medicalStaff != null)
            {
                medicalStaff.IsFirstLogin = false;
                medicalStaff.Password = Helper.HashPassword(authorisationDTO.NewPassword);
                await _hospitalDBContext.SaveChangesAsync();
                return true;
            }

            return false;
        }
        public async Task<bool> ForgotPassword(ResetPasswordDTO forgot)
        {
            var medicalStaff = await GetByUsername(forgot.Username);

            if (medicalStaff != null && medicalStaff.DOB.Date == forgot.DOB.Date)
            {
                medicalStaff.Password = Helper.HashPassword(forgot.NewPassword);
                await _hospitalDBContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<MedicalStaff> GetByUsername(string username)
        {
            return await _hospitalDBContext.MedicalStaffs.FirstOrDefaultAsync(x => x.UserName == username);
        }
        public async Task<Patient> GetPatient(int userId)
        {
            return await _hospitalDBContext.Patients.FirstOrDefaultAsync(s => s.Id == userId);

        }
        public async Task<IEnumerable<AppointmentHistoryDTO>> GetTotalAppointmentsForDoctorOverAPeriod(int doctorId, DateTime startDate, DateTime endDate)
        {
            var totalAppointments = await _hospitalDBContext.Appointments
                .Where(a => a.DoctorId == doctorId &&
                                 a.TimeSlot.Date >= startDate.Date &&
                                 a.TimeSlot.Date <= endDate.Date &&
                                 a.Status == BookingStatus.Booked)
                .Select(a => new AppointmentHistoryDTO
                {
                    PatientName = a.Patient.FirstName + " " + a.Patient.LastName,
                    Created_Date = a.Created_Date,
                    Selected_Date = a.TimeSlot.Date,
                    TimeSlot = a.TimeSlot
                })
                .ToListAsync();
            return totalAppointments;
        }
        public async Task UpdateExpiredAppointments()
        {
            DateTime currentDate = DateTime.Now;
            
            var expiredAppointments = _hospitalDBContext.Appointments
                .Where(a => a.Status == BookingStatus.Pending && a.TimeSlot.Date <= currentDate.Date)
                .ToList();

            foreach (var appointment in expiredAppointments)
            {
                appointment.Status = BookingStatus.Cancelled;
            }

            await _hospitalDBContext.SaveChangesAsync();
        }
    }
}
