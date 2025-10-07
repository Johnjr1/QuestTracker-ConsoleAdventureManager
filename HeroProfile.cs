using System.Dynamic;


// 1. User Registration & Login (Hero Profile)
// Skapa ny hjälteprofil med:
// Username (hjältenamn)
// Password (lösenord) – styrkekontroll (minst 6 tecken, 1 siffra, 1 stor bokstav, 1 specialtecken).
// Email eller Phone för 2FA.

class HeroProfile
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string ContactInfo { get; set; }

}