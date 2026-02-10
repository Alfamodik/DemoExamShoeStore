using ShoeStore.Model;
using System.Windows;

namespace ShoeStore;

public partial class Login : Window
{
    public Login()
    {
        InitializeComponent();
        ShoeStoreContext.Instance.Users.Any();
    }

    private void OnLoginClick(object sender, RoutedEventArgs e)
    {
        User? user = ShoeStoreContext.Instance.Users.FirstOrDefault(user => user.Login == login.Text && user.Password == password.Text);

        if (user == null)
        {
            MessageBox.Show("Неверный логин или пароль");
            return;
        }

        if (user.Role == "Менеджер")
        {
            ShowSearchCatalog(AccessRights.Manager, user);
            return;
        }
        
        if (user.Role == "Администратор")
        {
            ShowSearchCatalog(AccessRights.Admin, user);
            return;
        }
        
        new Catalog(AccessRights.User, user).Show();
        Close();
    }

    private void ShowSearchCatalog(AccessRights accessRights, User user)
    {
        new SearchCatalog(accessRights, user).Show();
        Close();
    }

    private void LoginAsGuest(object sender, RoutedEventArgs e)
    {
        new Catalog(AccessRights.Guest).Show();
        Close();
    }
}