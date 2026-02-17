using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoeStore.Core;
using ShoeStore.Core.Model;

namespace ShoeStore.Web.Pages
{
    public class OrdersModel : PageModel
    {
        public AccessRights AccessRights { get; private set; }
        public List<Order> Orders { get; private set; } = new();

        public void OnGet(AccessRights accessRights)
        {
            AccessRights = accessRights;

            Orders = ShoeStoreContext.Instance.Orders
                .Include(o => o.ProductArticleNavigation)
                .Include(o => o.PickUpPoint)
                .OrderByDescending(o => o.Status)
                .ThenByDescending(o => o.Id)
                .ToList();
        }
    }
}
