using System.Dynamic;


// 1. User Registration & Login (Hero Profile)
// Skapa ny hjälteprofil med:
// Username (hjältenamn)
// Password (lösenord) – styrkekontroll (minst 6 tecken, 1 siffra, 1 stor bokstav, 1 specialtecken).
// Email eller Phone för 2FA.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace QuestGuildTerminal
{
    public class User
    {
        public string Username { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public List<Quest> Quests { get; set; } = new();

        public static List<User> Users { get; } = new();
        public static User? LoggedInUser { get; private set; }


        // Registrering av hjälte
        public static void Register()
        {
            Console.Write("Ange din hjältes namn: ");
            var username = Console.ReadLine()!;
            if (Users.Exists(u => u.Username == username))
            {
                Console.WriteLine("Hjälten finns redan.");
                return;
            }

            Console.Write("Ange ett lösenord: ");
            string password = Console.ReadLine()!;

            // === Lösenordsstyrka ===
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
                Console.WriteLine("Lösenordet är för svagt. Försök igen.");
                return;
            }

            // === Skapa användare ===
            Users.Add(new User
            {
                Username = username,
                PasswordHash = Hash(password)
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

        // Hjälpmetoder
        private static string Hash(string input)
        {
            using var sha = SHA256.Create();
            return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(input)));
        }
    }
}
