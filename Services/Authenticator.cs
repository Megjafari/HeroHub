using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using HeroHub.Helpers;
using HeroHub.Models;

namespace HeroHub.Services
{
    public static class Authenticator      //klass för att hantera registrering och inloggning av användare
    {
        private static User? currentHero;        //statisk variabel för att lagra den aktuella inloggade användaren

        public static void Register()
        {
            
            Console.WriteLine("\n---Register New Hero---");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Enter Hero Name: ");
            
            Console.ResetColor();
            string username = Console.ReadLine()!;
            
            string password;
            do
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("---Enter Password--- (min 6 chars, 1 uppercase, 1 digit, 1 special char): ");
                Console.ResetColor();
                password = MenuHelper.ReadPassword();

                if (!PasswordValid(password))
                {
                    Console.WriteLine("Invalid Password!");
                }
            }while (!PasswordValid(password));


            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("---Enter Phone Number for 2FA---: ");
            Console.ResetColor();
            string email = Console.ReadLine()!;

            currentHero = new User      //skapa en ny användare med angivna uppgifter
            {
                username = username,
                password = password,
                email = email
            };
            Console.Clear();
            Console.WriteLine($"Hero '{username}' registered successfully!\n");
        }
        private static bool PasswordValid(string password)
        {
            return password.Length >= 6 &&
                   password.Any(char.IsDigit) &&
                   password.Any(char.IsUpper) &&
                   password.Any(ch => !char.IsLetterOrDigit(ch));
        }

        public static User Login()
        {
            Console.WriteLine("\n---Hero Login---");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("Enter Hero Name: ");
            Console.ResetColor();
            string username = Console.ReadLine()!;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("Enter Password: ");
            Console.ResetColor();
            string password = Console.ReadLine()!;

            currentHero = new User      //skapa en ny användare med angivna uppgifter
            {
                username = username,
                password = password,
                phonenumber = "0736690901"
            };

            Random random = new Random();       //skapa en slumpmässig verifieringskod
            string verificationCode = random.Next(1000, 9999).ToString();   //generera en fyrsiffrig kod

            TwoFactorService.SendVerificationCode(currentHero.phonenumber, verificationCode);   //skicka verifieringskoden via SMS
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("Enter the verification code sent to your phone: ");
            Console.ResetColor();
            string inputCode = Console.ReadLine()!;

            if (inputCode == verificationCode)      //kontrollera om den angivna koden är korrekt
            {
                Console.WriteLine("Two-factor authentication successful!");
            }
            else
            {
                Console.WriteLine("Invalid verification code. Login failed.");
                return null!;
            }


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
