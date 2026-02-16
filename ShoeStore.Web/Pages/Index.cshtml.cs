using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ShoeStore.Web.Pages;

public class IndexModel : PageModel
{
    public IActionResult OnGet() => RedirectToPage("/Login");
}
