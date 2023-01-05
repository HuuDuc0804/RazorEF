using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Security.Requirements
{
    public class AppAuthorizationHander : IAuthorizationHandler
    {
        private readonly ILogger<AppAuthorizationHander> _logger;
        private readonly UserManager<AppUser> _userManager;

        public AppAuthorizationHander(ILogger<AppAuthorizationHander> logger, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            var requirements = context.PendingRequirements.ToList();
            foreach (var requirement in requirements)
            {
                if (requirement is GenZRequirement)
                {
                    if (IsGenZ(context.User, (GenZRequirement)requirement))
                    {
                        context.Succeed(requirement);
                    }
                }
                if (requirement is ArticleUpdateRequirement)
                {
                    if (CanUpdateArticle(context.User, context.Resource, (ArticleUpdateRequirement)requirement))
                    {
                        context.Succeed(requirement);
                    }
                }
            }
            return Task.CompletedTask;
        }

        private bool CanUpdateArticle(ClaimsPrincipal user, object? resource, ArticleUpdateRequirement requirement)
        {
            if (user.IsInRole("Admin")) return true;
            var article = resource as Article;
            var dateCreate = article == null ? DateTime.MinValue : article.Created;
            var dateCanUpdate = new DateTime(requirement.Year, requirement.Month, requirement.Day);
            if (dateCreate <= dateCanUpdate)
            {
                _logger.LogInformation("Quá ngày được phép cập nhật");
                return false;
            }
            return true;
        }

        private bool IsGenZ(ClaimsPrincipal user, GenZRequirement requirement)
        {
            var appUserTask = _userManager.GetUserAsync(user);
            Task.WaitAll(appUserTask);
            var appUser = appUserTask.Result;

            if (appUser.BirthDate == null)
            {
                _logger.LogInformation($"{appUser.UserName} chưa xác lập ngày sinh");
                return false;
            }
            int year = appUser.BirthDate.Value.Year;

            var success = (year >= requirement.FromYear && year <= requirement.ToYear);
            if (success)
            {
                _logger.LogInformation($"{appUser.UserName} có năm sinh {year}: Thỏa mãn");
            }
            else
            {
                _logger.LogInformation($"{appUser.UserName} có năm sinh {year}: Không thỏa mãn");
            }
            return success;
        }
    }
}