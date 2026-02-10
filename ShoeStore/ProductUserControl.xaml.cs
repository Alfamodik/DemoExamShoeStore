using ShoeStore.Model;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ShoeStore
{
    public partial class ProductUserControl : UserControl
    {
        public ProductUserControl()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Product? product = (DataContext as Product);

            if (product == null)
                return;

            if (product.Discount > 0)
            {
                CostFormated.TextDecorations = TextDecorations.Strikethrough;
                CostFormated.Foreground = new SolidColorBrush(Colors.Red);

                CostWithDiscount.Visibility = Visibility.Visible;
            }
            else
            {
                CostWithDiscount.Visibility = Visibility.Hidden;
            }

            if (product.AmountInStorage == 0)
                MainGrin.Background = new SolidColorBrush(Colors.LightBlue);

            else if (product.Discount > 15)
                MainGrin.Background = new SolidColorBrush(Color.FromRgb(46, 139, 87));

            if (product.Image != null)
            {
                BitmapImage bitmapImage = new();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(product.Image);
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                ProductImage.Source = bitmapImage;
            }
            else
            {
                ProductImage.Source = new BitmapImage(new Uri(@"\Resources\picture.png", UriKind.Relative));
            }
        }
    }
}
