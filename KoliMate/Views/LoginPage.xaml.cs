using SQLite;
using System;
using System.IO;
using System.Linq;
using KoliMate.Models;
using KoliMate.ViewModels;

namespace KoliMate.Views;

public partial class LoginPage : ContentPage
{

    private LoginPageViewModel vm;

    public LoginPage(LoginPageViewModel vm)
    {
        InitializeComponent();
        this.vm = vm;
        BindingContext = vm;
    }


    
}
