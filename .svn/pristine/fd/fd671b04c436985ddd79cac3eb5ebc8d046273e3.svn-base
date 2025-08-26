using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JoinGo.Service.Fn
{
	public class CommonFunctions
	{
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        //獲取用戶端IP位置
        public string GetClientIpAddress()
        {
            var ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = HttpContext.Current.Request.UserHostAddress;
            }
            if (ipAddress == "::1")
            {
                ipAddress = "127.0.0.1";
            }
            if (!string.IsNullOrEmpty(ipAddress))
            {
                var ipAddresses = ipAddress.Split(',');
                return ipAddresses[0].Trim();
            }
            return string.Empty;
        }


        //檢查密碼是否符合強度規則(密碼至少8碼且包含:大寫英文、數字、特殊符號，至少其中三種)
        public bool ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return false;
            if (password.Length < 8) return false;

            int typesCount = 0;
            if (password.Any(char.IsUpper)) typesCount++;
            if (password.Any(char.IsLower)) typesCount++;
            if (password.Any(char.IsDigit)) typesCount++;
            if (password.Any(c => !char.IsLetterOrDigit(c))) typesCount++;

            return typesCount >= 3;
        }



        // 雜湊密碼
        public string HashPassword(string plainPassword)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainPassword);
        }

        // 驗證密碼
        public bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
        }



    }
}