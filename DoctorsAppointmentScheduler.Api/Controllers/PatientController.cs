using DoctorsAppointmentScheduler.Data;
using DoctorsAppointmentScheduler.Data.Entities;
using DoctorsAppointmentScheduler.Data.Services;
using DoctorsAppointmentScheduler.DTO;
using DoctorsScheduler.Models;
using DoctorsScheduler.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorsAppointmentScheduler.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly ILogger<PatientController> _logger;
        private readonly IPatientService _patientsServices;

        public PatientController(ILogger<PatientController> logger, IPatientService patientsServices)
        {
            _logger = logger;
            _patientsServices = patientsServices;
            
        }
        [HttpPost("RegisterPatient")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]        
        public async Task<IActionResult> RegisterPatient(PatientDTO patientDTO)
        {
            try
            {
                if (_patientsServices.GetPatientByUsername(patientDTO.Username).Result != null)
                {
                    return BadRequest("A patient with the given username already exists.");
                }
                if (_patientsServices.DoesPatientAlreadyExist(patientDTO.PhoneNumber))
                {
                    return BadRequest("A patient with the given phone number already exists");
                }

                var patient = await _patientsServices.RegisterPatient(patientDTO);

                return Ok("Registered Successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering a patient.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the request.");
            }
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(AuthenticationDTO loginModel)
        {
            try
            {
                var patient = await _patientsServices.LoginPatient(loginModel);
                if (patient != null)
                {
                    return Ok(patient);
                }
                return Unauthorized(new { message = "Invalid username or password" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred during login. Please try again." });
            }
        }
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ResetPasswordDTO forgot)
        {
            try
            {
                var patient = await _patientsServices.GetPatientByUsername(forgot.Username);
                if (patient == null)
                {
                    return BadRequest("Username not found.");
                }
                if (!patient.DOB.Date.Equals(forgot.DOB.Date))
                {
                    return BadRequest("Date of birth does not match.");
                }
                var passwordUpdated = await _patientsServices.ForgotPassword(forgot);
                if (passwordUpdated)
                {

                    return Ok("Password Changed Successfully");
                }

                return Unauthorized("Invalid credentials");
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during resetting password.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred during login. Please try again." });
            }
        }
        [HttpGet("GetAllDoctors/{specialtyId}")]
        public async Task<IActionResult> GetAllDoctors(int specialtyId)
        {
            try
            {
                var doctors = await _patientsServices.GetAllDoctorsWithSpecialities(specialtyId);
                if (doctors != null)
                {
                    return Ok(doctors);
                }
                return NotFound("Doctors not found");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching all doctors: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching all doctors. Please try again later.");
            }
        }
        [HttpGet("GetDateAvailability/{id}")]
        public async Task<IActionResult> GetDateAvailability(int id)
        {
            try
            {
                var availableDates = await _patientsServices.AvailableDates(id);
                    return Ok(availableDates);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching dates: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching dates. Please try again later.");
            }
        }
        [HttpPost("GetTimeSlotAvailability")]
        public async Task<IActionResult> GetTimeSlotAvailability(TimeSlotRequestDTO date)
        {
            try
            {
                var availableTimeSlots = await _patientsServices.AvailableTimeSlot(date);

                return Ok(availableTimeSlots);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching dates: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching dates. Please try again later.");
            }
        }       
        [HttpPost("BookAppointment")]
        public async Task<IActionResult> BookAppointment(AppointmentDTO appointmentDTO)
        {
            try
            {
                if (_patientsServices.DoesExistAppointmentForSameDoctor(appointmentDTO))
                {
                    return BadRequest("Duplicate appointment request. Please select a different date. ");
                }
                if (_patientsServices.DoesAppointmentExistForDifferentDoctor(appointmentDTO))
                {
                    return BadRequest("Prior appointment booked. Please select another time slot. ");
                }
                var appointment = await _patientsServices.BookAppointment(appointmentDTO);
                if (appointment == null)
                {
                    return BadRequest("Appointment already exists or cannot be applied for.");
                }
                return Ok("Appointment successfully applied.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while booking an appointment.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the request.");
            }
        }
        [HttpGet("GetBookedHistory/{id}")]
        public async Task<IActionResult> GetBookedHistory(int id)
        {
            try
            {
                var appointments = await _patientsServices.BookedHistory(id);
                if(appointments != null)
                return Ok(appointments);
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while booking an appointment.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the request.");
            }
        }
        [HttpGet("GetAppointmentHistory/{id}")]
        public async Task<IActionResult> GetAppointmentHistory(int id)
        {
            try
            {
                var appointments = await _patientsServices.AppointmentHistory(id);
                if (appointments != null)
                    return Ok(appointments);
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while booking an appointment.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the request.");
            }
        }
        [HttpGet("GetPatient/{id}")]
        public async Task<IActionResult> GetPatient(int id)
        {
            try
            {
                var doctors = await _patientsServices.GetPatient(id);
                if (doctors != null)
                {
                    return Ok(doctors);
                }
                return NotFound("Patient not found");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching  Patient: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching Patient. Please try again later.");
            }
        }
        [HttpPost("EditPatient/{id}")]
        public async Task<IActionResult> EditPatient(int id, [FromBody] EditUserDTO userDTO)
        {
            try
            {
                if (_patientsServices.DoesPatientAlreadyExistId(userDTO.PhoneNumber,id))
                {
                    return BadRequest("A patient with the given phone number already exists");
                }
                var status = await _patientsServices.EditPatient(id, userDTO);
                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while editing the patient.");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
        [HttpGet("GetSpeciality")]
        public async Task<IActionResult> GetAllSpeciality()
        {
            try
            {
                var specialities = await _patientsServices.AvailableSpecialities();

                if (specialities == null)
                {
                    return BadRequest("No specialities found ");
                }

                return Ok(specialities);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error occurred while fetching appointments.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(AuthenticationDTO loginModel)
        {
            var patient = await _patientsServices.ResetPassword(loginModel);
            if (patient != null)
            {

                return Ok(patient);
            }

            return Unauthorized("Invalid username ");
        }
    }
}
