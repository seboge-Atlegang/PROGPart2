namespace CMCS.Web.Models
{
    public class AppUser
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserName { get; set; } = string.Empty; // email-like
        public string Password { get; set; } = string.Empty; // plain text (only for demo)
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = Roles.Lecturer;
    }
}
