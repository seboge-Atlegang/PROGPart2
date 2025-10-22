
using Microsoft.AspNetCore.Identity;


namespace CMCS.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string Password { get; internal set; }
        public string Role { get; internal set; }
    }
}