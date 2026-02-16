using Microsoft.EntityFrameworkCore;
using ShoeStore.Core.Model;
using ShoeStore.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ShoeStore
{
    public partial class SearchCatalog : Window
    {
        private readonly AccessRights _accessRights;
        private readonly User? _user;

        public SearchCatalog(AccessRights accessRights, User? user = null)
        {
            _accessRights = accessRights;
            _user = user;

            InitializeComponent();
            InitializeView();
        }

        private void InitializeView()
        {
            FindItems();

            foreach (Supplier item in ShoeStoreContext.Instance.Suppliers)
                SuppliersCompoBox.Items.Add(item.Supplier1);
        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            new Login().Show();
            Close();
        }

        private void FindItems()
        {
            if (SortTypeCompoBox == null || SearchBar == null)
                return;

            IEnumerable<Product> products = ShoeStoreContext.Instance.Products
                .Include(p => p.ProductCategory)
                .Include(p => p.Manufacturer)
                .Include(p => p.Supplier)
                .Include(p => p.Orders);
            
            products = products
                .Where(SearchItems)
                .Where(p => SuppliersCompoBox.SelectedIndex == 0 || p.Supplier!.Supplier1 == SuppliersCompoBox.SelectedItem.ToString())
                .OrderByDescending(p => p.AmountInStorage);

            if (SortTypeCompoBox.SelectedIndex == 1)
                products = products.Reverse();

            NoFoundText.Visibility = products.Any() ? Visibility.Collapsed : Visibility.Visible;
            ProductsListBox.ItemsSource = products.ToList();
        }

        private bool SearchItems(Product product)
        {
            string serachText = SearchBar.Text;

            if (string.IsNullOrWhiteSpace(serachText))
                return true;

            if (product.Product1!.Contains(serachText, StringComparison.CurrentCultureIgnoreCase))
                return true;

            if (product.Description!.Contains(serachText, StringComparison.CurrentCultureIgnoreCase))
                return true;

            if (product.Manufacturer!.Manufacturer1!.Contains(serachText, StringComparison.CurrentCultureIgnoreCase))
                return true;

            if (product.Supplier!.Supplier1!.Contains(serachText, StringComparison.CurrentCultureIgnoreCase))
                return true;

            return false;
        }

        private void SearchBarTextChanged(object sender, TextChangedEventArgs e) => FindItems();

        private void OnSupplierSelected(object sender, RoutedEventArgs e) => FindItems();

        private void OnSortTypeSelected(object sender, RoutedEventArgs e) => FindItems();

        private void OpenAddProductWindow(object sender, RoutedEventArgs e)
        {
            new ProductEditWindow(_accessRights, _user).Show();
            Close();
        }

        private void OpenEditProductWindow(object sender, MouseButtonEventArgs e)
        {
            if (ProductsListBox.SelectedItem is not Product selectedProduct)
                return;
            
            new ProductEditWindow(_accessRights, _user, selectedProduct).Show();
            Close();
        }

        private void OpenOrders(object sender, RoutedEventArgs e)
        {
            new Orders(_accessRights, _user).Show();
            Close();
        }
    }
}
