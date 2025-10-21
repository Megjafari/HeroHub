using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeroHub.Models
{
    public class User           //representerar en användare i systemet med login information och en lista av quests
    {
        public string username { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string? phonenumber { get; set; } // Made nullable to fix CS8618
        public List<Quest> quests { get; set; } = new List<Quest>();

    }
}
