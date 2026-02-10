using Microsoft.EntityFrameworkCore;
using ShoeStore.Model;
using System.Windows;

namespace ShoeStore
{
    public partial class Catalog : Window
    {
        private readonly AccessRights _accessRights;
        private readonly User? _user;

        public Catalog(AccessRights accessRights, User? user = null)
        {
            _accessRights = accessRights;
            _user = user;

            InitializeComponent();
            InitializeView();
        }

        private void InitializeView()
        {
            ProductsListBox.ItemsSource = ShoeStoreContext.Instance.Products
                .Include(p => p.ProductCategory)
                .Include(p => p.Manufacturer)
                .Include(p => p.Supplier)
                .ToList();
        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            new Login().Show();
            Close();
        }
    }
}
