using System.Threading;
using AppRaisal.Data;
using AppRaisal.Data.Enitities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppRaisal.Utils;
using System.Windows;
using Microsoft.VisualBasic.CompilerServices;

namespace AppRaisal.Models
{
    public class AppraisalsModel: GenericModel
    {
        private ApplicationContext DbContext
        {
            get
            {
                return new ApplicationContext();
            }
        }

        // Get only appraisals relevant to the current user
        public async Task<List<AppraisalPeriod>> GetPeriodsAsync()
        {
            User user = await GetCurrentUser();
            List<AppraisalPeriod> periods = await (from period in DbContext.AppraisalPeriods.Include(p => p.Items)
                                                   where period.Items.Any(i => i.UserName == user.Username)
                                                   || (period.Status == PeriodStatus.Active.ToString())
                                                   select period).ToListAsync();
            return periods;
        }

        public async Task<List<AppraisalItem>> GetItemDetailsAsync(AppraisalPeriod period)
        {
            if (period == null)
                return null;
            User currentUser = await GetCurrentUser();
            var result = await (from item in DbContext.AppraisalItems.Include(i => i.AppraisalPeriod)
                                where item.AppraisalPeriodId == period.Id
                                where item.Status != ItemStatus.Cancelled.ToString()
                                where item.UserName == currentUser.Username
                                select item).ToListAsync();
            return result;
        }

        public async Task<List<AppraisalItem>> GetInProgressDetailsAsync()
        {
            User currentUser = await GetCurrentUser();
            ApplicationContext ctx = DbContext;
            var result = await (from item in ctx.AppraisalItems.Include(i => i.AppraisalPeriod)
                                where item.UserName == currentUser.Username
                                where item.AppraisalPeriod.Status == PeriodStatus.Finished.ToString()
                                where item.Status == ItemStatus.InProgress.ToString()
                                where item.Type == ItemType.Planned.ToString()
                                where ctx.AppraisalItems.Where(i => i.RelatedId == item.Id).FirstOrDefault() == null
                                select item).ToListAsync();
            return result;
        }

        public async Task<AppraisalItem> RemoveItemDetail(AppraisalItem item)
        {
            if (item == null || item.Id == 0)
                return null;
            return await Task.Run(() =>
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    db.AppraisalItems.Remove(item);
                    db.SaveChanges();
                    return item;
                }
            });
        }

        public async Task<AppraisalItem> UpdateItemDetail(AppraisalItem item)
        {
            if (item == null || item.Content == string.Empty || item.Content == null)
                return null;
            // Check for 0 Id (new item)
            if (item.Id == 0)
            {
                return await AddItemDetail(item);
            }
            return await Task.Run(() =>
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    db.AppraisalItems.Update(item);
                    db.SaveChanges();
                    return item;
                }
            });
        }

        public async Task<AppraisalItem> AddItemDetail(AppraisalItem item)
        {
            if (item == null || item.Content == string.Empty || item.Content == null)
                return null;
            return await Task.Run(async () =>
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    if (item.Id != 0)
                    {
                        AppraisalItem byId = db.AppraisalItems.Where(i => i.Id == item.Id).FirstOrDefault();
                        if (byId != null)
                        {
                            return await UpdateItemDetail(item); // We already have it in the database
                        }
                    }
                    db.AppraisalItems.Add(item);
                    db.SaveChanges();
                    return item;
                }
            });
        }

        #region Commands
        #endregion

    }
}
