using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HeroHub.Models;


namespace HeroHub.Services
{
    public class GuildAdvisorAI     //AI-tjänst för att hjälpa till med quests
    {
        private readonly ChatClient _client;    //OpenAI ChatClient för att kommunicera med GPT-3.5-turbo modellen
        public GuildAdvisorAI(string apiKey = null) //konstruktor som tar emot API-nyckel
        {
            var finalApiKey = apiKey ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");   //hämta API-nyckel från parameter eller miljövariabel

            if (string.IsNullOrEmpty(finalApiKey))  //kontrollera att API-nyckel finns
            {
                throw new InvalidOperationException(
                    " OpenAI API key is missing! " +
                    "Please set the OPENAI_API_KEY environment variable. " +
                    "In Visual Studio: Project → Properties → Debug → Environment Variables");
            }
            _client = new ChatClient("gpt-3.5-turbo", finalApiKey); //initiera ChatClient med modellen och API-nyckeln

        }

        // 1. Generera quest description från titel
        public async Task<string> GenerateQuestDescriptionAsync(string questTitle)  
        {
            var prompt = $"Skapa en episk och engagerande quest-beskrivning för '{questTitle}'. Max 50 ord. Gör den heroisk och spännande.";

            try
            {
                ChatCompletion completion = await _client.CompleteChatAsync(prompt);
                return completion.Content[0].Text;
            }
            catch (Exception ex)
            {
                return $"Kunde inte generera quest: {ex.Message}";
            }
        }

        // 2. Föreslå prioritet baserat på deadline
        public async Task<string> SuggestPriorityAsync(string questTitle, DateTime deadline)
        {
            var daysUntilDeadline = (deadline - DateTime.Now).Days;
            var prompt = $"Quest: {questTitle}. Deadline om {daysUntilDeadline} dagar. Rekommendera en prioritet (Hög/Medel/Låg) med kort motivering.";

            try
            {
                ChatCompletion completion = await _client.CompleteChatAsync(prompt);
                return completion.Content[0].Text;
            }
            catch (Exception ex)
            {
                return $"Kunde inte föreslå prioritet: {ex.Message}";
            }
        }

        // 3. Sammanfatta quests
        public async Task<string> SummarizeQuestsAsync(List<Quest> quests)
        {
            var questList = "";
            foreach (var quest in quests)
            {
                questList += $"- {quest.Title} (Deadline: {quest.IsNearDeadline:yyyy-MM-dd})\n";
            }

            var prompt = $"Sammanfatta dessa quests i en heroisk briefing på max 100 ord:\n{questList}";

            try
            {
                ChatCompletion completion = await _client.CompleteChatAsync(prompt);
                return completion.Content[0].Text;
            }
            catch (Exception ex)
            {
                return $"Kunde inte sammanfatta quests: {ex.Message}";
            }
        }
    }

 
}