using System.Text;
using System.Security.Cryptography;
using Spectre.Console;


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
        Console.Clear();
        
        // Skapa registreringspanel med episk styling
        var creationPanel = new Panel(
            "[bold yellow]⚔️ Welcome to the Guild, brave soul! ⚔️[/]\n" +
            "[italic]Let us forge your legend in the annals of adventure...[/]"
        )
        {
            Header = new PanelHeader("[bold cyan]🏰 HERO CREATION CHAMBER 🏰[/]", Justify.Center),
            Border = BoxBorder.Double,
            BorderStyle = new Style(Color.Blue)
        };
        
        AnsiConsole.Write(creationPanel);
        
        AnsiConsole.MarkupLine("[yellow]Ange din hjältes namn: [/]");
        var username = Console.ReadLine()!;
        if (Users.Exists(u => u.Username == username))
        {
            AnsiConsole.MarkupLine("[red]❌ A hero with that name already exists in the guild![/]");
            AnsiConsole.MarkupLine("[italic]Choose a different name, brave adventurer...[/]");
            AnsiConsole.MarkupLine("\n[grey]Press any key to try again...[/]");
            Console.ReadKey(true);
            return;
        }

        //Säker inmatning av telefonnummer
        string phoneNumber = "";
        while (true)
        {
            AnsiConsole.MarkupLine("[yellow]Enter Your Phone Number (including country code, e.g. +46701234567):[/]");
            phoneNumber = Console.ReadLine()!;

            // Kontrollera att det börjar med + och innehåller bara siffror efteråt
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                Console.WriteLine("You Have To Enter Your Phone Number");
                continue;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^\+\d{6,15}$"))
            {
                Console.WriteLine("Invalid Format! Example: +46701234567");
                continue;
            }

            break;
        }

        // Telefonverifiering
        bool verified = SMSVerification.SendVerificationCode(phoneNumber);
        if (!verified)
        {
            Console.WriteLine("Something Went Wrong, Try Again!");
            return;
        }

        Console.WriteLine("Your Phone Number Has Been Verified");

        string password = "";
        while (true)
        {
            AnsiConsole.MarkupLine("[yellow]Choose Your Password: (At least 6 characters, 1 number, 1 capital letter, 1 special character) [/]");
            password = Console.ReadLine()!;

            Console.Write("Confirm Your Password: ");
            string confirmPassword = Console.ReadLine()!;

            // Skriv lösenordet två gånger och kontrollera att de matchar
            if (password != confirmPassword)
            {
                Console.WriteLine("⚠️ The Passwords Do Not Match. Try Again!");
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
                strength = "Weak";
            else if (score == 2 || score == 3)
                strength = "Medium";
            else
                strength = "Strong";

            Console.WriteLine($"\nPassword Strenght: {strength}");

            if (score < 3)
            {
                Console.WriteLine("The Password Is Too Weak. Please Try Again. (at least 6 characters, 1 number, 1 uppercase letter, 1 special character)");
                continue;
            }
            break;
        }

        //Skapa användare och lägg till i listan
        Users.Add(new User
        {
            Username = username,
            PasswordHash = Hash(password),
            PhoneNumber = phoneNumber
        });

        // Visa meddelande om lyckad registrering
        var successPanel = new Panel(
            $"[bold green]🎉 Welcome To The Guild, {username}! 🎉[/]\n\n" +
            "[italic]Your Legend Begins Now, Brave Adventurer![/]\n" +
            "The Guild Halls Await Your Heroic Deeds..."
        )
        {
            Header = new PanelHeader("[bold green]✨ HERO CREATED SUCCESSFULLY ✨[/]", Justify.Center),
            Border = BoxBorder.Double,
            BorderStyle = new Style(Color.Green)
        };
        
        AnsiConsole.Write(successPanel);
        Console.WriteLine("Your Hero Was Created!");
        AnsiConsole.MarkupLine("\n[grey]Press any key to return to the guild hall...[/]");
        Console.ReadKey(true);
    }

    // Inloggning av hjälte
    public static bool Login()
    {
        Console.Clear();
        
        var loginPanel = new Panel(
            "[bold yellow]🔑 Enter The Guild Halls 🔑[/]\n" +
            "[italic]Prove Your Identity, Brave Adventurer...[/]"
        )
        {
            Header = new PanelHeader("[bold cyan]🏰 GUILD ENTRANCE 🏰[/]", Justify.Center),
            Border = BoxBorder.Double,
            BorderStyle = new Style(Color.Blue)
        };
        
        AnsiConsole.Write(loginPanel);
        
        Console.Write("Enter The Name Of Your Hero: ");
        var username = Console.ReadLine()!;
        Console.Write("Enter Your Password: ");
        var password = Console.ReadLine()!;

        var user = Users.Find(u => u.Username == username);
        if (user == null || user.PasswordHash != Hash(password))
        {
            AnsiConsole.MarkupLine("[red]❌ Invalid Hero Name Or Incantation![/]");
            AnsiConsole.MarkupLine("[italic]The Guild Guards Do Not Recognize You...[/]");
            AnsiConsole.MarkupLine("\n[grey]Press Any Key To Try Again...[/]");
            Console.ReadKey(true);
            return false;
        }

        // Telefonverifiering 2FA
        bool verified = SMSVerification.SendVerificationCode(user.PhoneNumber);
        if (!verified)
        {
            Console.WriteLine("The Code Was Wrong, Try Again...");
            return false;
        }

        LoggedInUser = user;
        
        // Visa välkomsmeddelande när inloggningen lyckas
        var welcomePanel = new Panel(
            $"[bold green]🎉 Welcome Back, {user.Username}! 🎉[/]\n\n" +
            "[italic]Your Quests Await In The Guild Hall...[/]"
        )
        {
            Header = new PanelHeader("[bold green]✨ GUILD HALL ACCESS GRANTED ✨[/]", Justify.Center),
            Border = BoxBorder.Double,
            BorderStyle = new Style(Color.Green)
        };
        
        AnsiConsole.Write(welcomePanel);
        Console.WriteLine($"Welcome, {user.Username}!");
        AnsiConsole.MarkupLine("\n[grey]Press any key to enter the guild hall...[/]");
        Console.ReadKey(true);
        
        return true;
    }


    // Utloggning
    public static void Logout()
    {
        var user = LoggedInUser?.Username ?? "Adventurer";
        LoggedInUser = null;
        
        var logoutPanel = new Panel(
            $"[bold yellow]👋 Farewell, {user}! 👋[/]\n\n" +
            "[italic]May Your Adventures Be Legendary...[/]\n" +
            "The Guild Halls Await Your Return..."
        )
        {
            Header = new PanelHeader("[bold red]🚪 GUILD HALL EXIT 🚪[/]", Justify.Center),
            Border = BoxBorder.Double,
            BorderStyle = new Style(Color.Red)
        };
        
        AnsiConsole.Write(logoutPanel);
        Console.WriteLine("You Have Logged Out Sucesfully.");
    }

    // Hjälpmetoder för säkert lösenord
    private static string Hash(string input)
    {
        using var sha = SHA256.Create();
        return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(input)));
    }
}
