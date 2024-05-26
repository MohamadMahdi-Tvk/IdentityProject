using IdentityProject.UI.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace IdentityProject.UI.Helpers
{
    public class AddMyClaims : UserClaimsPrincipalFactory<User>
    {
        public AddMyClaims(UserManager<User> userManager, IOptions<IdentityOptions> options) : base(userManager, options)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            identity.AddClaim(new Claim("FullName", $"{user.FirstName} {user.LastName}"));

            return identity;
        }

    }

    public class AddClaim : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal != null)
            {
                var identity = principal.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    identity.AddClaim(new Claim("TestClaim", "Yes", ClaimValueTypes.String));

                    identity.AddClaim(new Claim("Credit", "10000", ClaimValueTypes.String));
                }
            }
            return Task.FromResult(principal);
        }
    }
}
