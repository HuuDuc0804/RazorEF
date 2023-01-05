using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize(Roles = "Admin")]

public class CreateModel : RolePageModel
{
    public CreateModel(RoleManager<IdentityRole> roleManager, AppDbContext context) : base(roleManager, context)
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
    public void OnGet()
    {
    }
    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            var newRole = new IdentityRole(Input.Name);
            var result = await _roleManager.CreateAsync(newRole);
            if (result.Succeeded)
            {
                StatusMessage = $"Bạn vừa tạo Role mới: {Input.Name}";
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
        return Page();
    }
}
