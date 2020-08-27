using PsecaLjubavWeb.DB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PsecaLjubavWeb.Pages.ViewModels
{
    public class DogViewModel : DB.Models.Dog
    {
        public string AdopterEmail { get; set; }

        public DogViewModel(Dog dog)
        {
            this.ID = dog.ID;
            this.Name = dog.Name;
            this.Image = dog.Image;
            this.BirthDate = dog.BirthDate;
            this.Sex = dog.Sex;
            this.Race = dog.Race;
            this.UpForAdoption = dog.UpForAdoption;
            this.AdopterName = dog.AdopterName;
        }
    }
}
