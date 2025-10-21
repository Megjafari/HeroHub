using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;


namespace HeroHub.Services
{
	public static class TwoFactorService    //klass för att hantera tvåfaktorsautentisering via SMS
	{
		private static string accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID")!;    //Twilio Account SID
		private static string authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN")!;       //Twilio Auth Token

		public static void SendVerificationCode(string phoneNumber, string code)    //metod för att skicka verifieringskod via SMS
		{
			TwilioClient.Init(accountSid, authToken);   //initiera Twilio-klienten med kontouppgifter


			var message = MessageResource.Create(       //skapa och skicka SMS-meddelandet
				body: $"Your verification code is: {code}",
				from: new PhoneNumber("+19785413194"), //Twilio number
				to: new PhoneNumber("+46736690901")       //Recipient number
			);
			Console.WriteLine($"Sent message to {phoneNumber}!");
		}
	}
}
