using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize(Roles = "Admin")]

public class AddRoleClaimModel : RolePageModel
{
    public AddRoleClaimModel(RoleManager<IdentityRole> roleManager, AppDbContext context) : base(roleManager, context)
    {
    }
    public class InputModel
    {
        [Required(ErrorMessage = "Không được bỏ trống")]
        [Display(Name = "Kiểu Claim")]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "{0} phải dài từ {2} đến {1} ký tự.")]
        public string ClaimType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Không được bỏ trống")]
        [Display(Name = "Giá trị")]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "{0} phải dài từ {2} đến {1} ký tự.")]
        public string ClaimValue { get; set; } = string.Empty;
    }
    [BindProperty]
    public InputModel Input { set; get; } = new InputModel();

    public IdentityRole role { get; set; } = new IdentityRole();
    public async Task<IActionResult> OnGetAsync(string roleid)
    {
        role = await _roleManager.FindByIdAsync(roleid);
        if (role == null) return NotFound($"Không tìm thấy role với ID: {roleid}");
        return Page();
    }
    public async Task<IActionResult> OnPostAsync(string roleid)
    {
        role = await _roleManager.FindByIdAsync(roleid);
        if (role == null) return NotFound($"Không tìm thấy role với ID: {roleid}");

        if ((await _roleManager.GetClaimsAsync(role))
            .Any(c => c.Type == Input.ClaimType && c.Value == Input.ClaimValue))
        {
            ModelState.AddModelError(string.Empty, "Claim này đã có trong Role");
            return Page();
        }

        if (ModelState.IsValid)
        {
            var newClaim = new Claim(Input.ClaimType, Input.ClaimValue);
            var result = await _roleManager.AddClaimAsync(role, newClaim);
            if (result.Succeeded)
            {
                StatusMessage = $"Vừa tạo mới Claim cho Role {role.Name}";
                return RedirectToPage("./Edit", new {roleid = role.Id});
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
