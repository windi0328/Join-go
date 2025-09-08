using Antlr.Runtime.Misc;
using JoinGo.Models;
using JoinGo.Models.Author;
using JoinGo.Models.ViewModels;
using JoinGo.Service.Act;
using JoinGo.Service.User;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace JoinGo.Controllers
{

    public class ActivityController : Controller

    {

        private static ActService ActService = new ActService();

        //取得列表資料(活動管理)
        public ActionResult ActivityList(string search = "", int page = 1)
        {
            if (!ChkAuthor.CheckSession() && (AuthorModel.Current.Role == "User" || AuthorModel.Current.Role == "Admin")) { return RedirectToAction("LogOut", "Home"); }

            ViewBag.page = page;
            return View();
        }





        //取得列表資料(活動管理)
        public ActionResult _ParActivityList(string search = "", int page = 1)
        {
            if (!ChkAuthor.CheckSession() && (AuthorModel.Current.Role == "User" || AuthorModel.Current.Role == "Admin")) { return RedirectToAction("LogOut", "Home"); }

            var result = ActService.GetActivityList(search);
            ViewBag.TotalCount = result.Count();
            ViewBag.page = page;
            int pageSize = 10;
            return PartialView(result.ToPagedList(page, pageSize));
        }


        //新增(活動管理)
        public ActionResult CreateActivity()
        {
            if (!ChkAuthor.CheckSession()) { return RedirectToAction("LogOut", "Home"); }
            using (JoinGoEntities db = new JoinGoEntities())
            {

                ViewBag.ActivityCategorySelect = new SelectList(ActService.GetCategoryList(), "Value", "Text");
            }
            return View();

        }

        public JsonResult GetSubCategories(int ParentCatID)
        {
            try
            {
                var subCategories = ActService.GetSubCategoryList(ParentCatID);
                return Json(new { items = subCategories }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // 方便除錯，回傳錯誤訊息
                return Json(new { items = new List<object>(), error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateActivity(ActivityVM data, HttpPostedFileBase PicFile)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "資料驗證失敗" });
            }

            string fileName = null;
            if (PicFile != null && PicFile.ContentLength > 0)
            {
                fileName = Path.GetFileName(PicFile.FileName);
                string path = Path.Combine(Server.MapPath("~/Uploads/ActivityPics"), fileName);
                PicFile.SaveAs(path);
            }

            using (JoinGoEntities db = new JoinGoEntities())
            {
                Activity activity = new Activity
                {
                    Name = data.Name,
                    Category = data.Category,
                    Location = data.Location,
                    StartDate = data.StartDate,
                    EndDate = data.EndDate,
                    ApplyStartDate = data.ApplyStartDate,
                    ApplyEndDate = data.ApplyEndDate,
                    Contact = data.Contact,
                    ContactPhone = data.ContactPhone,
                    PicFile = fileName,
                    Description = data.Description,
                    SubCategory = data.SubCategory
                };

                db.Activity.Add(activity);
                db.SaveChanges();
            }

            return Json(new { success = true, message = "新增成功" });
        }



        // 編輯 (活動管理)
        public ActionResult EditActivity(int ActID)
        {
            if (!ChkAuthor.CheckSession()) { return RedirectToAction("LogOut", "Home"); }
            using (JoinGoEntities db = new JoinGoEntities())
            {
                var activity = db.Activity.Find(ActID);
                if (activity == null)
                {
                    return HttpNotFound();
                }

                // 塞回 ViewModel
                ActivityVM vm = new ActivityVM
                {
                    ActID = activity.ActID,
                    Name = activity.Name,
                    Category = activity.Category,
                    SubCategory = activity.SubCategory,
                    Location = activity.Location,
                    StartDate = activity.StartDate,
                    EndDate = activity.EndDate,
                    ApplyStartDate = activity.ApplyStartDate,
                    ApplyEndDate = activity.ApplyEndDate,
                    Contact = activity.Contact,
                    ContactPhone = activity.ContactPhone,
                    Description = activity.Description,
                    PicFile = activity.PicFile
                };

                // 下拉選單
                ViewBag.ActivityCategorySelect = new SelectList(ActService.GetCategoryList(), "Value", "Text", vm.Category);

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditActivity(int ActID, ActivityVM data, HttpPostedFileBase PicFile)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "資料驗證失敗" });
            }

            using (JoinGoEntities db = new JoinGoEntities())
            {
                var activity = db.Activity.Find(ActID);
                if (activity == null)
                {
                    return Json(new { success = false, message = "找不到該活動" });
                }

                // 更新資料
                activity.Name = data.Name;
                activity.Category = data.Category;
                activity.SubCategory = data.SubCategory;
                activity.Location = data.Location;
                activity.StartDate = data.StartDate;
                activity.EndDate = data.EndDate;
                activity.ApplyStartDate = data.ApplyStartDate;
                activity.ApplyEndDate = data.ApplyEndDate;
                activity.Contact = data.Contact;
                activity.ContactPhone = data.ContactPhone;
                activity.Description = data.Description;

                // 圖片處理 (有上傳新檔案才更新)
                if (PicFile != null && PicFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(PicFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Uploads/ActivityPics"), fileName);
                    PicFile.SaveAs(path);
                    activity.PicFile = fileName;
                }

                db.SaveChanges();
            }

            return Json(new { success = true, message = "修改成功" });
        }


        public ActionResult DetailsActivity(int ActID)
        {
            // 驗證登入或權限
            if (!ChkAuthor.CheckSession())
            {
                return RedirectToAction("LogOut", "Home");
            }

            using (JoinGoEntities db = new JoinGoEntities())
            {
                // 找活動
                var activity = db.Activity.Find(ActID);
                if (activity == null)
                {
                    return HttpNotFound();
                }

                // 塞回 ViewModel
                ActivityVM vm = new ActivityVM
                {
                    ActID = activity.ActID,
                    Name = activity.Name,
                    Category = activity.Category,
                    SubCategory = activity.SubCategory,
                    Location = activity.Location,
                    StartDate = activity.StartDate,
                    EndDate = activity.EndDate,
                    ApplyStartDate = activity.ApplyStartDate,
                    ApplyEndDate = activity.ApplyEndDate,
                    Contact = activity.Contact,
                    ContactPhone = activity.ContactPhone,
                    Description = activity.Description,
                    PicFile = activity.PicFile,
                    Created = activity.Created,
                    CreatedIP = activity.CreatedIP,
                    Creator = activity.Creator,    // 建檔人員 ID
                    Updator = activity.Updator,    // 更新人員 ID
                    Updated = activity.Updated,
                    UpdatedIP = activity.UpdatedIP
                };

                // 類別中文
                vm.CategoryName = ActService.GetCategoryList()
                                            .FirstOrDefault(c => c.Value == vm.Category?.ToString())?.Text;

                // 子類別中文
                vm.SubCategoryName = ActService.GetSubCategoryList(vm.Category ?? 0)
                                               .FirstOrDefault(s => s.Value == vm.SubCategory?.ToString())?.Text;

                // 建立者姓名
                vm.CreatorName = db.Account.Where(u => u.ACID == activity.Creator)
                                         .Select(u => u.Name)
                                         .FirstOrDefault();

                // 更新者姓名
                vm.UpdatorName = db.Account.Where(u => u.ACID == activity.Updator)
                                           .Select(u => u.Name)
                                           .FirstOrDefault();

                return View(vm);
            }
        }


        //刪除動作(公告)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteActivity(int ActID)
        {
            if (!ChkAuthor.CheckSession()) { return RedirectToAction("LogOut", "Home"); }
            string result = ActService.DeleteActivity(ActID);
            return Json(result, JsonRequestBehavior.AllowGet);



        }
    }
}


