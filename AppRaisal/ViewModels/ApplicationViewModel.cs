using AppRaisal.Interfaces;
using AppRaisal.Models;
using AppRaisal.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace AppRaisal.ViewModels
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        private GenericModel model = new GenericModel();
        private IPageViewModel currentModel;
        private Dictionary<string, IPageViewModel> viewModelCache;

        public IPageViewModel CurrentModel
        {
            get
            {
                return currentModel;
            }
            set
            {
                currentModel = value;
                OnPropertyChanged("CurrentModel");
            }
        }

        public Dictionary<string, IPageViewModel> ViewModelCache
        {
            get
            {
                return viewModelCache ?? (viewModelCache = new Dictionary<string, IPageViewModel>());
            }

            set
            {
                viewModelCache = value;
                OnPropertyChanged("ViewModelCache");
            }
        }

        public ApplicationViewModel()
        {
            LoginUser();
            ChangeCurrentModelCommand.Execute(new HomePageViewModel("| APPRAISAL |"));
        }

        #region commands
        private RelayCommand changeCurrentModelCommand;

        // Receives an IPageViewModel as argument
        public RelayCommand ChangeCurrentModelCommand
        {
            get
            {
                return changeCurrentModelCommand ??
                    (changeCurrentModelCommand = new RelayCommand(obj =>
                    {
                        IPageViewModel page = obj as IPageViewModel;
                        if (!ViewModelCache.ContainsKey(page.Id))
                            AddToCache(page);
                        CurrentModel = ViewModelCache[page.Id];
                    }));
            }
        }

        private ICommand homePageCommand;
        public ICommand HomePageCommand
        {
            get
            {
                return homePageCommand ??
                    (homePageCommand = new RelayCommand(obj =>
                    {
                        ChangeCurrentModelCommand.Execute(new HomePageViewModel());
                    }));
            }
        }

        private RelayCommand settingsPageCommand;
        public RelayCommand SettingsPageCommand
        {
            get
            {
                return settingsPageCommand ??
                    (settingsPageCommand = new RelayCommand(obj =>
                    {
                        ChangeCurrentModelCommand.Execute(new SettingsPageViewModel());
                    }));
            }
        }

        public RelayCommand Shutdown
        {
            get
            {
                return new RelayCommand(win =>
                {
                    Window w = win as Window;
                    w.Close();
                });
            }
        }

        #endregion

        private async void LoginUser()
        {
            await model.CheckCreateUser();
        }

        private void AddToCache(IPageViewModel page)
        {
            ViewModelCache.Add(page.Id, page);
            OnPropertyChanged("ViewModelCache");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
