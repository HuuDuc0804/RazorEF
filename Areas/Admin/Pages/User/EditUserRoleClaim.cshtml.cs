using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Admin.User
{
    public class EditUserRoleClaimModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public EditUserRoleClaimModel(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

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

        public AppUser user { get; set; } = new AppUser();
        public IdentityUserClaim<string>? userClaim { get; set; }

        public void OnGet() => NotFound("Không được truy cập");
        public async Task<IActionResult> OnGetAddClaimAsync(string userid)
        {
            user = await _userManager.FindByIdAsync(userid);
            if (user == null) return NotFound("Không tìm thấy User");
            return Page();
        }
        public async Task<IActionResult> OnPostAddClaimAsync(string userid)
        {
            user = await _userManager.FindByIdAsync(userid);
            if (user == null) return NotFound("Không tìm thấy User");

            if (ModelState.IsValid)
            {
                var claims = _context.UserClaims.Where(c => c.UserId == user.Id);
                if (claims.Any(c => c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue))
                {
                    ModelState.AddModelError(string.Empty, "Claim này đã có");
                }
                await _userManager.AddClaimAsync(user, new Claim(Input.ClaimType, Input.ClaimValue));
                StatusMessage = $"Đã thêm thuộc tính {Input.ClaimType} cho User: {user.UserName}";
                return RedirectToPage("./AddRole", new { id = userid });
            }
            return Page();
        }

        public async Task<IActionResult> OnGetEditClaimAsync(int claimid)
        {
            userClaim = _context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (userClaim == null) return NotFound("Không tìm thấy Claim");

            user = await _userManager.FindByIdAsync(userClaim.UserId);
            if (user == null) return NotFound("Không tìm thấy User");

            Input = new InputModel()
            {
                ClaimType = userClaim.ClaimType,
                ClaimValue = userClaim.ClaimValue,
            };
            return Page();
        }
        public async Task<IActionResult> OnPostEditClaimAsync(int claimid)
        {
            userClaim = _context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (userClaim == null) return NotFound("Không tìm thấy Claim");

            user = await _userManager.FindByIdAsync(userClaim.UserId);
            if (user == null) return NotFound("Không tìm thấy User");

            if (ModelState.IsValid)
            {
                if (_context.UserClaims.Any(c => c.UserId == user.Id
                                         && c.ClaimType == Input.ClaimType
                                         && c.ClaimValue == Input.ClaimValue
                                         && c.Id != userClaim.Id))
                {
                    ModelState.AddModelError(string.Empty, "Claim này đã có");
                    return Page();
                }
                userClaim.ClaimType = Input.ClaimType;
                userClaim.ClaimValue = Input.ClaimValue;
                _context.UserClaims.Update(userClaim);
                await _context.SaveChangesAsync();
                StatusMessage = $"Đã cập nhật thuộc tính {Input.ClaimType} cho User: {user.UserName}";
                return RedirectToPage("./AddRole", new { id = user.Id });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int claimid)
        {
            userClaim = _context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (userClaim == null) return NotFound("Không tìm thấy Claim");

            user = await _userManager.FindByIdAsync(userClaim.UserId);
            if (user == null) return NotFound("Không tìm thấy User");

            await _userManager.RemoveClaimAsync(user, new Claim(userClaim.ClaimType, userClaim.ClaimValue));

            StatusMessage = $"Đã xóa thuộc tính {Input.ClaimType} cho User: {user.UserName}";
            return RedirectToPage("./AddRole", new { Id = user.Id });
        }
    }
}
