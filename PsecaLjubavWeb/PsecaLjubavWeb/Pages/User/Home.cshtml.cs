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
        public List<DB.Models.Dog> Dogs;

        private DB.Models.User user;
        public HomeModel(DB.Database database) : base(database)
        {
            FillDogList();
        }

        public void OnGet()
        {
            this.user = GetCurrentUser(HttpContext);
            this.Username = user.Username;
        }

        private void FillDogList()
        {
            Dogs = new List<DB.Models.Dog>();

            DB.Models.Dog apa = new DB.Models.Dog();
            apa.Name = "Apa";
            apa.Image = "apa.jpg";
            apa.Sex = "Female";
            apa.Race = "Mešanac";
            apa.BirthDate = DateTime.Parse("12.6.2020");
            DB.Models.Dog pega = new DB.Models.Dog();
            pega.Name = "Pega";
            pega.Image = "pega.jpg";
            pega.Sex = "Male";
            pega.Race = "Mešanac";
            pega.BirthDate = DateTime.Parse("12.6.2020");

            Dogs.Add(apa);
            Dogs.Add(pega);
        }
    }
}