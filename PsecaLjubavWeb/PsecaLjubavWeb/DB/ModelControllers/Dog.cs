using Microsoft.AspNetCore.Http;
using Neo4jClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsecaLjubavWeb.DB.Models
{
    public class Dog
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public DateTime BirthDate { get; set; }
        public string Sex { get; set; }
        public string Race { get; set; }
        public bool UpForAdoption { get; set; }
        public string AdopterName { get; set; }

        public string GetDateAsReadableString()
        {
            //TODO: parse birthdate
            DateTime current = DateTime.Now;
            int months = (current.Year - BirthDate.Year) * 12 + current.Month - BirthDate.Month + (current.Day >= BirthDate.Day ? 0 : -1);
            int years = months / 12;
            months -= years * 12;
            StringBuilder strBuilder = new StringBuilder();
            if (years != 0)
            {
                strBuilder.Append($"{years} g.");
                /*if (years > 1 && years < 5)
                {
                    strBuilder.Append($"{years} godine");
                } else
                {
                    strBuilder.Append($"{years} godina");
                }   */        
                if (months != 0)
                {
                    strBuilder.Append($" i ");
                }
                
            }
            if (years > 0 || months != 0)
            {
                if (years == 0 && months == 0)
                {
                    return "beba";
                }
                else if (years != 0)
                {
                    strBuilder.Append($"{months} m.");
                }
                else if (months == 1)
                {
                    strBuilder.Append($"{months} mesec");
                }
                else if (months > 1 && months < 5)
                {
                    strBuilder.Append($"{months} meseca");
                }
                else
                {
                    strBuilder.Append($"{months} meseci");
                }
            }
            return strBuilder.ToString();
        }
        public string GetSexAsReadableString()
        {
            if (Sex == "Male")
            {
                return "Mužjak";
            } else if (Sex == "Female")
            {
                return "Ženka";
            }
            return "Nepoznat pol";
        }
    }

    public class DogController : BaseController
    {
        private const string uploadsFolder = "uploads";
        private readonly string fullUploadsPath;

        public DogController(Database db, string webRootPath) : base(db)
        {
            fullUploadsPath = Path.Combine(webRootPath, uploadsFolder);
        }

        public async Task<Dog> EditDog(User owner, string name, string race, string birthDate, string sex, bool adoption, string adopter, IFormFile image, string id, string prevImage)
        {
            //Create object
            Dog newDog = new Dog();
            newDog.Name = name;
            newDog.Race = race;
            newDog.BirthDate = DateTime.Parse(birthDate);
            newDog.Sex = sex;
            newDog.UpForAdoption = adoption;
            newDog.AdopterName = adopter;
            //Save to database
            if (id == (-1).ToString())
            {
                newDog.ID = Guid.NewGuid().ToString();
            } else
            {
                newDog.ID = id;
            }

            if (image != null)
            {
                string extension = Path.GetExtension(image.FileName);
                string imageFileName = newDog.ID + extension;
                var file = Path.Combine(fullUploadsPath, imageFileName);
                using (var fileStream = new FileStream(file, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }
                newDog.Image = imageFileName;
            } else
            {
                newDog.Image = prevImage;
            }            
            graphClient.Cypher
                        .Match("(user: User)")
                        .Where((User user) => user.Username == owner.Username)
                        .Merge("(user)-[:LOVES]->(dog:Dog {ID:{dogID}})")
                        .Set("dog = {newDog}")
                        .WithParams( new
                        {
                            dogID = newDog.ID,
                            newDog
                        })
                        .ExecuteWithoutResults();

            return newDog;
        }

        public List<Dog> GetDogs(User owner)
        {
            var results = graphClient.Cypher
                        .Match("(user:User)-[:LOVES]->(dog:Dog)")
                        .Where((User user) => user.Username == owner.Username)
                        .Return(dog => dog.As<Dog>())
                        .Results;
            return results.ToList<Dog>();
        }
        public Dog GetDog(User owner, string dogID)
        {
            var results = graphClient.Cypher
                        .Match("(user:User)-[:LOVES]->(dog:Dog)")
                        .Where((User user) => user.Username == owner.Username)
                        .AndWhere((Dog dog) => dog.ID == dogID)
                        .Return(dog => dog.As<Dog>())
                        .Results;
            return results.ToList<Dog>().First();
        }

        public List<Dog> GetOtherPeoplesDogs(User owner, Dog filter)
        {
            var results = graphClient.Cypher
                        .Match("(user:User)-[:LOVES]->(dog:Dog)")
                        .Where((User user, Dog dog) => user.Username != owner.Username && dog.UpForAdoption == filter.UpForAdoption)
                        .Return(dog => dog.As<Dog>())
                        .Results;
            return results.ToList<Dog>();
        }

        public void WantToAdopt(User newOwner, string dogID)
        {
            Dog adoptee = new Dog();
            adoptee.ID = dogID;
            adoptee.UpForAdoption = false;
            adoptee.AdopterName = newOwner.Username;
            graphClient.Cypher
                        .Match("(dog: Dog)")
                        .Where((Dog dog) => dog.ID == adoptee.ID && dog.UpForAdoption == true)
                        .Set("dog.UpForAdoption = {upForAdoption}, dog.AdopterName = {adopterName}")
                        .WithParams(new
                        {
                            upForAdoption = adoptee.UpForAdoption,
                            adopterName = adoptee.AdopterName
                        })
            .ExecuteWithoutResults();
        }

        public void ConfirmAdoption(User oldOwner, Dog adoptee, bool confirm)
        {
            if (confirm)
            {
                graphClient.Cypher
                    .Match("(owner: User)-[rel:LOVES]->(dog: Dog)", "(newOwner: User)")
                    .Where((User owner) => owner.Username == oldOwner.Username)
                    .AndWhere((User newOwner) => newOwner.Username == adoptee.AdopterName)
                    .AndWhere((Dog dog) => dog.ID == adoptee.ID && dog.AdopterName == adoptee.AdopterName)
                    .Merge("(newOwner)-[newRel:LOVES]->(dog)")
                    .Set("dog.AdopterName = {adopterName}")
                    .Delete("rel")
                    .WithParam("adopterName", "")
                    .ExecuteWithoutResults();
            } else
            {
                bool adoption = true;
                string adopter = "";
                graphClient.Cypher
                    .Match("(owner: User)-[rel:LOVES]->(dog: Dog)")
                    .Where((User owner) => owner.Username == oldOwner.Username)
                    .AndWhere((Dog dog) => dog.ID == adoptee.ID)
                    .Set("dog.UpForAdoption = {upForAdoption}, dog.AdopterName = {emptyAdopter}")
                    .WithParams(new
                    {
                        upForAdoption = adoption,
                        emptyAdopter = adopter
                    })
                    .ExecuteWithoutResults();
            }
        }
    }
}
