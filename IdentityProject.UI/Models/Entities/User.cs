using Microsoft.AspNetCore.Identity;

namespace IdentityProject.UI.Models.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<Blog> Blogs { get; set; }
    }
}
