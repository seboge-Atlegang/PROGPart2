using CMCS.Web.Models;


// Add this to your ViewModels folder
namespace CMCS.Web.ViewModels
{
    public class MyClaimsViewModel
    {
        public List<ClaimRecord> Claims { get; set; }
        public Dictionary<int, ClaimDocument> Documents { get; set; }
    }
}