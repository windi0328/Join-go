using JoinGo.Models.Author;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using JoinGo.Models;
using System.Data.SqlClient;

namespace JoinGo.Service
{
	public class MenuMethod
	{
	private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
		/**
         * 取得節點之子項目
         */
		protected DataTable GetSubitem(string mfid, string ConnStr)
		{
			DataTable dt = new DataTable();
			int ACID = AuthorModel.Current.ACID;
			string role = "";

			using (JoinGoEntities db = new JoinGoEntities())
			{
				var Acc = db.Account.FirstOrDefault(o => o.ACID == ACID);
				if (Acc == null)
				{
					return dt;
				}
				role = Acc.Role?.Trim(); // 取得角色並去除空白
			}

			string sqlstr = "SELECT * FROM MenuFunction AS M WHERE M.Enable = 1";

			if (!string.IsNullOrEmpty(mfid))
			{
				sqlstr += " AND M.ParentMFID = " + mfid;
			}

			if (!string.IsNullOrEmpty(role))
			{
				sqlstr += " AND M.Role = '" + role + "'";
			}

			sqlstr += " ORDER BY M.Serial";

			dt = getDateTable(sqlstr, ConnStr);

			return dt;
		}

		/**
         * 造訪樹狀選單(至第5層為止)
         */



		public string compose(string ConnStr)
		{
			string UserName = AuthorModel.Current.LoginName;
			string result = "";
			DataTable dt1 = new DataTable();
			dt1 = GetSubitem("0", ConnStr);
			string text1 = "";
			text1 += " <ul class='navbar-nav'>";

			for (int m1 = 0; m1 < dt1.Rows.Count; m1++)
			{
				string mfid1 = dt1.Rows[m1]["MFID"].ToString();
				string progname1 = dt1.Rows[m1]["Name"].ToString();
				string uri1 = dt1.Rows[m1]["Url"].ToString();
				string parentid1 = dt1.Rows[m1]["ParentMFID"].ToString();


				// if leaf
				if (!String.IsNullOrEmpty(uri1))
				{
					text1 += composeLeaf(progname1, uri1);
				}
				else
				{
					DataTable dt2 = new DataTable();
					dt2 = GetSubitem(mfid1, ConnStr);

					text1 += "<li class='nav-item dropdown'>" +
"				<a class='mx-4 dropdown-toggle' role='button' data-bs-toggle='dropdown' aria-expanded='false' href=\"javascript:void(0)\">" + progname1 + "</a>";

					if (dt2.Rows.Count > 0) text1 += "<ul class='dropdown-menu'>";

					string text2 = "";
					for (int m2 = 0; m2 < dt2.Rows.Count; m2++)
					{
						string mfid2 = dt2.Rows[m2]["MFID"].ToString();
						string progname2 = dt2.Rows[m2]["Name"].ToString();
						string uri2 = dt2.Rows[m2]["Url"].ToString();
						string parentid2 = dt2.Rows[m2]["ParentMFID"].ToString();
 
						// if leaf
						//第二層選單
						if (!String.IsNullOrEmpty(uri2))
						{
							text2 += composeLeaf2(progname2, uri2);
						}
						//第三層選單以後的選單
						else
						{
							DataTable dt3 = new DataTable();
							dt3 = GetSubitem(mfid2, ConnStr);
							text2 += "<li><a class='dropdown-item' href=\"javascript:void(0)\">" + progname2 + "</a>";
							if (dt3.Rows.Count > 0) text2 += "<ul class='dropdown-menu'>";
							string text3 = "";
							for (int m3 = 0; m3 < dt3.Rows.Count; m3++)
							{
								string mfid3 = dt3.Rows[m3]["MFID"].ToString();
								string progname3 = dt3.Rows[m3]["Name"].ToString();
								string uri3 = dt3.Rows[m3]["Url"].ToString();
								string parentid3 = dt3.Rows[m3]["ParentMFID"].ToString();

								// if leaf
								if (!String.IsNullOrEmpty(uri3))
								{
									text3 += composeLeaf2(progname3, uri3);
								}
								else
								{
									DataTable dt4 = new DataTable();
									dt4 = GetSubitem(mfid3, ConnStr);
									text3 += "<li class='dropdown-submenu'><a class='afont dropdown-item ' href=\"javascript:void(0)\">" + progname3 + "</a>";
									if (dt4.Rows.Count > 0) text3 += "<ul class='dropdown-menu'>";
									string text4 = "";
									for (int m4 = 0; m4 < dt4.Rows.Count; m4++)
									{
										string mfid4 = dt4.Rows[m4]["MFID"].ToString();
										string progname4 = dt4.Rows[m4]["Name"].ToString();
										string uri4 = dt4.Rows[m4]["Url"].ToString();
										string parentid4 = dt3.Rows[m4]["ParentMFID"].ToString();

										// if leaf
										if (!String.IsNullOrEmpty(uri4))
										{
											text4 += composeLeaf2(progname4, uri4);
										}
										else
										{
											DataTable dt5 = new DataTable();
											dt5 = GetSubitem(mfid4, ConnStr);
											text4 += "<li class='dropdown-submenu'><a class='afont dropdown-item' href=\"javascript:void(0)\">" + progname4 + "</a>";
											if (dt5.Rows.Count > 0) text4 += "<ul class='dropdown-menu'>";
											string text5 = "";
											for (int m5 = 0; m5 < dt5.Rows.Count; m5++)
											{
												string mfid5 = dt5.Rows[m5]["MFID"].ToString();
												string progname5 = dt5.Rows[m4]["Name"].ToString();
												string uri5 = dt5.Rows[m5]["Url"].ToString();

												// 檢查至第5層為止,不再往下長

												// if leaf
												text5 += composeLeaf2(progname5, uri5);
											}
											text4 += text5;
											if (dt5.Rows.Count > 0) text4 += "</ul></li>";
										}
									}
									text3 += text4;
									if (dt4.Rows.Count > 0) text3 += "</ul></li>";
								}
							}
							text2 += text3;
							if (dt3.Rows.Count > 0) text2 += "</ul></li>";
						}
					}
					text1 += text2;
					if (dt2.Rows.Count > 0) text1 += "</ul></li>";
					//text1 += "</li>";
				}

				//end tag
				//if (m1 == dt1.Rows.Count - 1)
				//{
				//    text1 += "</ul>";
				//}
			}
	
			//text1 += "<li class='nav-item'><div class='align-items-center d-lg-block'><div class='nav-user text-right mb-2'><a><i class='fa fa-user mr-1' aria-hidden='true'></i>" + UserName + "</a><span>｜</span><a onclick='SLogout();' style='cursor:pointer;' class='modal_btn'>登出</a></div></div></li>";
			text1 += "</ul>";
			//text1 += "<div class='float-lg-right my-3 mx-lg-4 mx-md-0'><div><svg width='18' height='18' fill='currentColor' class='bi bi-person-fill viewBox='0 0 18 18'><path d='M3 14s-1 0-1-1 1-4 6-4 6 3 6 4-1 1-1 1H3zm5-6a3 3 0 1 0 0-6 3 3 0 0 0 0 6z'></svg>" + UserName + " 你好！<a onclick='SLogout();' style='cursor:pointer;' class='mx-2'>登出</a></div></div>";
			result = text1;
			return result;
		}

		/**
         * 組合葉項目
         */
		protected string composeLeaf(string name, string uri)
		{
			string content_begin = "<li class='nav-item'><a class='nav-link' href=\"" + uri + "\">";
			string content_name = name;// "各式報表列印";
			string content_end = "<span class='sr-only'>(current)</span></a></li>";
			string result = "";
			result = content_begin + content_name + content_end;
			return result;
		}

		protected string composeLeaf2(string name, string uri)
		{
			string content_begin = "<li><a class='dropdown-item' href=\"" + uri + "\">";
			string content_name = name;// "各式報表列印";
			string content_end = "</a></li>";
			string result = "";
			result = content_begin + content_name + content_end;
			return result;
		}

		public static DataTable getDateTable(string sqlStr, string ConnStr)
		{
			SqlDataAdapter addapter = new SqlDataAdapter();
			DataTable dt = new DataTable();
			SqlCommand cmd = new SqlCommand();
			cmd.CommandType = CommandType.Text;
			cmd.CommandText = sqlStr;
			/*Properties.Settings.Default.ConnectionString*/
			cmd.Connection = new SqlConnection(ConnStr);
			addapter.SelectCommand = cmd;
			addapter.Fill(dt);
			return dt;
		}

		public static List<MenuFunction> getChildMenuList()
		{
			List<MenuFunction> result = new List<MenuFunction>();
			using (JoinGoEntities db = new JoinGoEntities())
			{
				result = db.MenuFunction.Where(o => o.ParentMFID != 0).ToList();
			}

			return result;
		}
	}
}