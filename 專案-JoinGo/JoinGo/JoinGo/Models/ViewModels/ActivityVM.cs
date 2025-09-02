using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JoinGo.Models.ViewModels
{
    public class ActivityVM
    {
        public int ActID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
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
        public Nullable<int> Creator { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public string CreatedIP { get; set; }
        public Nullable<int> Updator { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public string UpdatedIP { get; set; }

        public virtual Category Category1 { get; set; }

        public string UpdatorName { get; set; }///更新者姓名

        public string CreatorName { get; set; }
        ///建立者姓名


        public string CategoryName { get; set; }//類別名稱
    }
}