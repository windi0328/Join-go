using JoinGo.Models.Author;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JoinGo.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            if (!ChkAuthor.CheckSession() && AuthorModel.Current.Role == "Admin") { return RedirectToAction("LogOut", "Home"); }
            return View();
        }
    }
}