using CMCS.Web.Models;

namespace CMCS.Web.ViewModels
{
    public class AdminDashboardViewModel
    {
        public string UserName { get; set; }
        public int TotalUsers { get; set; }
        public int TotalClaims { get; set; }
        public int PendingClaims { get; set; }
        public List<ClaimRecord> RecentClaims { get; set; } = new List<ClaimRecord>();
    }
}

