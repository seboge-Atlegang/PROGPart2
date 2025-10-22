using System.ComponentModel.DataAnnotations;


namespace CMCS.Web.ViewModels
{
    public class RegisterVm
    {
        [Required, EmailAddress] public string Email { get; set; }
        [Required] public string FullName { get; set; }
        [Required, DataType(DataType.Password)] public string Password { get; set; }
        [Required, DataType(DataType.Password), Compare("Password")] public string ConfirmPassword { get; set; }
    }
}



