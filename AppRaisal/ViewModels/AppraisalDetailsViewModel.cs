using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AppRaisal.Data;
using AppRaisal.Data.Enitities;
using AppRaisal.Interfaces;
using AppRaisal.Models;
using AppRaisal.Utils;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AppRaisal.ViewModels
{
    public class AppraisalDetailsViewModel : IPageViewModel, INotifyPropertyChanged
    {
        private const int PLANNED_MIN_ITEMS = 5;
        private const int ACHIEVED_MIN_ITEMS = 5;
        private const int TRAINING_MIN_ITEMS = 2;
        private const int TRAINING_SUGGESTED_MIN_ITEMS = 2;
        private const int SWOT_S_MIN_ITEMS = 3;
        private const int SWOT_W_MIN_ITEMS = 3;
        private const int SWOT_O_MIN_ITEMS = 3;
        private const int SWOT_T_MIN_ITEMS = 3;

        /*
         * Public fields for accessing different item types
         */
        public ObservableCollection<AppraisalItemDetailViewModel> Planned { get => returnDefault(ItemType.Planned.ToString()); }
        public ObservableCollection<AppraisalItemDetailViewModel> Achieved { get => returnDefault(ItemType.Achieved.ToString()); }
        public ObservableCollection<AppraisalItemDetailViewModel> Trainings { get => returnDefault(ItemType.Training.ToString()); }
        public ObservableCollection<AppraisalItemDetailViewModel> Trainings_Suggested { get => returnDefault(ItemType.Training_Suggested.ToString()); }
        public ObservableCollection<AppraisalItemDetailViewModel> SWOT_Strength { get => returnDefault(ItemType.SWOT_Strength.ToString()); }
        public ObservableCollection<AppraisalItemDetailViewModel> SWOT_Weakness { get => returnDefault(ItemType.SWOT_Weakness.ToString()); }
        public ObservableCollection<AppraisalItemDetailViewModel> SWOT_Opportunity { get => returnDefault(ItemType.SWOT_Opportunity.ToString()); }
        public ObservableCollection<AppraisalItemDetailViewModel> SWOT_Threat { get => returnDefault(ItemType.SWOT_Threat.ToString()); }
        public string Id { get; set; }
        public string Title { get; set; } = "Appraisal Details";
        private AppraisalsModel model = new AppraisalsModel();

        public string PeriodName { get; set; }

        private AppraisalPeriod period;

        public AppraisalPeriod Period
        {
            get
            {
                return period;
            }
            set
            {
                period = value;
                Id = $"Details {period.Id}";
                PeriodName = period.Name;
                OnPropertyChanged("Period");
            }
        }

        private Dictionary<string, ObservableCollection<AppraisalItemDetailViewModel>> items;
        public Dictionary<string, ObservableCollection<AppraisalItemDetailViewModel>> Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
                OnPropertyChanged("Items");
            }
        }

        public AppraisalDetailsViewModel(AppraisalPeriod period)
        {
            Period = period;
        }

        #region Commands
        private RelayCommand deleteItemCommand;
        public RelayCommand DeleteItemCommand
        {
            get
            {
                return deleteItemCommand ??
                    (deleteItemCommand = new RelayCommand(obj =>
                    {
                        AppraisalItemDetailViewModel itemvm = obj as AppraisalItemDetailViewModel;
                        if (itemvm.IsFinishedPeriod)
                            return;
                        if (itemvm.Item == null || itemvm.Item.Id == 0)
                            return;
                        string type = itemvm.Item.Type;
                        // If item status is InProgrees, we change the item Type
                        if (itemvm.Item.Status == ItemStatus.InProgress.ToString())
                        {
                            // If item type is achieved, move it to Planned
                            if (type == ItemType.Achieved.ToString())
                            {
                                itemvm.Item.Type = ItemType.Planned.ToString();
                                _ = model.UpdateItemDetail(itemvm.Item);
                                Items[type].Remove(itemvm);
                                AddDetail(itemvm);
                            } else if (type == ItemType.Planned.ToString() && itemvm.Item.RelatedId > 0)
                            {
                                itemvm.Item.Status = ItemStatus.Cancelled.ToString();
                                _ = model.UpdateItemDetail(itemvm.Item);
                                Items[type].Remove(itemvm);
                            } else 
                            {
                                // Delete item in the database
                                _ = model.RemoveItemDetail(itemvm.Item);
                                Items[type].Remove(itemvm);
                            }
                        } else
                        {
                            // Delete item in the database
                            _ = model.RemoveItemDetail(itemvm.Item);
                                Items[type].Remove(itemvm);
                        }
                        // Normalize
                        _ = NormalizeItems();
                    }));
            }
        }

        private RelayCommand updateItem;
        public RelayCommand UpdateItem
        {
            get
            {
                return updateItem ??
                    (updateItem = new RelayCommand(async obj =>
                    {
                        AppraisalItemDetailViewModel itemvm = obj as AppraisalItemDetailViewModel;
                        AppraisalItem item = itemvm.Item;
                        if (item != null && (item.Content != string.Empty || item.Content != null))
                        {
                            AppraisalItem updated = await model.UpdateItemDetail(item);
                        }
                        await NormalizeItems();
                    }));
            }
        }

        #endregion
        
        public async Task LoadDetails()
        {
            await CheckInProgressItems();
            Dictionary<string, ObservableCollection<AppraisalItemDetailViewModel>> result = 
                new Dictionary<string, ObservableCollection<AppraisalItemDetailViewModel>>();
            List<AppraisalItem> items = await model.GetItemDetailsAsync(Period);
            foreach (AppraisalItem item in items)
            {
                if (!result.ContainsKey(item.Type))
                    result[item.Type] = new ObservableCollection<AppraisalItemDetailViewModel>();
                result[item.Type].Add(new AppraisalItemDetailViewModel(item));
            }
            Items = result;
            await NormalizeItems();
        }

        public async Task CheckInProgressItems()
        {
            List<AppraisalItem> items = await model.GetInProgressDetailsAsync();
            foreach (var item in items)
            {
                int relatedId = item.Id;
                item.Id = 0;
                item.Type = ItemType.Achieved.ToString();
                item.AppraisalPeriod = null;
                item.AppraisalPeriodId = Period.Id;
                item.RelatedId = relatedId;
                await model.AddItemDetail(item);
            }
        }

        /// <summary>
        /// Make sure all items have enough default items:
        /// Ex. min 5 items for Planned, 3 for SWOT cadran etc..
        /// </summary>
        private async Task NormalizeItems()
        {
            if (Period.Status == PeriodStatus.Active.ToString())
            {
                // Planned
                await SupplementList(ItemType.Planned.ToString(), PLANNED_MIN_ITEMS);

                // Achieved
                await SupplementList(ItemType.Achieved.ToString(), ACHIEVED_MIN_ITEMS);

                // Training
                await SupplementList(ItemType.Training.ToString(), TRAINING_MIN_ITEMS);

                // Trainings Suggested
                await SupplementList(ItemType.Training_Suggested.ToString(), TRAINING_SUGGESTED_MIN_ITEMS);

                // SWOT Strength
                await SupplementList(ItemType.SWOT_Strength.ToString(), SWOT_S_MIN_ITEMS);

                // SWOT Weaknesses
                await SupplementList(ItemType.SWOT_Weakness.ToString(), SWOT_W_MIN_ITEMS);

                // SWOT Opportunity
                await SupplementList(ItemType.SWOT_Opportunity.ToString(), SWOT_O_MIN_ITEMS);

                // SWOT Threat
                await SupplementList(ItemType.SWOT_Threat.ToString(), SWOT_T_MIN_ITEMS);
            }
            // Notify items
            OnPropertyChanged("Planned");
            OnPropertyChanged("Achieved");
            OnPropertyChanged("Trainings");
            OnPropertyChanged("Trainings_Suggested");
            OnPropertyChanged("SWOT_Strength");
            OnPropertyChanged("SWOT_Weakness");
            OnPropertyChanged("SWOT_Opportunity");
            OnPropertyChanged("SWOT_Threat");
        }

        private async Task SupplementList(string type, int target)
        {
            User current = await model.GetCurrentUser();
            if (!Items.ContainsKey(type))
                Items[type] = new ObservableCollection<AppraisalItemDetailViewModel>();
            int diff = Items[type].Count - target;
            if (diff < 0)
            {
                for (int i = diff; i < 0; i++)
                {
                    AppraisalItem newItem = new AppraisalItem 
                    { 
                        Type = type, 
                        Status = ItemStatus.Active.ToString(), 
                        AppraisalPeriodId = Period.Id,
                        UserName = current.Username
                    };
                    Items[type].Add(new AppraisalItemDetailViewModel(newItem));
                }
            } else
            {
                // Each list should have at least one empty item
                AppraisalItemDetailViewModel empty = Items[type].FirstOrDefault(vm => vm.Item.Content == null || vm.Item.Content == string.Empty);
                if (empty == null)
                {
                    // We don't have any empty items, add a new one
                    AppraisalItem newItem = new AppraisalItem 
                    { 
                        Type = type, 
                        Status = ItemStatus.Active.ToString(), 
                        AppraisalPeriodId = Period.Id,
                        UserName = current.Username
                    };
                    Items[type].Add(new AppraisalItemDetailViewModel(newItem));
                }
            }
        }

        private RelayCommand loadedCommand;
        public RelayCommand LoadedCommand
        {
            get
            {
                return loadedCommand ??
                    (loadedCommand = new RelayCommand(async obj =>
                    {
                        await LoadDetails();
                    }));
            }
        }

        private void AddDetail(AppraisalItemDetailViewModel itemvm)
        {
            string type = itemvm.Item.Type;
            while (Items[type].Count > 0 && string.IsNullOrEmpty(Items[type].Last().Content))
                Items[type].RemoveAt(Items[type].Count - 1);
            Items[type].Add(itemvm);
        }

        private ObservableCollection<AppraisalItemDetailViewModel> returnDefault(string k)
        {
            if (Items == null)
                Items = new Dictionary<string, ObservableCollection<AppraisalItemDetailViewModel>>();
            if (!Items.ContainsKey(k))
                Items[k] = new ObservableCollection<AppraisalItemDetailViewModel>();
            return Items[k];
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        

    }
}
