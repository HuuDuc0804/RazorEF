using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class RolePageModel : PageModel
{
    protected readonly RoleManager<IdentityRole> _roleManager;
    protected readonly ArticleContext _context;

    public RolePageModel(RoleManager<IdentityRole> roleManager, ArticleContext context)
    {
        _roleManager = roleManager;
        _context = context;
    }

    [TempData]
    public string? StatusMessage { get; set; }
}