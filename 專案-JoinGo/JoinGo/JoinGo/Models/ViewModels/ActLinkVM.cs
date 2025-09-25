using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JoinGo.Models.ViewModels
{
	public class ActLinkVM
	{
        public bool IsLiked { get; set; } // 用戶是否按過讚
        public string Category1Name { get; set; }
        public string Category11Name { get; set; }
        public int ActID { get; set; }
        public string Name { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<System.DateTime> ApplyStartDate { get; set; }
        public Nullable<System.DateTime> ApplyEndDate { get; set; }
        public Nullable<int> Category { get; set; }
        public string Location { get; set; }
        public Nullable<int> Pay { get; set; }
        public Nullable<int> MaxParticipants { get; set; }
        public Nullable<int> WaitLimit { get; set; }
        public Nullable<int> CurrentCount { get; set; }
        public string PicFile { get; set; }
        public string Files { get; set; }
        public string Contact { get; set; }
        public string ContactPhone { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<bool> AllowCancel { get; set; }
        public Nullable<int> CancelDeadlineDays { get; set; }
        public Nullable<int> ViewCount { get; set; }
        public Nullable<int> LikeCount { get; set; }
    }
}