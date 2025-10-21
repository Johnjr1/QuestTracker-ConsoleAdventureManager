using Spectre.Console;
public static class Menu
{

    // Startmeny för Quest Tracker
    public static async Task Start()
    {
        while (true)
        {
            Console.Clear(); // Clear console for better readability
            Console.WriteLine("\n=== QUEST GUILD TERMINAL ===");
            Console.WriteLine("1) Registrera hjälte");
            Console.WriteLine("2) Logga in");
            Console.WriteLine("3) Avsluta");
            Console.Write("Val: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1": User.Register(); break;
                case "2":
                    if (User.Login()) await HeroMenu();
                    break;
                case "3": return;
                default: Console.WriteLine("Ogiltigt val."); break;
            }
        }
    }

    // Hjälte-meny efter inloggning
    private static async Task HeroMenu()
    {
        Console.Clear();
        var user = User.LoggedInUser!;
        while (true)
        {
            Console.Clear(); // Clear console for better readability
            Console.WriteLine($"\n=== GUILD MENY — {user.Username} ===");
            Console.WriteLine("1) Lägg till uppdrag");
            Console.WriteLine("2) Visa uppdrag");
            Console.WriteLine("3) Markera uppdrag som klart");
            Console.WriteLine("4) Uppdatera uppdrag");
            Console.WriteLine("5) Guild Advisor (AI hjälp)");
            Console.WriteLine("6) Guild report");
            Console.WriteLine("7) Logga ut");
            Console.Write("Val: ");
            var c = Console.ReadLine();

            switch (c)
            {
                case "1": await QuestManager.AddQuest(user); break;
                case "2":
                    QuestManager.ShowQuests(user);
                    AnsiConsole.MarkupLine("\n[grey]Tryck på en tangent för att återgå till menyn...[/]");
                    Console.ReadKey(true);
                    break;
                case "3":
                    QuestManager.CompleteQuest(user);
                    AnsiConsole.MarkupLine("\n[grey]Tryck på en tangent för att återgå till menyn...[/]");
                    Console.ReadKey(true);
                    break;
                case "4":
                    QuestManager.UpdateQuest(user);
                    AnsiConsole.MarkupLine("\n[grey]Tryck på en tangent för att återgå till menyn...[/]");
                    Console.ReadKey(true);
                    break;
                case "5":
                    Console.Write("Ange titeln för uppdraget du vill generera med AI: ");
                    var title = Console.ReadLine() ?? "";
                    var generatedQuest = await GuildAdvisorAI.GenerateQuestFromTitle(title);

                    if (generatedQuest != null)
                    {
                        Console.WriteLine("\n🧙‍♂️ Guild Advisor skapade ett nytt uppdrag:");
                        Console.WriteLine($"Titel: {generatedQuest.Title}");
                        Console.WriteLine($"Beskrivning: {generatedQuest.Description}");
                        Console.WriteLine($"Prioritet: {generatedQuest.Priority}");
                        Console.WriteLine($"Deadline: {generatedQuest.DueDate:d}");

                        // Lägg till i användarens quest-lista
                        user.Quests.Add(generatedQuest);
                        Console.WriteLine("\nUppdraget har sparats till din lista!");
                    }
                    else
                    {
                        Console.WriteLine("AI kunde inte skapa uppdraget. Försök igen senare.");
                    }
                    break;
                case "6":
                    QuestManager.ShowFullQuestReport(user);
                    AnsiConsole.MarkupLine("\n[grey]Tryck på en tangent för att återgå till menyn...[/]");
                    Console.ReadKey(true);
                    break;
                case "7": User.Logout(); return;
                default:
                    Console.WriteLine("Ogiltigt val.");
                    AnsiConsole.MarkupLine("\n[grey]Tryck på en tangent för att återgå till menyn...[/]");
                    Console.ReadKey(true);
                    break;
            }
        }
    }
}