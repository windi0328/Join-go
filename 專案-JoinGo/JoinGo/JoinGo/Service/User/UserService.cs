using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using JoinGo.Models;
using JoinGo.Models.Author;
using JoinGo.Models.ViewModels;
using JoinGo.Service.Fn;
using System.Data.Entity;  


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



        #region 首頁活動
        //取得資料(首頁活動)
        public ActCardVM GetActCard()
        {
            try
            {
                using (JoinGoEntities db = new JoinGoEntities())
                {
                    var today = DateTime.Today;
                    int? currentUserId = null;

                    if (ChkAuthor.CheckSession())
                    {
                        currentUserId = AuthorModel.Current.ACID;
                    }


                    // 精選 (前 10 筆)
                    var ActList1Full = db.Activity
                                    .Where(a => DbFunctions.TruncateTime(a.ApplyStartDate) <= today &&
                        DbFunctions.TruncateTime(a.ApplyEndDate) >= today)
                                    .OrderByDescending(a => a.ApplyStartDate)
                                    .Take(10)
                                    .Select(a => new ActCardVM
                                    {
                                        ActID = a.ActID,
                                        Name = a.Name,
                                        ViewCount = a.ViewCount,
                                        LikeCount = a.LikeCount,
                                        IsLiked = currentUserId != null && a.ActivityLike.Any(l => l.ACID == currentUserId && l.IsLiked),
                                        PicFile = a.PicFile,
                                        Category1Name = a.Category1.Name,
                                        Category11Name = a.Category11.Name
                                    })
                                    .ToList();


                    // 各分類前 10 筆
                    var ActList2Full = db.Activity
                                         .Where(a => DbFunctions.TruncateTime(a.ApplyStartDate) <= today &&
                        DbFunctions.TruncateTime(a.ApplyEndDate) >= today && a.Category == 1)
                                         .OrderByDescending(a => a.ApplyStartDate)
                                         .Take(10)
                                         .Select(a => new ActCardVM
                                         {
                                             ActID = a.ActID,
                                             Name = a.Name,
                                             ViewCount = a.ViewCount,
                                             LikeCount = a.LikeCount,
                                             IsLiked = currentUserId != null && a.ActivityLike.Any(l => l.ACID == currentUserId && l.IsLiked),
                                             PicFile = a.PicFile,
                                             Category1Name = a.Category1.Name,
                                             Category11Name = a.Category11.Name
                                         })
                                         .ToList();

                    var ActList3Full = db.Activity
                                           .Where(a => DbFunctions.TruncateTime(a.ApplyStartDate) <= today &&
                        DbFunctions.TruncateTime(a.ApplyEndDate) >= today && a.Category == 2)
                                           .OrderByDescending(a => a.ApplyStartDate)
                                           .Take(10)
                                           .Select(a => new ActCardVM
                                           {
                                               ActID = a.ActID,
                                               Name = a.Name,
                                               ViewCount = a.ViewCount,
                                               LikeCount = a.LikeCount,
                                               IsLiked = currentUserId != null && a.ActivityLike.Any(l => l.ACID == currentUserId && l.IsLiked),
                                               PicFile = a.PicFile,
                                               Category1Name = a.Category1.Name,
                                               Category11Name = a.Category11.Name
                                           })
                                           .ToList();
                    var ActList4Full = db.Activity
                                  .Where(a => DbFunctions.TruncateTime(a.ApplyStartDate) <= today &&
                        DbFunctions.TruncateTime(a.ApplyEndDate) >= today && a.Category == 3)
                                  .OrderByDescending(a => a.ApplyStartDate)
                                  .Take(10)
                                  .Select(a => new ActCardVM
                                  {
                                      ActID = a.ActID,
                                      Name = a.Name,
                                      ViewCount = a.ViewCount,
                                      LikeCount = a.LikeCount,
                                      IsLiked = currentUserId != null && a.ActivityLike.Any(l => l.ACID == currentUserId && l.IsLiked),
                                      PicFile = a.PicFile,
                                      Category1Name = a.Category1.Name,
                                      Category11Name = a.Category11.Name
                                  })
                                  .ToList();

                    var ActList5Full = db.Activity
                                   .Where(a => DbFunctions.TruncateTime(a.ApplyStartDate) <= today &&
                        DbFunctions.TruncateTime(a.ApplyEndDate) >= today && a.Category == 4)
                                   .OrderByDescending(a => a.ApplyStartDate)
                                   .Take(10)
                                   .Select(a => new ActCardVM
                                   {
                                       ActID = a.ActID,
                                       Name = a.Name,
                                       ViewCount = a.ViewCount,
                                       LikeCount = a.LikeCount,
                                       IsLiked = currentUserId != null && a.ActivityLike.Any(l => l.ACID == currentUserId && l.IsLiked),
                                       PicFile = a.PicFile,
                                       Category1Name = a.Category1.Name,
                                       Category11Name = a.Category11.Name
                                   })
                                   .ToList();


                    var ActList6Full = db.Activity
                                   .Where(a => DbFunctions.TruncateTime(a.ApplyStartDate) <= today &&
                        DbFunctions.TruncateTime(a.ApplyEndDate) >= today && a.Category == 5)
                                   .OrderByDescending(a => a.ApplyStartDate)
                                   .Take(10)
                                   .Select(a => new ActCardVM
                                   {
                                       ActID = a.ActID,
                                       Name = a.Name,
                                       ViewCount = a.ViewCount,
                                       LikeCount = a.LikeCount,
                                       IsLiked = currentUserId != null && a.ActivityLike.Any(l => l.ACID == currentUserId && l.IsLiked),
                                       PicFile = a.PicFile,
                                       Category1Name = a.Category1.Name,
                                       Category11Name = a.Category11.Name
                                   })
                                   .ToList();
                    // 組裝 ActCardVM
                    var query = new ActCardVM
                    {
                        ActList1 = ActList1Full.Take(9).ToList(),
                        ActList2 = ActList2Full.Take(9).ToList(),
                        ActList3 = ActList3Full.Take(9).ToList(),
                        ActList4 = ActList4Full.Take(9).ToList(),
                        ActList5 = ActList5Full.Take(9).ToList(),
                        ActList6 = ActList6Full.Take(9).ToList(),

                        IsMore1 = ActList1Full.Count > 9,
                        IsMore2 = ActList2Full.Count > 9,
                        IsMore3 = ActList3Full.Count > 9,
                        IsMore4 = ActList4Full.Count > 9,
                        IsMore5 = ActList5Full.Count > 9,
                        IsMore6 = ActList6Full.Count > 9
                    };


                    return query;
                }
            }
            catch (Exception ex)
            {
                logger.Debug("[UserService]錯誤function：GetActCard 取得首頁活動資料,錯誤訊息：" + ex.InnerException + ex.ToString());
                return null;
            }
        }

            //public ActCardVM GetActCard()
            //{
            //    using (JoinGoEntities db = new JoinGoEntities())
            //    {

            //        var ActList1 = db.Activity.Where(o => o.ApplyStartDate <= DateTime.Now && o.ApplyEndDate >= DateTime.Now).OrderByDescending(o=>o.ApplyStartDate).Take(9); //精選
            //        var ActList2 = db.Activity.Where(o => o.ApplyStartDate <= DateTime.Now && o.ApplyEndDate >= DateTime.Now && o.Category==1).OrderByDescending(o => o.ApplyStartDate).Take(9); //學習成長
            //        var ActList3 = db.Activity.Where(o => o.ApplyStartDate <= DateTime.Now && o.ApplyEndDate >= DateTime.Now && o.Category==2).OrderByDescending(o => o.ApplyStartDate).Take(9); //藝文休閒
            //        var ActList4 = db.Activity.Where(o => o.ApplyStartDate <= DateTime.Now && o.ApplyEndDate >= DateTime.Now && o.Category==3).OrderByDescending(o => o.ApplyStartDate).Take(9); //生活體驗
            //        var ActList5 = db.Activity.Where(o => o.ApplyStartDate <= DateTime.Now && o.ApplyEndDate >= DateTime.Now && o.Category==4).OrderByDescending(o => o.ApplyStartDate).Take(9); //健康樂活
            //        var ActList6 = db.Activity.Where(o => o.ApplyStartDate <= DateTime.Now && o.ApplyEndDate >= DateTime.Now && o.Category==5).OrderByDescending(o => o.ApplyStartDate).Take(9); //生活關懷

            //        var query = new ActCardVM
            //        {
            //            ActList1 = ActList1.ToList(),
            //            ActList2 = ActList2.ToList(),
            //            ActList3 = ActList3.ToList(),
            //            ActList4 = ActList4.ToList(),
            //            ActList5 = ActList5.ToList(),
            //            ActList6 = ActList6.ToList(),
            //            //為了判斷是否需要出現更多活動按鈕
            //            IsMore1 = ActList1.Count() > 9,
            //            IsMore2 = ActList2.Count() > 9,
            //            IsMore3 = ActList3.Count() > 9,
            //            IsMore4 = ActList4.Count() > 9,
            //            IsMore5 = ActList5.Count() > 9,
            //            IsMore6 = ActList6.Count() > 9,
            //        };
            //        return query;
            //    }
            //}
            #endregion



        }
}