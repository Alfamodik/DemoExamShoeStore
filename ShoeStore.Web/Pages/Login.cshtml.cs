using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoeStore.Core;
using ShoeStore.Web.Model;

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
        ShoeStore2Context context = new();

        User? user = context.Users
            .FirstOrDefault(user => user.Login == Login && user.Password == Password);

        if (user == null)
        {
            ErrorMessage = "Неверный логин или пароль";
            return Page();
        }

        if (user.Role == "Менеджер")
            return RedirectToPage("/SearchCatalog", new { accessRights = AccessRights.Manager, userId = user.Id });

        if (user.Role == "Администратор")
            return RedirectToPage("/SearchCatalog", new { accessRights = AccessRights.Admin, userId = user.Id });

        return RedirectToPage("/Catalog", new { accessRights = AccessRights.User, userId = user.Id });
    }

    public IActionResult OnPostGuest()
    {
        return RedirectToPage("/Catalog", new { accessRights = AccessRights.Guest });
    }
}
