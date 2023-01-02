using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

 [Authorize(Roles = "Admin")]
public class IndexModel : RolePageModel
{
    public IndexModel(RoleManager<IdentityRole> roleManager, ArticleContext context) : base(roleManager, context)
    {
    }
    public List<IdentityRole>? Roles { get; set; }
    public async Task OnGet()
    {
        Roles = await _roleManager.Roles.OrderBy(x=>x.Name).ToListAsync();
    }
    public void OnPost() => RedirectToPage();
}

