using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CantinaOnline.ViewModel
{
    public class AdminPageViewModel : ObservableObject
    {
        private int _usersEatingToday;
        public int UsersEatingToday
        {
            get => _usersEatingToday;
            set => SetProperty(ref _usersEatingToday, value);
        }

        public AdminPageViewModel()
        {
            LoadData();
        }

        private async void LoadData()
        {
            UsersEatingToday = await FirestoreService.GetUsersEatingToday();
        }
    }

}
