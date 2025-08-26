using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JoinGo.Models.Author;
using JoinGo.Models.ViewModels;
using JoinGo.Service;
using JoinGo.Service.User;
using JoinGo.Models;
using JoinGo.Service.Fn;

namespace JoinGo.Controllers
{
    public class UserController : Controller
    {
        private static UserService UserService = new UserService();
        private static CommonFunctions CommonFunctions = new CommonFunctions();

        // GET: User
        public ActionResult Index()
        {
            if (!ChkAuthor.CheckSession() && AuthorModel.Current.Role == "User") { return RedirectToAction("LogOut", "Home"); }
            return View();
        }


        //個人檔案
		public ActionResult Personal()
        {
            if (!ChkAuthor.CheckSession() && AuthorModel.Current.Role == "User") { return RedirectToAction("LogOut", "Home"); }
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
            if (!ChkAuthor.CheckSession()) { return RedirectToAction("LogOut", "Home"); }
            JsonResultModel result = UserService.EditPersonal(data);
            return Json(result, JsonRequestBehavior.AllowGet);
        }




        //帳號管理
        public ActionResult Account()
        {
            if (!ChkAuthor.CheckSession() && AuthorModel.Current.Role == "User") { return RedirectToAction("LogOut", "Home"); }
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
                var acc = db.Account.FirstOrDefault(a => a.ACID == model.ACID );

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


    }
}