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
            ProductsListBox.ItemsSource = ShoeStoreContext.Instance.Products.ToList();
        }
    }
}
