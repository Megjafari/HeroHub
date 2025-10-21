using HeroHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HeroHub.Services
{
    public class QuestManager       //klass för att hantera quests
    {
        private List<Quest> quests = new List<Quest>();     //lista för att lagra quests


        public void AddQuest(Quest quest)           //metod för att lägga till en quest i listan
        {
            quests.Add(quest);
            Console.WriteLine($"Quest '{quest.Title}' added!");
        }
        public void ShowAllQuests()     //metod för att visa alla quests i listan
        {
            if (quests.Count == 0)
            {
                Console.WriteLine("No quests available.");
                return;
            }
            Console.WriteLine("All Quests:");
            foreach (var quest in quests)
            {
                string status = quest.IsCompleted ? "Completed" : "Pending";        //kolla om questen är slutförd eller inte
                Console.WriteLine($"{quest.Title} - Due: {quest.DueDate}, Priority: {quest.Priority}, Status: {status}");   //visa titel, förfallodatum, prioritet och status för varje quest
            }
        }
        public void CompleteQuest(string title)      //metod för att markera en quest som slutförd baserat på titel
        {
            var quest = quests.FirstOrDefault(q => q.Title == title);   //hitta questen med den angivna titeln
            if (quest != null)      //om questen finns
            {
                quest.QuestCompleted();
                Console.WriteLine($"Quest '{title}' marked as completed!");
            }
            else
            {
                Console.WriteLine($"Quest '{title}' not found.");
            }
        }
        public void ShowReport()        //metod för att visa en rapport över quests
        {
            int totalQuests = quests.Count;
            int doneQuests = quests.Count(q => q.IsCompleted);
            int ActiveQuests = totalQuests - doneQuests;
            int nearDeadlineQuests = quests.Count(q => q.IsNearDeadline());
            Console.WriteLine("Quest Report:");
            Console.WriteLine($"Total Quests: {totalQuests} | Quests Done: {doneQuests} | Active Quests: {ActiveQuests} | Near Deadline Quests: {nearDeadlineQuests}");
            
        }

        public List<Quest> GetNearDeadlineQuests()           //metod för att hämta alla quests som närmar sig förfallodatumet
                                                             
        {
            return quests.Where(q => q.IsNearDeadline()).ToList();      //returnera en lista med quests som är nära förfallodatumet
        }          



    }
}

