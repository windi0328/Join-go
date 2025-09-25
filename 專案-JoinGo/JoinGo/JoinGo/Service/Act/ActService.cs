using Antlr.Runtime.Misc;
using JoinGo.Models;
using JoinGo.Models.Author;
using JoinGo.Models.ViewModels;
using JoinGo.Service.Fn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NLog;


namespace JoinGo.Service.Act
{
    public class ActService
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region 活動管理

        public List<ActivityVM> GetActivityList(string search, int page = 1)
        {
            using (JoinGoEntities db = new JoinGoEntities())
            {
                search = search?.Trim();

                var edulink = db.Activity.AsQueryable();

                // 搜尋條件
                if (!string.IsNullOrEmpty(search))
                {
                    edulink = edulink.Where(mf => mf.Name.Contains(search));
                }

                // 如果是普通使用者，只能看自己建立的
                if (AuthorModel.Current.Role == "User")
                {
                    edulink = edulink.Where(mf => mf.Creator == AuthorModel.Current.ACID);
                }

                // 排序
                edulink = edulink
                    .OrderByDescending(mf => mf.Created)
                    .ThenBy(mf => mf.StartDate);

                if (AuthorModel.Current.Role == "User")
                {
                    var currentUserId = AuthorModel.Current.ACID;
                    edulink = edulink.Where(mf => mf.Creator == currentUserId);
                }
                
                var query = edulink.Select(mf => new ActivityVM
                {
                    ActID = mf.ActID,
                    Name = mf.Name,
                    StartDate = mf.StartDate,
                    EndDate = mf.EndDate,
                    ApplyStartDate = mf.ApplyStartDate,
                    ApplyEndDate = mf.ApplyEndDate,
                    CategoryName = mf.Category1 != null ? mf.Category1.Name : null
                }).ToList();

                return query;
            }
        }

      

        public List<SelectListItem> GetCategoryList()
        {
            using (JoinGoEntities db = new JoinGoEntities())
            {
                var categoryList = db.Category
                    .Where(c => c.Enable == true && c.ParentCatID == null) // 大分類
                    .OrderBy(c => c.Serial)
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.CaID.ToString()
                    })
                    .ToList();

                return categoryList;
            }
        }

        public List<SelectListItem> GetSubCategoryList(int parentId)
        {
            using (var db = new JoinGoEntities())
            {
                var subCategoryList = db.Category
                    .Where(c => c.Enable == true && c.ParentCatID.HasValue && c.ParentCatID.Value == parentId)
                    .OrderBy(c => c.Serial)
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.CaID.ToString()
                    })
                    .ToList();

                return subCategoryList;
            }
        }




        //查看活動資料
        public ActivityVM DetailsActivity(int ActID)
        {
            ActivityVM result = new ActivityVM();
            try
            {
                using (JoinGoEntities db = new JoinGoEntities())
                {
                    var data = db.Activity.Where(mf => mf.ActID == ActID).FirstOrDefault();


                    if (data != null)
                    {
                        result.ActID = data.ActID;
                        result.Name = data.Name;
                        result.Description = data.Description;
                        result.StartDate = data.StartDate;
                        result.EndDate = data.EndDate;
                        result.ApplyStartDate = data.ApplyStartDate;
                        result.ApplyEndDate = data.ApplyEndDate;
                        result.Category = data.Category;
                        result.SubCategory = data.SubCategory;
                        result.Location = data.Location;
                        result.Pay = data.Pay;
                        result.MaxParticipants = data.MaxParticipants;
                        result.WaitLimit = data.WaitLimit;
                        result.CurrentCount = data.CurrentCount;
                        result.PicFile = data.PicFile;
                        result.CategoryName = data.Category1.Name;
                        result.CreatorName = db.Activity.FirstOrDefault(u => u.ActID == data.Creator)?.Name;
                        result.UpdatorName = db.Activity.FirstOrDefault(u => u.ActID == data.Updator)?.Name;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug("[ActService]錯誤function：DetailsActivity 查看資料,錯誤訊息：" + ex.InnerException + ex.ToString());
            }
            return result;
        }









        ////編輯資料(活動管理)
        // 取得單一活動資料 (for 編輯頁面載入)
        public ActivityVM GetActivityById(int actId)
        {
            using (var db = new JoinGoEntities())
            {
                var activity = db.Activity
                    .Where(mf => mf.ActID == actId)
                    .Select(mf => new ActivityVM
                    {
                        ActID = mf.ActID,
                        Name = mf.Name,
                        Description = mf.Description,
                        StartDate = mf.StartDate,
                        EndDate = mf.EndDate,
                        ApplyStartDate = mf.ApplyStartDate,
                        ApplyEndDate = mf.ApplyEndDate,
                        Category = mf.Category, // 假設有 Category Id
                        SubCategory = mf.SubCategory, // 假設有 SubCategory Id
                        Location = mf.Location,
                        Pay = mf.Pay,
                        MaxParticipants = mf.MaxParticipants,
                        WaitLimit = mf.WaitLimit,
                        CurrentCount = mf.CurrentCount,
                        PicFile = mf.PicFile,
                      CategoryName = mf.Category1.Name

                    })
                    .FirstOrDefault();

                return activity;
            }
        }

        // 更新活動 (for 編輯存檔)
        public bool UpdateActivity(ActivityVM model)
        {
            using (var db = new JoinGoEntities())
            {
                var activity = db.Activity.FirstOrDefault(mf => mf.ActID == model.ActID);
                if (activity == null) return false;

                activity.Name = model.Name;
                activity.Description = model.Description;
                activity.StartDate = model.StartDate;
                activity.EndDate = model.EndDate;
                activity.ApplyStartDate = model.ApplyStartDate;
                activity.ApplyEndDate = model.ApplyEndDate;
                activity.Category = model.Category;
                activity.SubCategory = model.SubCategory;
                activity.Location = model.Location;
                activity.Pay = model.Pay;
                activity.MaxParticipants = model.MaxParticipants;
                activity.WaitLimit = model.WaitLimit;
                activity.CurrentCount = model.CurrentCount;
                activity.PicFile = model.PicFile;
                activity.Updator = AuthorModel.Current.ACID;
                activity.Updated = DateTime.Now;

                db.SaveChanges();
                return true;
            }
        }






        //刪除單筆(服務業務)
        public string DeleteActivity(int ActID)
        {
            string result = "";
            using (JoinGoEntities db = new JoinGoEntities())
            {
                var ty = db.Activity.Where(a => a.ActID == ActID).FirstOrDefault();
                try
                {
                    db.Activity.Remove(ty);
                    db.SaveChanges();
                    result = "刪除成功";
                   // CommonFunctions.SymLog(AuthorModel.Current.ACID, AuthorModel.Current.UserID, AuthorModel.Current.LoginName, "教育資源連結", "刪除成功:" + ty.Title);

                }
                catch (Exception ex)
                {
                    logger.Debug("[ManageService]錯誤function：DeleteEduLink 刪除動作_單筆(教育資源連結),錯誤訊息：" + ex.InnerException + ex.ToString());
                    result = "系統繁忙中，請稍後再試";
                    //CommonFunctions.SymLog(AuthorModel.Current.ACID, AuthorModel.Current.UserID, AuthorModel.Current.LoginName, "教育資源連結", "刪除失敗:" + ty.Title + "_" + ex.InnerException + ex.ToString());
                }
            }
            return result;
        }




        #endregion

    }
}