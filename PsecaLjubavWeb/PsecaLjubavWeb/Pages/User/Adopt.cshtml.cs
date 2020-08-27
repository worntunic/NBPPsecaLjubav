using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PsecaLjubavWeb.DB.Models;
using PsecaLjubavWeb.Pages.PageControllers;

namespace PsecaLjubavWeb.Pages.User
{
    public class AdoptModel : UserPageModel
    {
        public List<DB.Models.Dog> Dogs;
        public DogController dogController;
        [BindProperty]
        public string AdoptDogID { get; set; }

        public AdoptModel(DB.Database db, IHostingEnvironment environment) : base(db)
        {
            dogController = new DogController(db, environment.WebRootPath);
        }

        public void OnGet()
        {
            DB.Models.User owner = GetCurrentUser(HttpContext);

            Dogs = dogController.GetOtherPeoplesDogs(owner, GetFilter());
        }

        private Dog GetFilter()
        {
            Dog dog = new Dog();
            dog.UpForAdoption = true;
            return dog;
        }

        public ActionResult OnPostAdoptDog()
        {
            DB.Models.User newOwner = GetCurrentUser(HttpContext);
            dogController.WantToAdopt(newOwner, AdoptDogID);
            return new RedirectToPageResult("/User/Home");
        }
    }
}