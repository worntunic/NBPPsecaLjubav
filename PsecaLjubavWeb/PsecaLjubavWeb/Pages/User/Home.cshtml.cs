using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PsecaLjubavWeb.DB.Models;
using PsecaLjubavWeb.Pages.PageControllers;

namespace PsecaLjubavWeb.Pages.User
{
    public class HomeModel : UserPageModel
    {
        public string Username { get; set; }
        public List<DB.Models.Dog> Dogs;
        private IHostingEnvironment environment;
        private DogController dogController;

        [BindProperty]
        public string PopupDogName { get; set; }
        [BindProperty]
        public string PopupDogRace { get; set; }
        [BindProperty]
        public string PopupDogBirthDate { get; set; }
        [BindProperty]
        public string PopupDogSex { get; set; }
        [BindProperty]
        public string PopupDogID { get; set; }
        [BindProperty]
        public bool PopupDogUpForAdoption { get; set; }
        [BindProperty]
        public IFormFile PopupDogImage { get; set; }
        [BindProperty]
        public bool PopupDogChangeImage { get; set; }
        [BindProperty]
        public string PopupDogPrevImageName { get; set; }
        //Adoption
        [BindProperty]
        public string AdoptionDogID { get; set; }
        [BindProperty]
        public bool AdoptionConfirmed { get; set; }

        private DB.Models.User user;

        public HomeModel(DB.Database database, IHostingEnvironment env) : base(database)
        {
            
            environment = env;
            dogController = new DogController(database, env.WebRootPath);
        }

        public void OnGetAsync()
        {
            this.user = GetCurrentUser(HttpContext);
            this.Username = user.Username;
            FillDogList();
        }

        private void FillDogList()
        {
            Dogs = new List<DB.Models.Dog>();
            DB.Models.User owner = GetCurrentUser(HttpContext);
            Dogs = dogController.GetDogs(owner);
            /*DB.Models.Dog apa = new DB.Models.Dog();
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
            Dogs.Add(pega);*/
        }
        public async Task<ActionResult> OnPostEditDogAsync()
        {
            DB.Models.User owner = GetCurrentUser(HttpContext);

            Dog dog = await dogController.EditDog(
                owner,
                PopupDogName,
                PopupDogRace,
                PopupDogBirthDate,
                PopupDogSex,
                PopupDogUpForAdoption,
                PopupDogImage,
                PopupDogID,
                PopupDogPrevImageName);
            return new RedirectToPageResult("/User/Home");
        }
        public async void OnPostAdoptDogAsync()
        {
            DB.Models.User owner = GetCurrentUser(HttpContext);
            /*Dog dog = new Dog();
            dog.*/
        }
    }
}