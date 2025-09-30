using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JoinGo.Models.ViewModels
{
    public class ApplyListVM
    {
        public int AID { get; set; }
        public int ActID { get; set; }
        public string ActTitle { get; set; }   // 來自 Activity.Name
        public DateTime? RegistrationDate { get; set; }
        public DateTime? StartDate { get; set; }
        public int Status { get; set; }        // 1=已報名, 0=取消


        public int ApplyID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
       
    }
}