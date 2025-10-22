using System.ComponentModel.DataAnnotations;


namespace CMCS.Web.ViewModels
{
    public class EditProfileVm
    {
        [Required] public string FullName { get; set; }
        [Phone] public string PhoneNumber { get; set; }
    }
}


