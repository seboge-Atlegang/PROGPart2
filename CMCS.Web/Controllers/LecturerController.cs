using Microsoft.AspNetCore.Mvc;
using CMCS.Web.Data;
using CMCS.Web.Models;
using CMCS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMCS.Web.Data;
using CMCS.Web.Models;
using CMCS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMCS.Web.Controllers;


[Authorize]
public class LecturerController : Controller
{
    private readonly InMemoryStore _store;
    public LecturerController(InMemoryStore store) => _store = store;


    [Authorize(Roles = Roles.Lecturer + "," + Roles.Admin)]
    public IActionResult Profile()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var user = _store.Users.FirstOrDefault(u => u.Id == userId);
        return View(user);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Lecturer + "," + Roles.Admin)]
    public IActionResult Profile(EditProfileVm vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var user = _store.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null) return NotFound();
        user.FullName = vm.FullName;
        user.Password = user.Password; // keep same
        TempData["Success"] = "Profile updated";
        return View(user);
    }

    // Admin-only list of lecturers
    [Authorize(Roles = Roles.Admin)]
    public IActionResult Index()
    {
        var list = _store.Users.Where(u => u.Role == Roles.Lecturer).ToList();
        return View(list);
    }

    [Authorize(Roles = Roles.Admin)]
    public IActionResult Details(string id)
    {
        var user = _store.Users.FirstOrDefault(u => u.Id == id);
        if (user == null) return NotFound();
        return View(user);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpGet]
    public IActionResult AssignRole(string id)
    {
        var user = _store.Users.FirstOrDefault(u => u.Id == id);
        if (user == null) return NotFound();
        var vm = new AssignRoleVm { UserId = id, Email = user.UserName, Roles = new List<string> { user.Role } };
        return View(vm);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    public IActionResult AssignRole(AssignRoleVm vm)
    {
        var user = _store.Users.FirstOrDefault(u => u.Id == vm.UserId);
        if (user == null) return NotFound();
        // pick the first selected role
        if (vm.Roles != null && vm.Roles.Any()) user.Role = vm.Roles.First();
        TempData["Success"] = "Roles updated";
        return RedirectToAction(nameof(Details), new { id = vm.UserId });
    }
}

