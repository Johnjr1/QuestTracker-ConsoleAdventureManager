using System.Dynamic;


// 1. User Registration & Login (Hero Profile)
// Skapa ny hjälteprofil med:
// Username (hjältenamn)
// Password (lösenord) – styrkekontroll (minst 6 tecken, 1 siffra, 1 stor bokstav, 1 specialtecken).
// Email eller Phone för 2FA.

// +-----------------------------------+
// |            User                   |
// +-----------------------------------+
// | - username: String                |
// | - password: String                |
// | - email: String                   |
// | - phone: String                   |
// | - quests: List<Quest>             |
// +-----------------------------------+
// | + register(): void                |
// | + login(): bool                   |
// | + verify2FA(code: String): bool   |
// | + addQuest(q: Quest): void        |
// | + getQuests(): List<Quest>        |

class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public List<Quest> Quests { get; set; }

    public void Register()
    {
        // Implement registration logic here
    }

    public bool Login()
    {
        // Implement login logic here
        return true;
    }

}