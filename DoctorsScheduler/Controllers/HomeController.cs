using DoctorsScheduler.Models;
using DoctorsScheduler.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using DoctorsAppointmentScheduler.DTO;
using Tweetinvi.Core.DTO;

namespace DoctorsScheduler.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MedicalStaffServices _medicalStaffServices ;
        private readonly PatientUserServices _patientUserServices;

        public HomeController(ILogger<HomeController> logger,MedicalStaffServices medicalStaffServices,PatientUserServices patientUserServices)
        {
            _logger = logger;
            _medicalStaffServices = medicalStaffServices;
            _patientUserServices = patientUserServices;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SelectRole(string role)
        {
            if (string.IsNullOrEmpty(role))
            {
                return BadRequest("Please select a role.");
            }

            if (role == "medicalStaff")
            {
                return RedirectToAction("MedicalStaffLogin");
            }
            else if (role == "patient")
            {
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
        public async Task<IActionResult> MedicalStaffLogin(AuthenticationDTO loginModel)
        {
            try
            {
                var medicalStaff = await _medicalStaffServices.LoginAsync(loginModel);
                if (medicalStaff == null)
                {
                    TempData["ErrorMessageLogin"] = "Login failed. Invalid username or password.";
                    return View("MedicalStaffLogin");
                }


                if (medicalStaff.IsFirstLogin)
                    return View("MedicalStaffFirstLogin");


                else if (medicalStaff.IsAdmin)
                {
                    var claims = new List<Claim>
                      {
                          new Claim(ClaimTypes.NameIdentifier,medicalStaff.MedicalStaffId.ToString() ),
                           new Claim(ClaimTypes.Role, "Admin")
                      };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,new ClaimsPrincipal(claimsIdentity));
                    return RedirectToAction("Index", "Admin");
                }                    
                    else
                {
                    var claims = new List<Claim>
                      {
                          new Claim(ClaimTypes.NameIdentifier,medicalStaff.MedicalStaffId.ToString() ),
                          new Claim(ClaimTypes.Role, "Doctor")
                      };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,new ClaimsPrincipal(claimsIdentity));
                    return RedirectToAction("Index", "Doctor");
                }                                 
            }
            
             catch (Exception ex)
             {
                
                _logger.LogError(ex, "An error occurred during MedicalStaffLogin.");
                ModelState.AddModelError(string.Empty, "An error occurred during login.");
                return View("MedicalStaffLogin", loginModel);
            }
        }
        public IActionResult MedicalStaffFirstLogin()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> MedicalStaffFirstLogin(ResetPasswordDTO loginModel)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return View(loginModel);
                }
                if (await _medicalStaffServices.ResetPasswordAsync(loginModel))
                {
                    TempData["SuccessMessageMedicalFirstLoginResetPassword"] = "Reset Success";
                    return View("MedicalStaffLogin", new AuthenticationDTO());
                }

                ModelState.AddModelError(string.Empty, "Invalid Credentials");
                return View("MedicalStaffFirstLogin", loginModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during patient registration.");
                ModelState.AddModelError(string.Empty, "Registration failed due to an unexpected error.");
                return View("MedicalStaffFirstLogin", loginModel);
            }
        }

            public IActionResult PatientLogin()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PatientLogin(AuthenticationDTO loginModel)
        {
            try
            {
                
                var loginResponse = await _patientUserServices.LoginAsync(loginModel);

                if (loginResponse.Success && loginResponse.Data != null)
                {
                    var id = loginResponse.Data;
                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                            new Claim(ClaimTypes.Role, "Patient")
                        };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,new ClaimsPrincipal(claimsIdentity));
                    return RedirectToAction("Index", "Patient");
                }
                else
                {
                    TempData["ErrorMessageLogin"] = "Login Failed";
                    return View("PatientLogin");
                }
            }
           
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during patient login.");
                ModelState.AddModelError(string.Empty, "Login failed due to an unexpected error.");
                return View("PatientLogin");
            }
        }

        public IActionResult RegisterPatient()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RegisterPatient(PatientDTO patient)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(patient);
                }
                var registration = await _patientUserServices.RegisterAsync(patient);

                if (registration.Success)
                {
                    TempData["SuccessMessageForRegisterPatient"] ="Registration Successful";
                    return View("PatientLogin");
                }
                else
                {
                    TempData["ErrorMessageForRegisterPatient"] = "Registration Failed";
                    ModelState.AddModelError(string.Empty, registration.Message ?? "Please try again");
                    return View("RegisterPatient");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during patient registration.");
                ModelState.AddModelError(string.Empty, "Registration failed due to an unexpected error.");
                return View("RegisterPatient");
            }
        }

        public IActionResult MedicalForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> MedicalForgotPassword(ResetPasswordDTO forgotPasswordDTO)
        {
            try
            {
                var request = await _medicalStaffServices.ForgotPasswordAsync(forgotPasswordDTO);
                if (request.Success)
                {
                    TempData["SuccessMessageMedicalForgotPassword"] = "Reset Success";
                    return View("MedicalStaffLogin", new AuthenticationDTO());
                }
                ModelState.AddModelError(string.Empty, request.Message ?? "Please try again");
                return View("MedicalForgotPassword", forgotPasswordDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during patient registration.");
                ModelState.AddModelError(string.Empty, "Registration failed due to an unexpected error.");
                return View("MedicalForgotPassword", forgotPasswordDTO);
            }
        }
        public IActionResult PatientForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PatientForgotPassword(ResetPasswordDTO forgotPasswordDTO)
        {
            try
            {
                var request = await _patientUserServices.ForgotPasswordAsync(forgotPasswordDTO);
                if (request.Success)
                {
                    TempData["SuccessMessageReset"] = "Reset Success";
                    return View("PatientLogin", new AuthenticationDTO());
                }
                ModelState.AddModelError(string.Empty, request.Message ?? "Please try again");
                return View("PatientForgotPassword", forgotPasswordDTO);                
            }            
             catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during patient registration.");
                ModelState.AddModelError(string.Empty, "Registration failed due to an unexpected error.");
                return View("PatientForgotPassword", forgotPasswordDTO);
            }
        }

        public async Task<IActionResult> Logout()
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store";
            Response.Headers["Expires"] = "-1";
            Response.Headers["Pragma"] = "no-cache";
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
    }
}