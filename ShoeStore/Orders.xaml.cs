using ShoeStore.Core.Model;
using ShoeStore.Core;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;

namespace ShoeStore
{
    public partial class Orders : Window
    {
        private readonly AccessRights _accessRights;
        private readonly User? _user;

        public Orders(AccessRights accessRights, User? user = null)
        {
            _accessRights = accessRights;
            _user = user;

            InitializeComponent();
            InitializeView();
        }

        private void InitializeView()
        {
            CreateButton.Visibility = _accessRights == AccessRights.Admin ? Visibility.Visible : Visibility.Collapsed;

            IEnumerable<Order> orders = ShoeStoreContext.Instance.Orders
                .Include(o => o.ProductArticleNavigation)
                .Include(o => o.PickUpPoint)
                .OrderByDescending(o => o.Status)
                .ThenByDescending(o => o.Id);

            OrdersListBox.ItemsSource = orders.ToList();
        }

        private void OpenCatalog(object sender, RoutedEventArgs e)
        {
            new SearchCatalog(_accessRights, _user).Show();
            Close();
        }

        private void OrdersListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (OrdersListBox.SelectedItem is not Order selectedOrder)
                return;

            new OrderEditWindow(_accessRights, _user, selectedOrder).Show();
            Close();
        }

        private void CreateOrder(object sender, RoutedEventArgs e)
        {
            new OrderEditWindow(_accessRights, _user).Show();
            Close();
        }
    }   
}
