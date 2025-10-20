public static class Menu
{
   public static async Task Start()
    {
        while (true)
        {
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

    private static async Task HeroMenu()
    {
        var user = User.LoggedInUser!;
        while (true)
        {
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
                case "1": QuestManager.AddQuest(user); break;
                case "2": QuestManager.ShowQuests(user); break;
                case "3": QuestManager.CompleteQuest(user); break;
                case "4": QuestManager.UpdateQuest(user); break;
                case "5": await GuildAdvisorAI.GuildAdvisor(); break;
                case "6": QuestManager.ShowFullQuestReport(user); break;
                case "7": User.Logout(); return;
                default: Console.WriteLine("Ogiltigt val."); break;
            }
        }
    }
}