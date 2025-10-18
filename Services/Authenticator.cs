using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using HeroHub.Models;

namespace HeroHub.Services
{
    public class Authenticator      //klass för att hantera registrering och inloggning av användare
    {
        private static User currentHero;        //statisk variabel för att lagra den aktuella inloggade användaren

        public static void Register()
        {
            Console.WriteLine("\n=== Register New Hero ===");
            Console.Write("Enter Hero Name: ");
            string username = Console.ReadLine()!;
            Console.Write("Enter Password: ");
            string password = Console.ReadLine()!;
            Console.Write("Enter Email: ");
            string email = Console.ReadLine()!;

            currentHero = new User      //skapa en ny användare med angivna uppgifter
            {
                username = username,
                password = password,
                email = email
            };

            Console.WriteLine($"Hero '{username}' registered successfully!\n");
        }

        public static User Login()
        {
            Console.WriteLine("\n=== Hero Login ===");
            Console.Write("Enter Hero Name: ");
            string username = Console.ReadLine()!;
            Console.Write("Enter Password: ");
            string password = Console.ReadLine()!;

            currentHero = new User      //skapa en ny användare med angivna uppgifter
            {
                username = username,
                password = password
            };

            Console.WriteLine($"Hero '{username}' logged in successfully!\n");
            Console.WriteLine("Welcome back");
            return currentHero;
        }

        public static User GetCurrentHero()     //metod för att hämta den aktuella inloggade användaren
        {
            return currentHero;     //returnerar den aktuella användaren
        }
    }
}
