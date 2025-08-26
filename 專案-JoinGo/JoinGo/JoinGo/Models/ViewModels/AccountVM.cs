using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JoinGo.Models.ViewModels
{
	public class AccountVM
	{
        public int ACID { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string EmailVerifyCode { get; set; }
        public bool EmailVerified { get; set; }
        public string Name { get; set; }
        public string Origin { get; set; }
        public string ProviderKey { get; set; }
        public string Role { get; set; }
        public string AvatarUrl { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public Nullable<System.DateTime> Birthday { get; set; }
        public string Company { get; set; }
        public string JobTitle { get; set; }
        public string EduLevel { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public Nullable<System.DateTime> LastPwdChange { get; set; }
        public Nullable<bool> PwdExpireNotified { get; set; }
        public Nullable<System.DateTime> LastLoginTime { get; set; }
        public bool Enable { get; set; }
        public Nullable<int> Creator { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public string CreatedIP { get; set; }
        public Nullable<int> Updator { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public string UpdatedIP { get; set; }
        public string PasswordResetToken { get; set; }
        public Nullable<System.DateTime> PasswordResetExpiry { get; set; }


        public HttpPostedFileBase AvatarPhoto { get; set; } //大頭貼照片


        public string Token { get; set; }
        public string NewPassword { get; set; } //新密碼
        public string ConfirmPassword { get; set; } //新密碼確認

        public string NewEmail { get; set; } //新Email


    }
}