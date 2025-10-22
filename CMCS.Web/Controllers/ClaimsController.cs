using CMCS.Web.Data;
using CMCS.Web.Models;
using CMCS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CMCS.Web.Controllers;

[Authorize]
public class ClaimsController : Controller
{
    private readonly InMemoryStore _store;
    private readonly IWebHostEnvironment _env;
    private readonly long _maxFileBytes = 10 * 1024 * 1024;
    private readonly string[] _allowedExt = new[] { ".pdf", ".docx", ".xlsx", ".doc" };

    public ClaimsController(InMemoryStore store, IWebHostEnvironment env)
    {
        _store = store;
        _env = env;
    }

    [Authorize(Roles = Roles.Lecturer + "," + Roles.Admin)]
    public IActionResult Create() => View(new ClaimCreateVm());

    [HttpPost]
    [Authorize(Roles = Roles.Lecturer + "," + Roles.Admin)]
    public async Task<IActionResult> Create(ClaimCreateVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.Identity?.Name ?? "unknown";

            var id = _store.NextClaimId();
            var claim = new ClaimRecord
            {
                Id = id,
                LecturerId = userId ?? "",
                LecturerName = userName,
                HoursWorked = vm.HoursWorked,
                HourlyRate = vm.HourlyRate,
                Amount = Math.Round(vm.HoursWorked * vm.HourlyRate, 2),
                Notes = vm.Notes,
                Status = ClaimStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            // Handle file upload
            if (vm.File != null && vm.File.Length > 0)
            {
                if (vm.File.Length > _maxFileBytes)
                {
                    ModelState.AddModelError("File", "File size must be less than 10MB.");
                    return View(vm);
                }

                var ext = Path.GetExtension(vm.File.FileName).ToLowerInvariant();
                if (!_allowedExt.Contains(ext))
                {
                    ModelState.AddModelError("File", "Only PDF, DOC, DOCX, and XLSX files are allowed.");
                    return View(vm);
                }

                // Create uploads directory if it doesn't exist
                var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsPath, fileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await vm.File.CopyToAsync(stream);
                }

                var doc = new ClaimDocument
                {
                    Id = _store.NextDocId(),
                    ClaimId = claim.Id,
                    FileName = vm.File.FileName,
                    FilePath = $"/uploads/{fileName}",
                    UploadedAt = DateTime.UtcNow
                };

                _store.Documents.Add(doc);
                claim.DocumentId = doc.Id;
            }

            // Save the claim
            _store.Claims.Add(claim);

            TempData["Success"] = "Claim submitted successfully!";
            return RedirectToAction(nameof(MyClaims));
        }
        catch (Exception ex)
        {
            // Log the exception here
            ModelState.AddModelError("", "An error occurred while submitting your claim. Please try again.");
            return View(vm);
        }
    }

    [Authorize(Roles = Roles.Lecturer + "," + Roles.Admin)]
    public IActionResult MyClaims()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            TempData["Error"] = "User not found.";
            return View(new MyClaimsViewModel());
        }

        var claims = _store.Claims
            .Where(c => c.LecturerId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToList();

        // Get all documents for these claims and create a dictionary for easy lookup
        var claimIds = claims.Select(c => c.Id).ToList();
        var documents = _store.Documents
            .Where(d => claimIds.Contains(d.ClaimId))
            .ToDictionary(d => d.ClaimId, d => d);

        var viewModel = new MyClaimsViewModel
        {
            Claims = claims,
            Documents = documents
        };

        return View(viewModel);
    }

    [Authorize(Roles = Roles.Coordinator + "," + Roles.Manager + "," + Roles.Admin)]
    public IActionResult Pending()
    {
        var pendingClaims = _store.Claims
            .Where(c => c.Status == ClaimStatus.Pending)
            .OrderBy(c => c.CreatedAt)
            .ToList();

        return View(pendingClaims);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Coordinator + "," + Roles.Manager + "," + Roles.Admin)]
    public IActionResult Approve(int id)
    {
        var claim = _store.Claims.FirstOrDefault(x => x.Id == id);
        if (claim == null)
        {
            TempData["Error"] = "Claim not found.";
            return RedirectToAction(nameof(Pending));
        }

        claim.Status = ClaimStatus.Approved;
        TempData["Success"] = $"Claim {id} approved successfully.";
        return RedirectToAction(nameof(Pending));
    }

    [HttpPost]
    [Authorize(Roles = Roles.Coordinator + "," + Roles.Manager + "," + Roles.Admin)]
    public IActionResult Reject(int id, string reason = null)
    {
        var claim = _store.Claims.FirstOrDefault(x => x.Id == id);
        if (claim == null)
        {
            TempData["Error"] = "Claim not found.";
            return RedirectToAction(nameof(Pending));
        }

        claim.Status = ClaimStatus.Rejected;
        if (!string.IsNullOrWhiteSpace(reason))
        {
            claim.Notes = $"Rejection reason: {reason}";
        }

        TempData["Success"] = $"Claim {id} rejected.";
        return RedirectToAction(nameof(Pending));
    }

    public IActionResult Details(int id)
    {
        var claim = _store.Claims.FirstOrDefault(x => x.Id == id);
        if (claim == null) return NotFound();

        var document = claim.DocumentId.HasValue
            ? _store.Documents.FirstOrDefault(d => d.Id == claim.DocumentId.Value)
            : null;

        ViewBag.Document = document;
        return View(claim);
    }
}