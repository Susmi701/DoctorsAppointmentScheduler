using DoctorsAppointmentScheduler.Data.Core;
using DoctorsAppointmentScheduler.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DoctorsAppointmentScheduler.DTO;
using DoctorsScheduler.Models;

namespace DoctorsAppointmentScheduler.Data.Services
{
    public class DoctorServices : IDoctorServices
    {
        private readonly HospitalDBContext _hospitalDBContext;
        public DoctorServices(HospitalDBContext hospitalDBContext)
        {
            _hospitalDBContext = hospitalDBContext;
        }
        public async Task<bool> MarkLeave(int doctorId, DateTime leaveDate)
        {
           
            var existingLeave = await _hospitalDBContext.Schedules
           .FirstOrDefaultAsync(s => s.DoctorId == doctorId && s.Leavedate == leaveDate.Date);
            if(existingLeave == null)
            {
                var staffSchedule = new StaffSchedule
                {
                    DoctorId = doctorId,
                    Leavedate = leaveDate.Date
                };
                _hospitalDBContext.Schedules.Add(staffSchedule);
                await _hospitalDBContext.SaveChangesAsync();
            }
            var appointmentsToUpdate = _hospitalDBContext.Appointments
               .Where(a => a.DoctorId == doctorId && a.TimeSlot.Date == leaveDate)
               .ToList();
            foreach (var appointment in appointmentsToUpdate)
            {
                appointment.Status = BookingStatus.Cancelled;
                _hospitalDBContext.Update(appointment);
            }
            await _hospitalDBContext.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<AppointmentHistoryDTO>> GetTotalAppointmentsForDoctor(int doctorId)
        {
            var totalAppointments = await _hospitalDBContext.Appointments
                .Where(a => a.DoctorId == doctorId &&
                                 a.Status == BookingStatus.Booked)
                .OrderByDescending(x => x.Created_Date)
                .Select(a => new AppointmentHistoryDTO
                {
                    PatientName = a.Patient.FirstName + " " + a.Patient.LastName,
                    Created_Date = a.Created_Date,
                    Selected_Date = a.TimeSlot,
                    TimeSlot = a.TimeSlot
                })
                .ToListAsync();
            return totalAppointments;
        }
        public async Task<IEnumerable<AppointmentHistoryDTO>> GetRequest(int id)
        {
            
            var requestDetails = await _hospitalDBContext.Appointments
                .Where(a => a.DoctorId == id && a.TimeSlot.Date > DateTime.Now && a.Status == BookingStatus.Pending)
                 .OrderBy(x => x.Created_Date)
                .Select(a => new AppointmentHistoryDTO
                {
                    AppointmentId = a.Id,
                    PatientName = a.Patient.FirstName + " " + a.Patient.LastName,
                    Selected_Date = a.TimeSlot.Date,
                    TimeSlot = a.TimeSlot

                })
                 .ToListAsync();

            return requestDetails;
        }
        public async Task<bool> UpdateBookingStatus(int appointmentId)
        {
            var appointmentToUpdate = _hospitalDBContext.Appointments
            .FirstOrDefault(a => a.Id == appointmentId);

            if (appointmentToUpdate != null)
            {
                appointmentToUpdate.Status = BookingStatus.Booked;
                await _hospitalDBContext.SaveChangesAsync();
                var appointmentsToCancel = _hospitalDBContext.Appointments
                .Where(a =>a.TimeSlot == appointmentToUpdate.TimeSlot &&
                            a.Id != appointmentToUpdate.Id &&
                            a.Status != BookingStatus.Cancelled);

                foreach (var appointment in appointmentsToCancel)
                {
                    appointment.Status = BookingStatus.Cancelled;
                }

                await _hospitalDBContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<MedicalStaffDTO> GetDoctor(int id)
        {
            var doctor=await _hospitalDBContext.MedicalStaffs
                .Where(x=>x.Id == id)
                .Include(d => d.Speciality)
                .FirstOrDefaultAsync();
            if(doctor == null)
            {
                return null;
            }
            var doctors = new MedicalStaffDTO
            {
                SpecialityName=doctor.Speciality.Name,
                SpecialityId=doctor.Speciality.Id,
                FirstName =doctor.FirstName,
                MiddleName=doctor.MiddleName,
                LastName=doctor.LastName,
                DOB=doctor.DOB,
                Address=doctor.Address,
                Email=doctor.Email,
                PhoneNumber=doctor.PhoneNumber,
            };
            return doctors;
        }
        public async Task<bool> EditDoctor(int id, MedicalStaffDTO medicalStaffDTO  )
        {

            var doctor = await _hospitalDBContext.MedicalStaffs.FindAsync(id);

            if (doctor != null)
            {
                doctor.FirstName = medicalStaffDTO.FirstName;
                doctor.MiddleName = medicalStaffDTO.MiddleName;
                doctor.LastName = medicalStaffDTO.LastName;
                doctor.DOB = medicalStaffDTO.DOB;
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
        public async Task<IEnumerable<CountDTO>> GetPatientDetails(int doctorId)
        {

            var patientDetails = await _hospitalDBContext.Patients
         .Where(p => _hospitalDBContext.Appointments
             .Any(a => a.DoctorId == doctorId && a.Status == BookingStatus.Booked && a.PatientId == p.Id))
         .OrderBy(p => p.FirstName)
         .Select(p => new CountDTO
         {
             Name = p.FirstName + " " + p.LastName,
             PhoneNumber = p.PhoneNumber,
             Count = _hospitalDBContext.Appointments
                 .Count(a => a.DoctorId == doctorId && a.Status == BookingStatus.Booked && a.PatientId == p.Id)
         })
         .ToListAsync();

            return patientDetails;
        }
        public async Task<IEnumerable<DateTime>> GetMarkedLeave(int doctorId)
        {
            var markedLeave = await _hospitalDBContext.Schedules
                    .Where(schedule => schedule.DoctorId == doctorId && schedule.Leavedate.Date>=DateTime.Today.Date)
                    .Select(schedule => schedule.Leavedate.Date)
                    .ToListAsync();

                return markedLeave;
        }
    }
}
