using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoeStore.Core;
using ShoeStore.Core.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ShoeStore.Web.Pages
{
    public class OrderEditModel : PageModel
    {
        public string? ErrorMessage { get; private set; }

        [BindProperty(SupportsGet = true)]
        public AccessRights AccessRights { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? Id { get; set; }

        [BindProperty]
        public int? OrderId { get; set; }

        [BindProperty]
        public string SelectedProductArticle { get; set; }

        [BindProperty]
        public int Amount { get; set; }

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

        public void OnGet(string? accessRights, int? id = null)
        {
            if (Enum.TryParse(accessRights, true, out AccessRights parsedAccess))
                AccessRights = parsedAccess;

            AccessRights = parsedAccess;
            Id = id;
            LoadLists();

            if (id.HasValue)
            {
                var order = ShoeStoreContext.Instance.Orders
                    .Include(o => o.ProductArticleNavigation)
                    .Include(o => o.PickUpPoint)
                    .Include(o => o.User)
                    .FirstOrDefault(o => o.Id == id.Value);

                if (order != null)
                {
                    OrderId = order.Id;
                    SelectedProductArticle = order.ProductArticle;
                    Amount = order.Amount ?? 0;
                    DateOrder = order.DateOrder.HasValue ? order.DateOrder.Value.ToDateTime(TimeOnly.MinValue) : null;
                    DateDelivery = order.DateDelivery.HasValue ? order.DateDelivery.Value.ToDateTime(TimeOnly.MinValue) : null;
                    SelectedPickUpPointId = order.PickUpPointId;
                    SelectedUserId = order.UserId;
                    Code = order.Code;
                    Status = order.Status ?? "Новый";
                }
            }
        }

        private void LoadLists()
        {
            var products = ShoeStoreContext.Instance.Products
                .Select(p => new { p.Article, Display = p.DisplayName ?? p.Article })
                .ToList();

            var pickUps = ShoeStoreContext.Instance.PickUpPoints
                .Select(p => new { p.Id, p.Address })
                .ToList();

            var users = ShoeStoreContext.Instance.Users
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

            Order order;

            if (OrderId.HasValue)
            {
                order = ShoeStoreContext.Instance.Orders.Find(OrderId.Value) ?? new Order();
            }
            else
            {
                order = new Order();
                ShoeStoreContext.Instance.Orders.Add(order);
            }

            order.ProductArticle = SelectedProductArticle;
            order.Amount = Amount;
            order.DateOrder = DateOnly.FromDateTime(DateOrder.Value);
            order.DateDelivery = DateDelivery.HasValue ? DateOnly.FromDateTime(DateDelivery.Value) : null;
            order.PickUpPointId = SelectedPickUpPointId.Value;
            order.UserId = SelectedUserId.Value;
            order.Code = Code;
            order.Status = Status;

            ShoeStoreContext.Instance.SaveChanges();

            return RedirectToPage("/Orders", new { accessRights = AccessRights });
        }

        public IActionResult OnPostDelete()
        {
            if (!OrderId.HasValue)
                return RedirectToPage("/Orders", new { accessRights = AccessRights });

            var order = ShoeStoreContext.Instance.Orders.Find(OrderId.Value);
            if (order != null)
            {
                ShoeStoreContext.Instance.Orders.Remove(order);
                ShoeStoreContext.Instance.SaveChanges();
            }

            return RedirectToPage("/Orders", new { accessRights = AccessRights });
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(SelectedProductArticle))
                return SetError("Выберите товар.");

            if (Amount <= 0)
                return SetError("Введите количество больше 0.");

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
