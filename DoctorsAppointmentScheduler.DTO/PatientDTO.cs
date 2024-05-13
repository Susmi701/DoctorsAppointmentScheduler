using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorsAppointmentScheduler.DTO
{
    public class PatientDTO
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "Username must be between 5 and 30 characters.")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 30 characters.")]
        public string Password { get; set; }

        
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "FirstName can not exceed 50 characters.")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "First name should only contain letters.")]
        public string FirstName { get; set; }

        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Middle name should only contain letters.")]
        [StringLength(50, ErrorMessage = "MiddleName can not exceed 50 characters.")]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Last name should only contain letters.")]
        [StringLength(50, ErrorMessage = "LastName can not exceed 50 characters.")]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [CustomValidation(typeof(PatientDTO), "ValidateDOB")]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(50, ErrorMessage = "Address can not exceed 50 characters.")]
        public string Address { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must have exactly 10 digits.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Phone number must contain only digits.")]
        public string PhoneNumber { get; set; }
        
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        public static ValidationResult ValidateDOB(DateTime dob)
        {
            DateTime minDate = DateTime.Now.AddYears(-130);
            DateTime maxDate = DateTime.Now;

            if (dob < minDate || dob > maxDate)
            {
                return new ValidationResult("Enter a valid Date of Birth");
            }

            return ValidationResult.Success;
        }
    }
    
}
