using JoinGo.Models;
using JoinGo.Models.Author;
using JoinGo.Models.ViewModels;
using JoinGo.Service.Act;
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
        public ActionResult ActivityList(string search = "",int page = 1)
        {
            if (!ChkAuthor.CheckSession() && ( AuthorModel.Current.Role == "User" || AuthorModel.Current.Role == "Admin")) { return RedirectToAction("LogOut", "Home"); }

            ViewBag.page = page;
            return View();
        }





        //取得列表資料(活動管理)(PartialView)
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
                    Description= data.Description
                };

                db.Activity.Add(activity);
                db.SaveChanges();
            }

            return Json(new { success = true, message = "新增成功" });
        }
    



































    }
}
