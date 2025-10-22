using CMCS.Web.Models;

namespace CMCS.Web.ViewModels
{
    public class CoordinatorDashboardViewModel
    {
        public string UserName { get; set; }
        public string UserRole { get; set; }
        public List<ClaimRecord> PendingClaims { get; set; } = new List<ClaimRecord>();
    }
}
