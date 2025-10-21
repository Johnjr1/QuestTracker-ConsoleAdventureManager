using System;
using dotenv.net;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

// Notifications (Guild Alerts)
// Om ett quest närmar sig deadline (t.ex. < 24 timmar kvar):
// Skicka SMS eller email → “⚔️ Hjälte, ditt uppdrag [Titel] måste vara klart imorgon!”.
// Användaren kan också manuellt begära en rapport i menyn för att se vilka uppdrag som är nära deadline.


// Twilio integration för SMS om deadlines
public static class Notifications
{
    static Notifications()
    {
        //Ladda .env
        DotEnv.Load();

        //Tar hand om credentials
        var accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
        var authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");

        if (string.IsNullOrWhiteSpace(accountSid) || string.IsNullOrWhiteSpace(authToken))
        {
            Console.Error.WriteLine("Missing TWILIO_ACCOUNT_SID or TWILIO_AUTH_TOKEN.");
            Environment.Exit(1);
        }

        //Skapar en fake telefon
        TwilioClient.Init(accountSid, authToken);
    }

    // Skicka SMS-varning för uppdragsdeadline
    public static void SendQuestDeadlineAlert(string Username, string phoneNumber, string questTitle, DateTime dueDate)
    {
        var from = Environment.GetEnvironmentVariable("TWILIO_PHONE_NUMBER"); // Twilio-nummer

        var msg = MessageResource.Create(
            to: new PhoneNumber(phoneNumber),
            from: from,
            body: $"⚔️ {Username}! Ditt uppdrag {questTitle} måste vara klart imorgon {dueDate:yyyy-MM-dd}!"
        );
    }
}
