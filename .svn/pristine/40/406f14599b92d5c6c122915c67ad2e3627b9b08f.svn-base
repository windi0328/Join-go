using JoinGo.Models;
using JoinGo.Models.Author;
using JoinGo.Models.ViewModels;
using JoinGo.Service.Fn;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace JoinGo.Service.Home
{
	public class HomeService
	{
		private static CommonFunctions CommonFunctions = new CommonFunctions();
		private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
		//註冊帳號
		public RegisterVM CreateAccount(RegisterVM register)
		{
			RegisterVM result = new RegisterVM();
			try
			{
				using (JoinGoEntities db = new JoinGoEntities())
				{

					if (!CommonFunctions.ValidatePassword(register.Password))
					{
						result.Msg = "密碼至少8碼且包含:大寫英文、數字、特殊符號，至少其中三種";
						result.Result = false;
					}

					Account acc = db.Account.Where(a => a.Email == register.Email).FirstOrDefault(); //相同email不可重複註冊
					if (acc == null)
					{
						// 1. 產生驗證碼（隨機字串）
						string verificationCode = CreateVerificationCode();

						// 2. 對驗證碼做雜湊或編碼（安全起見）
						using (var sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider())
						{
							byte[] hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(verificationCode));
							verificationCode = BitConverter.ToString(hashBytes).Replace("-", "");
						}

						// 3. 產生驗證連結（帶上驗證碼與帳號識別）
						string baseUrl = System.Configuration.ConfigurationManager.AppSettings["WebSiteUrl"].ToString();
						string verificationToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{register.Email}:{verificationCode}"));
						string verificationLink = $"{baseUrl}/Home/UserEmailCheck?token={verificationToken}";

						// 4. 組成信件內容 (HTML格式)
						string subject = "歡迎加入 JoinGo！請點擊驗證信啟用帳號";

						string body = $@"
							<div style='font-family: Arial, sans-serif; font-size: 16px; color: #333; padding: 20px; background-color: #f4f6f8;'>
								<div style='max-width: 600px; margin: auto; background-color: #ffffff; padding: 30px; border-radius: 8px; box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);'>
									<h2 style='color: #2c3e50; margin-bottom: 20px;'>帳號驗證通知</h2>
									<p>親愛的 {register.Name} 您好：</p>
									<p>感謝您註冊 <strong>JoinGo</strong>！為了保障您的帳號安全，請點擊下方按鈕完成驗證程序。</p>
									<div style='text-align: center; margin: 30px 0;'>
										<a href='{verificationLink}' 
										   style='display: inline-block; padding: 12px 30px; background-color: #3498db; color: #ffffff; text-decoration: none; font-weight: bold; border-radius: 5px; font-size: 16px;'>
										   驗證帳號
										</a>
									</div>
									<p>若上述按鈕無法正常開啟，您也可以直接點擊以下連結：</p>
									<p style='word-break: break-all; color: #555;'>{verificationLink}</p>
									<p>如有任何問題，歡迎聯繫我們的客服團隊，我們將竭誠為您服務。</p>
									<hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;' />
									<p style='color: #999; font-size: 14px;'>※ 此為系統自動寄出通知信件，請勿直接回覆此信。</p>
									<p style='font-size: 14px; color: #999;'>JoinGo 團隊敬上</p>
								</div>
							</div>";


						// 5. 呼叫寄信函式（你已有 SendMailByGmail 方法）
						bool Sendmessage = SendMailByGmail(subject, body, register.Email, null, null, "註冊驗證信");
						if (Sendmessage == true)
						{
							//建立帳號資料
							Account account = new Account()
							{
								Email = register.Email,
								EmailVerifyCode = verificationToken,
								PasswordHash = CommonFunctions.HashPassword(register.Password),
								Name = register.Name,
								Origin ="註冊",
								Role = "User",
								Enable = false, //Email驗證成功後才啟用
								Created = DateTime.Now,
								CreatedIP = CommonFunctions.GetClientIpAddress()
							};
							db.Account.Add(account);

							db.SaveChanges();

							result.Msg = "註冊成功，請至信箱收取驗證信。";
							result.Result = true;
						}
						else
						{
							result.Msg = "寄送失敗，請重新操作";
							result.Result = false;
						}
					}
					else
					{
						result.Msg = "此電子郵件已被註冊，不可重複註冊";
						result.Result = false;
					}
				}
			}
			catch (Exception ex)
			{
				logger.Debug("[HomeService]錯誤function：CreateAccount建立帳號,錯誤訊息：" + ex.InnerException + ex.ToString());
				result.Msg = "系統繁忙中，請稍後再試";
				result.Result = false;
			}

			return result;
		}





		// 範例：產生隨機驗證碼
		private string CreateVerificationCode(int length = 32)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			var random = new Random();
			return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
		}




		//寄送Email(單筆)_增加LOG(無傳入ClsID)
		public bool SendMailByGmail(string subject, string body, string EmailTo, string ACID, long? AID, string Type)
		{
			try
			{
				using (MailMessage msg = new MailMessage())
				{
					//設定smtp主機Port
					string smtpAddress = ConfigurationManager.AppSettings["smtpAddress"];
					int portNumber = int.Parse(ConfigurationManager.AppSettings["portNumber"]);
					bool enableSSL = bool.Parse(ConfigurationManager.AppSettings["enableSSL"]);
					string emailFrom = ConfigurationManager.AppSettings["emailFrom"];

					if (!string.IsNullOrWhiteSpace(EmailTo) && IsValidEmail(EmailTo)) // 確保不是空字符串 且 格式正確 且 全形自動轉換半形
					{
						var normalizedEmail = ConvertToHalfWidth(EmailTo);
						msg.To.Add(normalizedEmail);
					}

					//msg.To.Add(EmailTo);          
					msg.From = new MailAddress(emailFrom, "JoinGo", Encoding.UTF8);
					msg.Subject = subject;
					msg.SubjectEncoding = Encoding.UTF8;
					msg.Body = body;
					msg.IsBodyHtml = true;
					msg.BodyEncoding = Encoding.UTF8;//郵件內容編碼 
					msg.Priority = MailPriority.Normal;//郵件優先級 


					string password = "piph bocn igyd mwzw";
					using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
					{
						smtp.Credentials = new NetworkCredential(emailFrom, password);
						smtp.EnableSsl = enableSSL;
						smtp.Send(msg);
						//LogEmailStatus(ACID, AID, null, EmailTo, subject, Type, 1, null);    // 記錄成功發送狀態
						msg.Dispose();
					}
					
					return true;
				}
			}
			catch (Exception ex)
			{
				// 記錄成功發送狀態
				//LogEmailStatus(ACID, AID, null, EmailTo, subject, Type, 2, ex.InnerException + ex.ToString());
				logger.Debug("[HomeService]錯誤function：SendMailByGmail單筆寄送Email,錯誤訊息：" + ex.InnerException + ex.ToString());
				return false;
			}
		}



		//全形轉半形
		public static string ConvertToHalfWidth(string input)
		{
			return input.Normalize(NormalizationForm.FormKC);
		}

		//檢查Email格式
		public bool IsValidEmail(string email)
		{
			// 檢查是否為空或僅空白字符
			if (string.IsNullOrWhiteSpace(email))
			{
				return false;
			}

			// 檢查電子郵件是否包含 @ 符號
			if (!email.Contains("@"))
			{
				return false;
			}

			// 使用正則表達式檢查格式
			string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
			if (!Regex.IsMatch(email, pattern))
			{
				return false;
			}

			// 檢查電子郵件是否有有效的域名
			var parts = email.Split('@');
			if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[1]) || !parts[1].Contains("."))
			{
				return false;
			}

			// 使用 MailAddress 類進行進一步驗證
			try
			{
				var addr = new MailAddress(email);
				return addr.Address == email; // 確保原始地址和生成的地址一致
			}
			catch
			{
				return false; // 格式錯誤
			}
		}


		//檢查帳號(一般登入)
		public LoginVM CheckAccount(LoginVM acc)
		{
			using (JoinGoEntities db = new JoinGoEntities())
			{
				LoginVM result = new LoginVM();
				try
				{
					if (acc.VCode.Trim().ToLower().Equals(AuthorModel.Current.Code.ToLower())) //驗證碼正確
					{	
						Account data = db.Account.Where(a => a.Email == acc.Email).FirstOrDefault(); //系統有此Email
		
						if (data != null )
						{
							if (data.Origin != "註冊")
							{
								result.Msg = "此信箱是透過 Google 第三方登入註冊，請點選『Google 登入』以繼續使用。";
								result.Email = "";
								result.Password = "";
								result.Result = false;
							}
							else if (BCrypt.Net.BCrypt.Verify(acc.Password, data.PasswordHash))
							{
								if (data.Enable != true && data.EmailVerified != true && data.Origin == "註冊")
								{
									// 1. 產生驗證碼（隨機字串）
									string verificationCode = CreateVerificationCode();

									// 2. 對驗證碼做雜湊或編碼（安全起見）
									using (var sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider())
									{
										byte[] hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(verificationCode));
										verificationCode = BitConverter.ToString(hashBytes).Replace("-", "");
									}

									// 3. 產生驗證連結（帶上驗證碼與帳號識別）
									string baseUrl = System.Configuration.ConfigurationManager.AppSettings["WebSiteUrl"].ToString();
									string verificationToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{data.Email}:{verificationCode}"));
									string verificationLink = $"{baseUrl}/Home/UserEmailCheck?token={verificationToken}";

									// 4. 組成信件內容 (HTML格式)
									string subject = "歡迎加入 JoinGo！請點擊驗證信啟用帳號";

									string body = $@"
										<div style='font-family: Arial, sans-serif; font-size: 16px; color: #333; padding: 20px; background-color: #f4f6f8;'>
											<div style='max-width: 600px; margin: auto; background-color: #ffffff; padding: 30px; border-radius: 8px; box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);'>
												<h2 style='color: #2c3e50; margin-bottom: 20px;'>帳號驗證通知</h2>
												<p>親愛的 {data.Name} 您好：</p>
												<p>感謝您註冊 <strong>JoinGo</strong>！為了保障您的帳號安全，請點擊下方按鈕完成驗證程序。</p>
												<div style='text-align: center; margin: 30px 0;'>
													<a href='{verificationLink}' 
													   style='display: inline-block; padding: 12px 30px; background-color: #3498db; color: #ffffff; text-decoration: none; font-weight: bold; border-radius: 5px; font-size: 16px;'>
													   驗證帳號
													</a>
												</div>
												<p>若上述按鈕無法正常開啟，您也可以直接點擊以下連結：</p>
												<p style='word-break: break-all; color: #555;'>{verificationLink}</p>
												<p>如有任何問題，歡迎聯繫我們的客服團隊，我們將竭誠為您服務。</p>
												<hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;' />
												<p style='color: #999; font-size: 14px;'>※ 此為系統自動寄出通知信件，請勿直接回覆此信。</p>
												<p style='font-size: 14px; color: #999;'>JoinGo 團隊敬上</p>
											</div>
										</div>";


									// 5. 呼叫寄信函式（你已有 SendMailByGmail 方法）
									bool Sendmessage = SendMailByGmail(subject, body, data.Email, null, null, "註冊驗證信");
									if (Sendmessage == true)
									{
										data.EmailVerifyCode = verificationToken;
										db.SaveChanges();

										result.Msg = "此Email驗證尚未通過，Email驗證信已重新發送，請至信箱收取驗證信。";
										result.Result = true;
									}
									else
									{
										result.Msg = "寄送失敗，請重新操作";
										result.Result = false;
									}













								}
								else 
								{
									AuthorModel.CreateSession();
									AuthorModel.Current.ACID = data.ACID; //帳號ID
									AuthorModel.Current.LoginName = data.Name; //註冊姓名
									AuthorModel.Current.Email = data.Email; //Email
									AuthorModel.Current.Role = data.Role; //角色
									AuthorModel.Current.AvatarPhoto = data.AvatarUrl; //大頭貼

									result.Email = "";
									result.Password = "";
									result.Result = true;

									data.LastLoginTime = DateTime.Now; //最晚登入時間

									LoginLog LoginLog = new LoginLog()
									{
										ACID = data.ACID,
										Provider = "一般登入",
										Creator = data.ACID,
										Created = DateTime.Now,
										CreatedIP = CommonFunctions.GetClientIpAddress()
									};
									db.LoginLog.Add(LoginLog);
									db.SaveChanges();
								}
							}
							else
							{
								result.Msg = "輸入密碼有誤，請重新輸入(大小寫需正確)";
								result.Email = "";
								result.Password = "";
								result.Result = false;
							}
						}
						else
						{
							result.Msg = "此信箱尚未註冊。";
							result.Email = "";
							result.Password = "";
							result.Result = false;
						}
					}
					else
					{
						result.Msg = "驗證碼輸入錯誤，請重新輸入";
						result.Email = acc.Email;
						result.Password = acc.Password;
						result.Result = false;
					}
				}
				catch (Exception ex)
				{
					result.Msg = "系統錯誤";
					result.Email = acc.Email;
					result.Password = "";
					result.Result = false;
					logger.Debug("CheckAccount：" + ex.InnerException + ex.Message);
				}
				return result;
			}


		}







	}
}