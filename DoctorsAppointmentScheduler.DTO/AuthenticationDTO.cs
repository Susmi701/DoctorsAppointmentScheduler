using System.ComponentModel.DataAnnotations;

namespace DoctorsAppointmentScheduler.DTO
{
    public class AuthenticationDTO
    {

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}
