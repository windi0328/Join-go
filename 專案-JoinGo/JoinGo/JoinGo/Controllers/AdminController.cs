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


		//public bool CheckIdentityMenu(string Url)
		//{
		//	using (INSCEntities db = new INSCEntities())
		//	{
		//		var identityType = db.Identity
		//			.Where(i => i.IDID == AuthorModel.Current.IdID)
		//			.FirstOrDefault();

		//		if (identityType == null)
		//		{
		//			return false;
		//		}

		//		var group = identityType.GUID;

		//		// 根據 URL 找menu功能
		//		var menu = db.MenuFunction.Where(o => o.Url == Url).FirstOrDefault();
		//		if (menu == null)
		//		{
		//			return false; // 如果找不到menu功能，返回 false
		//		}

		//		var isCheck = db.GroupFunction
		//			.Where(g => g.GUID == group && g.MFID == menu.MFID)
		//			.FirstOrDefault();

		//		return isCheck != null;

		//	}
		//}





		#region 線上問答管理設定

		//取得列表資料(線上問答管理設定)
		//public ActionResult QnaManageList(int page = 1)
		//{
		//	if (!ChkAuthor.CheckSession()) { return RedirectToAction("LogOut", "Home"); }
		//	if (!CheckIdentityMenu("/Site/QnaManageList"))
		//	{
		//		TempData["errmsg"] = "您無此權限，進入該頁面。";
		//		return RedirectToAction("News", "Home");
		//	}
		//	ViewBag.GetQnaDataScopeSelect = new SelectList(SiteSelectService.GetFnaDataScopeSelect(), "Value", "Text", "");
		//	ViewBag.page = page;
		//	return View();
		//}


		//// 取得列表資料(線上問答管理設定)(PartialView)
		//public ActionResult _ParQnaManageList(string search, string querytype, int page = 1)
		//{
		//	if (!ChkAuthor.CheckSession()) { return RedirectToAction("LogOut", "Home"); }
		//	if (!CheckIdentityMenu("/Site/QnaManageList"))
		//	{
		//		TempData["errmsg"] = "您無此權限，進入該頁面。";
		//		return RedirectToAction("News", "Home");
		//	}
		//	//群組權限
		//	MenuRelaVM RM = HomeService.CkCRUD("QnaManageList");
		//	ViewBag.MCreate = RM.MCreate;
		//	ViewBag.MEdit = RM.MEdit;
		//	ViewBag.MDelete = RM.MDelete;
		//	ViewBag.MApproval = RM.MApproval;

		//	var result = QnaManageService.GetQnaManagList(search, querytype, page);
		//	ViewBag.TotalCount = result.Count();
		//	ViewBag.page = page;
		//	int pageSize = 10;
		//	return PartialView(result.ToPagedList(page, pageSize));

		//}


		////新增資料(線上問答管理設定)(PartialView)

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



		////新增動作(線上問答管理設定)
		////[HttpPost]
		////[ValidateAntiForgeryToken]
		//public ActionResult CreateQna(QuestionVM data)
		//{
		//	if (!ChkAuthor.CheckSession()) { return RedirectToAction("LogOut", "Home"); }
		//	if (!CheckIdentityMenu("/Site/QnaManageList"))
		//	{
		//		TempData["errmsg"] = "您無此權限，進入該頁面。";
		//		return RedirectToAction("News", "Home");
		//	}
		//	string result = QnaManageService.CreateQuestion(data);
		//	return Json(result, JsonRequestBehavior.AllowGet);
		//}




		////查看資料(線上問答管理設定)(PartialView)
		//public ActionResult _ParDetailQna(int QuID)
		//{
		//	if (!ChkAuthor.CheckSession()) { return RedirectToAction("LogOut", "Home"); }
		//	if (!CheckIdentityMenu("/Site/QnaManageList"))
		//	{
		//		TempData["errmsg"] = "您無此權限，進入該頁面。";
		//		return RedirectToAction("News", "Home");
		//	}
		//	QuestionVM result = QnaManageService.DetailQuestion(QuID);
		//	return PartialView(result);
		//}




		////編輯資料(線上問答管理設定)(PartialView)
		//public ActionResult _ParEditQnaManage(int QuID)
		//{
		//	if (!ChkAuthor.CheckSession()) { return RedirectToAction("LogOut", "Home"); }
		//	if (!CheckIdentityMenu("/Site/QnaManageList"))
		//	{
		//		TempData["errmsg"] = "您無此權限，進入該頁面。";
		//		return RedirectToAction("News", "Home");
		//	}
		//	ViewBag.GetTypeNameSelect = new SelectList(SiteSelectService.GetTypeNameSelect("QAT"), "Value", "Text", "");
		//	QuestionVM result = QnaManageService.EditQuestionData(QuID);
		//	return PartialView(result);


		//}


		////編輯動作(線上問答管理設定)
		////[HttpPost]
		////[ValidateAntiForgeryToken]
		//public ActionResult EditQna(QuestionVM data)
		//{
		//	if (!ChkAuthor.CheckSession()) { return RedirectToAction("LogOut", "Home"); }
		//	if (!CheckIdentityMenu("/Site/QnaManageList"))
		//	{
		//		TempData["errmsg"] = "您無此權限，進入該頁面。";
		//		return RedirectToAction("News", "Home");
		//	}
		//	string result = QnaManageService.EditQna(data);
		//	return Json(result, JsonRequestBehavior.AllowGet);
		//}


		////刪除資料(線上問答管理設定)
		////[HttpPost]
		////[ValidateAntiForgeryToken]
		//public ActionResult DeleteQna(int QuID)
		//{
		//	if (!ChkAuthor.CheckSession()) { return RedirectToAction("LogOut", "Home"); }
		//	if (!CheckIdentityMenu("/Site/QnaManageList"))
		//	{
		//		TempData["errmsg"] = "您無此權限，進入該頁面。";
		//		return RedirectToAction("News", "Home");
		//	}
		//	string result = QnaManageService.DeleteQna(QuID);
		//	return Json(result, JsonRequestBehavior.AllowGet);
		//}


		////刪除所選的資料(線上問答管理設定)
		////[HttpPost]
		////[ValidateAntiForgeryToken]
		//public ActionResult DeleteQnaChecbox(int[] selectedQuIDs)
		//{
		//	if (!ChkAuthor.CheckSession()) { return RedirectToAction("LogOut", "Home"); }
		//	if (!CheckIdentityMenu("/Site/QnaManageList"))
		//	{
		//		TempData["errmsg"] = "您無此權限，進入該頁面。";
		//		return RedirectToAction("News", "Home");
		//	}
		//	string result = QnaManageService.DeleteQnaChecbox(selectedQuIDs);
		//	return Json(result, JsonRequestBehavior.AllowGet);

		//}

		#endregion




	}
}