using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

    public List<CartItem> Items { get; private set; } = [];
    public decimal Total { get; private set; }

    public void OnGet()
    {
        LoadCart();
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
}