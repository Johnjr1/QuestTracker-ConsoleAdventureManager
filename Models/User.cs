using System.Text;
using System.Security.Cryptography;


// 1. User Registration & Login (Hero Profile)
// Skapa ny hjälteprofil med:
// Username (hjältenamn)
// Password (lösenord) – styrkekontroll (minst 6 tecken, 1 siffra, 1 stor bokstav, 1 specialtecken).
// Email eller Phone för 2FA.
public class User
{
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public List<Quest> Quests { get; set; } = new();

    public static List<User> Users { get; } = new();
    public static User? LoggedInUser { get; private set; }


    // Registrering av hjälte
    public static void Register()
    {
        Console.WriteLine("Ange din hjältes namn: ");
        var username = Console.ReadLine()!;
        if (Users.Exists(u => u.Username == username))
        {
            Console.WriteLine("Hjälten finns redan.");
            return;
        }

        Console.WriteLine("Ange ditt telefonnummer (inklusive landskod, t.ex. +46701234567): ");
        var phoneNumber = Console.ReadLine()!;

        // Telefonverifiering
        bool verified = SMSVerification.SendVerificationCode(phoneNumber);
        if (!verified)
        {
            Console.WriteLine("Telefonverifiering misslyckades. Registrering avbruten.");
            return;
        }

        Console.WriteLine("Telefonnumret har verifierats!");

        string password = "";
        while (true)
        {
            Console.WriteLine("Ange ett lösenord: (Minst 6 tecken, 1 siffra, 1 stor bokstav, 1 specialtecken) ");
            password = Console.ReadLine()!;

            Console.Write("Bekräfta lösenordet: ");
            string confirmPassword = Console.ReadLine()!;

            // Kontrollera att lösenorden matchar
            if (password != confirmPassword)
            {
                Console.WriteLine("⚠️ Lösenorden matchar inte. Försök igen!");
                continue;
            }

            // Lösenordsstyrka
            int score = 0;

            if (password.Length >= 8)
                score++;

            if (password.Any(char.IsUpper) && password.Any(char.IsLower))
                score++;

            if (password.Any(char.IsDigit))
                score++;

            if (password.Any(ch => !char.IsLetterOrDigit(ch)))
                score++;

            string strength;
            if (score <= 1)
                strength = "Svagt";
            else if (score == 2 || score == 3)
                strength = "Medel";
            else
                strength = "Starkt";

            Console.WriteLine($"\nLösenordsstyrka: {strength}");

            if (score < 3)
            {
                Console.WriteLine("Lösenordet är för svagt. Försök igen. Lösenordet måste vara minst 6 tecken, 1 siffra, 1 stor bokstav, 1 specialtecken");
                continue;
            }
            break;
        }

        //Skapa användare
        Users.Add(new User
        {
            Username = username,
            PasswordHash = Hash(password),
            PhoneNumber = phoneNumber
        });

        Console.WriteLine("Registrering lyckades!");
    }

    // Inloggning av hjälte
    public static bool Login()
    {
        Console.Write("Hjältenamn: ");
        var username = Console.ReadLine()!;
        Console.Write("Lösenord: ");
        var password = Console.ReadLine()!;

        var user = Users.Find(u => u.Username == username);
        if (user == null || user.PasswordHash != Hash(password))
        {
            Console.WriteLine("Fel användarnamn eller lösenord.");
            return false;
        }
        bool verified = SMSVerification.SendVerificationCode(user.PhoneNumber);
        if (!verified)
        {
            Console.WriteLine("Inloggningen avbröts pga fel verifieringskod.");
            return false;
        }

        LoggedInUser = user;
        Console.WriteLine($"Välkommen, {user.Username}!");
        return true;
    }


    // Utloggning
    public static void Logout()
    {
        LoggedInUser = null;
        Console.WriteLine("Du har loggat ut.");
    }

    // Hjälpmetoder för säkert lösenord
    private static string Hash(string input)
    {
        using var sha = SHA256.Create();
        return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(input)));
    }
}
