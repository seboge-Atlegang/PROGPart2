using Microsoft.AspNetCore.Mvc;
using CMCS.Web.Models;
using CMCS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CMCS.Web.Data;
using CMCS.Web.Models;
using CMCS.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace CMCS.Web.Controllers;


public class AccountController : Controller
{
    private readonly InMemoryStore _store;
    public AccountController(InMemoryStore store) => _store = store;


    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        return View(new LoginVm { ReturnUrl = returnUrl });
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVm vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var user = _store.Users.FirstOrDefault(u => string.Equals(u.UserName, vm.Email, StringComparison.OrdinalIgnoreCase) && u.Password == vm.Password);
        if (user == null)
        {
            ModelState.AddModelError("", "Invalid credentials");
            return View(vm);
        }

        var claims = new List<Claim>
{
new Claim(ClaimTypes.Name, user.UserName),
new Claim(ClaimTypes.NameIdentifier, user.Id),
new Claim(ClaimTypes.Role, user.Role),
new Claim("FullName", user.FullName ?? string.Empty)
};
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);


        if (!string.IsNullOrEmpty(vm.ReturnUrl) && Url.IsLocalUrl(vm.ReturnUrl)) return LocalRedirect(vm.ReturnUrl);

        // role based landing page (you asked separate landing pages)
        return user.Role switch
        {
            Roles.Lecturer => RedirectToAction("Create", "Claims"),
            Roles.Coordinator => RedirectToAction("Pending", "Claims"),
            Roles.Manager => RedirectToAction("Pending", "Claims"),
            Roles.Admin => RedirectToAction("Index", "Admin"),
            _ => RedirectToAction("Index", "Home")
        };
    }



    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }


    public IActionResult AccessDenied() => View();
}
