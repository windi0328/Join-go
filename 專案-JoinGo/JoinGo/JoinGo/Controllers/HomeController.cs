using JoinGo.Models;
using JoinGo.Models.Author;
using JoinGo.Models.ViewModels;
using JoinGo.Service.Fn;
using JoinGo.Service.Home;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using JoinGo.Service;

namespace JoinGo.Controllers
{
	public class HomeController : Controller
	{
		private static CommonFunctions CommonFunctions = new CommonFunctions();
		private static HomeService HomeService = new HomeService();
		private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

		//首頁
		public ActionResult Index()
		{
			return View();
		}


		public ActionResult Test()
		{
			using (JoinGoEntities db = new JoinGoEntities())
			{
				var aa = db.LoginLog.ToList();	
				return View(aa);
			}
		}




		//註冊 Partial View
		public ActionResult RegisterPartial()
		{
			return PartialView("_RegisterPartial");
		}



		//註冊表單送出 POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Register(RegisterVM register)
		{
			try
			{
				RegisterVM result = HomeService.CreateAccount(register);
				if (result.Result == true)
				{
					TempData["Msg"] = result.Msg;
				}
				else 
				{
					TempData["Error"] = result.Msg;
				}
				
				return RedirectToAction("Index", "Home");

			}
			catch (Exception ex)
			{
				logger.Debug("錯誤Function：Register，系統註冊+寄送註冊信件失敗，錯誤訊息：" + ex.ToString());
				TempData["Error"] = "系統繁忙中，請稍後再試";
				return RedirectToAction("Index", "Home");
			}
		}




		//驗證信箱
		public ActionResult UserEmailCheck(string token)
		{
			if (string.IsNullOrEmpty(token))
			{
				TempData["Error"] = "驗證碼無效";
				return RedirectToAction("Register", "Home");
			}

			using (JoinGoEntities db = new JoinGoEntities())
			{
				var user = db.Account.FirstOrDefault(a => a.EmailVerifyCode == token);
				if (user == null)
				{
					TempData["Error"] = "驗證碼不存在或已失效";
					return RedirectToAction("Index", "Home");
				}

				if (user.Enable)
				{
					TempData["Msg"] = "帳號已驗證，請直接登入";
					return RedirectToAction("Index", "Home");
				}

				user.Enable = true;
				user.EmailVerified = true;
				user.Updated = DateTime.Now;
				db.SaveChanges();
			}
			TempData["Msg"] = "驗證成功，請登入";
			return RedirectToAction("Index", "Home");
		}




		//第三方登入------------------------------------------------------------------------------------------------
		//提供你一個統一入口，來操作目前使用者的 OWIN 身份驗證狀態
		private IAuthenticationManager AuthenticationManager
		{
			get { return HttpContext.GetOwinContext().Authentication; }
		}



		// (1)導向 Google 登入
		public ActionResult ExternalLogin(string provider, string returnUrl)
		{
			// 創建一個 AuthenticationProperties 物件
			var properties = new AuthenticationProperties
			{
				RedirectUri = Url.Action("ExternalLoginCallback", "Home", new { ReturnUrl = returnUrl })
			};

			// 發起外部認證挑戰
			AuthenticationManager.Challenge(properties, provider);
			return new HttpUnauthorizedResult();
		}


		// (2) Google 登入回來
		[Route("Oauths/callback")]
		public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
		{
			var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
			if (loginInfo == null)
			{
				TempData["Error"] = "外部登入失敗，請重試一次";
				return RedirectToAction("Index", "Home");
			}

			string email = loginInfo.Email;
			string name = loginInfo.DefaultUserName;

			if (string.IsNullOrEmpty(email))
			{
				TempData["Error"] = "Google 未提供 Email，無法登入";
				return RedirectToAction("Index", "Home");
			}

			using (var db = new JoinGoEntities())
			{
				var user = db.Account.FirstOrDefault(u => u.Email == email);
				if (user == null)
				{
					user = new Account
					{
						Email = email,
						Name = name,
						Origin = loginInfo.Login.LoginProvider,
						ProviderKey = loginInfo.Login.ProviderKey,
						EmailVerified = true,
						Enable = true,
						Role = "User",
						Created = DateTime.Now,
						CreatedIP = CommonFunctions.GetClientIpAddress()
					};
					db.Account.Add(user);
					db.SaveChanges();
				}
				Account data = db.Account.Where(a => a.Email == email && a.Enable == true).FirstOrDefault(); //系統有此Email
				data.LastLoginTime = DateTime.Now;//最晚登入時間
				LoginLog LoginLog = new LoginLog()
				{
					ACID = data.ACID,
					Provider = loginInfo.Login.LoginProvider,
					ProviderKey = loginInfo.Login.ProviderKey,
					Creator = data.ACID,
					Created = DateTime.Now,
					CreatedIP = CommonFunctions.GetClientIpAddress()
				};
				db.LoginLog.Add(LoginLog);
				db.SaveChanges();

				AuthorModel.CreateSession();
				AuthorModel.Current.ACID = data.ACID;
				AuthorModel.Current.LoginName = data.Name;
				AuthorModel.Current.Email = data.Email;
				AuthorModel.Current.Role = data.Role;
				AuthorModel.Current.AvatarPhoto = data.AvatarUrl;
			}

			AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);

			if (!string.IsNullOrEmpty(returnUrl))
				return Redirect(returnUrl);

			if (AuthorModel.Current.Role == "User") //使用者
			{
				return RedirectToAction("Index", "User");
			}
			else if (AuthorModel.Current.Role == "Admin")
			{
				return RedirectToAction("Index", "Admin");
			}
			else
			{
				TempData["Error"] = "查無登入角色，請聯絡系統管理員";
				return RedirectToAction("LogOut", "Home");
			}
		}




		//登入頁面 Partial View
		public ActionResult LoginPartial()
		{
			return PartialView("_LoginPartial");
		}



		//登入驗證碼
		public void VerificationCode(string t = "")
		{
			AuthorModel.CreateSession();
			int CODELENGTH = 4; //改為4位數字
			int number;
			string RandomCode = string.Empty;
			Random r = new Random();
			for (int i = 0; i < CODELENGTH; i++)
			{
				number = r.Next(0, 10);
				RandomCode += number.ToString();
			}
			//for (int i = 0; i < CODELENGTH; i++)
			//{
			//	number = r.Next();
			//	//字元從0~9, A~Z中隨機產生,對應的ASCII碼分別為48~57, 65~90 a-z 97~122
			//	number = number % 36;
			//	if (number < 10)
			//		number += 48;
			//	else
			//		number += 55;
			//	RandomCode += ((char)number).ToString();
			//}
			//在Cookie中儲存驗證碼
			AuthorModel.Current.Code = RandomCode;

			//根據驗證碼的長度確定輸出圖片的寬度
			int iWidth = (int)Math.Ceiling(RandomCode.Length * 12m);
			int iHeight = 20;
			//建立影象
			Bitmap image = new Bitmap(iWidth, iHeight);
			//從影象獲取一個繪圖面
			Graphics g = Graphics.FromImage(image);
			Random o = new Random();
			//清空圖片背景色
			g.Clear(Color.White);
			//畫圖片的背景噪音線10條
			for (int i = 0; i < 10; i++)
			{
				int x1 = o.Next(image.Width);
				int x2 = o.Next(image.Width);
				int y1 = o.Next(image.Height);
				int y2 = o.Next(image.Height);
				//用銀色畫出噪音線
				g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
			}
			//畫圖片的前景噪音點50個
			for (int i = 0; i < 50; i++)
			{
				int x = o.Next(image.Width);
				int y = o.Next(image.Height);
				image.SetPixel(x, y, Color.FromArgb(o.Next()));
			}
			//畫圖片的框線
			g.DrawRectangle(new Pen(Color.Black), 0, 0, image.Width - 1, image.Height - 1);
			//定義繪製文字的字型
			Font f = new Font("Arial", 12, (FontStyle.Bold | FontStyle.Italic));
			//線性漸變畫刷
			System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.Purple, 1.2f, true);
			g.DrawString(RandomCode, f, brush, 2, 2);
			//建立記憶體流用於輸出圖片
			using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
			{
				//圖片格式制定為png
				image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
				//清除緩衝區流中的所有輸出
				Response.ClearContent();
				//輸出流的HTTP MIME型別設定為"image/Png"
				Response.ContentType = "image/Png";

				g.Dispose();
				image.Dispose();
				//輸出圖片的二進位制流
				Response.BinaryWrite(ms.ToArray());
			}

		}

		//驗證碼語音
		public async Task<ActionResult> VerificationCodeAudio()
		{
			// 檢查 AuthorModel.Current 或 Code 是否為 null

			if (AuthorModel.Current == null || AuthorModel.Current.Code == null)
			{
				logger.Error("AuthorModel.Current 或 AuthorModel.Current.Code 為 null");
				return Json(new { error = "發生錯誤：驗證碼無效" }, JsonRequestBehavior.AllowGet);
			}
			string captchaCode = string.Join(" ", AuthorModel.Current.Code.ToCharArray());
			string audioFilePath = Server.MapPath("~/wwwroot/audio/vcode.mp3");


			try
			{
				string directoryPath = Path.GetDirectoryName(audioFilePath);
				if (!Directory.Exists(directoryPath))
				{
					Directory.CreateDirectory(directoryPath);
				}
				else
				{
					// **先確保檔案不被占用再刪除**
					if (System.IO.File.Exists(audioFilePath))
					{
						try
						{
							System.IO.File.Delete(audioFilePath);
						}
						catch (IOException)
						{
							logger.Warn($"音檔 {audioFilePath} 被占用，嘗試強制刪除...");
							GC.Collect();
							GC.WaitForPendingFinalizers();
							System.IO.File.Delete(audioFilePath);
						}
					}
				}

				// **產生新 MP3**
				await Task.Run(() =>
				{
					using (SpeechSynthesizer synth = new SpeechSynthesizer())
					{
						// 確保語音引擎可用
						var installedVoices = synth.GetInstalledVoices();
						if (installedVoices.Count == 0)
						{
							throw new Exception("沒有可用的語音引擎");
						}
						// 嘗試選擇指定的語音，若沒有則選擇第一個可用語音
						var voiceFound = installedVoices.FirstOrDefault(v => v.VoiceInfo.Name.Contains("Microsoft Hanhan Desktop"));
						if (voiceFound != null)
						{
							synth.SelectVoice(voiceFound.VoiceInfo.Name);
							logger.Info($"選擇語音: {voiceFound.VoiceInfo.Name}");
						}
						else
						{
							// 若找不到指定語音，選擇預設語音
							synth.SelectVoice(installedVoices.First().VoiceInfo.Name);
						}


						synth.SelectVoice("Microsoft Hanhan Desktop");
						synth.Volume = 100;
						synth.Rate = 0;
						synth.SetOutputToWaveFile(audioFilePath);
						synth.Speak(captchaCode);
					}
				});

				// **等待檔案真正建立**
				int retries = 3;
				while (!System.IO.File.Exists(audioFilePath) && retries > 0)
				{
					await Task.Delay(500); // 每次等 0.5 秒再檢查
					retries--;
				}

				// **若 MP3 仍然不存在，回傳錯誤**
				if (!System.IO.File.Exists(audioFilePath))
				{
					return Json(new { error = "音檔建立失敗" }, JsonRequestBehavior.AllowGet);
				}

				// **加上時間戳，確保瀏覽器讀取最新檔案**
				string timestamp = DateTime.UtcNow.Ticks.ToString();
				string audioUrl = $"/wwwroot/audio/vcode.mp3?{timestamp}";

				return Json(new { url = audioUrl }, JsonRequestBehavior.AllowGet);
			}
			catch (NullReferenceException ex)
			{
				logger.Debug($"HomeController[VerificationCodeAudio]NullReferenceException 發生，可能是某個變數為 null: {ex}");
				return Json(new { error = "發生錯誤：" + ex.Message }, JsonRequestBehavior.AllowGet);
			}

			catch (Exception ex)
			{
				logger.Debug($"HomeController[VerificationCodeAudio]例外發生: {ex.Message}", ex);
				return Json(new { error = "發生錯誤：" + ex.Message }, JsonRequestBehavior.AllowGet);
			}
		}





		//登入 POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Login(LoginVM acc)
		{
			try
			{
				LoginVM result = HomeService.CheckAccount(acc);
				result.Email = AuthorModel.Current.Email;
				DateTime now = DateTime.Now;//現在時間
				using (JoinGoEntities db = new JoinGoEntities())
				{
					var account = db.Account.FirstOrDefault(a => a.ACID == AuthorModel.Current.ACID);

					if (result.Result == true)
					{
						if (AuthorModel.Current.Role == "User") //使用者
						{
							return RedirectToAction("Index", "User");
						}
						else if (AuthorModel.Current.Role == "Admin")
						{
							return RedirectToAction("Index", "Admin");
						}
						else
						{
							TempData["Error"] = "查無登入角色，請聯絡系統管理員";
							return RedirectToAction("LogOut", "Home");
						}
					}
					else {
						TempData["Error"] = result.Msg;
						return RedirectToAction("LogOut", "Home");
					}
				}
			}
			catch (Exception ex)
			{
				logger.Debug("錯誤Function：ChkAccount，登入失敗，錯誤訊息：" + ex.ToString());
				TempData["Error"] = "系統繁忙中，請稍後再試";
				return RedirectToAction("Index", "Home");
			}
		}





		//登出
		public ActionResult LogOut()
		{
			AuthorModel.ClearAppSession();//清除Session
			TempData["Msg"] = "登出成功!";
			return RedirectToAction("Index", "Home");
		}


		//忘記密碼  Partial View
		[HttpGet]
		public ActionResult ForgotPasswordPartial()
		{
			return PartialView("_ForgotPasswordPartial");
		}

		// 忘記密碼 POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ForgotPassword(ForgotPasswordVM model)
		{
			if (!ModelState.IsValid)
				return PartialView("_ForgotPasswordPartial", model);

			using (var db = new JoinGoEntities())
			{
				var acc = db.Account.FirstOrDefault(a => a.Email == model.Email);
				if (acc != null)
				{
					if (acc.Origin != "註冊") {
						return Json(new { success = false, message = "此信箱是透過 Google 第三方登入註冊，請點選『Google 登入』以繼續使用。" });
					}
					// 建立 Token
					string token = Guid.NewGuid().ToString("N");
					acc.PasswordResetToken = token;
					acc.PasswordResetExpiry = DateTime.Now.AddHours(1);

					db.SaveChanges();

					// 生成密碼重設連結
					string baseUrl = ConfigurationManager.AppSettings["WebSiteUrl"];
					string resetUrl = $"{baseUrl}/Home/Index?resetToken={token}";


					// 寄送 Email
					string subject = "JoinGo 密碼重設連結";
					string body = $@"
							<p>您好，</p>
							<p>請點擊以下連結來重設密碼（1 小時內有效）:</p>
							<p><a href='{resetUrl}'>{resetUrl}</a></p>
							<p>如果您未曾申請，請忽略此信。</p>";

					HomeService.SendMailByGmail(subject, body, acc.Email, null, null, "忘記密碼");

					// 返回提示
					return Json(new { success = true, message = "已寄送密碼重設連結，請至信箱收信" });
				}
				else
				{
					return Json(new { success = false, message = "查無此信箱" });
				}
			}
		}



		//重設密碼 Partial View
		[HttpGet]
		public ActionResult ResetPasswordPartial(string token)
		{
			if (string.IsNullOrWhiteSpace(token))
				return Content("連結無效");

			using (var db = new JoinGoEntities())
			{
				var acc = db.Account.FirstOrDefault(a => a.PasswordResetToken == token && a.PasswordResetExpiry > DateTime.Now);
				if (acc == null)
				{
					return Content("此密碼重設連結已失效或不存在");
				}
			}

			var model = new ForgotPasswordVM { Token = token };
			return PartialView("_ResetPasswordPartial", model);
		}


		//重設密碼 POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ResetPassword(ForgotPasswordVM model)
		{
			if (!ModelState.IsValid) return View(model);

			using (var db = new JoinGoEntities())
			{
				var acc = db.Account.FirstOrDefault(a =>
					a.PasswordResetToken == model.Token &&
					a.PasswordResetExpiry > DateTime.Now);

				if (acc != null)
				{
					acc.PasswordHash = CommonFunctions.HashPassword(model.NewPassword);
					acc.PasswordResetToken = null;
					acc.PasswordResetExpiry = null;
					acc.LastPwdChange = DateTime.Now;
					db.SaveChanges();

					TempData["Msg"] = "密碼已成功重設，請重新登入";
				}
				else
				{
					ModelState.AddModelError("", "連結已過期或無效");
				}
			}

			return RedirectToAction("Index", "Home");
		}





		//註冊=>服務條款
		public ActionResult  ServiceTerms()
		{
			return View();
		}






			//Menu
			public ActionResult MenuView()
		{
			MenuMethod MenuMethod = new MenuMethod();
			string ConnStr = ConfigurationManager.ConnectionStrings["Connectionstring"].ConnectionString;
			var ans = MenuMethod.compose(ConnStr);
			ViewBag.MenuContent = ans;
			//logger.Debug("Menu:" + ans);
			//直接把讀取到的程式寫入
			Response.Write(ans);
			return PartialView();
		}






	}
}