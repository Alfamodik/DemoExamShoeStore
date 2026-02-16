using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoeStore.Core;
using ShoeStore.Core.Model;

namespace ShoeStore.Web.Pages;

public class LoginModel : PageModel
{
    [BindProperty]
    public string Login { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public IActionResult OnPostLogin()
    {
        User? user = ShoeStoreContext.Instance.Users
            .FirstOrDefault(user => user.Login == Login && user.Password == Password);

        if (user == null)
        {
            ErrorMessage = "Неверный логин или пароль";
            return Page();
        }

        if (user.Role == "Менеджер")
            return RedirectToPage("/SearchCatalog", new { accessRights = AccessRights.Manager });

        if (user.Role == "Администратор")
            return RedirectToPage("/SearchCatalog", new { accessRights = AccessRights.Admin });

        return RedirectToPage("/Catalog", new { accessRights = AccessRights.User });
    }

    public IActionResult OnPostGuest()
    {
        return RedirectToPage("/Catalog", new { accessRights = AccessRights.Guest });
    }
}
