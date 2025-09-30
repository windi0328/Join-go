using Antlr.Runtime.Misc;
using JoinGo.Models;
using JoinGo.Models.Author;
using JoinGo.Models.ViewModels;
using JoinGo.Service;
using JoinGo.Service.Act;
using JoinGo.Service.Fn;
using JoinGo.Service.Home;
using JoinGo.Service.User;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Data.Entity; // EF 的 DbContext、DbSet


namespace JoinGo.Controllers
{
    public class UserController : Controller
    {
        private static UserService UserService = new UserService();
        private static CommonFunctions CommonFunctions = new CommonFunctions();
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        // GET: User


        public ActionResult Index()
        {
            if (!ChkAuthor.CheckSession() && (AuthorModel.Current.Role == "User" || AuthorModel.Current.Role == "Admin")) { return RedirectToAction("LogOut", "Home"); }
            ActCardVM result = UserService.GetActCard();
            return View(result);
        }


        //個人檔案
        public ActionResult Personal()
        {
            if (!ChkAuthor.CheckSession() && (AuthorModel.Current.Role == "User" || AuthorModel.Current.Role == "Admin")) { return RedirectToAction("LogOut", "Home"); }
            var ACID = AuthorModel.Current.ACID;
            AccountVM result = UserService.EditPersonal(ACID);

            using (JoinGoEntities db = new JoinGoEntities())
            {

                ViewBag.Gender = db.Option.Where(t => t.GroupKey == "性別" && t.Enable).OrderBy(t => t.Serial).ToList();

                ViewBag.EduLevel = new SelectList(db.Option.Where(t => t.GroupKey == "最高學歷" && t.Enable == true).OrderBy(t => t.Serial).ToList(), "Code", "Text");//最高學歷
            }
            return View(result);
        }




        //編輯個人資訊Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPersonal(AccountVM data)
        {
            if (!ChkAuthor.CheckSession() && (AuthorModel.Current.Role == "User" || AuthorModel.Current.Role == "Admin")) { return RedirectToAction("LogOut", "Home"); }
            JsonResultModel result = UserService.EditPersonal(data);
            return Json(result, JsonRequestBehavior.AllowGet);
        }




        //帳號管理
        public ActionResult Account()
        {
            if (!ChkAuthor.CheckSession() && (AuthorModel.Current.Role == "User" || AuthorModel.Current.Role == "Admin")) { return RedirectToAction("LogOut", "Home"); }
            var ACID = AuthorModel.Current.ACID;
            AccountVM result = UserService.EditAccount(ACID);

           return View(result);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ForgotPasswordVM model)
        {
            var result = new JsonResultModel();
            using (var db = new JoinGoEntities())
            {
                var acc = db.Account.FirstOrDefault(a => a.ACID == model.ACID);

                if (acc != null)
                {
                    acc.PasswordHash = CommonFunctions.HashPassword(model.NewPassword);
                    acc.LastPwdChange = DateTime.Now;
                    db.SaveChanges();

                    result.Result = true;
                    result.Message = "密碼已成功重設";
                }
                else
                {
                    result.Result = true;
                    result.Message = "查無此帳號";
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetEmail(AccountVM model)
        {
            var result = new JsonResultModel();
            using (var db = new JoinGoEntities())
            {
                var acc = db.Account.FirstOrDefault(a => a.ACID == model.ACID);

                if (acc != null)
                {
                    acc.Email = model.NewEmail;
                    db.SaveChanges();

                    result.Result = true;
                    result.Message = "Email已成功重設";
                }
                else
                {
                    result.Result = true;
                    result.Message = "查無此帳號";
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadMoreActivities(int page = 1, int tab = 1)
        {
            int pageSize = 9; // 每次載入 9 個活動
            using (var db = new JoinGoEntities())
            {
                var today = DateTime.Today;
                int? currentUserId = null;
                if (ChkAuthor.CheckSession())
                {
                    currentUserId = AuthorModel.Current.ACID;
                }

                IQueryable<Activity> query = db.Activity;
                // 篩選有效活動 (日期在範圍內)
                query = query.Where(a =>
                    DbFunctions.TruncateTime(a.ApplyStartDate) <= today &&
                    DbFunctions.TruncateTime(a.ApplyEndDate) >= today
                );

                // 分類篩選
                switch (tab)
                {
                    case 2: query = query.Where(a => a.Category == 1); break; // 學習活動
                    case 3: query = query.Where(a => a.Category == 2); break; // 旅遊活動
                    case 4: query = query.Where(a => a.Category == 3); break; // 美食活動
                    case 5: query = query.Where(a => a.Category == 4); break; // 運動活動
                    case 6: query = query.Where(a => a.Category == 5); break; // 其他活動
                    // case 1 或 default 不額外篩選，顯示全部
                }

                // 排序在實體層面
                query = query.OrderByDescending(a => a.ApplyStartDate.HasValue)
                             .ThenByDescending(a => a.ApplyStartDate);

                // 分頁
                var activities = query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
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

                // 如果沒有更多活動
                if (!activities.Any())
                    return Content("");

                // 回傳 PartialView
                return PartialView("_ActivityCardsPartial", activities);
            }
        }

        //public ActionResult LoadMoreActivities(int page = 1, int tab = 1)
        //{
        //    int pageSize = 9; // 每次載入 9 個活動
        //    using (JoinGoEntities db = new JoinGoEntities())
        //    {
        //        IQueryable<ActCardVM> query;
        //        var now = DateTime.Now;
        //        query = db.Activity
        //                    .Where(a => a.ApplyStartDate <= now && a.ApplyEndDate >= now)
        //                    .OrderByDescending(a => a.ApplyStartDate)
        //                    .Take(10)
        //                    .Select(a => new ActCardVM
        //                    {
        //                        ActID = a.ActID,
        //                        Name = a.Name,
        //                        ViewCount = a.ViewCount,
        //                        LikeCount = a.LikeCount,
        //                        IsLiked = a.ActivityLike.Any(l => l.ACID == AuthorModel.Current.ACID && l.IsLiked),
        //                        PicFile = a.PicFile,
        //                        Category1Name = a.Category1.Name,
        //                        Category11Name = a.Category11.Name
        //                    });

        //        switch (tab)
        //        {
        //            case 1: // 精選推薦 (允許 Start/EndDate 為 null 也顯示)
        //                    //query = db.Activity.Where(a =>
        //                    //    (a.ApplyStartDate == null || a.ApplyStartDate.Value.Date <= DateTime.Now.Date) &&
        //                    //    (a.ApplyEndDate == null || a.ApplyEndDate.Value.Date >= DateTime.Now.Date)
        //                    //);

        //                //因為LINQ不可以直接用.Date會出錯。結束日期多一天。假設結束資料庫存09/25 00:00 ，到9/25 23:59都可以報名。(資料庫存datetime但使用者介面只選日期)，以下只會有日期對日期比對:

        //                //query = db.Activity.Where(a =>
        //                //    (a.ApplyStartDate == null || DbFunctions.TruncateTime(a.ApplyStartDate) <= DateTime.Today) &&
        //                //    (a.ApplyEndDate == null || DbFunctions.TruncateTime(a.ApplyEndDate) >= DateTime.Today)
        //                //);

        //                break;

        //            case 2: // 學習活動
        //                query = query.Where(a => a.Category == 1);
        //                break;

        //            case 3: // 旅遊活動
        //                query = query.Where(a => a.Category == 2);
        //                break;

        //            case 4: // 美食活動
        //                query = query.Where(a => a.Category == 3);
        //                break;

        //            case 5: // 運動活動
        //                query = query.Where(a => a.Category == 4); 
        //                break;

        //            case 6: // 其他
        //                query = query.Where(a => a.Category == 5);
        //                break;

        //            default: // 不篩選，顯示全部
        //                //query = db.Activity;
        //                break;
        //        }


        //        var activities = query
        //            .OrderByDescending(a => a.ApplyStartDate.HasValue) // 有值的排前面
        //            .ThenByDescending(a => a.ApplyStartDate)          // 再依日期排序(晚到早?看你覺得)
        //            .Skip((page - 1) * pageSize)
        //            .Take(pageSize)
        //            .ToList();

        //        //var activities = query
        //        //    .OrderByDescending(a => a.ApplyStartDate ?? DateTime.MinValue) // null 時排最後
        //        //    .Skip((page - 1) * pageSize)
        //        //    .Take(pageSize)
        //        //    .ToList();

        //        if (!activities.Any())
        //            return Content(""); // 沒有更多活動

        //        return PartialView("_ActivityCardsPartial", activities);
        //    }
        //}


        public ActionResult DetailsAct(int ActID)
        {
            using (var db = new JoinGoEntities())
            {
                // 從資料庫抓活動（注意大小寫）
                var activity = db.Activity.FirstOrDefault(a => a.ActID == ActID);

                if (activity == null)
                {
                    return HttpNotFound();
                }
                activity.ViewCount = (activity.ViewCount ?? 0) + 1; //瀏覽數增加
                db.SaveChanges();

                return View(activity);
            }
        }

        // GET: 顯示報名頁
        public ActionResult Register(int ActID)
        {
            using (var db = new JoinGoEntities())
            {
                var activity = db.Activity.FirstOrDefault(a => a.ActID == ActID);
                if (activity == null)
                {
                    return HttpNotFound();
                }

                var model = new ApplyVM
                {
                    ActID = ActID,
                    ActTitle = activity.Name,
                    ApplyStartDate = activity.ApplyStartDate,
                    ApplyEndDate = activity.ApplyEndDate
                };

                return View(model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(ApplyVM model)
        {
            if (!ChkAuthor.CheckSession())
            {
                return Json(new { success = false, message = "請先登入再報名" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new JoinGoEntities())
                    {
                        var activity = db.Activity.FirstOrDefault(a => a.ActID == model.ActID);
                        if (activity == null)
                        {
                            return Json(new { success = false, message = "找不到活動" });
                        }

                        var apply = new Apply
                        {
                            ActID = model.ActID,
                            ACID = AuthorModel.Current.ACID,
                            Name = model.Name,
                            Email = model.Email,
                            Phone = model.Phone,
                            RegistrationDate = DateTime.Now,
                            Status = 1,
                            Creator = AuthorModel.Current?.ACID,
                            Created = DateTime.Now,
                            CreatedIP = Request.UserHostAddress
                        };

                        db.Apply.Add(apply);
                        db.SaveChanges();

                        // ✅ 報名成功後寄信
                        var userService = new UserService();
                        userService.SendRegisterMail(
                            model.Email,
                            model.Name,
                            activity.Name,
                            activity.StartDate,
                            activity.EndDate
                        );
                    }

                    return Json(new { success = true, message = "報名成功！確認信已寄至您的信箱" });
                }
                catch (Exception ex)
                {
                    logger.Debug("[UserController]錯誤function：Register 送出報名,錯誤訊息：" + ex.InnerException + ex.ToString());
                    return Json(new { success = false, message = "報名失敗：" + ex.Message });
                }
            }
            return Json(new { success = false, message = "資料驗證失敗" });
        }

        public ActionResult MyApplyList()
        {
            using (var db = new JoinGoEntities())
            {
                int acid = AuthorModel.Current.ACID;

                var list = db.Apply
                             .Where(a => a.ACID == acid)
                             .Select(a => new ApplyListVM
                             {
                                 AID = a.AID,
                                 ActID = a.ActID,
                                 ActTitle = a.Activity.Name,   // 關聯到活動名稱
                                 RegistrationDate = a.RegistrationDate,
                                 StartDate = a.Activity.StartDate,
                                 Status = (int)a.Status
                             })
                             .ToList();

                return View(list);
            }
        }
        public ActionResult MyApplyDetails(int id)
        {
            using (var db = new JoinGoEntities())
            {
                var record = db.Apply
                    .Where(a => a.AID == id)
                    .Select(a => new ApplyDetailVM
                    {
                        AID = a.AID,
                        ActID = a.ActID,
                        ActivityTitle = a.Activity.Name,
                        Name = a.Name,
                        Email = a.Email,
                        Phone = a.Phone,
                        RegistrationDate = a.RegistrationDate,
                        Status = (int)a.Status
                    })
                    .FirstOrDefault();


                return View(record);
            }
        }
        // 取消報名
        [HttpPost]
        public ActionResult MyCancelApply(int id)
        {
            using (var db = new JoinGoEntities())
            {
                var apply = db.Apply.FirstOrDefault(a => a.AID == id && a.ACID == AuthorModel.Current.ACID);
                if (apply == null)
                    return Json(new { success = false, message = "找不到報名資料" });

                if (apply.Activity.StartDate.HasValue && DateTime.Now >= apply.Activity.StartDate.Value)
                    return Json(new { success = false, message = "活動已開始，無法取消" });

                apply.Status = 2;
                apply.CanceledDate = DateTime.Now;
                db.SaveChanges();

                return Json(new { success = true, message = "已成功取消報名" });
            }
        }

        // GET 編輯報名
        public ActionResult MyEditApply(int id)
        {
            using (var db = new JoinGoEntities())
            {
                var apply = db.Apply.FirstOrDefault(a => a.AID == id && a.ACID == AuthorModel.Current.ACID);
                if (apply == null)
                    return HttpNotFound();

                if (apply.Activity.StartDate.HasValue && DateTime.Now >= apply.Activity.StartDate.Value)
                    return new HttpStatusCodeResult(403, "活動已開始，無法編輯報名");

                var model = new ApplyVM
                {
                    AID = apply.AID,
                    ActID = apply.ActID,
                    ActTitle = apply.Activity.Name,
                    Name = apply.Name,
                    Email = apply.Email,
                    Phone = apply.Phone,
                    RegistrationDate = apply.RegistrationDate,
                    Status = (int)apply.Status
                };

                return View(model);
            }
        }

        // POST 編輯報名
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditApply(ApplyVM model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "資料驗證失敗" });

            using (var db = new JoinGoEntities())
            {
                var apply = db.Apply.FirstOrDefault(a => a.AID == model.AID && a.ACID == AuthorModel.Current.ACID);
                if (apply == null)
                    return Json(new { success = false, message = "找不到這筆報名紀錄" });

                if (apply.Activity.StartDate.HasValue && DateTime.Now >= apply.Activity.StartDate.Value)
                    return Json(new { success = false, message = "活動已開始，無法編輯報名" });

                apply.Name = model.Name;
                apply.Email = model.Email;
                apply.Phone = model.Phone;
                db.SaveChanges();
            }

            return Json(new { success = true, message = "編輯完成！" });
        }



        //按愛心
        [HttpPost]
        public JsonResult ToggleLike(int actId)
        {
            int? currentUserId = null;
            if (ChkAuthor.CheckSession())
            {
                currentUserId = AuthorModel.Current.ACID;
            }
            if (currentUserId == null)
            {
                return Json(new { success = false, message = "請先登入再收藏活動" });
            }
            using (var db = new JoinGoEntities())
            {
                var like = db.ActivityLike.FirstOrDefault(l => l.ACID == currentUserId && l.ActID == actId);
                var activity = db.Activity.FirstOrDefault(a => a.ActID == actId);


                if (like == null)
                {
                    // 新增按讚紀錄
                    like = new ActivityLike
                    {
                        ACID = currentUserId,
                        ActID = actId,
                        IsLiked = true,
                        Creator = currentUserId,
                        Created = DateTime.Now
                    };
                    db.ActivityLike.Add(like);

                    // 活動讚數 +1
                    activity.LikeCount = (activity.LikeCount ?? 0) + 1;
                }
                else
                {
                    // 切換讚 / 取消
                    like.IsLiked = !like.IsLiked;
                    like.Updator = currentUserId;
                    like.Updated = DateTime.Now;

                    // 活動讚數增減
                    if (like.IsLiked)
                        activity.LikeCount = (activity.LikeCount ?? 0) + 1;
                    else
                        activity.LikeCount = Math.Max((activity.LikeCount ?? 0) - 1, 0); // 避免負數
                }

                db.SaveChanges();

                return Json(new { success = true, likeCount = activity.LikeCount, isLiked = like.IsLiked });
            }
        }

    }
}
    
   
