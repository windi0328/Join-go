using JoinGo.Models;
using JoinGo.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JoinGo.Service.App
{
    public class AppService
    {
        //private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region 活動管理

        public List<ApplyVM> GetApplyList(string search, int page = 1)
        {
            using (JoinGoEntities db = new JoinGoEntities())
            {
                search = search?.Trim();

                var query = from apply in db.Apply
                            join act in db.Activity on apply.ActID equals act.ActID
                            join cat in db.Category on act.ActID equals cat.CaID into g
                            from cat in g.DefaultIfEmpty()
                            where string.IsNullOrEmpty(search) || apply.Name.Contains(search)
                            orderby act.StartDate descending, apply.Created descending
                            select new ApplyVM
                            {
                                AID = apply.AID,
                                ActID = apply.ActID,
                                Name = apply.Name,
                                RegistrationDate = apply.RegistrationDate,
                                Status = apply.Status,

                                // 活動資訊
                                StartDate = act.StartDate,
                                EndDate = act.EndDate,
                                ApplyStartDate = act.ApplyStartDate,
                                ApplyEndDate = act.ApplyEndDate,
                                CategoryName = cat.Name
                            };

                return query.ToList();
            }
        }












        #endregion
    }

}