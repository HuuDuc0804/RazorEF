using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize(Roles = "Admin")]

public class EditRoleClaimModel : RolePageModel
{
    public EditRoleClaimModel(RoleManager<IdentityRole> roleManager, ArticleContext context) : base(roleManager, context)
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
    IdentityRoleClaim<string>? claim { get; set; }
    public async Task<IActionResult> OnGetAsync(int? claimid)
    {
        if (claimid == null) return NotFound("Không tìm thấy claim");
        claim = _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
        if (claim == null) return NotFound("Không tìm thấy claim");

        role = await _roleManager.FindByIdAsync(claim.RoleId);
        if (role == null) return NotFound($"Không tìm thấy role với ID: {claim.RoleId}");

        Input = new InputModel()
        {
            ClaimType = claim.ClaimType,
            ClaimValue = claim.ClaimValue,
        };

        return Page();
    }
    public async Task<IActionResult> OnPostAsync(int? claimid)
    {
        if (claimid == null) return NotFound("Không tìm thấy claim");
        claim = _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
        if (claim == null) return NotFound("Không tìm thấy claim");

        role = await _roleManager.FindByIdAsync(claim.RoleId);
        if (role == null) return NotFound($"Không tìm thấy role với ID: {claim.RoleId}");

        if (_context.RoleClaims.Any(c => c.RoleId == role.Id
                                 && c.ClaimType == Input.ClaimType
                                 && c.ClaimValue == Input.ClaimValue
                                 && c.Id != claim.Id))
        {
            ModelState.AddModelError(string.Empty, "Claim này đã có trong Role");
            return Page();
        }

        if (ModelState.IsValid)
        {
            claim.ClaimType = Input.ClaimType;
            claim.ClaimValue = Input.ClaimValue;
            _context.RoleClaims.Update(claim);
            await _context.SaveChangesAsync();

            StatusMessage = $"Vừa cập nhật Claim cho Role: {role.Name}";
            return RedirectToPage("./Edit", new { roleid = role.Id });
        }
        return Page();
    }
    public async Task<IActionResult> OnPostDeleteAsync(int? claimid)
    {
        if (claimid == null) return NotFound("Không tìm thấy claim");
        claim = _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
        if (claim == null) return NotFound("Không tìm thấy claim");

        role = await _roleManager.FindByIdAsync(claim.RoleId);
        if (role == null) return NotFound($"Không tìm thấy role với ID: {claim.RoleId}");

        var result = await _roleManager.RemoveClaimAsync(role, new Claim(claim.ClaimType, claim.ClaimValue));

        if (result.Succeeded)
        {
            StatusMessage = $"Bạn vừa xóa Claim: {claim.ClaimType}";
            return RedirectToPage("./Edit", new { roleid = role.Id });
        }
        else
        {
            result.Errors.ToList().ForEach(erorr =>
            {
                ModelState.AddModelError(string.Empty, erorr.Description);
            });
            return Page();
        }
    }
}
