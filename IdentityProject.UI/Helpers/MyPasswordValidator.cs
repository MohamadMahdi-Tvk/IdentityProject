using IdentityProject.UI.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityProject.UI.Helpers
{
    public class MyPasswordValidator : IPasswordValidator<User>
    {
        List<string> CommonPassword = new List<string>()
        {
            "123456","654321","zxcV@34567","password","qwerty","123456789"
        };
        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string password)
        {
            if (CommonPassword.Contains(password))
            {
                var result = IdentityResult.Failed(new IdentityError
                {
                    Code = "CommonPassword",
                    Description = "پسورد شما قابل شناسایی توسط ربات های هکر است! لطفا یک پسورد قوی انتخاب کنید",
                });
                return Task.FromResult(result);
            }
            return Task.FromResult(IdentityResult.Success);
        }
    }
}
