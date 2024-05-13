using DoctorsAppointmentScheduler.Data.Core;
using DoctorsAppointmentScheduler.Data.Entities;
using DoctorsAppointmentScheduler.DTO;
using DoctorsScheduler.Models;
using DoctorsScheduler.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Core.DTO;

namespace DoctorsAppointmentScheduler.Data.Services
{
    public class PatientServices:IPatientService
    {
        private readonly HospitalDBContext _hospitalDBContext;

        public PatientServices(HospitalDBContext hospitalDBContext)
        {
            _hospitalDBContext = hospitalDBContext;
        }
        public async Task<int?> LoginPatient(AuthenticationDTO authorisationDTO)
        {
            var patient = await GetPatientByUsername(authorisationDTO.Username);
            if (patient != null && patient.Password == Helper.HashPassword(authorisationDTO.Password))
            {
                return patient.Id;
            }
            return null;
        }
        public async Task<bool> RegisterPatient(PatientDTO userDTO)
        {           
                var patient = new Patient
                {
                    UserName = userDTO.Username,
                    Password = Helper.HashPassword(userDTO.Password),
                    FirstName = userDTO.FirstName,
                    MiddleName = userDTO.MiddleName,
                    LastName = userDTO.LastName,
                    DOB = userDTO.DOB,
                    Address = userDTO.Address,
                    PhoneNumber = userDTO.PhoneNumber,
                    Email = userDTO.Email,
                };
                _hospitalDBContext.Patients.Add(patient);
                await _hospitalDBContext.SaveChangesAsync();
                return true;                      
        }
        public async Task<bool> ForgotPassword(ResetPasswordDTO forgot)
        {
                var patient = await GetPatientByUsername(forgot.Username);
                if (patient != null && patient.DOB.Date == forgot.DOB.Date)
                {
                    patient.Password = Helper.HashPassword(forgot.NewPassword);
                    await _hospitalDBContext.SaveChangesAsync();
                    return true;
                }
                return false;
        }
        public async Task<IEnumerable<DoctorSpecialityDTO>> GetAllDoctorsWithSpecialities(int specialtyId)
        {

            var staffWithSpecialities = await _hospitalDBContext.MedicalStaffs
        .Include(staff => staff.Speciality)
        .Where(staff => !staff.IsAdmin && staff.IsActive&& staff.SpecialityId==specialtyId)
        .Select(staff => new DoctorSpecialityDTO
        {
            Id = staff.Id,
            DoctorName ="Dr. "+ staff.FirstName + " " + staff.LastName,
        })
        .ToListAsync();

            return staffWithSpecialities;
        }
        public async Task<IEnumerable<DateTime>> AvailableDates(int selectedDoctorId)
        {
            DateTime startDate = DateTime.Now.AddDays(1);
            DateTime endDate = startDate.AddDays(10);
            List<DateTime> dateRange = new List<DateTime>();
            while (startDate <= endDate)
            {
                dateRange.Add(startDate.Date);
                startDate = startDate.AddDays(1);
            }
            var leaveDate =await _hospitalDBContext.Schedules
                .Where(x => x.DoctorId == selectedDoctorId && x.Leavedate.Date >= DateTime.Now.AddDays(1).Date && x.Leavedate.Date <= endDate.Date)
                .Select(x => x.Leavedate.Date)
                .ToListAsync();
            var availableDates = dateRange.Except(leaveDate).ToList();
            return availableDates;
        }
        public async Task<IEnumerable<DateTime>> AvailableTimeSlot(TimeSlotRequestDTO date)
        {
            var bookedTimeSlots = await _hospitalDBContext.Appointments
                 .Where(x => x.DoctorId == date.DoctorId &&
                             x.TimeSlot.Date == date.Date.Date &&
                             x.Status == BookingStatus.Booked)
                 .Select(x => x.TimeSlot)
                 .ToListAsync();
            var availableTimeSlots = date.TimeSlots.Except(bookedTimeSlots);
            return availableTimeSlots;
        }
        public async Task<Appointment> BookAppointment(AppointmentDTO appointmentDTO)
        {
          
            var appointment = new Appointment
            {
                PatientId = appointmentDTO.PatientId,
                DoctorId = appointmentDTO.DoctorId,
                TimeSlot = appointmentDTO.Timeslot,
                Status = BookingStatus.Pending,
                Created_Date = DateTime.UtcNow
            };
            _hospitalDBContext.Appointments.Add(appointment);
            await _hospitalDBContext.SaveChangesAsync();
            return appointment;
        }
        public async Task<IEnumerable<AppointmentHistoryDTO>> BookedHistory(int userId)
        {
            var appointments = await _hospitalDBContext.Appointments
        .Where(a => a.PatientId == userId && a.Status == BookingStatus.Booked)
        .OrderByDescending(a => a.Created_Date)
        .Select(a => new AppointmentHistoryDTO
        {
            PatientName = a.Doctor.FirstName + " " + a.Doctor.LastName,
            Created_Date = a.Created_Date.Date,
            Selected_Date = a.TimeSlot.Date,
            TimeSlot = a.TimeSlot,
            Status=BookingStatus.Booked
        })
        .ToListAsync();
            return appointments;
        }
        public async Task<IEnumerable<AppointmentHistoryDTO>> AppointmentHistory(int userId)
        {
            var appointments = await _hospitalDBContext.Appointments
                    .Where(a => a.PatientId == userId)
                    .OrderByDescending(a => a.Created_Date)
                    .Select(a => new AppointmentHistoryDTO
                    {
                        PatientName = a.Doctor.FirstName + " " + a.Doctor.LastName,
                        Created_Date = a.Created_Date.Date,
                        Selected_Date = a.TimeSlot.Date,
                        TimeSlot = a.TimeSlot,
                        Status = a.Status
                    })
                    .ToListAsync();
            return appointments;
        }
        public async Task<bool> ResetPassword(AuthenticationDTO authorisationDTO)
        {
            var patient = await GetPatientByUsername(authorisationDTO.Username);

            if (patient != null)
            {
                patient.Password = Helper.HashPassword( authorisationDTO.Password);
                await _hospitalDBContext.SaveChangesAsync();
                return true;
            }

            return false;
        }
        public async  Task<Patient?> GetPatientByUsername(string username)
        {
            return await  _hospitalDBContext.Patients.FirstOrDefaultAsync(x => x.UserName == username);
        }       
        public bool DoesPatientAlreadyExist(string phoneNumber)
        {
            return _hospitalDBContext.Patients.Any(x => x.PhoneNumber == phoneNumber);
        }      
        public bool DoesPatientAlreadyExistId(string phoneNumber, int id)
        {
            var s= _hospitalDBContext.Patients.Any(x => x.PhoneNumber == phoneNumber && x.Id != id);
            return s;
        }
        public async Task<EditUserDTO> GetPatient(int id)
        {
            var patient =await _hospitalDBContext.Patients
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            if (patient == null)
            {
                return null;
            }
            var patients = new EditUserDTO
            {
                Username= patient.UserName,
                FirstName = patient.FirstName,
                MiddleName = patient.MiddleName,
                LastName = patient.LastName,
                DOB = patient.DOB,
                Address = patient.Address,
                Email = patient.Email,
                PhoneNumber = patient.PhoneNumber,
            };
            return patients;
        }
        public async Task<bool> EditPatient(int id, EditUserDTO usersDTO)
        {

            var patient = await _hospitalDBContext.Patients.FindAsync(id);

            if (patient != null)
            {
                patient.FirstName = usersDTO.FirstName;
                patient.MiddleName = usersDTO.MiddleName;
                patient.LastName = usersDTO.LastName;
                patient.DOB = usersDTO.DOB;
                patient.Address = usersDTO.Address;
                patient.PhoneNumber = usersDTO.PhoneNumber;
                patient.Email = usersDTO.Email;
                await _hospitalDBContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<IEnumerable<DoctorSpecialityDTO>> AvailableSpecialities()
        {
            var staffWithSpecialities = await _hospitalDBContext.MedicalStaffs
        .Include(staff => staff.Speciality)
        .Where(staff => !staff.IsAdmin && staff.IsActive)
        .Select(staff => new DoctorSpecialityDTO
        {
            Id = staff.Speciality.Id,

            Speciality = staff.Speciality.Name,
        })
        .Distinct()
        .ToListAsync();

            return staffWithSpecialities;
        }
        public bool DoesExistAppointmentForSameDoctor(AppointmentDTO appointmentDTO)
        {
            var existingAppointmentForSameDoctor = _hospitalDBContext.Appointments
                  .Where(a =>
                      a.DoctorId == appointmentDTO.DoctorId &&
                      a.PatientId == appointmentDTO.PatientId &&
                      a.TimeSlot.Date == appointmentDTO.date.Date
                      )
                  .FirstOrDefault();
            if( existingAppointmentForSameDoctor != null )
            {
                return true;
            }
            return false;
        }
        public bool DoesAppointmentExistForDifferentDoctor(AppointmentDTO appointmentDTO)
        {
            var existingAppointmentForDifferentDoctor = _hospitalDBContext.Appointments
                  .Where(a =>
                      a.PatientId == appointmentDTO.PatientId &&
                      a.TimeSlot.Date == appointmentDTO.date.Date &&
                      a.TimeSlot == appointmentDTO.Timeslot&&
                      a.Status==BookingStatus.Booked
                      )
                  .FirstOrDefault();
            if (existingAppointmentForDifferentDoctor != null)
            {
                return true;
            }
            return false;
        }
    }


}
