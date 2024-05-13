using Microsoft.AspNetCore.Mvc;

namespace DoctorsAppointmentScheduler.Controllers
{
    public class DoctorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
