using JoinGo.Models.Author;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JoinGo.Models;
using JoinGo.Service.Admin;
using PagedList;
using JoinGo.Models.ViewModels;

namespace JoinGo.Controllers
{
    public class AdminController : Controller
    {

		private static AdminService AdminService = new AdminService();
		// GET: Admin
		public ActionResult Index()
        {
            if (!ChkAuthor.CheckSession() && AuthorModel.Current.Role == "Admin") { return RedirectToAction("LogOut", "Home"); }
            return View();
        }




		#region 聯絡我們管理

		//取得列表資料(聯絡我們管理)
		public ActionResult QnaManageList(int page = 1)
		{
			if (!ChkAuthor.CheckSession() && AuthorModel.Current.Role == "Admin") { return RedirectToAction("LogOut", "Home"); }
			ViewBag.page = page;
			return View();
		}


		// 取得列表資料(聯絡我們管理)(PartialView)
		public ActionResult _ParQnaManageList(string search, int? SearchStatus, int page = 1)
		{
			if (!ChkAuthor.CheckSession() && AuthorModel.Current.Role == "Admin") { return RedirectToAction("LogOut", "Home"); }

			var result = AdminService.GetQnaManagList(search, SearchStatus);
			ViewBag.TotalCount = result.Count();
			ViewBag.page = page;
			int pageSize = 10;
			return PartialView(result.ToPagedList(page, pageSize));

		}


		////新增資料(聯絡我們管理)(PartialView)

		//public ActionResult _ParCreateQna(int page = 1)
		//{
		//	if (!ChkAuthor.CheckSession()) { return RedirectToAction("LogOut", "Home"); }
		//	if (!CheckIdentityMenu("/Site/QnaManageList"))
		//	{
		//		TempData["errmsg"] = "您無此權限，進入該頁面。";
		//		return RedirectToAction("News", "Home");
		//	}
		//	ViewBag.GetTypeNameSelect = new SelectList(SiteSelectService.GetTypeNameSelect("QAT"), "Value", "Text", "");
		//	//ViewBag.GetGroupSelect = new SelectList(SelectService.GetIdentitySelect(), "Value", "Text", "");
		//	ViewBag.CreatorName = QnaManageService.GetACIDName(AuthorModel.Current.ACID);
		//	ViewBag.CreatorEmail = QnaManageService.GetACIDEmail(AuthorModel.Current.ACID);
		//	ViewBag.page = page;
		//	return PartialView();

		//}



	



		//查看資料(聯絡我們管理)(PartialView)
		public ActionResult _ParDetailQna(int QuID)
		{
			if (!ChkAuthor.CheckSession() && AuthorModel.Current.Role == "Admin") { return RedirectToAction("LogOut", "Home"); }

			QuestionVM result = AdminService.DetailQuestion(QuID);
			return PartialView(result);
		}




		//編輯資料(聯絡我們管理)(PartialView)
		public ActionResult _ParEditQnaManage(int QuID)
		{
			if (!ChkAuthor.CheckSession() && AuthorModel.Current.Role == "Admin") { return RedirectToAction("LogOut", "Home"); }

			QuestionVM result = AdminService.EditQuestionData(QuID);
			return PartialView(result);

		}


		//編輯存檔動作(聯絡我們管理)
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult EditQna(QuestionVM data)
		{
			if (!ChkAuthor.CheckSession() && AuthorModel.Current.Role == "Admin") { return RedirectToAction("LogOut", "Home"); }
			string result = AdminService.EditQna(data);
			return Json(result, JsonRequestBehavior.AllowGet);
		}


		//刪除資料動作(聯絡我們管理)
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteQna(int QuID)
		{
			if (!ChkAuthor.CheckSession() && AuthorModel.Current.Role == "Admin") { return RedirectToAction("LogOut", "Home"); }
			string result = AdminService.DeleteQna(QuID);
			return Json(result, JsonRequestBehavior.AllowGet);
		}


		//聯絡我們表單(不用登入)
		public ActionResult ContactUs()
		{
			return View();
		}

		//新增存檔動作(聯絡我們管理)
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CreateQna(QuestionVM data)
		{
			string result = AdminService.CreateQuestion(data);
			return Json(result, JsonRequestBehavior.AllowGet);
		}


		#endregion


		#region 常見問題管理

		//取得列表資料(常見問題管理)
		public ActionResult FAQList(int page = 1)
		{
			if (!ChkAuthor.CheckSession() && AuthorModel.Current.Role == "Admin") { return RedirectToAction("LogOut", "Home"); }
			ViewBag.page = page;
			return View();
		}


		// 取得列表資料(常見問題管理)(PartialView)
		public ActionResult _ParFAQManageList(string search, int page = 1)
		{
			if (!ChkAuthor.CheckSession() && AuthorModel.Current.Role == "Admin") { return RedirectToAction("LogOut", "Home"); }

			var result = AdminService.GetFAQManagList(search);
			ViewBag.TotalCount = result.Count();
			ViewBag.page = page;
			int pageSize = 10;
			return PartialView(result.ToPagedList(page, pageSize));

		}


		//新增資料(常見問題管理)(PartialView)
		public ActionResult _ParCreateFAQ(int page = 1)
		{
			if (!ChkAuthor.CheckSession() && AuthorModel.Current.Role == "Admin") { return RedirectToAction("LogOut", "Home"); }
			ViewBag.page = page;
			return PartialView();
		}



		//新增存檔動作(常見問題管理)
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CreateFAQ(QuestionVM data)
		{
			string result = AdminService.CreateFAQ(data);
			return Json(result, JsonRequestBehavior.AllowGet);
		}





		//查看資料(常見問題管理)(PartialView)
		public ActionResult _ParDetailFAQ(int QuID)
		{
			if (!ChkAuthor.CheckSession() && AuthorModel.Current.Role == "Admin") { return RedirectToAction("LogOut", "Home"); }

			QuestionVM result = AdminService.DetailFAQ(QuID);
			return PartialView(result);
		}




		//編輯資料(常見問題管理)(PartialView)
		public ActionResult _ParEditFAQ(int QuID)
		{
			if (!ChkAuthor.CheckSession() && AuthorModel.Current.Role == "Admin") { return RedirectToAction("LogOut", "Home"); }

			QuestionVM result = AdminService.EditFAQ(QuID);
			return PartialView(result);

		}


		//編輯存檔動作(常見問題管理)
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult EditFAQ(QuestionVM data)
		{
			if (!ChkAuthor.CheckSession() && AuthorModel.Current.Role == "Admin") { return RedirectToAction("LogOut", "Home"); }
			string result = AdminService.EditFAQ(data);
			return Json(result, JsonRequestBehavior.AllowGet);
		}


		//刪除資料動作(聯絡我們管理)
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteFAQ(int QuID)
		{
			if (!ChkAuthor.CheckSession() && AuthorModel.Current.Role == "Admin") { return RedirectToAction("LogOut", "Home"); }
			string result = AdminService.DeleteFAQ(QuID);
			return Json(result, JsonRequestBehavior.AllowGet);
		}



		//常見問題(不用登入)
		public ActionResult FAQ(string search, int page = 1)
		{
			var result = AdminService.GetFAQManagList(search);
			ViewBag.TotalCount = result.Count();
			ViewBag.page = page;
			int pageSize = 10;
			return View(result.ToPagedList(page, pageSize));
		}

		#endregion

	}
}