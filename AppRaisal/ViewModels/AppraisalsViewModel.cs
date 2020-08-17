using AppRaisal.Data.Enitities;
using AppRaisal.Interfaces;
using AppRaisal.Models;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Constants = AppRaisal.Data;
using System.Text;
using System.Threading.Tasks;
using AppRaisal.Utils;
using AppRaisal.Data;
using System.Windows;
using System.Threading;

namespace AppRaisal.ViewModels
{
    public class AppraisalsViewModel : IPageViewModel, INotifyPropertyChanged
    {
        private ApplicationViewModel appctx
        {
            get
            {
                return App.Current.MainWindow.DataContext as ApplicationViewModel;
            }
        }
        public string Id { get; set; } = "New Appraisals";
        public string Title { get; set; } = "Appraisals";

        private AppraisalsModel _model = new AppraisalsModel();

        private List<AppraisalPeriod> openPeriods;

        public List<AppraisalPeriod> OpenPeriods
        {
            get
            {
                return openPeriods ?? (openPeriods = new List<AppraisalPeriod>());
            }
            set
            {
                openPeriods = value;
                OnPropertyChanged("OpenPeriods");
            }
        }

        private List<AppraisalPeriod> closedPeriods;
        public List<AppraisalPeriod> ClosedPeriods
        {
            get
            {
                return closedPeriods ?? (closedPeriods = new List<AppraisalPeriod>());
            }
            set
            {
                closedPeriods = value;
                OnPropertyChanged("ClosedPeriods");
            }
        }

        public AppraisalsViewModel()
        {
        }

        #region DataAccessors
        public async void LoadPeriods()
        {
            List<AppraisalPeriod> open = new List<AppraisalPeriod>();
            List<AppraisalPeriod> closed = new List<AppraisalPeriod>();

            List<AppraisalPeriod> periods = await _model.GetPeriodsAsync();
            foreach (AppraisalPeriod period in periods)
            {
                if (period.Status == Constants.PeriodStatus.Active.ToString())
                    open.Add(period);
                else
                    closed.Add(period);
            }
            OpenPeriods = open;
            ClosedPeriods = closed;
        }
        #endregion

        #region Commands
        private RelayCommand showDetailsCommand;
        public RelayCommand ShowDetailsCommand
        {
            get
            {
                return showDetailsCommand ??
                    (showDetailsCommand = new RelayCommand(obj =>
                    {
                        AppraisalPeriod period = obj as AppraisalPeriod;
                        AppraisalDetailsViewModel vm = new AppraisalDetailsViewModel(period);
                        appctx.ChangeCurrentModelCommand.Execute(vm);
                    }));
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
