using Microsoft.Win32;
using ShoeStore.Core.Model;
using ShoeStore.Core;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ShoeStore
{
    public partial class ProductEditWindow : Window
    {
        private readonly AccessRights _accessRights;
        private readonly User? _user;
        private readonly Product _product;
        private readonly bool _isEdit;

        public ProductEditWindow(AccessRights accessRights, User? user = null, Product? product = null)
        {
            _accessRights = accessRights;
            _user = user;

            _isEdit = product != null;
            _product = product ?? new();

            InitializeComponent();
            LoadComboBoxes();
            LoadData();
        }

        private void LoadComboBoxes()
        {
            SupplierComboBox.ItemsSource = ShoeStoreContext.Instance.Suppliers.ToList();
            CategoryComboBox.ItemsSource = ShoeStoreContext.Instance.ProductCategories.ToList();
            ManufacturerComboBox.ItemsSource = ShoeStoreContext.Instance.Manufacturers.ToList();
        }

        private void LoadData()
        {
            NameTextBox.Text = _product.Product1;
            DescriptionTextBox.Text = _product.Description;
            SupplierComboBox.SelectedItem = _product.Supplier;
            UnitTextBox.Text = _product.Unit;

            PriceTextBox.Text = _product.Cost.ToString();
            QuantityTextBox.Text = _product.AmountInStorage.ToString();
            DiscountTextBox.Text = _product.Discount.ToString();

            CategoryComboBox.SelectedItem = _product.ProductCategory;
            ManufacturerComboBox.SelectedItem = _product.Manufacturer;

            LoadImage(_product.Image);
        }

        private void LoadImage(byte[]? imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
            {
                ProductImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/picture.png"));
                return;
            }

            using MemoryStream stream = new(imageBytes);

            BitmapImage bitmap = new();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();

            ProductImage.Source = bitmap;
        }

        private void SelectImage(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                Filter = "Images|*.png;*.jpg;*.jpeg"
            };

            if (dialog.ShowDialog() != true)
                return;

            _product.Image = File.ReadAllBytes(dialog.FileName);
            LoadImage(_product.Image);
        }
        
        private void SaveProduct(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Введите название товара.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(PriceTextBox.Text, out decimal cost) || cost <= 0)
            {
                MessageBox.Show("Цена должна быть числом больше 0.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(DiscountTextBox.Text, out decimal discount) || discount < 0 || discount > 100)
            {
                MessageBox.Show("Скидка должна быть числом от 0 до 100%.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(QuantityTextBox.Text, out int amountInStorage) || amountInStorage < 0)
            {
                MessageBox.Show("Количество на складе должно быть целым числом больше или равна 0.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SupplierComboBox.SelectedItem is not Supplier)
            {
                MessageBox.Show("Выберите поставщика.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CategoryComboBox.SelectedItem is not ProductCategory)
            {
                MessageBox.Show("Выберите категорию товара.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ManufacturerComboBox.SelectedItem is not Manufacturer)
            {
                MessageBox.Show("Выберите производителя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _product.Product1 = NameTextBox.Text;
            _product.Description = DescriptionTextBox.Text;
            _product.SupplierId = (SupplierComboBox.SelectedItem as Supplier)!.Id;
            _product.Unit = UnitTextBox.Text;
            _product.Cost = cost;
            _product.AmountInStorage = amountInStorage;
            _product.Discount = discount;
            _product.ProductCategoryId = (CategoryComboBox.SelectedItem as ProductCategory)!.Id;
            _product.ManufacturerId = (ManufacturerComboBox.SelectedItem as Manufacturer)!.Id;

            if (!_isEdit)
            {
                _product.Article = GenerateUniqueArticle();
                ShoeStoreContext.Instance.Products.Add(_product);
            }

            ShoeStoreContext.Instance.SaveChanges();
            new SearchCatalog(_accessRights, _user).Show();
            Close();
        }

        private void BackClick(object sender, RoutedEventArgs e)
        {
            new SearchCatalog(_accessRights, _user).Show();
            Close();
        }

        public static string GenerateUniqueArticle()
        {
            string article;

            List<string> existingArticles = [.. ShoeStoreContext.Instance.Products.Select(p => p.Article)];

            do
                article = GenerateArticle();
            while (existingArticles.Contains(article));

            return article;
        }

        private static string GenerateArticle()
        {
            Random random = new();

            char firstLetter = (char)random.Next('A', 'Z' + 1);
            int digitsPart = random.Next(0, 1000);
            char secondLetter = (char)random.Next('A', 'Z' + 1);
            int lastDigit = random.Next(0, 10);

            return $"{firstLetter}{digitsPart:D3}{secondLetter}{lastDigit}";
        }
    }
}
