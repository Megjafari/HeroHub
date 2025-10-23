using HeroHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeroHub.Services;



namespace HeroHub.Helpers
{
    public static class MenuHelper      // Hanterar menygränssnittet och användarinteraktioner
    {
        private static QuestManager _questManager = new QuestManager();     // Hanterar quest relaterade operationer  
        private static GuildAdvisorAI _aiAdvisor;       // AI tjänst för guild-rådgivning

        static MenuHelper() // Static constructor to initialize AI advisor
        {
            try
            {
                _aiAdvisor = new GuildAdvisorAI();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize Guild Advisor AI: {ex.Message}");
                _aiAdvisor = null;
            }
        }
        public static void StartApplication()   // Startar huvudapplikationen och hanterar huvudmenyn
        {
            ShowWelcomeScreen();        

            bool running = true;
            while (running)
            {
                try
                {
                    running = ShowMainMenu();
                }
                catch (Exception ex)
                {
                    ShowErrorMessage($"An error occurred: {ex.Message}");
                    PressAnyKeyToContinue();
                }
            }

            ShowGoodbyeScreen();
        }

        private static bool ShowMainMenu()  // Visar huvudmenyn och hanterar användarval
        {
            Console.Clear();
            Console.WriteLine("=======================================");
            Console.WriteLine("        QUEST GUILD TERMINAL");
            Console.WriteLine("=======================================");
            Console.WriteLine("1. Register Hero");
            Console.WriteLine("2. Login Hero");
            Console.WriteLine("3. Exit");
            Console.WriteLine("=======================================");
            Console.Write("Choose your path: ");

            var choice = Console.ReadLine();

            switch (choice) 
            {
                case "1":
                    Authenticator.Register();   
                    PressAnyKeyToContinue();
                    return true;
                case "2":
                    LoginHero();
                    return true;
                case "3":
                    return false;
                default:
                    ShowErrorMessage("Invalid choice. Please try again.");
                    PressAnyKeyToContinue();
                    return true;
            }
        }

        private static void ShowAIMenu()    // Visar AI-meny och hanterar AI-relaterade funktioner
        {
            if (_aiAdvisor == null)
            {
                ShowErrorMessage("AI features are disabled. Please check your API key configuration.");
                PressAnyKeyToContinue();
                return;
            }
            bool inAIMenu = true;
            while (inAIMenu)
            {
                Console.Clear();
                Console.WriteLine("=======================================");
                Console.WriteLine("        GUILD ADVISOR - AI ASSISTANT");
                Console.WriteLine("=======================================");
                Console.WriteLine("1. Generate Quest Description");
                Console.WriteLine("2. Suggest Quest Priority");
                Console.WriteLine("3. Summarize All Quests");
                Console.WriteLine("4. Back to Main Menu");
                Console.WriteLine("=======================================");
                Console.Write("Choose wisdom: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        GenerateQuestDescription();
                        break;
                    case "2":
                        SuggestQuestPriority();
                        break;
                    case "3":
                        SummarizeQuests();
                        break;
                    case "4":
                        inAIMenu = false;
                        break;
                    default:
                        ShowErrorMessage("Invalid choice.");
                        break;
                }

                if (choice != "4")
                {
                    PressAnyKeyToContinue();
                }
            }
        }

        private static void LoginHero()     // Hanterar inloggning av hjälte
        {
            var user = Authenticator.Login();
            if (user != null)
            {
                ShowSuccessMessage($"Welcome back, {user.username}!");
                PressAnyKeyToContinue();
                ShowHeroMenu();
            }
            else
            {
                ShowErrorMessage("Login failed. Please try again.");
                PressAnyKeyToContinue();
            }
        }

        private static void ShowHeroMenu()  // Visar hjältemeny och hanterar hjälte-relaterade funktioner
        {
            bool inHeroMenu = true;
            while (inHeroMenu)  
            {
                Console.Clear();
                Console.WriteLine($"=======================================");
                Console.WriteLine($"   WELCOME, {Authenticator.GetCurrentHero()?.username ?? "Hero"}!");
                Console.WriteLine("        GUILD HEADQUARTERS");
                Console.WriteLine("=======================================");
                Console.WriteLine("1. Add New Quest");
                Console.WriteLine("2. View All Quests");
                Console.WriteLine("3. Update/Complete Quest");
                Console.WriteLine("4. Request Guild Advisor Help (AI)");
                Console.WriteLine("5. Show Guild Report");
                Console.WriteLine("6. Logout");
                Console.WriteLine("=======================================");
                Console.Write("Choose your action: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddNewQuest();
                        break;
                    case "2":
                        _questManager.ShowAllQuests();
                        break;
                    case "3":
                        CompleteQuest();
                        break;
                    case "4":
                        ShowAIMenu();
                        break;
                    case "5":
                        _questManager.ShowReport();
                        break;
                    case "6":
                        inHeroMenu = false;
                        ShowSuccessMessage("Logged out successfully.");
                        break;
                    default:
                        ShowErrorMessage("Invalid choice.");
                        break;
                }

                if (choice != "6")
                {
                    PressAnyKeyToContinue();
                }
            }
        }

        // Quest Management Methods
        private static void AddNewQuest()   // Hanterar tillägg av nya quests
        {
            Console.Write("Enter quest title: ");
            var title = Console.ReadLine();

            Console.Write("Enter quest description (or press enter for AI generation): ");
            var description = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(description))
            {
                description = _aiAdvisor.GenerateQuestDescriptionAsync(title).Result;
                ShowInfoMessage($"AI-generated description: {description}");
            }

            Console.Write("Enter due date (yyyy-mm-dd): ");
            if (!DateTime.TryParse(Console.ReadLine(), out var dueDate))
            {
                ShowErrorMessage("Invalid date format.");
                return;
            }

            ShowQuestPriorityMenu();
            var priorityChoice = Console.ReadLine();
            var priority = GetPriorityText(int.Parse(priorityChoice ?? "2"));

            var quest = new Quest
            {
                Title = title,
                Description = description,
                DueDate = dueDate,
                Priority = priority,
                IsCompleted = false
            };

            _questManager.AddQuest(quest);
            ShowSuccessMessage("Quest added to your journal!");
        }

        private static void CompleteQuest()     // Hanterar slutförande av quests
        {
            Console.Write("Enter the title of the quest to complete: ");
            var title = Console.ReadLine();
            _questManager.CompleteQuest(title);
        }

        // AI Methods
        private static async void GenerateQuestDescription()        // Hanterar generering av quest-beskrivningar med AI
        {
            if (_aiAdvisor == null)
            {
                ShowErrorMessage("AI advisor is not available.");
                return;
            }
            Console.Write("Enter quest title for AI description: ");
            var title = Console.ReadLine();
            var description = await _aiAdvisor.GenerateQuestDescriptionAsync(title);
            ShowInfoMessage($"AI Guild Advisor says:\n{description}");
        }

        private static async void SuggestQuestPriority()        // Hanterar förslag på quest-prioritet med AI
        {
            if (_aiAdvisor == null)
            {
                ShowErrorMessage("AI advisor is not available.");
                return;
            }
            Console.Write("Enter quest title: ");
            var title = Console.ReadLine();

            Console.Write("Enter due date (yyyy-mm-dd): ");
            if (DateTime.TryParse(Console.ReadLine(), out var dueDate))
            {
                var suggestion = await _aiAdvisor.SuggestPriorityAsync(title, dueDate);
                ShowInfoMessage($"AI Priority Suggestion:\n{suggestion}");
            }
            else
            {
                ShowErrorMessage("Invalid date format.");
            }
        }

        private static async void SummarizeQuests()     // Hanterar sammanfattning av alla quests med AI
        {

            // Note: You might need to modify QuestManager to expose the quests list
            // For now, this will use AI without context
            var summary = await _aiAdvisor.SummarizeQuestsAsync(new List<Quest>());
            ShowInfoMessage($"AI Quest Summary:\n{summary}");
        }

        // UI Display Methods
        private static void ShowWelcomeScreen() // Visar välkomstskärmen
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=======================================");
            Console.WriteLine("      WELCOME TO QUEST GUILD");
            Console.WriteLine("           HEROHUB ");
            Console.WriteLine("=======================================");
            Console.WriteLine("Where legends begin their journey...");
            Console.WriteLine("=======================================");
            Console.ResetColor();
            PressAnyKeyToContinue();
        }

        private static void ShowGoodbyeScreen()     // Visar avskedsskärmen
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=======================================");
            Console.WriteLine("        FAREWELL, BRAVE HERO!");
            Console.WriteLine("   May your adventures continue...");
            Console.WriteLine("=======================================");
            Console.ResetColor();
        }

        // Helper Methods
        public static void ShowQuestPriorityMenu()      // Visar meny för att välja quest-prioritet
        {
            Console.WriteLine("\n Select Quest Priority:");
            Console.WriteLine("1. High ");
            Console.WriteLine("2. Medium ");
            Console.WriteLine("3. Low ");
            Console.Write("Choose priority: ");
        }

        public static string GetPriorityText(int priorityChoice)        // Returnerar textrepresentation av prioritet baserat på användarval
        {
            return priorityChoice switch
            {
                1 => "High ",
                2 => "Medium ",
                3 => "Low ",
                _ => "Medium "
            };
        }

        public static void PressAnyKeyToContinue()      // Väntar på användarens inmatning för att fortsätta
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        public static void ShowSuccessMessage(string message)       // Visar ett framgångsmeddelande
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($" {message}");
            Console.ResetColor();
        }

        public static void ShowErrorMessage(string message)     // Visar ett felmeddelande
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($" {message}");
            Console.ResetColor();
        }

        public static void ShowWarningMessage(string message)       // Visar ett varningsmeddelande
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($" {message}");
            Console.ResetColor();
        }

        public static void ShowInfoMessage(string message)      // Visar ett informationsmeddelande
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($" {message}");
            Console.ResetColor();
        }
        public static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo key; // Läs in tangenttryckningar utan att visa dem på skärmen

            do
            {
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password[0..^1];
                    Console.Write("\b \b");
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
            } while (true);

            Console.WriteLine();
            return password;
        }
    }
}