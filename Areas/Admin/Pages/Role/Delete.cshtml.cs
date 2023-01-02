using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

 [Authorize(Roles = "Admin")]

public class DeleteModel : RolePageModel
{
    public DeleteModel(RoleManager<IdentityRole> roleManager, ArticleContext context) : base(roleManager, context)
    {
    }
    public IdentityRole role { get; set; } = new IdentityRole();
    public async Task<IActionResult> OnGet(string roleid)
    {
        role = await _roleManager.FindByIdAsync(roleid);
        if (role != null)
        {
            return Page();
        }
        return NotFound("Không tìm thấy role");
    }
    public async Task<IActionResult> OnPostAsync(string roleid)
    {
        role = await _roleManager.FindByIdAsync(roleid);
        if (ModelState.IsValid)
        {
            if (role != null)
            {
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    StatusMessage = $"Bạn vừa xóa Role: {role.Name}";
                    return RedirectToPage("./Index");
                }
                else
                {
                    result.Errors.ToList().ForEach(erorr =>
                    {
                        ModelState.AddModelError(string.Empty, erorr.Description);
                    });
                }
            }
        }
        return Page();
    }
}
