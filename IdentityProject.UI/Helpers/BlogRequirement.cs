using IdentityProject.UI.Models.Dto.Blog;
using Microsoft.AspNetCore.Authorization;

namespace IdentityProject.UI.Helpers
{
    public class BlogRequirement : IAuthorizationRequirement
    {
    }

    public class IsBlogForUserAuthorization : AuthorizationHandler<BlogRequirement, BlogDto>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, BlogRequirement requirement, BlogDto resource)
        {
            if (context.User.Identity?.Name == resource.UserName)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
