using SQLite;
using System;
using System.IO;
using System.Linq;
using KoliMate.Models;

namespace KoliMate.Views;

public partial class LoginPage : ContentPage
{
    private SQLiteConnection _db;
    private User _currentUser;

    public LoginPage()
    {
        InitializeComponent();

    }

    private void OnNextClicked(object sender, EventArgs e)
    {
        MessageLabel.Text = "";
        string neptun = NeptunEntry.Text?.Trim().ToUpper();

        if (string.IsNullOrEmpty(neptun))
        {
            MessageLabel.Text = "Add meg a Neptun-kódot.";
            return;
        }

        _currentUser = _db.Table<User>().FirstOrDefault(u => u.NeptunCode == neptun);

        if (_currentUser == null)
        {
            MessageLabel.Text = "Nincs ilyen felhasználó az adatbázisban.";
            return;
        }

        // Ha megtaláltuk, mutassuk a jelszó mezõt
        PasswordPanel.IsVisible = true;

        if (_currentUser.IsActive)
        {
            PasswordLabel.Text = "Add meg a jelszavad:";
            ConfirmEntry.IsVisible = false;
        }
        else
        {
            PasswordLabel.Text = "Állíts be új jelszót:";
            ConfirmEntry.IsVisible = true;
        }
    }

    private void OnPasswordSubmitClicked(object sender, EventArgs e)
    {
        string pwd = PasswordEntry.Text;
        string confirm = ConfirmEntry.Text;

        if (_currentUser == null)
        {
            MessageLabel.Text = "Elõbb add meg a Neptun-kódot.";
            return;
        }

        if (_currentUser.IsActive)
        {
            // Bejelentkezés
            if (pwd == _currentUser.Password)
            {
                Application.Current.MainPage = new AppShell();
            }
            else
            {
                MessageLabel.Text = "Hibás jelszó.";
            }
        }
        else
        {
            // Új jelszó beállítása
            if (string.IsNullOrEmpty(pwd) || string.IsNullOrEmpty(confirm))
            {
                MessageLabel.Text = "Mindkét mezõt töltsd ki.";
                return;
            }
            if (pwd != confirm)
            {
                MessageLabel.Text = "A jelszavak nem egyeznek.";
                return;
            }

            _currentUser.Password = pwd;
            _currentUser.IsActive = true;
            _db.Update(_currentUser);

            MessageLabel.TextColor = Colors.Green;
            MessageLabel.Text = "Jelszó beállítva, most már beléphetsz.";
            PasswordLabel.Text = "Add meg a jelszavad:";
            ConfirmEntry.IsVisible = false;
        }
    }
}
