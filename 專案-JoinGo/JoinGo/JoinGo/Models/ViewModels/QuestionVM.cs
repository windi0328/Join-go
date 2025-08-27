using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JoinGo.Models.ViewModels
{
	public class QuestionVM
	{
        public int QuID { get; set; }
        public int Type { get; set; }
        public string Questioner { get; set; }
        public string Email { get; set; }
        public string Question1 { get; set; }
        public string Response { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<bool> IsTop { get; set; }
        public Nullable<int> Creator { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public string CreatedIP { get; set; }
        public Nullable<int> Updator { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public string UpdatedIP { get; set; }

        public string StatusName { get; set; }　//狀態名稱
        public string CreatorName { get; set; } //建立者名稱
        public string UpdatorName { get; set; } //更新者名稱
        public int IsSendlen { get; set; }//是否寄送Email
    }
}