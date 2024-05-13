using DoctorsAppointmentScheduler.DTO;
using DoctorsScheduler.Models;
using DoctorsScheduler.Services;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace DoctorsScheduler.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<HomeController> _logger;
        private readonly AdminUserServices _adminUserServices;

        public AdminController(IHttpContextAccessor httpContextAccessor, ILogger<HomeController> logger, AdminUserServices adminUserServices)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _adminUserServices = adminUserServices;

        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ManageDoctor()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> AddDoctor()
        {
            try
            {
                
                var request = await _adminUserServices.GetAllSpecialityAsync();
                var dto = new DoctorRegistrationViewModel
                {
                    Specialities = request
                };

                if (request == null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to retrieve doctors.");
                    return View("BookAppointment", Enumerable.Empty<DoctorSpecialityDTO>());
                }

                return View("AddDoctor", dto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while retrieving doctors.");
                _logger.LogError(ex, "Error occurred while fetching doctors.");
                return View("BookAppointment", Enumerable.Empty<DoctorSpecialityDTO>()); // Pass an empty list to the view
            }
        }
        public IActionResult LoginView()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RegisterDoctor(DoctorRegistrationViewModel doctorRegistrationViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    doctorRegistrationViewModel.Specialities = await _adminUserServices.GetAllSpecialityAsync(); 
                    return View("AddDoctor", doctorRegistrationViewModel);
                }

                var loginModel = await _adminUserServices.RegisterDoctorAsync(doctorRegistrationViewModel.MedicalStaff);
                if (loginModel.Success)
                {
                    TempData["SuccessMessageForRegisterDoctor"] = "Registration successfull";
                    return View("LoginView", loginModel.Data);
                }
                TempData["ErrorMessageForRegisterDoctor"] = "User already exist failed,failed";
                ModelState.AddModelError(string.Empty, loginModel.Message ?? "Please try again");
                doctorRegistrationViewModel.Specialities = await _adminUserServices.GetAllSpecialityAsync();
                return View("AddDoctor", doctorRegistrationViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during patient registration.");
                ModelState.AddModelError(string.Empty, "Registration failed due to an unexpected error.");
                return View("AddDoctor");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllDoctors()
        {
            try
            {
                var doctors = await _adminUserServices.GetAllDoctorsAsync();

                if (doctors == null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to retrieve doctors.");
                    return View("Index");
                }
                return View("GetAllDoctors", doctors);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while retrieving doctors.");
                _logger.LogError(ex, "Error occurred while fetching doctors.");
                return View("Index");
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteDoctor(int appointmentId)
        {
            try
            {

                var deleted = await _adminUserServices.DeleteDoctorAsync(appointmentId);

                if (!deleted)
                {
                    TempData["ErrorMessageDeleteDoctor"] = "Delete Failed";
                    ModelState.AddModelError(string.Empty, "Delete Failed");
                    return RedirectToAction("GetAllDoctors");
                }

                return RedirectToAction("GetAllDoctors");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while retrieving doctors.");
                _logger.LogError(ex, "Error occurred while fetching doctors.");
                return RedirectToAction("GetAllDoctors");
            }
        }
        public IActionResult Report()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> TotalAppointmentsOverAPeriod()
        {
            try
            {
                var fromDate = Request.Query["fromDate"];
                var toDate = Request.Query["toDate"];

                if (DateTime.TryParse(fromDate, out DateTime fromDateValue) &&
                    DateTime.TryParse(toDate, out DateTime toDateValue))
                {
                    var date = new DateDTO
                    {
                        FromDate = fromDateValue,
                        ToDate = toDateValue
                    };
                    var appointmentHistory = await _adminUserServices.GetTotalAppointmentsOverAPeriodAsync(date);

                    if (appointmentHistory == null)
                    {
                        ModelState.AddModelError(string.Empty, "Failed to retrieve appointment history.");
                        return View("GetTotalAppointments", Enumerable.Empty<AppointmentHistoryDTO>());
                    }

                    return View("GetTotalAppointments", appointmentHistory);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid date format.");
                    return View("GetTotalAppointments", Enumerable.Empty<AppointmentHistoryDTO>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching appointment history.");
                return View("GetTotalAppointments", Enumerable.Empty<AppointmentHistoryDTO>());
            }

        }
        [HttpGet]
        public async Task<IActionResult> GetTotalAppointments()
        {
            try
            {

                var appointmentHistory = await _adminUserServices.GetAllTotalAppointmentsAsync();

                if (appointmentHistory == null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to retrieve doctors.");
                    return View("GetTotalAppointments", Enumerable.Empty<AppointmentHistoryDTO>());
                }

                return View("GetTotalAppointments", appointmentHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching doctors.");
                return View("GetTotalAppointments", Enumerable.Empty<AppointmentHistoryDTO>());
            }

        }
        [HttpGet]
        public async Task<IActionResult> SpecialityWithLeastPatient()
        {
            try
            {

                var specialityWithLeastPatient = await _adminUserServices.SpecialityWithLeastPatientAsync();

                if (specialityWithLeastPatient == null)
                {
                    ModelState.AddModelError(string.Empty, "No data available");
                    return View("SpecialityWithLeastPatient", Enumerable.Empty<CountDTO>());
                }

                return View("SpecialityWithLeastPatient", specialityWithLeastPatient);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while retrieving Speciality With Least Patient.");
                _logger.LogError(ex, "Error occurred while fetching Speciality With Least Patient.");
                return View("SpecialityWithLeastPatient", Enumerable.Empty<CountDTO>());
            }

        }
        [HttpGet]
        public async Task<IActionResult> DoctorsWithLeastPatient()
        {
            try
            {

                var doctorsWithLeastPatient = await _adminUserServices.DoctorsWithLeastPatientAsync();

                if (doctorsWithLeastPatient == null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to retrieve doctors.");
                    return View("SpecialityWithLeastPatient", Enumerable.Empty<CountDTO>());
                }

                return View("SpecialityWithLeastPatient", doctorsWithLeastPatient);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while retrieving doctors.");
                _logger.LogError(ex, "Error occurred while fetching doctors.");
                return View("DoctorsWithLeastPatient", Enumerable.Empty<CountDTO>());
            }

        }
        [HttpGet]
        public async Task<IActionResult> GetAllDoctorsWithSpeciality()
        {
            try
            {
                var doctors = await _adminUserServices.GetAllDoctorsAsync();
                if (doctors == null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to retrieve doctors.");
                    return View("GetAllDoctorsWithSpeciality", Enumerable.Empty<DoctorSpecialityDTO>());
                }
                return View("GetAllDoctorsWithSpeciality", doctors);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while retrieving doctors.");
                _logger.LogError(ex, "Error occurred while fetching doctors.");
                return View("GetAllDoctorsWithSpeciality", Enumerable.Empty<DoctorSpecialityDTO>());
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetDoctorAppointments(int id)
        {
            try
            {
                //int id = int.Parse(selectedDoctorId);
                var appointments = await _adminUserServices.DoctorTotalAppointmentsAsync(id);

                if (appointments == null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to retrieve doctors.");
                    return View("GetDoctorAppointments", Enumerable.Empty<GetAppointmentsViewModel>());
                }
                var appointment = new GetAppointmentsViewModel
                {
                    ID = id,
                    appointmentHistoryForDoctors = appointments
                };
                return View("GetDoctorAppointments", appointment);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while retrieving doctors.");
                _logger.LogError(ex, "Error occurred while fetching doctors.");
                return View("GetDoctorAppointments", Enumerable.Empty<GetAppointmentsViewModel>());
            }

        }
        [HttpGet]
        public async Task<IActionResult> DoctorTotalAppointmentsOverAPeriod(int doctorId)
        {
            try
            {
                var fromDate = Request.Query["fromDate"];
                var toDate = Request.Query["toDate"];
                if (DateTime.TryParse(fromDate, out DateTime fromDateValue) &&
                    DateTime.TryParse(toDate, out DateTime toDateValue))
                {
                    var request = new DateDTO
                    {

                        FromDate = fromDateValue,
                        ToDate = toDateValue
                    };
                    var appointmentHistory = await _adminUserServices.DoctorTotalAppointmentsOverAPeriodAsync(request,doctorId);

                    if (appointmentHistory == null)
                    {
                        ModelState.AddModelError(string.Empty, "Failed to retrieve appointment history.");
                        return View("GetDoctorAppointments", Enumerable.Empty<AppointmentHistoryDTO>());
                    }
                    var appointment = new GetAppointmentsViewModel
                    {
                        ID = doctorId,
                        appointmentHistoryForDoctors = appointmentHistory
                    };
                    return View("GetDoctorAppointments", appointment);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid date format.");
                    return View("GetDoctorAppointments", Enumerable.Empty<AppointmentHistoryDTO>());
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while retrieving appointment history.");
                _logger.LogError(ex, "Error occurred while fetching appointment history.");
                return View("GetDoctorAppointments", Enumerable.Empty<AppointmentHistoryDTO>());
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetDoctor(int id)
        {
            try
            {

                var request = await _adminUserServices.GetDoctorsForAdminAsync(id);
                if (request == null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to retrieve doctors.");
                    return View("Index");
                }
                var specialities = await _adminUserServices.GetAllSpecialityAsync();
                ViewData["CategoryList"] = new SelectList(specialities, "Id", "Speciality");
                ViewBag.id = id;
                return View("EditDoctor", request);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while retrieving doctors.");
                _logger.LogError(ex, "Error occurred while fetching doctors.");
                return View("Index");
            }
        }
        [HttpPost]
        public async Task<IActionResult> EditDoctor(int id, MedicalStaffDTO medicalStaffDTO)
        
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var specialities = await _adminUserServices.GetAllSpecialityAsync();
                    ViewData["CategoryList"] = new SelectList(specialities, "Id", "Speciality");
                    ViewBag.id = id;
                    return View(medicalStaffDTO);
                }
                var request = await _adminUserServices.EditDoctorForAdminAsync(id, medicalStaffDTO);
                if (!request.Success)
                {
                    TempData["ErrorMessageForEditingDoctor"] = request.Message ?? "Edit Failed";
                    return RedirectToAction("GetDoctor", new { id=id });
                }
                TempData["SuccessMessageForEditingDoctor"] = request.Message ?? "Edit success";
                return RedirectToAction("GetDoctor", new { id =id});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while editing doctors.");
                return RedirectToAction("GetDoctor", new { id=id });
            }
        }
        [HttpGet]
        public async Task<IActionResult> PatientDetails()
        {
            try
            {
                var patientDetails = await _adminUserServices.PatientDetailsAsync();
                if (patientDetails == null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to retrieve appointments.");
                    return View("PatientDetails", Enumerable.Empty<CountDTO>());
                }
                return View("PatientDetails", patientDetails);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while fetching appointments .");
                _logger.LogError(ex, "Error occurred while fetching appointments.");
                return View("PatientDetails", Enumerable.Empty<CountDTO>());
            }
        }


    }
}
