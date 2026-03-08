using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoeStore.Core;
using ShoeStore.Web.Model;

namespace ShoeStore.Web.Pages
{
    public class OrderEditModel : PageModel
    {
        public string? ErrorMessage { get; private set; }

        [BindProperty(SupportsGet = true)]
        public AccessRights AccessRights { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? Id { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? UserId { get; set; }

        [BindProperty]
        public int? OrderId { get; set; }

        //[BindProperty]
        //public string SelectedProductArticle { get; set; }

        //[BindProperty]
        //public int Amount { get; set; }

        [BindProperty]
        public DateTime? DateOrder { get; set; }

        [BindProperty]
        public DateTime? DateDelivery { get; set; }

        [BindProperty]
        public int? SelectedPickUpPointId { get; set; }

        [BindProperty]
        public int? SelectedUserId { get; set; }

        [BindProperty]
        public string Code { get; set; }

        [BindProperty]
        public string Status { get; set; } = "Новый";

        public SelectList Products { get; private set; } = new SelectList(Enumerable.Empty<SelectListItem>());
        public SelectList PickUpPoints { get; private set; } = new SelectList(Enumerable.Empty<SelectListItem>());
        public SelectList Users { get; private set; } = new SelectList(Enumerable.Empty<SelectListItem>());

        public void OnGet(string? accessRights, int userId, int? id = null)
        {
            if (Enum.TryParse(accessRights, true, out AccessRights parsedAccess))
                AccessRights = parsedAccess;

            ShoeStore2Context context = new();

            AccessRights = parsedAccess;
            Id = id;
            UserId = userId;

            LoadLists();

            if (id.HasValue)
            {
                var order = context.Orders
                    //.Include(o => o.ProductArticleNavigation)
                    .Include(o => o.PickUpPoint)
                    .Include(o => o.User)
                    .FirstOrDefault(o => o.Id == id.Value);

                if (order != null)
                {
                    OrderId = order.Id;
                    //SelectedProductArticle = order.ProductArticle;
                    //Amount = order.Amount ?? 0;
                    DateOrder = order.DateOrder;
                    DateDelivery = order.DateDelivery;
                    SelectedPickUpPointId = order.PickUpPointId;
                    SelectedUserId = order.UserId;
                    Code = order.Code;
                    Status = order.Status ?? "Новый";
                }
            }
        }

        private void LoadLists()
        {
            ShoeStore2Context context = new();

            var products = context.Products
                .Select(p => new { p.Article, Display = p.DisplayName ?? p.Article })
                .ToList();

            var pickUps = context.PickUpPoints
                .Select(p => new { p.Id, p.Address })
                .ToList();

            var users = context.Users
                .Select(u => new { u.Id, u.Login })
                .ToList();

            Products = new SelectList(products, "Article", "Display");
            PickUpPoints = new SelectList(pickUps, "Id", "Address");
            Users = new SelectList(users, "Id", "Login");
        }

        public IActionResult OnPost()
        {
            if (!ValidateInputs())
                return Page();

            ShoeStore2Context context = new();

            Order order;

            if (OrderId.HasValue)
            {
                order = context.Orders.Find(OrderId.Value) ?? new Order();
            }
            else
            {
                order = new Order();
                context.Orders.Add(order);
            }

            //order.ProductArticle = SelectedProductArticle;
            //order.Amount = Amount;
            order.DateOrder = DateOrder.Value;
            order.DateDelivery = DateDelivery.HasValue ? DateDelivery.Value : null;
            order.PickUpPointId = SelectedPickUpPointId.Value;
            order.UserId = SelectedUserId.Value;
            order.Code = Code;
            order.Status = Status;

            context.SaveChanges();

            return RedirectToPage("/Orders", new { accessRights = AccessRights, userId = UserId });
        }

        public IActionResult OnPostDelete()
        {
            if (!OrderId.HasValue)
                return RedirectToPage("/Orders", new { accessRights = AccessRights, userId = UserId });

            ShoeStore2Context context = new();

            var order = context.Orders.Find(OrderId.Value);
            if (order != null)
            {
                context.Orders.Remove(order);
                context.SaveChanges();
            }

            return RedirectToPage("/Orders", new { accessRights = AccessRights, userId = UserId });
        }

        private bool ValidateInputs()
        {
            //if (string.IsNullOrWhiteSpace(SelectedProductArticle))
                //return SetError("Выберите товар.");

            //if (Amount <= 0)
                //return SetError("Введите количество больше 0.");

            if (!DateOrder.HasValue)
                return SetError("Укажите дату заказа.");

            if (!SelectedPickUpPointId.HasValue)
                return SetError("Выберите пункт выдачи.");

            if (!SelectedUserId.HasValue)
                return SetError("Выберите пользователя.");

            if (string.IsNullOrWhiteSpace(Code) || Code.Length > 10)
                return SetError("Введите код получения не более 10 символов.");

            return true;
        }


        private bool SetError(string message)
        {
            ErrorMessage = message;
            LoadLists();
            return false;
        }
    }
}
