using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoeStore.Core;
using ShoeStore.Web.Model;

namespace ShoeStore.Web.Pages
{
    public class OrdersModel : PageModel
    {
        public AccessRights AccessRights { get; private set; }
        public int UserId { get; private set; }

        public int TotalOrders { get; private set; }
        public int CompletedOrders { get; private set; }

        public List<Order> Orders { get; private set; } = new();

        public void OnGet(AccessRights accessRights, int userId)
        {
            ShoeStore2Context context = new();

            AccessRights = accessRights;
            UserId = userId;

            Orders = context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductArticleNavigation)
                .Include(o => o.PickUpPoint)
                .OrderBy(o => o.Status)
                .ThenByDescending(o => o.DateOrder)
                .ToList();

            TotalOrders = Orders.Count;
            CompletedOrders = Orders.Count(o => o.Status == "Завершен");
        }
    }
}
