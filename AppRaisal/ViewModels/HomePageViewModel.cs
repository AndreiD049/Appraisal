using AppRaisal.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace AppRaisal.ViewModels
{
    public class HomePageViewModel: IPageViewModel
    {
        public string Id { get; set; } = "Home Page";
        public string Title { get; set; } = "Home";
        public List<IPageViewModel> MenuItems { get; set; }
        public HomePageViewModel(string title = "Default Title")
        {
            Title = title;
            MenuItems = new List<IPageViewModel>
            {
                new AppraisalsViewModel(),
                new SettingsPageViewModel()
            };
        }
    }
}
