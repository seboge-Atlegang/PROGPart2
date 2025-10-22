using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CMCS.Web.Models;
using CMCS.Web.ViewModels;
using CMCS.Web.Data;

namespace CMCS.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly InMemoryStore _store;

        public HomeController(InMemoryStore store)
        {
            _store = store;
        }

        [Authorize]
        public IActionResult Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.Identity?.Name ?? "User";

            if (User.IsInRole(Roles.Admin))
            {
                // Admin Dashboard ViewModel
                var adminVm = new AdminDashboardViewModel
                {
                    UserName = userName,
                    TotalUsers = _store.Users.Count,
                    TotalClaims = _store.Claims.Count,
                    PendingClaims = _store.Claims.Count(c => c.Status == ClaimStatus.Pending),
                    RecentClaims = _store.Claims.OrderByDescending(c => c.CreatedAt).Take(5).ToList()
                };
                return View("AdminDashboard", adminVm);
            }
            else if (User.IsInRole(Roles.Lecturer))
            {
                // Lecturer Dashboard ViewModel
                var lecturerVm = new LecturerDashboardViewModel
                {
                    UserName = userName,
                    MyClaims = _store.Claims.Where(c => c.LecturerId == userId).ToList(),
                    PendingClaimsCount = _store.Claims.Count(c => c.LecturerId == userId && c.Status == ClaimStatus.Pending),
                    ApprovedClaimsCount = _store.Claims.Count(c => c.LecturerId == userId && c.Status == ClaimStatus.Approved)
                };
                return View("LecturerDashboard", lecturerVm);
            }
            else if (User.IsInRole(Roles.Coordinator) || User.IsInRole(Roles.Manager))
            {
                // Coordinator/Manager Dashboard ViewModel
                var coordinatorVm = new CoordinatorDashboardViewModel
                {
                    UserName = userName,
                    PendingClaims = _store.Claims.Where(c => c.Status == ClaimStatus.Pending).ToList(),
                    UserRole = User.IsInRole(Roles.Coordinator) ? "Coordinator" : "Manager"
                };
                return View("CoordinatorDashboard", coordinatorVm);
            }

            // Default view if no specific role
            return View();
        }
    }
}