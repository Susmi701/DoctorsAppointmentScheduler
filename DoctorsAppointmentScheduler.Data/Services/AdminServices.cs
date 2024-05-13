using DoctorsAppointmentScheduler.Data.Core;
using DoctorsAppointmentScheduler.Data.Entities;
using DoctorsAppointmentScheduler.DTO;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace DoctorsAppointmentScheduler.Data.Services
{
    public class AdminServices : MedicalStaffService, IAdminServices
    {
        private readonly HospitalDBContext _hospitalDBContext;

        public AdminServices(HospitalDBContext hospitalDBContext) : base(hospitalDBContext)
        {
            _hospitalDBContext = hospitalDBContext;
        }
        public async Task<AuthenticationDTO> CreateDoctor(MedicalStaffDTO medicalStaffDTO)
        {
            try
            {
             
                    MedicalStaff IsUnique;
                    var doctor = new MedicalStaff
                    {
                        FirstName = medicalStaffDTO.FirstName,
                        MiddleName = medicalStaffDTO.MiddleName,
                        LastName = medicalStaffDTO.LastName,
                        DOB = medicalStaffDTO.DOB.Date,
                        Address = medicalStaffDTO.Address,
                        PhoneNumber = medicalStaffDTO.PhoneNumber,
                        Email = medicalStaffDTO.Email,
                        SpecialityId = medicalStaffDTO.SpecialityId,
                        IsActive = true,
                        IsFirstLogin = true,
                        IsAdmin = false,
                    };

                    do
                    {
                        string randomUsername = Helper.GenerateRandomUsername();
                        doctor.UserName = randomUsername;
                        IsUnique = await GetByUsername(randomUsername);
                    } while (IsUnique != null);
                    var Password = Helper.GenerateRandomPassword();
                    doctor.Password = Helper.HashPassword(Password);
                    _hospitalDBContext.MedicalStaffs.Add(doctor);
                    await _hospitalDBContext.SaveChangesAsync();
                    return new AuthenticationDTO
                    {
                        Username = doctor.UserName,
                        Password = Password,
                    };

                
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public bool DoesDoctorAlreadyExist(string phoneNumber)
        {
            return _hospitalDBContext.MedicalStaffs.Any(x => x.PhoneNumber == phoneNumber);
        }
        public async Task<bool> DeleteDoctor(int id)
        {
            var doctor = await _hospitalDBContext.MedicalStaffs.FindAsync(id);

            if (doctor != null)
            {
                if (doctor.IsAdmin)
                {
                    return false;
                }
                doctor.IsActive = false;
                var appointmentsToUpdate = _hospitalDBContext.Appointments
             .Where(a => a.DoctorId == id && a.TimeSlot.Date > DateTime.UtcNow);

                foreach (var appointment in appointmentsToUpdate)
                {
                    
                    appointment.Status = BookingStatus.Cancelled;
                    _hospitalDBContext.Update(appointment);
                }

                await _hospitalDBContext.SaveChangesAsync();
                return true;
            }
            
            return false;
        }
        public async Task<IEnumerable<AppointmentHistoryDTO>> GetAppointmentsWithStatus(DateTime startDate, DateTime endDate)
        {
            var appointments = await _hospitalDBContext.Appointments
                .Where(a => a.TimeSlot.Date >= startDate.Date && a.TimeSlot.Date <= endDate.Date && a.Doctor.IsActive)
                .Select(x => new AppointmentHistoryDTO
                {
                    DoctorName = x.Doctor.FirstName + " " + x.Doctor.LastName,
                    PatientName = x.Patient.FirstName + " " + x.Patient.LastName,
                    Created_Date = x.Created_Date,
                    Selected_Date = x.TimeSlot.Date,
                    TimeSlot = x.TimeSlot,
                    Status = x.Status
                })
                .OrderByDescending(x => x.Created_Date)
                .ToListAsync();
            return appointments;
        }
        public async Task<IEnumerable<AppointmentHistoryDTO>> GetAllAppointments()
        {
            var appointments = await _hospitalDBContext.Appointments
                .Where(appointment => appointment.Doctor.IsActive)
                .OrderByDescending(x => x.Created_Date)
                .Select(x => new AppointmentHistoryDTO
                {
                    DoctorName = x.Doctor.FirstName + " " + x.Doctor.LastName,
                    PatientName = x.Patient.FirstName + " " + x.Patient.LastName,
                    Created_Date = x.Created_Date,
                    Selected_Date = x.TimeSlot.Date,
                    TimeSlot = x.TimeSlot,
                    Status = x.Status
                })
                .ToListAsync();
            return appointments;
        }
        public async Task<IEnumerable<CountDTO>> GetSpecialitiesByPatientCount()
        {
            var specialitiesFromDoctors = await _hospitalDBContext.MedicalStaffs
                        .Where(doctor => doctor.IsActive && !doctor.IsAdmin)
                        .Select(doctor => doctor.Speciality.Name)
                        .Distinct()
                        .ToListAsync();
            var specialitiesWithPatients = await _hospitalDBContext.Appointments
                    .Where(appointment => appointment.Status == BookingStatus.Booked && appointment.Doctor.IsActive)

                    .GroupBy(appointment => appointment.Doctor.Speciality.Name)
                    .Select(group => new CountDTO
                    {
                        Name = group.Key,
                        Count = group.Count()
                    })
                    .OrderBy(spc => spc.Count)
                    .ToListAsync();
            var specialitiesWithoutPatients = specialitiesFromDoctors.Except(specialitiesWithPatients.Select(s => s.Name));
            var specialitiesWithZeroCount = specialitiesWithoutPatients.Select(speciality => new CountDTO
            {
                Name = speciality,
                Count = 0
            });

            var mergedSpecialities = specialitiesWithPatients.Concat(specialitiesWithZeroCount);
            var specialitiesWithLeastPatients = mergedSpecialities.OrderBy(spc => spc.Count);

            return specialitiesWithLeastPatients;
        }
        public async Task<IEnumerable<CountDTO>> GetDoctorsByPatientCount()
        {
            
            var doctorsWithPatientCount = await _hospitalDBContext.MedicalStaffs
       .Where(doctor => doctor.IsActive && !doctor.IsAdmin)
       .Select(doctor => new CountDTO
       {
           
           Name = "Dr."+doctor.FirstName + " " + doctor.LastName,
           Count = doctor.Appointments.Count(appointment => appointment.Status == BookingStatus.Booked)
       })
       .OrderBy(dto => dto.Count)
       .ToListAsync();


            return doctorsWithPatientCount;
        }
        public async Task<IEnumerable<DoctorSpecialityDTO>> GetAllSpeciality()
        {
            var speciality = await _hospitalDBContext.Specialities
                .Select(specialityList => new DoctorSpecialityDTO
                {
                    Id = specialityList.Id,
                    Speciality = specialityList.Name,
                })
                .ToListAsync();
            return speciality;
        }
        public async Task<IEnumerable<DoctorSpecialityDTO>> GetAllDoctorsWithSpecialities()
        {
            var staffWithSpecialities = await _hospitalDBContext.MedicalStaffs
        .Include(staff => staff.Speciality)
        .Where(staff => !staff.IsAdmin && staff.IsActive)
        .Select(staff => new DoctorSpecialityDTO
        {
            Id = staff.Id,
            DoctorName = staff.FirstName + " " + staff.MiddleName + " " + staff.LastName,
            Speciality = staff.Speciality.Name,

        })
        .ToListAsync();

            return staffWithSpecialities;
        }
        public async Task<bool> EditDoctorForAdmin(int id, MedicalStaffDTO medicalStaffDTO)
        {

            var doctor = await _hospitalDBContext.MedicalStaffs.FindAsync(id);

            if (doctor != null)
            {
                doctor.SpecialityId = medicalStaffDTO.SpecialityId;
                doctor.FirstName = medicalStaffDTO.FirstName;
                doctor.MiddleName = medicalStaffDTO.MiddleName;
                doctor.LastName = medicalStaffDTO.LastName;
                doctor.DOB = medicalStaffDTO.DOB.Date;
                doctor.Address = medicalStaffDTO.Address;
                doctor.PhoneNumber = medicalStaffDTO.PhoneNumber;
                doctor.Email = medicalStaffDTO.Email;
                await _hospitalDBContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false; // Doctor not found
            }
        }
        public async Task<IEnumerable<CountDTO>> GetAllPatients()
        {

            var patients = await _hospitalDBContext.Patients
                .Where(p => _hospitalDBContext.Appointments
            .Any(a =>a.Status == BookingStatus.Booked && a.PatientId == p.Id))
                .Select(patientList => new CountDTO
                {
                    Name = patientList.FirstName + " " + patientList.LastName,
                    PhoneNumber = patientList.PhoneNumber,
                    Count = _hospitalDBContext.Appointments
            .Count(appointment => appointment.PatientId == patientList.Id && appointment.Status == BookingStatus.Booked)
            
                })
                .ToListAsync();
            return patients;
        }
    }

}
