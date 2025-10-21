using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HeroHub.Models
{
    public class Quest          //representerar en quest med titel, beskrivning, förfallodatum, prioritet och slutförandestatus
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public string? Priority { get; set; }
        public bool IsCompleted { get; set; }       //indikerar om questen är slutförd eller inte   //bool är antingen true eller false

        public void QuestCompleted()       //metod för att markera questen som slutförd
        {
            IsCompleted = true;
        }

        public bool IsNearDeadline()      //metod för att kontrollera om questen närmar sig förfallodatumet (inom 24h)
        {
            return !IsCompleted && (DueDate - DateTime.Now).TotalHours <= 24;   
        }
    }
}
