using JoinGo.Models;
using JoinGo.Models.Author;
using JoinGo.Service.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;


namespace JoinGo.Controllers
{
    public class ApplyController : Controller
    {

        private static AppService AppService = new AppService();

        //取得列表資料(活動管理)
        public ActionResult ApplyList(string search = "", int page = 1)
        {
            if (!ChkAuthor.CheckSession() && (AuthorModel.Current.Role == "User" || AuthorModel.Current.Role == "Admin")) { return RedirectToAction("LogOut", "Home"); }

            ViewBag.page = page;
            return View();
        }





        //取得列表資料(活動管理)
        public ActionResult _ParApplyList(string search = "", int page = 1)
        {
            if (!ChkAuthor.CheckSession() && (AuthorModel.Current.Role == "User" || AuthorModel.Current.Role == "Admin")) { return RedirectToAction("LogOut", "Home"); }

            var result = AppService.GetApplyList(search);
            ViewBag.TotalCount = result.Count();
            ViewBag.page = page;
            int pageSize = 10;
            return PartialView(result.ToPagedList(page, pageSize));
        }
    }
}