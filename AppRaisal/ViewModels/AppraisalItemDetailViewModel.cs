using AppRaisal.Data.Enitities;
using AppRaisal.Interfaces;
using AppRaisal.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppRaisal.ViewModels
{
    public class AppraisalItemDetailViewModel : IPageViewModel
    {
        public string Id { get; set; } = "Detail View";
        public string Title { get; set; } = "Detail";

        public AppraisalItem Item { get; set; }
        public bool IsReadOnly
        {
            get
            {
                return Item == null ||
                       Item.Status != ItemStatus.Active.ToString() ||
                       (!string.IsNullOrEmpty(Item.Content) &&
                       IsFinishedPeriod);
            }
        }

        public bool IsFinishedPeriod
        {
            get
            {
                return (Item?.AppraisalPeriod != null &&
                        Item?.AppraisalPeriod?.Status != PeriodStatus.Active.ToString());
            }
        }

        public string Tooltip
        {
            get
            {
                string result = "This item was added automatically from your previous appraisals.";
                if (Item?.AppraisalPeriod?.Status != PeriodStatus.Active.ToString() &&
                    !string.IsNullOrEmpty(Item.Content))
                    result = "This appraisal was Finished and cannot be modified";
                return result;
            }
        }

        public string IconVisibility
        {
            get
            {
                return !IsReadOnly ? "Hidden" : "Visible";
            }
        }

        public string Content
        {
            get
            {
                return Item.Content;
            }
            set
            {
                Item.Content = value;
            }
        }

        public AppraisalItemDetailViewModel(AppraisalItem item)
        {
            Item = item;
            Id = $"Detail View {Item.Id}";
        }
    }
}
