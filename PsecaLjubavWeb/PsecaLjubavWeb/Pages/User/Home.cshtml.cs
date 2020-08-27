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
using PsecaLjubavWeb.Pages.ViewModels;

namespace PsecaLjubavWeb.Pages.User
{
    public class HomeModel : UserPageModel
    {
        public string Username { get; set; }
        public List<ViewModels.DogViewModel> Dogs;
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
        public string PopupDogAdopter { get; set; }
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
            List<Dog> regDogs = new List<DB.Models.Dog>();
            DB.Models.User owner = GetCurrentUser(HttpContext);
            regDogs = dogController.GetDogs(owner);
            UserController userController = new UserController(db);
            Dogs = new List<ViewModels.DogViewModel>();
            foreach (Dog dog in regDogs)
            {
                DogViewModel viewDog = new DogViewModel(dog);
                if (!string.IsNullOrEmpty(dog.AdopterName))
                {
                    UserResult uResult = userController.GetUser(dog.AdopterName);
                    if (uResult.GetStatus() == UserResult.UserResultStatus.FOUND)
                    {
                        viewDog.AdopterEmail = uResult.GetUser().Email;
                    }
                }
                Dogs.Add(viewDog);
            }
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
                PopupDogAdopter,
                PopupDogImage,
                PopupDogID,
                PopupDogPrevImageName);
            return new RedirectToPageResult("/User/Home");
        }
        public ActionResult OnPostAdoptDog()
        {
            DB.Models.User owner = GetCurrentUser(HttpContext);
            Dog dog = dogController.GetDog(owner, AdoptionDogID);
            dogController.ConfirmAdoption(owner, dog, AdoptionConfirmed);
            return new RedirectToPageResult("/User/Home");
        }
    }
}