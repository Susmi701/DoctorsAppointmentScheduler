using DoctorsAppointmentScheduler.Data.Entities;
using DoctorsAppointmentScheduler.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace DoctorsAppointmentScheduler.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SelectRole(string role)
        {
            if (role == "medicalStaff")
            {
                return RedirectToAction("MedicalStaffLogin");
            }
            else if (role == "patient")
            {
                // Redirect to Patient related action or view
                return RedirectToAction("PatientLogin");
            }
            else
            {
                return BadRequest("Invalid role selected.");
            }
        }
        public IActionResult MedicalStaffLogin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> MedicalStaffLogin(LoginViewModel model)
        {
            using (var client = new HttpClient())
            {
                var apiUrl = "https://localhost:7058/api/MedicalStaff/Login";
                var json = JsonConvert.SerializeObject(model);
                var response = await client.PostAsync(apiUrl, new StringContent(json, System.Text.Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var userString = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<MedicalStaff>(userString);


                    if (user.Speciality.Name == "Admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else  
                    {
                        return RedirectToAction("DoctorPage", "Doctor");
                    }
                    
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password");
                    return View(model);
                }
            }
        }
        
        

    }
}