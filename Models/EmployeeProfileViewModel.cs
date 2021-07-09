using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HumanResources.Models
{
    public class EmployeeProfileViewModel
    {
        public string EmployeeNo { get; set; }       
        public string FullName { get; set; }      
        public string PassportNumber { get; set; }
        public string Gender { get; set; }
        public string DateOfBirth { get; set; }        
        public byte[] Picture { get; set; }
        public string PostalAddress { get; set; }
     
        public string City { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
 
        public string CellularPhoneNumber { get; set; }
        public string WorkPhoneNumber { get; set; }
        public string EMail { get; set; }
        public string CompanyEMail { get; set; }
        public string JobTitle { get; set; }
  
        public string ResignationDate { get; set; }
        public string SuspensionDate { get; set; }
        public string DemisedDate { get; set; }
        public string Retirementdate { get; set; }
        public string RetrenchmentDate { get; set; }
        public string DateOfLeaving { get; set; }
        public string EndOfContractDate { get; set; }
        public string Date_Of_Joining_the_Company { get; set; }
        public string Date_Of_Leaving_the_Company { get; set; }
     
        public string HROrgUnit { get; set; }
        public string HRPosition { get; set; }
        public int CurrentYear { get; set; }
        public string Status { get; set; }
    }
}