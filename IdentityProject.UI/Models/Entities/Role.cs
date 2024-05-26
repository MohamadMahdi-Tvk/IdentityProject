using Microsoft.AspNetCore.Identity;

namespace IdentityProject.UI.Models.Entities
{
    public class Role : IdentityRole
    {
        public string Description { get; set; }
    }
}
