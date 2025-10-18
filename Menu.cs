public static class Menu
{
    public static void Start()
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
                    if (User.Login()) HeroMenu();
                    break;
                case "3": return;
                default: Console.WriteLine("Ogiltigt val."); break;
            }
        }
    }

    private static void HeroMenu()
    {
        var user = User.LoggedInUser!;
        while (true)
        {
            Console.WriteLine($"\n=== GUILD MENY — {user.Username} ===");
            Console.WriteLine("1) Lägg till uppdrag");
            Console.WriteLine("2) Visa uppdrag");
            Console.WriteLine("3) Markera uppdrag som klart");
            Console.WriteLine("4) Request Guild Advisor help (AI)");
            Console.WriteLine("5) Show guild report");
            Console.WriteLine("6) Logga ut");
            Console.Write("Val: ");
            var c = Console.ReadLine();

            switch (c)
            {
                case "1": QuestManager.AddQuest(user); break;
                case "2": QuestManager.ShowQuests(user); break;
                case "3": QuestManager.CompleteQuest(user); break;
                case "4": Console.WriteLine("Be om hjälp kommer vara här."); break;
                case "5": Console.WriteLine("show guild report kommer vara här"); break;
                case "6": User.Logout(); return;
                default: Console.WriteLine("Ogiltigt val."); break;
            }
        }
    }
}