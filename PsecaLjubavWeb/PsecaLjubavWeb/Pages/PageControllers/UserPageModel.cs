using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PsecaLjubavWeb.DB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace PsecaLjubavWeb.Pages.PageControllers
{
    public class UserPageModel : PageModel
    {
        protected DB.Database db;

        public UserPageModel(DB.Database db)
        {
            this.db = db;
        } 

        protected DB.Models.User GetCurrentUser(HttpContext httpContext)
        {
            var claims = ((ClaimsIdentity)httpContext.User.Identity).Claims;
            string username = claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            if (!string.IsNullOrEmpty(username))
            {
                UserController userController = new UserController(db);
                UserResult uResult = userController.GetUser(username);
                if (uResult.GetStatus() == UserResult.UserResultStatus.FOUND)
                {
                    return uResult.GetUser();
                }
            }
            return null;
        }

        public async Task<ActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync();
            return RedirectToPage("/Index");
        }
    }
}
