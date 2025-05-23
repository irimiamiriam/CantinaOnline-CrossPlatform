﻿using Camera.MAUI;
using Camera.MAUI.ZXingHelper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class ScanPageViewModel : ObservableObject
{
    private int _usersEatingToday;
    public int UsersEatingToday
    {
        get => _usersEatingToday;
        set => SetProperty(ref _usersEatingToday, value);
    }

    public ScanPageViewModel()
    {
        LoadData();
    }

    private async void LoadData()
    {
        UsersEatingToday = await (new FirestoreService()).GetUsersEatingToday();
    }

}
