using DoctorsAppointmentScheduler.DTO;
using DoctorsScheduler.Models;
using DoctorsScheduler.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace DoctorsScheduler.Controllers
{
    [Authorize(Roles = "Patient")]
    public class PatientController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<HomeController> _logger;
        private readonly PatientUserServices _patientUserServices;

        public PatientController(IHttpContextAccessor httpContextAccessor, ILogger<HomeController> logger, PatientUserServices patientUserServices)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _patientUserServices = patientUserServices;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult BookAppointment()
        {
            return View();
        }
        public async Task<IActionResult> GetSpeciality()
        {
            var specialties = await _patientUserServices.GetAllSpecialityAsync();
            ViewBag.Specialties = new SelectList(specialties, "Id", "Speciality");
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllDoctors(int specialtyId)
        {
            try
            {
                
                var doctorWithSpecality = await _patientUserServices.GetAllDoctorsAsync(specialtyId);
                
                if (doctorWithSpecality == null)
                {
                    ModelState.AddModelError(string.Empty, "No doctors available");
                    return View("GetAllDoctors", Enumerable.Empty<DoctorSpecialityDTO>());
                }

                return PartialView("_GetAllDoctors", doctorWithSpecality);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while retrieving doctors.");
                _logger.LogError(ex, "Error occurred while fetching doctors.");
                return View("GetAllDoctors", Enumerable.Empty<DoctorSpecialityDTO>());
            }
        }
        public IActionResult GetAllAvailableDates()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAvailableDates(int selectedDoctorId)
        {
            try
            {
                //int id = int.Parse(selectedDoctorId);
                var availableDate = await _patientUserServices.GetAvailableDatesAsync(selectedDoctorId);

                if (availableDate == null)
                {
                    ModelState.AddModelError(string.Empty, "No available dates");
                    return View("GetAllAvailableDates", Enumerable.Empty<DateTime>());
                }
                var date = new AvailableDateDTO
                {
                    DoctorId = selectedDoctorId,
                    Date = availableDate
                };
                return PartialView("GetAllAvailableDates", date);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while retrieving doctors.");
                _logger.LogError(ex, "Error occurred while fetching doctors.");
                return View("GetAllDoctors", Enumerable.Empty<DateTime>());
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAvailableTimeSlot(string doctorId, string selectedDate)
        {
            try
            {
                if (DateTime.TryParse(selectedDate, out DateTime parsedDate) && int.TryParse(doctorId, out int selectedDoctorId))
                {
                    var availableTime = await _patientUserServices.GetAvailableTimeSlotAsync(parsedDate.Date, selectedDoctorId);

                    if (availableTime == null)
                    {
                        ModelState.AddModelError(string.Empty, "No available time slots");
                        return View("GetAllAvailableDates", selectedDoctorId);
                    }
                    var model = new AvailableDateDTO
                    {
                        DoctorId = selectedDoctorId,

                        Date = availableTime
                    };
                    return View("BookingSlot", model);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to retrieve available time slots.");
                    return PartialView("GetAllAvailableDates", Enumerable.Empty<DateTime>());
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while retrieving doctors.");
                _logger.LogError(ex, "Error occurred while fetching doctors.");
                return View("GetAllAvailableDates", Enumerable.Empty<DateTime>());
            }
        }
        [HttpGet]
        public async Task<IActionResult> BookingSlot(string doctorId, string selectedDateTime)
        {
            try
            {
                if (DateTime.TryParse(selectedDateTime, out DateTime parsedDate) &&
                    int.TryParse(doctorId, out int selectedDoctorId))
                {
                    var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    int id = int.Parse(userId);
                    var appointmentDTO = new AppointmentDTO
                    {
                        PatientId = id,
                        DoctorId = selectedDoctorId,
                        date = parsedDate.Date,
                        Timeslot = parsedDate

                    };
                    var booked = await _patientUserServices.BookAppointmentAsync(appointmentDTO);

                    if (!booked.Success)
                    {
                        TempData["ErrorMessageForBooking"] = booked.Message ?? "Booking Failed";
                        return RedirectToAction("GetAllAvailableDates", new { selectedDoctorId = selectedDoctorId });
                    }
                    TempData["SuccessMessageForBooking"] = "Appointment successfully applied.";
                    return RedirectToAction("GetSpeciality");
                }
                    TempData["ErrorMessageForBooking"] = "Cannot complete booking,Please try again";
                    ModelState.AddModelError(string.Empty, "Invalid selected date.");
                    return View("GetAllDoctors");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while booking.");
                _logger.LogError(ex, "Error occurred while booking.");
                return View("GetAllAvailableDates"); // Pass an empty list to the view
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAppointmentHistory()
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int id = int.Parse(userId);
                var appointmentHistory = await _patientUserServices.GetAppointmentHistoryAsync(id);

                if (appointmentHistory == null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to retrieve doctors.");
                    return View("BookAppointment", Enumerable.Empty<AppointmentHistoryDTO>());
                }

                return View("GetAppointmentHistory", appointmentHistory);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while retrieving doctors.");
                _logger.LogError(ex, "Error occurred while fetching doctors.");
                return View("BookAppointment", Enumerable.Empty<AppointmentHistoryDTO>());
            }

        }
        [HttpGet]
        public async Task<IActionResult> BookedHistory()
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int id = int.Parse(userId);
                var bookedHistory = await _patientUserServices.GetBookedHistoryAsync(id);

                if (bookedHistory == null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to retrieve doctors.");
                    return View("BookAppointment", Enumerable.Empty<AppointmentHistoryDTO>());
                }

                return View("BookedHistory", bookedHistory);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while retrieving doctors.");
                _logger.LogError(ex, "Error occurred while fetching doctors.");
                return View("BookedHistory", Enumerable.Empty<AppointmentHistoryDTO>());
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetPatient()
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int id = int.Parse(userId);
                var request = await _patientUserServices.GetPatientAsync(id);
                if (request == null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to retrieve doctors.");
                    return View("Index");
                }

                return View("EditPatient", request);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while retrieving doctors.");
                _logger.LogError(ex, "Error occurred while fetching doctors.");
                return View("Index");
            }
        }
        [HttpPost]
        public async Task<IActionResult> EditPatient(EditUserDTO usersDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(usersDTO);
                }
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int id = int.Parse(userId);
                var request = await _patientUserServices.EditPatientAsync(id, usersDTO);

                if (request.Success)
                {
                    TempData["SuccessMessageForEditng"] = "Profile Edited Successfully";                    
                    return RedirectToAction("GetPatient");
                }
                else
                {
                    TempData["ErrorMessageForRegisterPatient"] = "Failed to edit patient";
                    ModelState.AddModelError(string.Empty, request.Message ?? "Please try again");
                    return View("EditPatient", usersDTO);
                }
               
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while editing patient.");
                _logger.LogError(ex, "Error occurred while editing patient.");
                return View("GetPatient", Enumerable.Empty<EditUserDTO>()); // Pass an empty list to the view
            }
        }

    }
}
