using CantinaOnline.Models;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using Plugin.Maui.Calendar;
using Plugin.Maui.Calendar.Models;
using Plugin.Maui.Calendar.Controls;
using QRCoder;

namespace CantinaOnline.Pages;

public partial class ElevPage : ContentPage
{
	
	public ElevPage(ElevModel user)
	{
	  InitializeComponent();	
	  BindingContext = user;
	  calendarView.SelectedDates = user.ZilePlatite;
        numeL.Text = user.Nume;
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(user.Id.ToString(), QRCodeGenerator.ECCLevel.L);
        PngByteQRCode qRCode = new PngByteQRCode(qrCodeData);
        byte[] qrCodeBytes = qRCode.GetGraphic(20);
        QrCodeImage.Source = ImageSource.FromStream(() => new MemoryStream(qrCodeBytes));
    }


}