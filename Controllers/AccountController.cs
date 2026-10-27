using CampusConnect.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CampusConnect.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _users;
    private readonly SignInManager<ApplicationUser> _signIn;
    public AccountController(UserManager<ApplicationUser> users, SignInManager<ApplicationUser> signIn)
    {
        _users = users; _signIn = signIn;
    }

    [HttpGet]
    public IActionResult Register() => User.Identity?.IsAuthenticated == true ? RedirectToAction("Index", "Home") : View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FullName = model.FullName };
        var result = await _users.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            await _users.AddToRoleAsync(user, "Student");
            await _signIn.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }
        foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
        return View(model);
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        if (!ModelState.IsValid) return View(model);
        var result = await _signIn.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            HttpContext.Session.SetString("LastLoginUtc", DateTime.UtcNow.ToString("O"));
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }
        ModelState.AddModelError(string.Empty, "Invalid login attempt. Check your email and password.");
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signIn.SignOutAsync();
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied() => View();
}

public class RegisterViewModel
{
    [Required, StringLength(100, MinimumLength = 2)]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required, MinLength(8), DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    [Required, DataType(DataType.Password), Compare(nameof(Password))]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class LoginViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }
}
