using HeroHub.Models;
using HeroHub.Services;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;





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
            
            var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                
                .PageSize(10)
                .AddChoices(new[] {
                        "Register Hero",
                        "Login Hero",
                        "Exit"
                 }));
                
            switch (choice) 
            {
                case "Register Hero":
                    Authenticator.Register();   
                    PressAnyKeyToContinue();
                    return true;
                case "Login Hero":
                    LoginHero();
                    return true;
                case "Exit":
                    return false;
                default:
                    ShowErrorMessage("Invalid choice. Please try again.");
                    PressAnyKeyToContinue();
                    return true;
     
            }

        }
        private static void ShowAIMenu()    // Visar AI-meny och hanterar AI-relaterade funktioner
        {
            Console.Clear();
            if (_aiAdvisor == null)
            {
                ShowErrorMessage("AI features are disabled. Please check your API key configuration.");
                PressAnyKeyToContinue();
                return;
            }
            bool inAIMenu = true;
            while (inAIMenu)
            {
                var choice = AnsiConsole.Prompt(
           new SelectionPrompt<string>()
               .Title("[purple] GUILD ADVISOR - AI ASSISTANT[/]")
               .PageSize(10)
               .AddChoices(new[] {
                    "Generate Quest Description",
                    "Suggest Quest Priority",
                    "Summarize All Quests",
                    "Back to Main Menu"
               }));

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
            Console.Clear();
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
            Console.Clear();
            bool inHeroMenu = true;
            while (inHeroMenu)  
            {
                var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[yellow] WELCOME, {Authenticator.GetCurrentHero()?.username ?? "Hero"}![/]")
                .PageSize(10)
                .AddChoices(new[] {
                    "Add New Quest",
                    "View All Quests",
                    "Update/Complete Quest",
                    "Request Guild Advisor Help (AI)",
                    "Show Guild Report",
                    "Logout"
                }));

                switch (choice)
                {
                    case "Add New Quest":
                        AddNewQuest();
                        break;
                    case "View All Quests":
                        _questManager.ShowAllQuests();
                        break;
                    case "Update/Complete Quest":
                        CompleteQuest();
                        break;
                    case "Request Guild Advisor Help (AI)":
                        ShowAIMenu();
                        break;
                    case "Show Guild Report":
                        _questManager.ShowReport();
                        break;
                    case "Logout":
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
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("Enter quest title: ");
            Console.ResetColor();
            var title = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("Enter quest description (or press enter for AI generation): ");
            Console.ResetColor();
            var description = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(description))
            {
                description = _aiAdvisor.GenerateQuestDescriptionAsync(title).Result;
                ShowInfoMessage($"AI-generated description: {description}");
            }
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("Enter due date (yyyy-mm-dd): ");
            Console.ResetColor();
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
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("Enter the title of the quest to complete: ");
            Console.ResetColor();
            var title = Console.ReadLine();
            _questManager.CompleteQuest(title);
        }

        // AI Methods
        private static async void GenerateQuestDescription()        // Hanterar generering av quest-beskrivningar med AI
        {   
            Console.Clear();
            if (_aiAdvisor == null)
            {
                ShowErrorMessage("AI advisor is not available.");
                return;
            }
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("Enter quest title for AI description: ");
            Console.ResetColor();
            var title = Console.ReadLine();
            var description = await _aiAdvisor.GenerateQuestDescriptionAsync(title);
            ShowInfoMessage($"AI Guild Advisor says:\n{description}");
        }

        private static async void SuggestQuestPriority()        // Hanterar förslag på quest-prioritet med AI
        {
            Console.Clear();
            if (_aiAdvisor == null)
            {
                ShowErrorMessage("AI advisor is not available.");
                return;
            }
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("Enter quest title: ");
            Console.ResetColor();
            var title = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("Enter due date (yyyy-mm-dd): ");
            Console.ResetColor();
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
            Console.Clear();
            // Note: You might need to modify QuestManager to expose the quests list
            // For now, this will use AI without context
            var summary = await _aiAdvisor.SummarizeQuestsAsync(new List<Quest>());
            ShowInfoMessage($"AI Quest Summary:\n{summary}");
        }

        // UI Display Methods
        private static void ShowWelcomeScreen() // Visar välkomstskärmen
        {
            Console.Clear();
            AnsiConsole.Write(
                new FigletText("HeroHub")
                    .Centered()
                    .Color(Color.Orange1));
            AnsiConsole.Write(
                new Panel("[bold White]Welcome, brave hero! Your adventures await...[/]")
                    .Border(BoxBorder.Rounded)
                    .BorderColor(Color.Orange1)
                    .Padding(1, 1));
             PressAnyKeyToContinue();
        }

        private static void ShowGoodbyeScreen()     // Visar avskedsskärmen
        {
            Console.Clear();
            AnsiConsole.Write(
                new FigletText("FAREWELL, BRAVE HERO! May your adventures continue...")
                    .Centered()
                    .Color(Color.Wheat1));
            
            PressAnyKeyToContinue();

            
        }

        // Helper Methods
        public static void ShowQuestPriorityMenu()      // Visar meny för att välja quest-prioritet
        {
            Console.Clear();
            Console.WriteLine("\n Select Quest Priority:");
            Console.WriteLine("1. High ");
            Console.WriteLine("2. Medium ");
            Console.WriteLine("3. Low ");
            Console.Write("Choose priority: ");
        }

        public static string GetPriorityText(int priorityChoice)        // Returnerar textrepresentation av prioritet baserat på användarval
        {
            Console.Clear();
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
        public static string ReadPassword() // Läser in ett lösenord utan att visa det på skärmen
        {
            string password = "";
            ConsoleKeyInfo key; // Läs in tangenttryckningar utan att visa dem på skärmen

            do
            {
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Backspace && password.Length > 0)     // Hantera backspace
                {
                    password = password[0..^1];     // Ta bort sista tecknet
                    Console.Write("\b \b");     // Ta bort asterisken från skärmen
                }
                else if (key.Key == ConsoleKey.Enter)       
                {
                    break;
                }
                else if (!char.IsControl(key.KeyChar))  // Lägg till tecken till lösenordet
                {
                    password += key.KeyChar;
                    Console.Write("*"); // Visa asterisk för varje tecken
                }
            } while (true);

            Console.WriteLine();
            return password;
        }
    }
}