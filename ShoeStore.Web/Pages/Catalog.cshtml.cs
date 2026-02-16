using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoeStore.Core;
using ShoeStore.Core.Model;

namespace ShoeStore.Web.Pages
{
    public class CatalogModel : PageModel
    {
        public List<Product>? Products { get; private set; }

        public AccessRights? AccessRights { get; set; }
        public int? UserId { get; set; }

        public void OnGet(string? accessRights)
        {
            if (Enum.TryParse(accessRights, true, out AccessRights parsedAccess))
                AccessRights = parsedAccess;

            Products = [.. ShoeStoreContext.Instance.Products
                .Include(p => p.ProductCategory)
                .Include(p => p.Manufacturer)
                .Include(p => p.Supplier)
                .OrderByDescending(p => p.AmountInStorage)];
        }

        public IActionResult OnPostLogout()
        {
            return RedirectToPage("/Login");
        }
    }
}
