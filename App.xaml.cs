﻿using CantinaOnline.Pages;
using Microsoft.Maui.Controls;
using Application = Microsoft.Maui.Controls.Application;


namespace CantinaOnline
{
    public partial class App : Application
    {
        private readonly FirestoreService firestore;
        private Window _mainWindow;

        public App(FirestoreService firestore)
        {
         //   InitializeComponent();
            this.firestore = firestore;

            _mainWindow = new Window(new ContentPage
            {
                Content = new ActivityIndicator { IsRunning = true }
                
            });
            CheckConnection();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return _mainWindow;
        }

        private async void CheckConnection()
        {
            await Task.Delay(2000);

            bool isConnected = IsConnectedToInternet() && firestore.CheckConnection();

           
                _mainWindow.Page = new NavigationPage(new MainPage(isConnected, firestore));
             

        }

        private bool IsConnectedToInternet()
        {
            return Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
        }
    }
}