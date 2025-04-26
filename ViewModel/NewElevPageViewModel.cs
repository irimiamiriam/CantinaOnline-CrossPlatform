using CantinaOnline.Models;
using CommunityToolkit.Mvvm.Input;
using QRCoder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CantinaOnline.ViewModel
{
    class NewElevPageViewModel : INotifyPropertyChanged
    {
        private ElevModel _user;
        public CultureInfo Culture => new CultureInfo("ro-RO");

        private ImageSource _qrCodeImage;
        public ImageSource QrCodeImage
        {
            get => _qrCodeImage;
            set
            {
                _qrCodeImage = value;
                OnPropertyChanged();
            }
        }

        private List<DateTime> _selectedDates;
        public List<DateTime> SelectedDates
        {
            get => _selectedDates;
            set
            {
                _selectedDates = value;
                OnPropertyChanged();
            }
        }

        private List<DateTime> _zileRestante;
        public List<DateTime> ZileRestante
        {
            get => _zileRestante;
            set
            {
                _zileRestante = value;
                OnPropertyChanged();
            }
        }

        public string Nume => _user.Nume;

        public ICommand DayTappedCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public NewElevPageViewModel(ElevModel user)
        {
            _user = user;
            DayTappedCommand = new RelayCommand<DateTime>(OnDayTapped);
            GenerateQR();
            InitializeDates();
        }

        private void GenerateQR()
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(_user.Id.ToString(), QRCodeGenerator.ECCLevel.L);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeBytes = qrCode.GetGraphic(20);

            QrCodeImage = ImageSource.FromStream(() => new MemoryStream(qrCodeBytes));
        }

        private void InitializeDates()
        {
            var paidDates = _user.ZilePlatite.Where(kvp => kvp.Value == 0).Select(kvp => kvp.Key).ToList();
            var unpaidDates = _user.ZilePlatite.Where(kvp => kvp.Value == 1).Select(kvp => kvp.Key).ToList();

            SelectedDates = paidDates;
            ZileRestante = unpaidDates;
        }

        private async void OnDayTapped(DateTime dateTapped)
        {
            try
            {
                if (dateTapped.Date < DateTime.Now.Date || dateTapped.DayOfWeek == DayOfWeek.Saturday || dateTapped.DayOfWeek == DayOfWeek.Sunday)
                    return;

                if (_user.ZilePlatite.TryGetValue(dateTapped.Date, out int status))
                {
                    if (status == 0)
                    {
                        bool confirm = await Application.Current.MainPage.DisplayAlert("Confirmare", "Adaugă restanță?", "Da", "Nu");
                        if (confirm)
                        {
                            if (DateTime.Now.Date == dateTapped.Date && DateTime.Now.Hour >= 8)
                            {
                                await Application.Current.MainPage.DisplayAlert("Alerta", "Nu puteti adauga restanta dupa ora 8", "Ok");
                                return;
                            }

                            _user.ZilePlatite[dateTapped.Date] = 1;
                            FirestoreService.UpdateZilePlatite(dateTapped.Date, _user, 1);
                        }
                    }
                    else if (status == 1)
                    {
                        bool confirm = await Application.Current.MainPage.DisplayAlert("Confirmare", "Anulează restanța?", "Da", "Nu");
                        if (confirm)
                        {
                            if (DateTime.Now.Date == dateTapped.Date && DateTime.Now.Hour > 8)
                            {
                                await Application.Current.MainPage.DisplayAlert("Alerta", "Nu puteți anula restanța după ora 8", "Ok");
                                return;
                            }

                            _user.ZilePlatite[dateTapped.Date] = 0;
                            FirestoreService.UpdateZilePlatite(dateTapped.Date, _user, 0);
                        }
                    }
                }

                // Recalculate SelectedDates and ZileRestante after update
                InitializeDates();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in OnDayTapped: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Eroare", ex.Message, "OK");
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
