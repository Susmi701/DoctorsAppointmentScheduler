using Microsoft.AspNetCore.Mvc;

namespace DoctorsAppointmentScheduler.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
