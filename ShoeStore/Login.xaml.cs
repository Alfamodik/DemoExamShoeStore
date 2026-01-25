using ShoeStore.Model;
using System.Windows;

namespace ShoeStore;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ShoeStoreContext.Instance.Users.Any();
    }

    private void Login(object sender, RoutedEventArgs e)
    {
        User? user = ShoeStoreContext.Instance.Users.FirstOrDefault(user => user.Login == login.Text && user.Password == password.Text);

        if (user == null)
        {
            MessageBox.Show("Неверный логин или пароль");
            return;
        }

        AccessRights accessRights = AccessRights.User;

        if (user.Role == "Менеджер")
            accessRights = AccessRights.Manager;
        else if (user.Role == "Администратор")
            accessRights = AccessRights.Admin;

        new Catalog(accessRights, user).Show();
        Close();
    }

    private void LoginAsGuest(object sender, RoutedEventArgs e)
    {
        new Catalog(AccessRights.Guest).Show();
        Close();
    }
}