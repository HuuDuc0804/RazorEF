using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Admin.User
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;

        public IndexModel(UserManager<AppUser> userManager)
        {
            this._userManager = userManager;
        }
        [TempData]
        public string? StatusMessage { get; set; }

        public class UserAndRoles : AppUser
        {
            public string? RoleNames { get; set; }
        }
        public List<UserAndRoles>? Users { get; set; }

        public const int ITEMS_PER_PAGE = 10;

        [BindProperty(SupportsGet = true, Name = "p")]
        public int CurrentPage { get; set; }

        [BindProperty]
        public int CountPages { get; set; }

        public int totalUser { get; set; }
        public async Task OnGet()
        {
            var query = _userManager.Users.OrderBy(x => x.UserName);
            totalUser = await query.CountAsync();
            CountPages = (int)Math.Ceiling((double)totalUser / (double)ITEMS_PER_PAGE);
            if (CurrentPage < 1)
                CurrentPage = 1;
            if (CurrentPage > CountPages)
                CurrentPage = CountPages;

            Users = await query.Skip((CurrentPage - 1) * ITEMS_PER_PAGE)
                         .Take(ITEMS_PER_PAGE)
                         .Select(u => new UserAndRoles()
                         {
                             Id = u.Id,
                             UserName = u.UserName,
                         })
                         .ToListAsync();
            foreach (var user in Users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.RoleNames = string.Join(", ", roles.OrderBy(r => r));
            }
        }
        public void OnPost() => RedirectToPage();
    }
}
