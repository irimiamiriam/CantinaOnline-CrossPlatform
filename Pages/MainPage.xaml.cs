using CantinaOnline.Models;
using CantinaOnline.ViewModels;
using Google.Cloud.Firestore.V1;
using CommunityToolkit.Maui;

namespace CantinaOnline.Pages;

public partial class MainPage : ContentPage
{
	private FirestoreService _firestore;

	public MainPage(bool isConnected, FirestoreService firestore)
	{

		InitializeComponent();
		BindingContext = new MainPageViewModel(firestore, isConnected);
        NavigationPage.SetHasNavigationBar(this, false);


    }
}
