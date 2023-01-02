using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

 [Authorize(Roles = "Admin")]

public class EditModel : RolePageModel
{
    public EditModel(RoleManager<IdentityRole> roleManager, ArticleContext context) : base(roleManager, context)
    {
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Không được bỏ trống")]
        [Display(Name = "Tên của Role")]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "{0} phải dài từ {2} đến {1} ký tự.")]
        public string Name { get; set; } = string.Empty;
    }
    [BindProperty]
    public InputModel Input { set; get; } = new InputModel();
    public async Task<IActionResult> OnGet(string roleid)
    {
        var role = await _roleManager.FindByIdAsync(roleid);
        if (role != null)
        {
            Input.Name = role.Name;
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string roleid)
    {
        var role = await _roleManager.FindByIdAsync(roleid);
        if (ModelState.IsValid)
        {
            if (role != null)
            {
                role.Name = Input.Name;
                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    StatusMessage = $"Bạn vừa cập nhật Role: {role.Name}";
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
