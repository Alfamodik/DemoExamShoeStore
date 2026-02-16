using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoeStore.Core;
using ShoeStore.Core.Model;

namespace ShoeStore.Web.Pages
{
    public class SearchCatalogModel : PageModel
    {
        public List<Product> Products { get; private set; } = new();
        public List<string> Suppliers { get; private set; } = new();

        public AccessRights? AccessRights { get; set; }
        public string SelectedSupplier { get; set; } = string.Empty;
        public string SortType { get; set; } = "desc";
        public string SearchText { get; set; } = string.Empty;

        public void OnGet(string? selectedSupplier, string? sortType, string? searchText, string? accessRights)
        {
            if (Enum.TryParse(accessRights, true, out AccessRights parsedAccess))
                AccessRights = parsedAccess;

            SelectedSupplier = selectedSupplier ?? string.Empty;
            SortType = string.IsNullOrWhiteSpace(sortType) ? "desc" : sortType;
            SearchText = searchText ?? string.Empty;

            using ShoeStoreContext context = new();

            Suppliers = context.Suppliers
                .Select(s => s.Supplier1!)
                .ToList();

            IQueryable<Product> products = context.Products
                .Include(p => p.ProductCategory)
                .Include(p => p.Manufacturer)
                .Include(p => p.Supplier);

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string lower = SearchText.ToLower();
                products = products.Where(p =>
                    (p.Product1 != null && p.Product1.ToLower().Contains(lower)) ||
                    (p.Description != null && p.Description.ToLower().Contains(lower)) ||
                    (p.Manufacturer != null && p.Manufacturer.Manufacturer1 != null && p.Manufacturer.Manufacturer1.ToLower().Contains(lower)) ||
                    (p.Supplier != null && p.Supplier.Supplier1 != null && p.Supplier.Supplier1.ToLower().Contains(lower))
                );
            }

            if (!string.IsNullOrWhiteSpace(SelectedSupplier))
            {
                products = products.Where(p => p.Supplier != null && p.Supplier.Supplier1 == SelectedSupplier);
            }

            products = SortType == "asc"
                ? products.OrderBy(p => p.AmountInStorage)
                : products.OrderByDescending(p => p.AmountInStorage);

            Products = products.ToList();
        }
    }
}
