using CMCS.Web.Models;

namespace CMCS.Web.ViewModels
{
    public class LecturerDashboardViewModel
    {
        public string UserName { get; set; }
        public List<ClaimRecord> MyClaims { get; set; } = new List<ClaimRecord>();
        public int PendingClaimsCount { get; set; }
        public int ApprovedClaimsCount { get; set; }
        public decimal TotalEarned => MyClaims.Where(c => c.Status == ClaimStatus.Approved).Sum(c => c.Amount);
    }
}
