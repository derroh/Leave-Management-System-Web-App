using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HumanResources.CustomValidation
{
    public class IsValidEmailAddress : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is null)
                return false;
            else
                return RegexFunctions.IsValidEmailAddress(value.ToString());

        }
    }
    public class IsEmailAddressAvailable : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is null)
                return false;
            else
                return AppFunctions.IsEmailAvailable(value.ToString());

        }
    }
}