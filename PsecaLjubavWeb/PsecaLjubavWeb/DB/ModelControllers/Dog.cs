using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PsecaLjubavWeb.DB.Models
{
    public class Dog
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public DateTime BirthDate { get; set; }
        public string Sex { get; set; }
        public string Race { get; set; }

        public string GetDateAsReadableString()
        {
            //TODO: parse birthdate
            return "3 meseca";
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
}
