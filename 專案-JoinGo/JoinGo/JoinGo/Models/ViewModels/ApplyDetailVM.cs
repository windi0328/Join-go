using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JoinGo.Models.ViewModels
{
    public class ApplyDetailVM
    {
        public int AID { get; set; }
        public int ActID { get; set; }
        public string ActivityTitle { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public int Status { get; set; }
    }
}