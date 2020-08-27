using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using PsecaLjubavWeb.DB;
using PsecaLjubavWeb.DB.Models;

namespace PsecaLjubavWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly Database db;
        public IndexModel(Database db)
        {
            this.db = db;
        }

        public bool HasErrorMessage { get; set; } = false;
        [BindProperty]
        public string Username { get; set; }
        [BindProperty, DataType(DataType.Password)]
        public string Password { get; set; }
        [BindProperty]
        public string Email { get; set; }
        public string Message { get; set; }
        public async Task<IActionResult> OnPostLogin()
        {
            //var user = configuration.GetSection("SiteUser").Get<SiteUser>();
            UserController userController = new UserController(db);
            LoginResult lResult = userController.LoginUser(Username, Password);
            if (lResult.GetStatus() == LoginResult.LoginStatus.LoginSuccess) { 
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, Username)
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return RedirectToPage("/User/Home");
            } else
            {
                HasErrorMessage = true;
                Message = lResult.GetStatus().ToString();
            }
            //Message = "Invalid attempt";
            return Page();
        }
        public async Task<IActionResult> OnPostRegister()
        {
            //var user = configuration.GetSection("SiteUser").Get<SiteUser>();
            /*UserController userController = configuration.GetSection("UserController").Get<UserController>();
            LoginResult lResult = userController.LoginUser(UserName, Password);*/
            UserController userController = new UserController(db);
            RegistrationResult rResult = userController.RegisterUser(Username, Password, Email);
            if (rResult.GetStatus() == RegistrationResult.RegistrationStatus.RegistrationSuccess)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, Username)
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return RedirectToPage("/User/Home");
            } else
            {
                HasErrorMessage = true;
                Message = rResult.GetStatus().ToString();
            }
            //Message = "Invalid attempt";
            return Page();
        }
    }
}
