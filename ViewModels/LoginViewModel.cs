using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HumanResources.CustomValidation;
using System.Linq;
using System.Web;

namespace HumanResources.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter username")]
        //[IsValidEmailAddress(ErrorMessage = "Please enter a valid email")]
        //  [IsEmailAddressAvailable(ErrorMessage = "An account with the email provided already exists")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Please enter a password")]
        //  [IsPasswordStrong(ErrorMessage = "Please provide a strong password")]
        public string Password { get; set; }
    }
}