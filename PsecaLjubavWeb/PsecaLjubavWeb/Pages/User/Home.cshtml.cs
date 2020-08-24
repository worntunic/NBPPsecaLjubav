using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PsecaLjubavWeb.Pages.PageControllers;

namespace PsecaLjubavWeb.Pages.User
{
    public class HomeModel : UserPageModel
    {
        public string Username { get; set; }

        private DB.Models.User user;

        public HomeModel(DB.Database database) : base(database)
        {
            
        }

        public void OnGet()
        {
            this.user = GetCurrentUser(HttpContext);
            this.Username = user.Username;
        }
    }
}