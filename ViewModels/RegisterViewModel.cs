using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HumanResources.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Please enter first name")]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "Please enter last name")]
        public string LastName { get; set; }


        [Required(ErrorMessage = "Please enter email")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Please enter a password")]
        //  [IsPasswordStrong(ErrorMessage = "Please provide a strong password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password")]
        [Compare("Password", ErrorMessage = "Password and confirm password does not match")]
        public string ConfirmPassword { get; set; }


        [Required(ErrorMessage = "Please enter your employee Id")]
        public string EmployeeNo { get; set; }
    }
}