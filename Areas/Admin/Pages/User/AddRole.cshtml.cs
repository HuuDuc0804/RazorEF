// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Admin.User
{
    public class AddRoleModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AddRoleModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public AppUser user { set; get; } = new AppUser();

        [BindProperty]
        [Display(Name = "Các role gán cho user")]
        public IList<string> RoleNames { set; get; }
        public SelectList allRoles { get; set; }
        public async Task<IActionResult> OnGetAsync(string id)
        {

            user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Không thể load dữ liệu với thành viên ID =  '{id}'.");
            }
            RoleNames = await _userManager.GetRolesAsync(user);

            var roleNames = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
            allRoles = new SelectList(roleNames);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Không tìm thấy thành viên với ID '{id}.");
            }
            // RoleNames
            var oldRoleNames = await _userManager.GetRolesAsync(user);
            var deleteRoles = oldRoleNames.Where(r => !RoleNames.Contains(r));
            var addRoles = RoleNames.Where(r => !oldRoleNames.Contains(r));
            // Nạp lại danh sách tất cả các Role
            var roleNames = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
            allRoles = new SelectList(roleNames);
            var resultDelete = await _userManager.RemoveFromRolesAsync(user, deleteRoles);
            if (!resultDelete.Succeeded)
            {
                foreach (var error in resultDelete.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            var resultAddRole = await _userManager.AddToRolesAsync(user, addRoles);
            if (!resultAddRole.Succeeded)
            {
                foreach (var error in resultAddRole.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            StatusMessage = $"Đã cập nhật Role của thành viên {user.UserName} thành công.";

            return RedirectToPage("./Index");
        }
    }
}
