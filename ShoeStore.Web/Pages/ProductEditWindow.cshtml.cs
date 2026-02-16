using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoeStore.Core;
using ShoeStore.Core.Model;
using SixLabors.ImageSharp;

namespace ShoeStore.Web.Pages
{
    public class ProductEditWindowModel : PageModel
    {
        [BindProperty] public string? Article { get; set; }
        [BindProperty] public string? Name { get; set; }
        [BindProperty] public string? Description { get; set; }
        [BindProperty] public int? SupplierId { get; set; }
        [BindProperty] public int? ManufacturerId { get; set; }
        [BindProperty] public int? ProductCategoryId { get; set; }
        [BindProperty] public decimal? Cost { get; set; }
        [BindProperty] public string? Unit { get; set; }
        [BindProperty] public int? AmountInStorage { get; set; }
        [BindProperty] public decimal? Discount { get; set; }
        [BindProperty] public IFormFile? ImageFile { get; set; }

        public string? ErrorMessage { get; private set; }
        public string ImageSrc { get; private set; } = "/Resources/picture.png";

        public List<SelectListItem> SupplierItems { get; private set; } = new();
        public List<SelectListItem> ManufacturerItems { get; private set; } = new();
        public List<SelectListItem> CategoryItems { get; private set; } = new();

        public AccessRights? AccessRights { get; set; }

        public void OnGet(string? article, string? accessRights)
        {
            if (Enum.TryParse(accessRights, true, out AccessRights parsedAccess))
                AccessRights = parsedAccess;

            LoadSelects();

            if (string.IsNullOrWhiteSpace(article))
                return;

            Product? product = ShoeStoreContext.Instance.Products
                .FirstOrDefault(p => p.Article == article);

            if (product == null)
                return;

            Article = product.Article;
            Name = product.Product1;
            Description = product.Description;
            SupplierId = product.SupplierId;
            ManufacturerId = product.ManufacturerId;
            ProductCategoryId = product.ProductCategoryId;
            Cost = product.Cost;
            Unit = product.Unit;
            AmountInStorage = product.AmountInStorage;
            Discount = product.Discount;

            if (product.Image != null)
                ImageSrc = $"data:image/png;base64,{Convert.ToBase64String(product.Image)}";
        }

        public IActionResult OnPost(string handler, string? accessRights)
        {
            if (Enum.TryParse(accessRights, true, out AccessRights parsedAccess))
                AccessRights = parsedAccess;

            LoadSelects();

            if (handler == "Back")
                return RedirectToPage("/SearchCatalog", new { accessRights = AccessRights?.ToString() });

            if (handler == "Delete")
            {
                if (string.IsNullOrWhiteSpace(Article))
                    return RedirectToPage("/SearchCatalog", new { accessRights = AccessRights?.ToString() });

                Product? productToDelete = ShoeStoreContext.Instance.Products
                    .Include(p => p.Orders)
                    .FirstOrDefault(p => p.Article == Article);

                if (productToDelete == null)
                    return RedirectToPage("/SearchCatalog", new { accessRights = AccessRights?.ToString() });

                if (productToDelete.Orders.Count != 0)
                {
                    ErrorMessage = "Нельзя удалить товар с заказами.";
                    return Page();
                }

                ShoeStoreContext.Instance.Products.Remove(productToDelete);
                ShoeStoreContext.Instance.SaveChanges();

                return RedirectToPage("/SearchCatalog", new { accessRights = AccessRights?.ToString() });
            }

            if (!Validate())
                return Page();

            Product? product = null;

            if (!string.IsNullOrWhiteSpace(Article))
                product = ShoeStoreContext.Instance.Products
                    .FirstOrDefault(p => p.Article == Article);

            if (product == null)
            {
                product = new Product
                {
                    Article = GenerateUniqueArticle()
                };
                ShoeStoreContext.Instance.Products.Add(product);
            }

            product.Product1 = Name;
            product.Description = Description;
            product.SupplierId = SupplierId;
            product.ManufacturerId = ManufacturerId;
            product.ProductCategoryId = ProductCategoryId;
            product.Cost = Cost;
            product.Unit = Unit;
            product.AmountInStorage = AmountInStorage;
            product.Discount = Discount;

            if (ImageFile != null)
            {
                using MemoryStream ms = new();
                ImageFile.CopyTo(ms);
                product.Image = ms.ToArray();
            }

            ShoeStoreContext.Instance.SaveChanges();

            return RedirectToPage("/SearchCatalog", new { accessRights = AccessRights?.ToString() });
        }

        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return SetError("Введите название товара.");

            if (Name.Length > 45)
                return SetError("Название не должно превышать 45 символов.");

            if (string.IsNullOrWhiteSpace(Description))
                return SetError("Введите описание товара.");

            if (Description.Length > 255)
                return SetError("Описание не должно превышать 255 символов.");

            if (Cost is null || Cost <= 0)
                return SetError("Цена должна быть больше 0.");

            if (Discount is null || Discount < 0 || Discount > 99)
                return SetError("Скидка должна быть числом от 0 до 99.");

            if (AmountInStorage is null || AmountInStorage <= 0)
                return SetError("Количество должно быть > 0.");

            if (SupplierId is null)
                return SetError("Выберите поставщика.");

            if (ManufacturerId is null)
                return SetError("Выберите производителя.");

            if (ProductCategoryId is null)
                return SetError("Выберите категорию.");

            if (string.IsNullOrWhiteSpace(Unit) || Unit.Length > 10)
                return SetError("Введите единицу измерения не более 10 символов.");

            if (ImageFile != null && !IsCorrectImageResolution(ImageFile, 1920, 1080))
                return SetError("Разрешение изображения должно быть 1920 x 1080.");

            return true;
        }

        private bool IsCorrectImageResolution(IFormFile file, int requiredWidth, int requiredHeight)
        {
            using MemoryStream ms = new();
            file.CopyTo(ms);
            ms.Position = 0;

            using Image image = Image.Load(ms);
            return image.Width == requiredWidth && image.Height == requiredHeight;
        }

        private bool SetError(string message)
        {
            ErrorMessage = message;
            return false;
        }

        private void LoadSelects()
        {
            SupplierItems = ShoeStoreContext.Instance.Suppliers
                .Select(s => new SelectListItem(s.Supplier1, s.Id.ToString()))
                .ToList();

            ManufacturerItems = ShoeStoreContext.Instance.Manufacturers
                .Select(m => new SelectListItem(m.Manufacturer1, m.Id.ToString()))
                .ToList();

            CategoryItems = ShoeStoreContext.Instance.ProductCategories
                .Select(c => new SelectListItem(c.ProductCategory1, c.Id.ToString()))
                .ToList();
        }

        public static string GenerateUniqueArticle()
        {
            List<string> existing = ShoeStoreContext.Instance.Products.Select(p => p.Article).ToList();
            string article;

            do article = GenerateArticle();
            while (existing.Contains(article));

            return article;
        }

        private static string GenerateArticle()
        {
            Random random = new();

            char first = (char)random.Next('A', 'Z' + 1);
            int digits = random.Next(0, 1000);
            char second = (char)random.Next('A', 'Z' + 1);
            int last = random.Next(0, 10);

            return $"{first}{digits:D3}{second}{last}";
        }
    }
}
