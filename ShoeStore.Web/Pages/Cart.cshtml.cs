using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoeStore.Core;
using ShoeStore.Web.Model;

namespace ShoeStore.Web.Pages;

public class CartModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string AccessRights { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public int UserId { get; set; }

    [BindProperty]
    public int? SelectedPickUpPointId { get; set; }

    public SelectList PickUpPoints { get; private set; } =
        new SelectList(Enumerable.Empty<SelectListItem>());
    
    [BindProperty]
    public DateTime? DateDelivery { get; set; }
    public string? ErrorMessage { get; private set; }
    public List<CartItem> Items { get; private set; } = [];
    public decimal Total { get; private set; }

    public void OnGet()
    {
        LoadCart();
        LoadPickUpPoints();
    }

    public IActionResult OnPostAdd(string productArticle, int amount = 1)
    {
        if (string.IsNullOrWhiteSpace(productArticle))
        {
            return BadRequest();
        }

        if (amount < 1)
        {
            amount = 1;
        }

        ShoeStore2Context context = new();

        Product? product = context.Products.FirstOrDefault(p => p.Article == productArticle);

        CartItem? existingCartItem = context.CartItems
            .FirstOrDefault(cartItem => cartItem.UserId == UserId && cartItem.ProductArticle == productArticle);

        if (existingCartItem == null)
        {
            CartItem cartItem = new CartItem
            {
                UserId = UserId,
                ProductArticle = productArticle,
                Amount = amount
            };

            context.CartItems.Add(cartItem);
        }
        else
        {
            existingCartItem.Amount += amount;
            
            if (product != null && product.AmountInStorage < existingCartItem.Amount)
                existingCartItem.Amount = product.AmountInStorage.Value;
        }

        context.SaveChanges();

        return RedirectToCart();
    }

    public IActionResult OnPostDec(string productArticle)
    {
        if (string.IsNullOrWhiteSpace(productArticle))
        {
            return BadRequest();
        }

        ShoeStore2Context context = new();

        CartItem? existingCartItem = context.CartItems
            .FirstOrDefault(cartItem => cartItem.UserId == UserId && cartItem.ProductArticle == productArticle);

        if (existingCartItem == null)
        {
            return RedirectToCart();
        }

        if (existingCartItem.Amount > 1)
        {
            existingCartItem.Amount--;
        }
        else
        {
            context.CartItems.Remove(existingCartItem);
        }

        context.SaveChanges();

        return RedirectToCart();
    }

    public IActionResult OnPostRemove(string productArticle)
    {
        if (string.IsNullOrWhiteSpace(productArticle))
        {
            return BadRequest();
        }

        ShoeStore2Context context = new();

        CartItem? existingCartItem = context.CartItems
            .FirstOrDefault(cartItem => cartItem.UserId == UserId && cartItem.ProductArticle == productArticle);

        if (existingCartItem != null)
        {
            context.CartItems.Remove(existingCartItem);
            context.SaveChanges();
        }

        return RedirectToCart();
    }

    public IActionResult OnPostClear()
    {
        ShoeStore2Context context = new();

        List<CartItem> cartItems = context.CartItems
            .Where(cartItem => cartItem.UserId == UserId)
            .ToList();

        if (cartItems.Count > 0)
        {
            context.CartItems.RemoveRange(cartItems);
            context.SaveChanges();
        }

        return RedirectToCart();
    }

    public IActionResult OnPostBuy()
    {
        ShoeStore2Context context = new();

        List<CartItem> cartItems = context.CartItems
            .Where(cartItem => cartItem.UserId == UserId)
            .Include(ci => ci.ProductArticleNavigation)
            .ToList();

        if (cartItems.Count == 0)
        {
            ErrorMessage = "Корзина пуста.";
            LoadCart();
            LoadPickUpPoints();
            return Page();
        }

        if (!SelectedPickUpPointId.HasValue)
        {
            ErrorMessage = "Выберите пункт выдачи.";
            LoadCart();
            LoadPickUpPoints();
            return Page();
        }

        if (!DateDelivery.HasValue)
        {
            ErrorMessage = "Укажите дату получения.";
            LoadCart();
            LoadPickUpPoints();
            return Page();
        }

        if (DateDelivery.Value.Date < DateTime.Today.AddDays(1))
        {
            ErrorMessage = "Дата получения должна быть не раньше завтрашнего дня.";
            LoadCart();
            LoadPickUpPoints();
            return Page();
        }

        Order order = new Order
        {
            UserId = UserId,
            Status = "Новый",
            DateOrder = DateTime.Now,
            DateDelivery = DateDelivery,
            Code = new Random().Next(900, 999).ToString(),
            PickUpPointId = SelectedPickUpPointId.Value
        };

        context.Orders.Add(order);
        context.SaveChanges();

        foreach (CartItem cartItem in cartItems)
        {
            OrderItem orderItem = new OrderItem() 
            {
                OrderId = order.Id,
                ProductArticle = cartItem.ProductArticle,
                Amount = cartItem.Amount,
                Price = cartItem.ProductArticleNavigation.Cost!.Value,
                Discount = cartItem.ProductArticleNavigation.Discount!.Value
            };

            context.OrderItems.Add(orderItem);
        }

        context.CartItems.RemoveRange(cartItems);
        context.SaveChanges();

        return RedirectToCart();
    }

    private void LoadCart()
    {
        ShoeStore2Context context = new();

        Items = context.CartItems
            .Include(cartItem => cartItem.ProductArticleNavigation)
            .Where(cartItem => cartItem.UserId == UserId)
            .ToList();

        Total = Items.Sum(cartItem =>
            cartItem.ProductArticleNavigation.CostWithDiscountValue * cartItem.Amount);
    }

    private RedirectToPageResult RedirectToCart()
    {
        return RedirectToPage("/Cart", new { accessRights = AccessRights, userId = UserId });
    }

    private void LoadPickUpPoints()
    {
        ShoeStore2Context context = new();

        List<object> pickUpPoints = context.PickUpPoints
            .Select(pickUpPoint => new
            {
                pickUpPoint.Id,
                pickUpPoint.Address
            })
            .Cast<object>()
            .ToList();

        PickUpPoints = new SelectList(pickUpPoints, "Id", "Address");
    }
}