using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin")]
public class IndexModel : RolePageModel
{
    public IndexModel(RoleManager<IdentityRole> roleManager, AppDbContext context) : base(roleManager, context)
    {
    }
    public class RoleModel : IdentityRole
    {
        public List<string> Claims { get; set; } = new List<string>();
    }
    public List<RoleModel> rolesModel { get; set; } = new List<RoleModel>();
    public async Task OnGet()
    {
        var roles = await _roleManager.Roles.OrderBy(x => x.Name).ToListAsync();
        foreach (var role in roles)
        {
            var claims = await _roleManager.GetClaimsAsync(role);
            var claimString = claims.Select(c =>c.Type + "=" + c.Value);
            var rm = new RoleModel()
            {
                Name = role.Name,
                Id = role.Id,
                Claims = claimString.ToList()
            };

            rolesModel.Add(rm);
        }
    }
    public void OnPost() => RedirectToPage();
}

