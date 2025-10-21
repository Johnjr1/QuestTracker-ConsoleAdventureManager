using System;
using dotenv.net;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

public static class SMSVerification
{

    public static bool SendVerificationCode(string phoneNumber)
    {
        //Ladda .env
        DotEnv.Load();

        //Tar hand om credentials för Twilio
        var accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
        var authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");

        if (string.IsNullOrWhiteSpace(accountSid) || string.IsNullOrWhiteSpace(authToken))
        {
            Console.Error.WriteLine("Missing TWILIO_ACCOUNT_SID or TWILIO_AUTH_TOKEN.");
            Environment.Exit(1);
        }

        //Skapar en fake telefon
        TwilioClient.Init(accountSid, authToken);

        Random random = new Random();
        int secretCode = random.Next(1000, 9999);

        //Skickar iväg ett sms med koden
        var from = Environment.GetEnvironmentVariable("TWILIO_PHONE_NUMBER"); // Twilio-nummer

        var msg = MessageResource.Create(
            to: new PhoneNumber(phoneNumber),
            from: from,
            body: $"Your verification code is: {secretCode}"
        );

        //Ber användaren mata in koden
        Console.WriteLine("Please Enter The Verification Code Sent To Your Phone:");
        string UserSecretCode = Console.ReadLine();
        if (UserSecretCode == secretCode.ToString())
        {
            Console.WriteLine("You Are Verified! Welcome To Quest Tracker.");
            return true; 
        }
        else
        {
            Console.WriteLine("Wrong Code. Verification Failed.");
            return false;
        }
    }
}