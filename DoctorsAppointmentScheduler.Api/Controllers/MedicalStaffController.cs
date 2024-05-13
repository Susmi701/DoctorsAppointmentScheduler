using DoctorsAppointmentScheduler.Data.Entities;

using DoctorsAppointmentScheduler.Data.Services;
using DoctorsAppointmentScheduler.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorsAppointmentScheduler.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalStaffController : ControllerBase
    {
        private readonly ILogger<MedicalStaffController> _logger;
        private readonly IAdminServices _adminServices;
        private readonly IDoctorServices _doctorServices;
        private readonly IMedicalStaffService _commonServices;
        public MedicalStaffController(ILogger<MedicalStaffController> logger, IAdminServices adminServices, IDoctorServices doctorServices, IMedicalStaffService commonServices)
        {
            _logger = logger;
            _adminServices = adminServices;
            _doctorServices = doctorServices;
            _commonServices = commonServices;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(AuthenticationDTO loginModel)
        {
            try
            {
                var medicalstaff = await _commonServices.Login(loginModel);
                if (medicalstaff != null)
                {
                    bool isFirstLogin = medicalstaff.IsFirstLogin;
                    bool isAdmin = medicalstaff.IsAdmin;
                    var medicalStaffLoginDTO = new MedicalStaffLoginDTO
                    {
                        MedicalStaffId = medicalstaff.Id,
                        IsAdmin = isAdmin,
                        IsFirstLogin = isFirstLogin
                    };
                    return Ok(medicalStaffLoginDTO);
                }
                return Unauthorized("Invalid username or password");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred during login. Please try again." });
            }
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO loginModel)
        {
            try
            {
                var patient = await _commonServices.ResetPassword(loginModel);
                if (patient)
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
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ResetPasswordDTO forgot)
        {
            try
            {
                var medicalStaff=await _commonServices.GetByUsername(forgot.Username);
                if (medicalStaff == null)
                {
                    return BadRequest("Username not found.");
                }
                if(!medicalStaff.DOB.Date.Equals(forgot.DOB.Date))
                {
                    return BadRequest("Date of birth does not match.");
                }

                var updatedPassword = await _commonServices.ForgotPassword(forgot);
                if (updatedPassword)
                {

                    return Ok("Password Changed Successfully");
                }

                return Unauthorized("Invalid request");
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during resetting password.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred during login. Please try again." });
            }
        }
        [HttpGet("GetDoctor/{id}")]
        public async Task<IActionResult> GetDoctor(int id)
        {
            try
            {
                var doctors = await _doctorServices.GetDoctor(id);
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
        [HttpGet("GetPatient")]
        public async Task<IActionResult> GetPatient(int userId)
        {
            var patient = await _commonServices.GetPatient(userId);
            return Ok(patient);
        }
        [HttpPost("GetTotalAppointmentsForDoctorOverAPeriod/{id}")]
        public async Task<IActionResult> GetTotalAppointmentsForDoctorOverAPeriod(int id, [FromBody] DateDTO  requestDTO)
        {
            var TotalAppointmet = await _commonServices.GetTotalAppointmentsForDoctorOverAPeriod( id, requestDTO.FromDate, requestDTO.ToDate);


            return Ok(TotalAppointmet);
        }

        #region
        [HttpPost("RegisterDoctor")]
        public async Task<IActionResult> RegisterDoctor(MedicalStaffDTO doctorDTO)
        {
            try
            {
                if (_adminServices.DoesDoctorAlreadyExist(doctorDTO.PhoneNumber))
                {
                    return BadRequest("A doctor with given phone number already exist");
                }
                var doctor = await _adminServices.CreateDoctor(doctorDTO);
                if(doctor== null) 
                { 
                  return BadRequest("Cannot Register Doctor");
                }
                
                return Ok(doctor);                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred during login. Please try again." });
            }
        }       
        [HttpPost("EditDoctorForAdmin/{id}")]
        public async Task<IActionResult> EditDoctorForAdmin(int id, [FromBody] MedicalStaffDTO medicalStaffDTO)
        {
            try
            {
                var doctor = await _doctorServices.GetDoctor(id);
                if (doctor.PhoneNumber==medicalStaffDTO.PhoneNumber|| !_adminServices.DoesDoctorAlreadyExist(medicalStaffDTO.PhoneNumber))
                {
                    var status = await _adminServices.EditDoctorForAdmin(id, medicalStaffDTO);
                    if (!status)
                    {
                        return BadRequest("Cannot Edit Doctor");
                    }
                    return Ok("Edit Successfull");
                    
                }
                return BadRequest("A doctor with given phone number already exist");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while editing the doctor: {ex.Message}");
            }
        }
        [HttpPost("DeleteDoctor")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<MedicalStaff>))]
        public async Task<IActionResult> DeleteDoctor([FromBody] int doctorId)
        {
            try
            {
                var doctor = await _adminServices.DeleteDoctor(doctorId);
                return Ok(doctor);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return StatusCode(500, $"An error occurred while deleting the doctor: {ex.Message}");
            }
        }
        [HttpGet("GetSpecialitiesWithLeastPatients")]
        public async Task<IActionResult> GetSpecialitiesWithLeastPatients()
        {
            var specialityWithCount = await _adminServices.GetSpecialitiesByPatientCount();


            return Ok(specialityWithCount);
        }
        [HttpPost("GetAppointmentsWithStatus")]
        public async Task<IActionResult> GetAppointmentsWithStatus(DateDTO date)
        {
            try
            {
                var totalAppointments = await _adminServices.GetAppointmentsWithStatus(date.FromDate.Date, date.ToDate.Date);
                return Ok(totalAppointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpGet("GetAllAppointments")]
        public async Task<IActionResult> GetAllAppointments()
        {
            try
            {
                var totalAppointments = await _adminServices.GetAllAppointments();
                return Ok(totalAppointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpGet("GetDoctorsByPatientCount")]
        public async Task<IActionResult> GetDoctorsByPatientCount()
        {
            var doctorWithCount = await _adminServices.GetDoctorsByPatientCount();
            return Ok(doctorWithCount);
        }
        [HttpGet("GetSpeciality")]
        public async Task<IActionResult> GetAllSpeciality()
        {
            try
            {
                var specialities = await _adminServices.GetAllSpeciality();

                if (specialities == null)
                {
                    return BadRequest("No appointments found ");
                }

                return Ok(specialities);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error occurred while fetching appointments.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
        [HttpGet("GetAllDoctors")]
        public async Task<IActionResult> GetAllDoctors()
        {
            try
            {
                var doctors = await _adminServices.GetAllDoctorsWithSpecialities();
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
        [HttpGet("GetAllPatient")]
        public async Task<IActionResult> GetPatientDetails( )
        {
            try
            {
                var patients = await _adminServices.GetAllPatients();
                return Ok(patients);
            }
            catch (Exception ex)
            {
                // Log the exception or handle the error as required
                _logger.LogError(ex, "An error occurred while fetching total appointments for doctor.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        #endregion


        #region
        [HttpPost("Leave")]
        public async Task<IActionResult> Leave(MarkLeaveDTO markLeave)
        {
            var leaveStatus = await _doctorServices.MarkLeave(markLeave.Id, markLeave.Date);
            return Ok(leaveStatus);
        }
        [HttpGet("GetMarkedLeave/{id}")]
        public async Task<IActionResult> GetMarkedLeave(int id)
        {
            try
            {
                var totalLeave = await _doctorServices.GetMarkedLeave(id);
                return Ok(totalLeave);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching marked leave.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        [HttpGet("GetTotalAppointments/{id}")]
        public async Task<IActionResult> GetTotalAppointments(int id)
        {
            try
            {
                var totalAppointments = await _doctorServices.GetTotalAppointmentsForDoctor(id);
                return Ok(totalAppointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching total appointments .");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        [HttpGet("GetRequestFromPatient/{id}")]
        public async Task<IActionResult> GetRequestFromPatient(int id)
        {
            try
            {
                var totalAppointments = await _doctorServices.GetRequest(id);

                if (totalAppointments == null)
                {
                    return BadRequest("No appointments found ");
                }

                return Ok(totalAppointments);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error occurred while fetching reuest.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] int appointmentId)
        {
            var status = await _doctorServices.UpdateBookingStatus(appointmentId);
            return Ok(status);
        }       
        [HttpPost("EditDoctor{id}")]
        public async Task<IActionResult> EditDoctor(int id, [FromBody] MedicalStaffDTO medicalStaffDTO)
        {
            try
            {
                var doctor = await _doctorServices.GetDoctor(id);
                if (doctor.PhoneNumber == medicalStaffDTO.PhoneNumber || !_adminServices.DoesDoctorAlreadyExist(medicalStaffDTO.PhoneNumber))
                {
                    var status = await _doctorServices.EditDoctor(id, medicalStaffDTO);
                    if (!status)
                    {
                        return BadRequest("Cannot Edit Doctor");
                    }
                    return Ok("Edit Successfull");

                }
                return BadRequest("A doctor with given phone number already exist");
               
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while editing the doctor: {ex.Message}");
            }
        }
        [HttpGet("GetPatientDetails/{id}")]
        public async Task<IActionResult> GetPatientDetails(int id)
        {
            try
            {
                var patients = await _doctorServices.GetPatientDetails(id);
                return Ok(patients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while patient details.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        #endregion


    }


}
