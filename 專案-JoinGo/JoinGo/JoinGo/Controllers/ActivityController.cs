using JoinGo.Models.Author;
using JoinGo.Service.Act;
using PagedList;
using System;
using System.Collections.Generic;
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



























    }
}
