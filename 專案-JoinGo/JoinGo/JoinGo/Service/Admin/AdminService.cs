using JoinGo.Models;
using JoinGo.Models.Author;
using JoinGo.Models.ViewModels;
using JoinGo.Service.Fn;
using JoinGo.Service.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JoinGo.Service.Admin
{
	public class AdminService
	{
		private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
		private static CommonFunctions CommonFunctions = new CommonFunctions();
        private static HomeService HomeService = new HomeService();

		#region 聯絡我們
		//取得列表資料(聯絡我們管理)
		public List<QuestionVM> GetQnaManagList(string search,int? SearchStatus)
        {
            using (JoinGoEntities db = new JoinGoEntities())
            {
                var query = db.Question.AsQueryable();
                query = query.Where(mf => mf.Type == 2);//只取聯絡我們的資料

                // 搜尋條件：關鍵字
                search = search?.Trim();
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(mf =>
                        mf.Questioner.Contains(search) ||
                        mf.Question1.Contains(search) ||
                        mf.Email.Contains(search));
                }
                // 搜尋條件：狀態
                if (SearchStatus.HasValue)
                {
                    query = query.Where(mf => mf.Status == SearchStatus.Value);
                }


                //將資料存到ViewModel
                var result = query.Select(mf => new QuestionVM
                {
                    QuID = mf.QuID,
                    Email = mf.Email,
                    Questioner = mf.Questioner,
                    Question1 = mf.Question1,
                    Status = mf.Status,
                    Created = mf.Created,
                    StatusName = mf.Status == 1 ? "尚未" : (mf.Status == 2 ? "處理中": ( mf.Status == 3 ? "完成": "" )) ,
                }).ToList();

                return result;
            }
        }


        //查看資料(聯絡我們)
        public QuestionVM DetailQuestion(int QuID)
        {
            QuestionVM result = new QuestionVM();
            try
            {
                using (JoinGoEntities db = new JoinGoEntities())
                {
                    Question data = db.Question.Where(a => a.QuID == QuID).FirstOrDefault();

                    if (data != null)
                    {
                        result.Type = data.Type;
                        result.Questioner = data.Questioner;
                        result.Email = data.Email;
                        result.Question1 = data.Question1;
                        result.Response = data.Response;
                        result.Status = data.Status;
                        result.StatusName = DisplayStatusDataScope(data.Status.ToString());
                        result.IsTop = data.IsTop;
                        result.Created = data.Created;
                        result.Updated = data.Updated;
                        result.CreatedIP = data.CreatedIP;
                        result.UpdatedIP = data.UpdatedIP;
                        result.CreatorName = db.Account.FirstOrDefault(u => u.ACID == data.Creator)?.Name;
                        result.UpdatorName = db.Account.FirstOrDefault(u => u.ACID == data.Updator)?.Name;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug("[AdminService]錯誤function：DetailQuestion 查看資料(聯絡我們),錯誤訊息：" + ex.InnerException + ex.ToString());
            }
            return result;
        }


        //編輯資料(聯絡我們)
        public QuestionVM EditQuestionData(int QuID)
        {
            QuestionVM result = new QuestionVM();
            try
            {
                using (JoinGoEntities db = new JoinGoEntities())
                {
                    Question data = db.Question.Where(a => a.QuID == QuID).FirstOrDefault();

                    if (data != null)
                    {

                        result.QuID = data.QuID;
                        result.Type = data.Type;
                        result.Questioner = data.Questioner;
                        result.Email = data.Email;
                        result.Question1 = data.Question1;
                        result.Response = data.Response;
                        result.Status = data.Status;
                        result.IsTop = data.IsTop;
                        result.Updated = data.Updated;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug("[AdminService]錯誤function：EditQuestionData 獲取編輯資料(聯絡我們),錯誤訊息：" + ex.InnerException + ex.ToString());
            }
            return result;
        }


        //編輯存檔動作(聯絡我們)
        public string EditQna(QuestionVM data)
        {

            string result = "";
            try
            {
                using (JoinGoEntities db = new JoinGoEntities())
                {
                    Question oldData = db.Question.Where(a => a.QuID == data.QuID).FirstOrDefault();

                    if (data != null)
                    {
                        oldData.Response = data.Response;
                        oldData.Status = data.Status;
                        oldData.Updated = DateTime.Now;
                        oldData.Updator = AuthorModel.Current.ACID;
                        oldData.UpdatedIP = CommonFunctions.GetClientIpAddress();
                        db.SaveChanges();

                        result = "編輯成功";
                        if (data.IsSendlen == 1)
                        {
							string subject = "JoinGo 聯絡我們回覆"; //信件主旨

                            string createdTime = oldData?.Created?.ToString("yyyy/MM/dd HH:mm") ?? "未提供時間";
                            // 信件內容 (HTML格式)
                            string body = $@"
                            <div style='font-family: Arial, Helvetica, sans-serif; font-size: 15px; color: #333; padding: 20px; background-color: #f4f6f8;'>
                                <div style='max-width: 600px; margin: auto; background-color: #ffffff; padding: 30px; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.05);'>
       
                                    <div style='text-align: center; margin-bottom: 25px;'>
                                        <h1 style='color: #2c3e50; font-size: 22px; margin: 0;'>JoinGo 聯絡我們回覆通知</h1>
                                    </div>

                                    <p style='margin-bottom: 16px;'>親愛的 <strong>{oldData.Questioner}</strong> 您好：</p>
                                    <p style='margin-bottom: 16px;'>感謝您於 <span style='color:#2c3e50;'>{createdTime}</span> 提交問題：</p>

                                    <div style='background-color:#f9f9f9; border-left: 4px solid #3498db; padding: 12px 16px; margin: 16px 0; border-radius: 4px;'>
                                        <p style='margin:0;'><strong>您的問題：</strong></p>
                                        <p style='margin:8px 0 0 0;'>{oldData.Question1}</p>
                                    </div>

                                    <p style='margin: 16px 0;'>我們的回覆如下：</p>
                                    <div style='background-color:#f9f9f9; border-left: 4px solid #27ae60; padding: 12px 16px; margin: 16px 0; border-radius: 4px;'>
                                        <p style='margin:0;'>{data.Response}</p>
                                    </div>

                                    <p style='margin: 16px 0;'>如有任何問題，歡迎聯繫我們的客服團隊，我們將竭誠為您服務。</p>
                                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;' />
                                    <p style='color: #999; font-size: 13px; margin: 0;'>※ 此為系統自動寄出通知信件，請勿直接回覆此信。</p>
                                    <p style='font-size: 13px; color: #999; margin: 8px 0 0 0;'>JoinGo 團隊敬上</p>
                                </div>
                            </div>";


                            bool emailSent = HomeService.SendMailByGmail(subject, body, oldData.Email, null, null, "聯絡我們"); //寄Emai
                            if (emailSent) //寄送成功
                            {
                                logger.Debug("聯絡我們回覆，寄送成功! (收件人" + oldData.Questioner + " QuestionID : " + oldData.QuID + ")");
                            }
                            else//寄送失敗
                            {
                                logger.Debug("聯絡我們回覆，寄送失敗! (收件人" + oldData.Questioner + " QuestionID : " + oldData.QuID + ")");
                            }
                        }
                    }
                    else
                    {
                        result = "查無資料";
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug("[AdminService]錯誤function：EditQna 編輯動作(聯絡我們),錯誤訊息：" + ex.InnerException + ex.ToString());
                result = "系統繁忙中，請稍後再試";
            }

            return result;
        }


        //刪除單筆(聯絡我們)
        public string DeleteQna(int QuID)
        {
            string result = "";
            try
            {
                using (JoinGoEntities db = new JoinGoEntities())
                {
                    Question Qt = db.Question.Where(a => a.QuID == QuID).FirstOrDefault();
                    db.Question.Remove(Qt);
                    db.SaveChanges();
                    result = "刪除成功";
                }
            }
            catch (Exception ex)
            {
                logger.Debug("[AdminService]錯誤function：DeleteQna 刪除動作_單筆(聯絡我們),錯誤訊息：" + ex.InnerException + ex.ToString());
                result = "系統繁忙中，請稍後再試";
            }

            return result;
        }


        //狀態對應文字
        string DisplayStatusDataScope(string Status)
        {
            // 顯示時轉換為中文描述
            var permissionMapping = new Dictionary<string, string>
            {
                { "1", "尚未" },
                { "2", "處理中" },
                { "3", "完成" }
            };

            if (string.IsNullOrEmpty(Status))
            {
                return string.Empty; // 或者返回一個預設的提示信息，如 "無權限"
            }

            var actPermDescriptions = Status.Split(',')
                                             .Select(p => permissionMapping.ContainsKey(p) ? permissionMapping[p] : "")
                                             .ToList();
            return string.Join("、", actPermDescriptions);
        }





        //新增存檔動作(聯絡我們)
        public string CreateQuestion(QuestionVM data)
        {

            string result = "";
            try
            {
                using (JoinGoEntities db = new JoinGoEntities())
                {
                    Question Qt = new Question();
                    Qt.Type = 2; //聯絡我們
                    Qt.Questioner = data.Questioner;
                    Qt.Email = data.Email;
                    Qt.Question1 = data.Question1;
                    Qt.Status = 0;//尚未

                    Qt.Created = DateTime.Now;
                    if (ChkAuthor.CheckSession()) { Qt.Creator = AuthorModel.Current.ACID; }
                    Qt.CreatedIP = CommonFunctions.GetClientIpAddress();

                    db.Question.Add(Qt);
                    db.SaveChanges();
                }
                result = "送出成功";
            }
            catch (Exception ex)
            {
                logger.Debug("[AdminService]錯誤function：CreateQuestion 新增動作(聯絡我們),錯誤訊息：" + ex.InnerException + ex.ToString());
                result = "系統繁忙中，請稍後再試";
            }

            return result;
        }

        #endregion



        #region 常見問題
        //取得列表資料(常見問題管理)
        public List<QuestionVM> GetFAQManagList(string search)
        {
            using (JoinGoEntities db = new JoinGoEntities())
            {
                var query = db.Question.AsQueryable();
                query = query.Where(mf => mf.Type == 1)//只取常見問題的資料
                             .OrderByDescending(mf => mf.IsTop)   // true 在前面
                             .ThenByDescending(mf => mf.Created); // 再依建立時間新到舊

                // 搜尋條件：關鍵字
                search = search?.Trim();
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(mf =>
                        mf.Question1.Contains(search) ||
                        mf.Response.Contains(search));
                }


                //將資料存到ViewModel
                var result = query.Select(mf => new QuestionVM
                {
                    QuID = mf.QuID,
                    Question1 = mf.Question1,
                    Response = mf.Response,
                    IsTop = mf.IsTop
                }).ToList();

                return result;
            }
        }


        //新增存檔動作(常見問題管理)
        public string CreateFAQ(QuestionVM data)
        {

            string result = "";
            try
            {
                using (JoinGoEntities db = new JoinGoEntities())
                {
                    Question Qt = new Question();
                    Qt.Type = 1; //常見問題
                    Qt.Question1 = data.Question1;
                    Qt.Response = data.Response;
                    Qt.IsTop = data.IsTop;
                    Qt.Created = DateTime.Now;
                    Qt.Creator = AuthorModel.Current.ACID; 
                    Qt.CreatedIP = CommonFunctions.GetClientIpAddress();

                    db.Question.Add(Qt);
                    db.SaveChanges();
                }
                result = "新增成功";
            }
            catch (Exception ex)
            {
                logger.Debug("[AdminService]錯誤function：CreateFAQ 新增動作(常見問題管理),錯誤訊息：" + ex.InnerException + ex.ToString());
                result = "系統繁忙中，請稍後再試";
            }

            return result;
        }


        //查看資料(常見問題管理)
        public QuestionVM DetailFAQ(int QuID)
        {
            QuestionVM result = new QuestionVM();
            try
            {
                using (JoinGoEntities db = new JoinGoEntities())
                {
                    Question data = db.Question.Where(a => a.QuID == QuID).FirstOrDefault();

                    if (data != null)
                    {
                        result.Type = data.Type;
                        result.Question1 = data.Question1;
                        result.Response = data.Response;
                        result.IsTop = data.IsTop;
                        result.Created = data.Created;
                        result.Updated = data.Updated;
                        result.CreatedIP = data.CreatedIP;
                        result.UpdatedIP = data.UpdatedIP;
                        result.CreatorName = db.Account.FirstOrDefault(u => u.ACID == data.Creator)?.Name;
                        result.UpdatorName = db.Account.FirstOrDefault(u => u.ACID == data.Updator)?.Name;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug("[AdminService]錯誤function：DetailFAQ 查看資料(常見問題管理),錯誤訊息：" + ex.InnerException + ex.ToString());
            }
            return result;
        }


        //編輯資料(常見問題管理)
        public QuestionVM EditFAQ(int QuID)
        {
            QuestionVM result = new QuestionVM();
            try
            {
                using (JoinGoEntities db = new JoinGoEntities())
                {
                    Question data = db.Question.Where(a => a.QuID == QuID).FirstOrDefault();

                    if (data != null)
                    {
                        result.QuID = data.QuID;
                        result.Type = data.Type;
                        result.Question1 = data.Question1;
                        result.Response = data.Response;
                        result.IsTop = data.IsTop;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug("[AdminService]錯誤function：EditFAQ 獲取編輯資料(常見問題管理),錯誤訊息：" + ex.InnerException + ex.ToString());
            }
            return result;
        }


        //編輯存檔動作(常見問題管理)
        public string EditFAQ(QuestionVM data)
        {

            string result = "";
            try
            {
                using (JoinGoEntities db = new JoinGoEntities())
                {
                    Question oldData = db.Question.Where(a => a.QuID == data.QuID).FirstOrDefault();

                    if (data != null)
                    {
                        oldData.Question1 = data.Question1;
                        oldData.Response = data.Response;
                        oldData.IsTop = data.IsTop;
                        oldData.Updated = DateTime.Now;
                        oldData.Updator = AuthorModel.Current.ACID;
                        oldData.UpdatedIP = CommonFunctions.GetClientIpAddress();
                        db.SaveChanges();

                        result = "編輯成功";
                    }
                    else
                    {
                        result = "查無資料";
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug("[AdminService]錯誤function：EditFAQ 編輯動作(常見問題管理),錯誤訊息：" + ex.InnerException + ex.ToString());
                result = "系統繁忙中，請稍後再試";
            }

            return result;
        }

        //刪除單筆(常見問題管理)
        public string DeleteFAQ(int QuID)
        {
            string result = "";
            try
            {
                using (JoinGoEntities db = new JoinGoEntities())
                {
                    Question Qt = db.Question.Where(a => a.QuID == QuID).FirstOrDefault();
                    db.Question.Remove(Qt);
                    db.SaveChanges();
                    result = "刪除成功";
                }
            }
            catch (Exception ex)
            {
                logger.Debug("[AdminService]錯誤function：DeleteFAQ 刪除動作_單筆(常見問題管理),錯誤訊息：" + ex.InnerException + ex.ToString());
                result = "系統繁忙中，請稍後再試";
            }

            return result;
        }


        #endregion




    }
}