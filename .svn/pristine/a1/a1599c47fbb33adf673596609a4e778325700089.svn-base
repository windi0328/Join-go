using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JoinGo.Models.Author
{
	public class AuthorModel
	{
   
            public const string SessionName = "__UserSession__";

            public static void CreateSession()
            {
                AuthorModel session = new AuthorModel();
                HttpContext.Current.Session[SessionName] = session;
            }

            public static AuthorModel Current
            {
                get
                {
                    AuthorModel session = System.Web.HttpContext.Current.Session[SessionName] as AuthorModel;
                    if (session == null)
                    {
                        throw new AppSessionExpireException("Session Expired.");
                    }
                    return session;
                }
            }

            public static bool IsSessionExpired()
            {
                try
                {
                    if (System.Web.HttpContext.Current.Session[SessionName] == null)
                    {
                        return true;
                    }
                    else
                    {
                        // ACID >= 1 才算session存在
                        if (AuthorModel.Current.ACID >= 1)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                catch (Exception e)
                {
                    return true;
                    // ignored ex
                }
            }

            public static void ClearAppSession()
            {
                System.Web.HttpContext.Current.Session[SessionName] = null;
            }

            public string Code { get; set; } //驗證碼
            public int ACID { get; set; }//帳號ID
            public string LoginName { get; set; }//登入姓名
            public string Email { get; set; }//Email
            public string Role { get; set; }//角色
            public string AvatarPhoto { get; set; }//大頭貼
        }
    }