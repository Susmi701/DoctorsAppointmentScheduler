using DoctorsAppointmentScheduler.DTO;
using DoctorsScheduler.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DoctorsScheduler.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class DoctorController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<HomeController> _logger;
        private readonly DoctorUseServices _doctoUserServices;

        public DoctorController(IHttpContextAccessor httpContextAccessor, ILogger<HomeController> logger, DoctorUseServices doctoUserServices)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _doctoUserServices = doctoUserServices;

        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetRequest()
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int id = int.Parse(userId);
                var request = await _doctoUserServices.GetRequestFromPatient(id);

                if (request == null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to retrieve doctors.");
                    return View("GetRequest", Enumerable.Empty<AppointmentHistoryDTO>()); // Pass an empty list to the view
                }

                return View("GetRequest", request);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while retrieving doctors.");
                _logger.LogError(ex, "Error occurred while fetching doctors.");
                return View("GetRequest", Enumerable.Empty<AppointmentHistoryDTO>()); // Pass an empty list to the view
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateBookingStatus(int appointmentId)
        {
            try
            {
                var request = await _doctoUserServices.UpdateBookingStatus(appointmentId);

                return RedirectToAction("GetRequest");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating booking status.");
                _logger.LogError(ex, "Error occurred while updating booking status.");
                return RedirectToAction("GetRequest");
            }
        }
        public IActionResult MarkLeave()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetMarkedLeave()
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int id = int.Parse(userId);
                var appointmentHistory = await _doctoUserServices.GetMarkedLeaveAsync(id);
                if (appointmentHistory == null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to retrieve appointments.");
                    return View("GetMarkedLeave", Enumerable.Empty<DateTime>());
                }
                return View("GetMarkedLeave", appointmentHistory);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while fetching appointments .");
                _logger.LogError(ex, "Error occurred while fetching appointments.");
                return View("GetMarkedLeave", Enumerable.Empty<DateTime>());
            }
        }
        [HttpPost]
        public async Task<IActionResult> MarkLeave(string selectedDate)
        {
            try
            {
                if (DateTime.TryParse(selectedDate, out DateTime date))
                {
                    var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    int id = int.Parse(userId);
                    var markLeaveDtO = new MarkLeaveDTO
                    {
                        Id = id,
                        Date = date,
                    };
                    var request = await _doctoUserServices.MarkLeave(markLeaveDtO);



                    return RedirectToAction("GetMarkedLeave");
                }
                return RedirectToAction("GetMarkedLeave");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while marking leave.");
                _logger.LogError(ex, "Error occurred while marking leave.");
                return View("MarkLeave");
            }
        }
        public IActionResult Reports()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> TotalAppointments()
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int id = int.Parse(userId);
                var appointmentHistory = await _doctoUserServices.GetTotalAppointmentsAsync(id);
                if (appointmentHistory == null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to retrieve appointments.");
                    return View("TotalAppointments", Enumerable.Empty<AppointmentHistoryDTO>());
                }
                return View("TotalAppointments", appointmentHistory);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while fetching appointments .");
                _logger.LogError(ex, "Error occurred while fetching appointments.");
                return View("TotalAppointments", Enumerable.Empty<AppointmentHistoryDTO>());
            }
        }
        [HttpGet]
        public async Task<IActionResult> TotalAppointmentsOverAPeriod()
        {
            try
            {
                var fromDate = Request.Query["fromDate"];
                var toDate = Request.Query["toDate"];
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int id = int.Parse(userId);

                if (DateTime.TryParse(fromDate, out DateTime fromDateValue) &&
                    DateTime.TryParse(toDate, out DateTime toDateValue))
                {
                    var request = new DateDTO
                    {
                        FromDate = fromDateValue,
                        ToDate = toDateValue
                    };
                    var appointmentHistory = await _doctoUserServices.GetTotalAppointmentsOverAPeriodAsync(request,id);

                    if (appointmentHistory == null)
                    {
                        ModelState.AddModelError(string.Empty, "Failed to retrieve appointment history.");
                        return View("TotalAppointments", Enumerable.Empty<AppointmentHistoryDTO>());
                    }

                    return View("TotalAppointments", appointmentHistory);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid date format.");
                    return View("TotalAppointments", Enumerable.Empty<AppointmentHistoryDTO>());
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while retrieving appointment history.");
                _logger.LogError(ex, "Error occurred while fetching appointment history.");
                return View("TotalAppointments", Enumerable.Empty<AppointmentHistoryDTO>());
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int id = int.Parse(userId);
                var request = await _doctoUserServices.GetDoctorsAsync(id);
                if (request == null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to retrieve doctors.");
                    return View("Index");
                }

                return View("EditUser", request);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while retrieving doctors.");
                _logger.LogError(ex, "Error occurred while fetching doctors.");
                return View("Index");
            }
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(MedicalStaffDTO medicalStaffDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(medicalStaffDTO);
                }
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int id = int.Parse(userId);
                var request = await _doctoUserServices.EditDoctorAsync(id, medicalStaffDTO);

                if (!request.Success)
                {
                    TempData["ErrorMessageForEditingDoctor"] = request.Message ?? "Edit Failed";
                    return RedirectToAction("GetUser"); 
                }
                TempData["SuccessMessageForEditingDoctor"] = request.Message ?? "Edit success";
                return RedirectToAction("GetUser");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessageForEditingDoctor"] = "Edit Failed";
                _logger.LogError(ex, "Error occurred while fetching doctors.");
                return RedirectToAction("GetUser");
            }
        }
        [HttpGet]
        public async Task<IActionResult> PatientDetails()
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int id = int.Parse(userId);
                var patientDetails = await _doctoUserServices.PatientDetailsAsync(id);               
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
