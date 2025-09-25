using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JoinGo.Models.ViewModels
{
    public class ApplyVM
    {
        public int AID { get; set; }
        public int ActID { get; set; }
        public int ACID { get; set; }
        [Required(ErrorMessage = "請輸入姓名")]
        [DisplayName("姓名")]
        public string Name { get; set; }

        [Required(ErrorMessage = "請輸入Email")]
        [EmailAddress(ErrorMessage = "Email格式不正確")]
        [DisplayName("Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "請輸入電話")]
        [DisplayName("聯絡電話")]
        public string Phone { get; set; }
        public Nullable<System.DateTime> RegistrationDate { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> Creator { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public string CreatedIP { get; set; }
        public Nullable<int> Updator { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public string UpdatedIP { get; set; }

        public string UpdatorName { get; set; }///更新者姓名

        public string CreatorName { get; set; } ///建立者姓名

        public string CategoryName { get; set; }//類別名稱


        public Nullable<int> SubCategory { get; set; } // 子類別ID
        public string SubCategoryName { get; set; }    // 子類別名稱（方便顯示）

        // 活動關聯的資訊
        public string ActTitle { get; set; }      // 活動標題
        public DateTime? StartDate { get; set; }  // 活動開始時間
        public DateTime? EndDate { get; set; }    // 活動結束時間
        public DateTime? ApplyStartDate { get; set; }  // 活動開始時間
        public DateTime? ApplyEndDate { get; set; }    // 活動結束時間

        public bool IsCanceled { get; set; } // 新增欄位：是否取消
        public DateTime? CanceledDate { get; set; } // 取消時間
    }


}
