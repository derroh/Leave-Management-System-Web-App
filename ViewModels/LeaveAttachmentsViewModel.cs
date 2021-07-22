using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HumanResources.ViewModels
{
    public class LeaveAttachmentsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FileType { get; set; }
        public string DateUploaded { get; set; }
    }
}