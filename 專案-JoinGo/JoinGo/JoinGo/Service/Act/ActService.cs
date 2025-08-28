using JoinGo.Models;
using JoinGo.Models.Author;
using JoinGo.Models.ViewModels;
using JoinGo.Service.Fn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;


namespace JoinGo.Service.Act
{
    public class ActService
    {

        #region 活動管理

        public List<ActivityVM> GetActivityList(string search, int page = 1)
        {
            using (JoinGoEntities db = new JoinGoEntities())
            {
                search = search?.Trim();
                var edulink = db.Activity
                               .Where(mf => mf.Name.Contains(search)).OrderByDescending(mf => mf.Created).ThenBy(mf => mf.StartDate).ToList();

                var query = edulink.Select(mf => new ActivityVM
                {
                    ActID = mf.ActID,
                    Name = mf.Name,
                    StartDate = mf.StartDate,
                    EndDate = mf.EndDate,
                    ApplyStartDate = mf.ApplyStartDate,
                    ApplyEndDate = mf.ApplyEndDate,
                    CategoryName = mf.Category1.Name
                }).ToList();

                return query;
            }
        }

        ////新增動作(教育資源連結)
        //public string CreateEduLink(EduLinkVM data)
        //{
        //    string result = "";
        //    try
        //    {
        //        using (SeNtpcEntities db = new SeNtpcEntities())
        //        {
        //            EduLink type = new EduLink();
        //            type.Title = data.Title;
        //            type.Link = data.Link;
        //            type.PicAlt = data.PicAlt;
        //            type.Serial = data.Serial;
        //            type.Enable = data.Enable;

        //            type.Creator = AuthorModel.Current.ACID;
        //            type.Created = DateTime.Now;
        //            type.CreatorIP = CommonFunctions.GetClientIpAddress();



        //            string[] allowedPhotoExtensions = {
        //                    ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".svg"
        //                };
        //            string[] allowedPhotoMimeTypes = {
        //                    "image/jpeg",
        //                    "image/png",
        //                    "image/gif",
        //                    "image/bmp",
        //                    "image/tiff",
        //                    "image/svg+xml"
        //                };

        //            if (data.Pic1 != null) //圖片上傳
        //            {
        //                // 在這裡加上檔案格式(白名單)
        //                var extension = Path.GetExtension(data.Pic1.FileName).ToLower();
        //                if (!allowedPhotoExtensions.Contains(extension))
        //                {
        //                    return "圖片: 不允許的檔案類型";
        //                }
        //                if (!allowedPhotoMimeTypes.Contains(data.Pic1.ContentType))
        //                {
        //                    return "圖片: 不合法的檔案格式";
        //                }

        //                var fileName = Path.GetFileNameWithoutExtension(data.Pic1.FileName) + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(data.Pic1.FileName); //加時間戳避免重複)
        //                string uploadPath = System.Configuration.ConfigurationManager.AppSettings["EduLinkPic"];
        //                var StorePath = System.Web.HttpContext.Current.Server.MapPath($"{uploadPath}");
        //                if (!Directory.Exists(StorePath)) //確認路徑,如果沒有就建立資料夾路徑
        //                {
        //                    Directory.CreateDirectory(StorePath);
        //                }
        //                var path = Path.Combine(StorePath, fileName);
        //                data.Pic1.SaveAs(path);

        //                type.PicName = fileName;
        //                //type.PicSize = data.Pic1.ContentLength.ToString();
        //            }

        //            db.EduLink.Add(type);
        //            db.SaveChanges();
        //        }
        //        result = "新增成功";
        //        CommonFunctions.SymLog(AuthorModel.Current.ACID, AuthorModel.Current.UserID, AuthorModel.Current.LoginName, "教育資源連結", "新增成功:" + data.Title);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Debug("[ManageService]錯誤function：CreateEduLink 新增動作(教育資源連結),錯誤訊息：" + ex.InnerException + ex.ToString());
        //        result = "系統繁忙中，請稍後再試";
        //        CommonFunctions.SymLog(AuthorModel.Current.ACID, AuthorModel.Current.UserID, AuthorModel.Current.LoginName, "教育資源連結", "新增失敗:" + data.Title + "_" + ex.InnerException + ex.ToString());
        //    }

        //    return result;
        //}


        ////查看資料(教育資源連結)
        //public EduLinkVM DetailsEduLink(int ELID)
        //{
        //    EduLinkVM result = new EduLinkVM();
        //    try
        //    {
        //        using (SeNtpcEntities db = new SeNtpcEntities())
        //        {
        //            var data = db.EduLink.Where(a => a.ELID == ELID).FirstOrDefault();


        //            if (data != null)
        //            {
        //                result.ELID = data.ELID;
        //                result.Title = data.Title;
        //                result.Link = data.Link;
        //                result.PicName = data.PicName;
        //                result.PicAlt = data.PicAlt;
        //                result.Enable = data.Enable;
        //                result.Serial = data.Serial;
        //                result.Created = data.Created;
        //                result.Updated = data.Updated;
        //                result.CreatorName = db.Account.FirstOrDefault(u => u.ACID == data.Creator)?.Name;
        //                result.UpdatorName = db.Account.FirstOrDefault(u => u.ACID == data.Updator)?.Name;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Debug("[ManageService]錯誤function：DetailsEduLink 查看資料(教育資源連結),錯誤訊息：" + ex.InnerException + ex.ToString());
        //    }
        //    return result;
        //}









        ////編輯資料(教育資源連結)
        //public EduLinkVM EditEduLink(int ELID)
        //{
        //    EduLinkVM result = new EduLinkVM();
        //    try
        //    {
        //        using (SeNtpcEntities db = new SeNtpcEntities())
        //        {
        //            var data = db.EduLink.Where(a => a.ELID == ELID).FirstOrDefault();


        //            if (data != null)
        //            {
        //                result.ELID = data.ELID;
        //                result.Title = data.Title;
        //                result.Link = data.Link;
        //                result.PicName = data.PicName;
        //                result.PicAlt = data.PicAlt;
        //                result.Enable = data.Enable;
        //                result.Serial = data.Serial;
        //                result.Created = data.Created;
        //                result.Updated = data.Updated;
        //                result.CreatorName = db.Account.FirstOrDefault(u => u.ACID == data.Creator)?.Name;
        //                result.UpdatorName = db.Account.FirstOrDefault(u => u.ACID == data.Updator)?.Name;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Debug("[ManageService]錯誤function：EditEduLink 編輯資料(教育資源連結),錯誤訊息：" + ex.InnerException + ex.ToString());
        //    }
        //    return result;
        //}







        ////編輯動作(教育資源連結)
        //public string EditEduLink(EduLinkVM data)
        //{
        //    string result = "";
        //    try
        //    {
        //        using (SeNtpcEntities db = new SeNtpcEntities())
        //        {
        //            var oldData = db.EduLink
        //                            .Where(t => t.ELID == data.ELID)
        //                            .FirstOrDefault();
        //            if (oldData != null)
        //            {
        //                oldData.Title = data.Title;
        //                oldData.Link = data.Link;
        //                oldData.PicAlt = data.PicAlt;
        //                oldData.Serial = data.Serial;
        //                oldData.Enable = data.Enable;

        //                oldData.Updator = AuthorModel.Current.ACID;
        //                oldData.Updated = DateTime.Now;
        //                oldData.UpdatorIP = CommonFunctions.GetClientIpAddress();



        //                string[] allowedPhotoExtensions = {
        //                    ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".svg"
        //                };
        //                string[] allowedPhotoMimeTypes = {
        //                    "image/jpeg",
        //                    "image/png",
        //                    "image/gif",
        //                    "image/bmp",
        //                    "image/tiff",
        //                    "image/svg+xml"
        //                };


        //                if (data.Pic1 != null && data.Pic1.ContentLength > 0) //相簿封面上傳
        //                {
        //                    // 在這裡加上檔案格式(白名單)
        //                    var extension = Path.GetExtension(data.Pic1.FileName).ToLower();
        //                    if (!allowedPhotoExtensions.Contains(extension))
        //                    {
        //                        return "圖片: 不允許的檔案類型";
        //                    }
        //                    if (!allowedPhotoMimeTypes.Contains(data.Pic1.ContentType))
        //                    {
        //                        return "圖片: 不合法的檔案格式";
        //                    }

        //                    var fileName = Path.GetFileNameWithoutExtension(data.Pic1.FileName) + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(data.Pic1.FileName); //加時間戳避免重複)
        //                    string uploadPath = System.Configuration.ConfigurationManager.AppSettings["EduLink"];
        //                    var StorePath = System.Web.HttpContext.Current.Server.MapPath($"{uploadPath}");
        //                    if (!Directory.Exists(StorePath)) //確認路徑,如果沒有就建立資料夾路徑
        //                    {
        //                        Directory.CreateDirectory(StorePath);
        //                    }
        //                    var path = Path.Combine(StorePath, fileName);
        //                    data.Pic1.SaveAs(path);

        //                    oldData.PicName = fileName;
        //                    //oldData.PicSize = data.Pic1.ContentLength.ToString();
        //                }


        //                db.SaveChanges();

        //                result = "編輯成功";
        //                CommonFunctions.SymLog(AuthorModel.Current.ACID, AuthorModel.Current.UserID, AuthorModel.Current.LoginName, "教育資源連結", "編輯成功:" + data.Title);
        //            }
        //            else
        //            {
        //                result = "查無資料";
        //                CommonFunctions.SymLog(AuthorModel.Current.ACID, AuthorModel.Current.UserID, AuthorModel.Current.LoginName, "教育資源連結", "編輯失敗:" + data.Title + "_查無資料");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Debug("[ManageService]錯誤function：EditEduLink 編輯動作(教育資源連結),錯誤訊息：" + ex.InnerException + ex.ToString());
        //        result = "系統繁忙中，請稍後再試";
        //        CommonFunctions.SymLog(AuthorModel.Current.ACID, AuthorModel.Current.UserID, AuthorModel.Current.LoginName, "教育資源連結", "編輯失敗:" + data.Title + "_" + ex.InnerException + ex.ToString());
        //    }

        //    return result;
        //}







        ////刪除單筆(服務業務)
        //public string DeleteEduLink(int ELID)
        //{
        //    string result = "";
        //    using (SeNtpcEntities db = new SeNtpcEntities())
        //    {
        //        var ty = db.EduLink.Where(a => a.ELID == ELID).FirstOrDefault();
        //        try
        //        {
        //            db.EduLink.Remove(ty);
        //            db.SaveChanges();
        //            result = "刪除成功";
        //            CommonFunctions.SymLog(AuthorModel.Current.ACID, AuthorModel.Current.UserID, AuthorModel.Current.LoginName, "教育資源連結", "刪除成功:" + ty.Title);

        //        }
        //        catch (Exception ex)
        //        {
        //            logger.Debug("[ManageService]錯誤function：DeleteEduLink 刪除動作_單筆(教育資源連結),錯誤訊息：" + ex.InnerException + ex.ToString());
        //            result = "系統繁忙中，請稍後再試";
        //            CommonFunctions.SymLog(AuthorModel.Current.ACID, AuthorModel.Current.UserID, AuthorModel.Current.LoginName, "教育資源連結", "刪除失敗:" + ty.Title + "_" + ex.InnerException + ex.ToString());
        //        }
        //    }
        //    return result;
        //}




        #endregion

    }
}