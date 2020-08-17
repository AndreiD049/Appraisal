using AppRaisal.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppRaisal.ViewModels
{
    class SettingsPageViewModel : IPageViewModel
    {
        public string Id { get; set; } = "Settings";
        public string Title { get; set; } = "Settings";

        public SettingsPageViewModel()
        {

        }

    }
}
