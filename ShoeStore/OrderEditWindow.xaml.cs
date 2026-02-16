using ShoeStore.Core;
using ShoeStore.Core.Model;
using System.Windows;

namespace ShoeStore
{
    public partial class OrderEditWindow : Window
    {
        private readonly AccessRights _accessRights;
        private readonly User? _currentUser;
        private readonly Order _order;
        private readonly bool _isEdit;

        public OrderEditWindow(AccessRights accessRights, User? user = null, Order? order = null)
        {
            _accessRights = accessRights;
            _currentUser = user;

            _isEdit = order != null;
            _order = order ?? new();

            InitializeComponent();
            LoadComboBoxes();
            LoadData();
        }

        private void LoadComboBoxes()
        {
            ProductComboBox.ItemsSource = ShoeStoreContext.Instance.Products.ToList();
            PickUpPointComboBox.ItemsSource = ShoeStoreContext.Instance.PickUpPoints.ToList();
            UserComboBox.ItemsSource = ShoeStoreContext.Instance.Users.ToList();
        }

        private void LoadData()
        {
            ProductComboBox.SelectedItem = ShoeStoreContext.Instance.Products
                    .FirstOrDefault(p => p.Article == _order.ProductArticle);

            AmountTextBox.Text = _order.Amount?.ToString();

            if (_order.DateOrder.HasValue)
                DateOrderPicker.SelectedDate = _order.DateOrder.Value.ToDateTime(TimeOnly.MinValue);

            if (_order.DateDelivery.HasValue)
                DateDeliveryPicker.SelectedDate = _order.DateDelivery.Value.ToDateTime(TimeOnly.MinValue);

            PickUpPointComboBox.SelectedItem = _order.PickUpPoint;
            UserComboBox.SelectedItem = _order.User;

            CodeTextBox.Text = _order.Code;
            StatusComboBox.Text = _order.Status;
        }

        private void SaveOrder(object sender, RoutedEventArgs e)
        {
            if (ProductComboBox.SelectedItem is not Product product)
            {
                MessageBox.Show("Выберите товар.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(AmountTextBox.Text, out int amount) || amount <= 0)
            {
                MessageBox.Show("Введите количество больше 0.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DateOrderPicker.SelectedDate == null)
            {
                MessageBox.Show("Укажите дату заказа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (PickUpPointComboBox.SelectedItem is not PickUpPoint pickUpPoint)
            {
                MessageBox.Show("Выберите пункт выдачи.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (UserComboBox.SelectedItem is not User user)
            {
                MessageBox.Show("Выберите пользователя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(CodeTextBox.Text) || CodeTextBox.Text.Length > 10)
            {
                MessageBox.Show("Введите код получения не более 10 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _order.ProductArticle = product.Article;
            _order.Amount = amount;
            _order.DateOrder = DateOnly.FromDateTime(DateOrderPicker.SelectedDate.Value);
            _order.DateDelivery = DateDeliveryPicker.SelectedDate != null
                ? DateOnly.FromDateTime(DateDeliveryPicker.SelectedDate.Value)
                : null;

            _order.PickUpPointId = pickUpPoint.Id;
            _order.UserId = user.Id;
            _order.Code = CodeTextBox.Text;
            _order.Status = StatusComboBox.Text;

            if (!_isEdit)
                ShoeStoreContext.Instance.Orders.Add(_order);

            ShoeStoreContext.Instance.SaveChanges();

            new Orders(_accessRights, _currentUser).Show();
            Close();
        }

        private void DeleteOrder(object sender, RoutedEventArgs e)
        {
            if (_isEdit)
            {
                ShoeStoreContext.Instance.Orders.Remove(_order);
                ShoeStoreContext.Instance.SaveChanges();
            }

            new Orders(_accessRights, _currentUser).Show();
            Close();
        }

        private void BackClick(object sender, RoutedEventArgs e)
        {
            new Orders(_accessRights, _currentUser).Show();
            Close();
        }
    }
}
