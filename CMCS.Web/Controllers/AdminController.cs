using Microsoft.AspNetCore.Mvc;
using CMCS.Web.Data;
using CMCS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using CMCS.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMCS.Web.ViewModels;
using CMCS.Web.Data;
using CMCS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMCS.Web.Controllers;


[Authorize(Roles = Roles.Admin)]
public class AdminController : Controller
{
    private readonly InMemoryStore _store;
    public AdminController(InMemoryStore store) => _store = store;


    public IActionResult Index()
    {
        var vm = new AdminDashboardViewModel
        {
            TotalUsers = _store.Users.Count,
            TotalClaims = _store.Claims.Count,
            PendingClaims = _store.Claims.Count(c => c.Status == ClaimStatus.Pending)
        };
        return View(vm);
    }

    public IActionResult Users() => View(_store.Users);


    public IActionResult UserDetails(string id)
    {
        var u = _store.Users.FirstOrDefault(x => x.Id == id);
        if (u == null) return NotFound();
        return View(u);
    }
}

