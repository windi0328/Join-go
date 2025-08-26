using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using JoinGo.Models;
using JoinGo.Models.Author;
using JoinGo.Models.ViewModels;
using JoinGo.Service.Fn;

namespace JoinGo.Service.User
{
	public class UserService
	{
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static CommonFunctions CommonFunctions = new CommonFunctions();


        //編輯個人資料(呈現要編輯的資料)
        public AccountVM EditPersonal(int ACID)
        {
            AccountVM result = new AccountVM();
            try
            {
                
                using (JoinGoEntities db = new JoinGoEntities())
                {
                    var data = db.Account.Where(a => a.ACID == ACID).FirstOrDefault();


                    if (data != null)
                    {
                        result.ACID = data.ACID;
                        result.Email = data.Email;
						result.Name = data.Name;
						result.Role = data.Role;
						result.AvatarUrl = data.AvatarUrl;
						result.Phone = data.Phone;
						result.Gender = data.Gender;
						result.Birthday = data.Birthday;
						result.Company = data.Company;
						result.JobTitle = data.JobTitle;
						result.EduLevel = data.EduLevel;
                        result.ContactName = data.ContactName;
                        result.ContactPhone = data.ContactPhone;
                    }
                }
            }
            catch (Exception ex)
            {
				logger.Debug("[UserService]錯誤function：EditPersonal 編輯資料(個人資訊),錯誤訊息：" + ex.InnerException + ex.ToString());
			}
            return result;
        }



        //編輯動作存檔(個人資訊)
        public JsonResultModel EditPersonal(AccountVM data)
        {
            var result = new JsonResultModel();
            try
            {
                using (JoinGoEntities db = new JoinGoEntities())
                {
                    var oldData = db.Account
                                    .Where(t => t.ACID == data.ACID)
                                    .FirstOrDefault();
                    if (oldData != null)
                    {
                        oldData.Name = data.Name;
                        //oldData.AvatarUrl = data.AvatarUrl;
                        oldData.Phone = data.Phone;
                        oldData.Gender = data.Gender;
                        oldData.Birthday = data.Birthday;
                        oldData.Company = data.Company;
                        oldData.JobTitle = data.JobTitle;
                        oldData.EduLevel = data.EduLevel;
                        oldData.ContactName = data.ContactName;
                        oldData.ContactPhone = data.ContactPhone;

                        oldData.Updator = AuthorModel.Current.ACID;
                        oldData.Updated = DateTime.Now;
                        oldData.UpdatedIP = CommonFunctions.GetClientIpAddress();

						string[] allowedPhotoExtensions = {
							".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".svg"
						};
						string[] allowedPhotoMimeTypes = {
							"image/jpeg",
							"image/png",
							"image/gif",
							"image/bmp",
							"image/tiff",
							"image/svg+xml"
						};

						if (data.AvatarPhoto != null && data.AvatarPhoto.ContentLength > 0)
						{
							// 在這裡加上檔案格式(白名單)
							var extension = Path.GetExtension(data.AvatarPhoto.FileName).ToLower();
							if (!allowedPhotoExtensions.Contains(extension))
							{
                                result.Result = false;
                                result.Message = "大頭貼: 不允許的檔案類型";
							}
							if (!allowedPhotoMimeTypes.Contains(data.AvatarPhoto.ContentType))
							{
                                result.Result = false;
                                result.Message = "大頭貼: 不合法的檔案格式";
							}

							var fileName = Path.GetFileNameWithoutExtension(data.AvatarPhoto.FileName) + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(data.AvatarPhoto.FileName); //加時間戳避免重複)
							string uploadPath = System.Configuration.ConfigurationManager.AppSettings["AvatarPhoto"];
							var StorePath = System.Web.HttpContext.Current.Server.MapPath($"{uploadPath}");
							if (!Directory.Exists(StorePath)) //確認路徑,如果沒有就建立資料夾路徑
							{
								Directory.CreateDirectory(StorePath);
							}
							var path = Path.Combine(StorePath, fileName);
							data.AvatarPhoto.SaveAs(path);

							oldData.AvatarUrl = fileName;
                            AuthorModel.Current.AvatarPhoto = fileName;

                        }


						db.SaveChanges();

                        result.Result = true;
                        result.Message = "編輯成功";
                        //CommonFunctions.SymLog(AuthorModel.Current.ACID, AuthorModel.Current.UserID, AuthorModel.Current.LoginName, "學校資訊", "編輯成功:" + data.SchoolName);
                    }
                    else
                    {
                        result.Result = false;
                        result.Message = "查無資料";
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug("[ManageService]錯誤function：EditSchool 編輯動作(學校資訊),錯誤訊息：" + ex.InnerException + ex.ToString());
                result.Result = false;
                result.Message = "系統繁忙中，請稍後再試";
                //CommonFunctions.SymLog(AuthorModel.Current.ACID, AuthorModel.Current.UserID, AuthorModel.Current.LoginName, "學校資訊", "編輯失敗:" + data.SchoolName + "_" + ex.InnerException + ex.ToString());
            }

            return result;
        }



        //編輯帳號(呈現要編輯的資料)
        public AccountVM EditAccount(int ACID)
        {
            AccountVM result = new AccountVM();
            try
            {

                using (JoinGoEntities db = new JoinGoEntities())
                {
                    var data = db.Account.Where(a => a.ACID == ACID).FirstOrDefault();


                    if (data != null)
                    {
                        result.ACID = data.ACID;
                        result.Email = data.Email;
                        result.Origin = data.Origin;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug("[UserService]錯誤function：EditPersonal 編輯資料(個人資訊),錯誤訊息：" + ex.InnerException + ex.ToString());
            }
            return result;
        }
    }
}