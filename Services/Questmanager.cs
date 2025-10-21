public static class QuestManager
{

    // Lägg till ett nytt uppdrag
    public static async Task AddQuest(User user)
    {
        // Fråga om chat ska hjälpa till att skapa uppdraget
        Console.WriteLine("\nVill du använda AI-hjälp för att skapa uppdraget?");
        Console.WriteLine("1) Ja, låt Guild Advisor skapa uppdraget");
        Console.WriteLine("2) Nej, skapa manuellt");
        Console.Write("Val: ");
        var aiChoice = Console.ReadLine();

        Quest? newQuest = null;

        if (aiChoice == "1")
        {
            // Använd AI för att generera uppdrag
            Console.Write("Skriv en titel för ditt uppdrag: ");
            var title = Console.ReadLine()!;
            newQuest = await GuildAdvisorAI.GenerateQuestFromTitle(title); // Anropa AI-tjänsten

            if (newQuest == null)
            {
                Console.WriteLine("AI kunde inte generera uppdraget. Skapar manuellt istället.");
            }
            else
            {
                // Lägg till det genererade uppdraget till användarens lista
                user.Quests.Add(newQuest);
                Console.WriteLine($"\n🧙‍♂️ Guild Advisor skapade uppdraget:\n" +
                                  $"Titel: {newQuest.Title}\n" +
                                  $"Beskrivning: {newQuest.Description}\n" +
                                  $"Prioritet: {newQuest.Priority}\n" +
                                  $"Deadline: {newQuest.DueDate:d}\n");
                Console.WriteLine("✅ Uppdraget skapat");
                Console.WriteLine("Tryck på valfri tangent för att gå tillbaks till menyn...");
                Console.ReadKey();
                return;
            }
        }

        // Manuell skapelse av uppdrag
        Console.WriteLine("Titel: ");
        var manualTitle = Console.ReadLine()!;
        Console.WriteLine("Beskrivning: ");
        var desc = Console.ReadLine()!;
        Console.WriteLine("Deadline (yyyy-mm-dd): ");
        DateTime.TryParse(Console.ReadLine(), out var deadline);
        Console.WriteLine("Prioritet (Low, Medium, High): ");
        var priority = Console.ReadLine()!;

        user.Quests.Add(new Quest
        {
            Title = manualTitle,
            Description = desc,
            DueDate = deadline == default ? DateTime.Now.AddDays(3) : deadline,
            Priority = string.IsNullOrWhiteSpace(priority) ? "Medium" : priority
        });

        Console.WriteLine("✅ Uppdraget skapat");
        Console.WriteLine("Tryck på valfri tangent för att gå tillbaks till menyn...");
        Console.ReadKey();
        return;
    }

    // Visa alla uppdrag som tillhör en användare
    public static void ShowQuests(User user)
    {
        if (user.Quests.Count == 0)
        {
            Console.WriteLine("Inga uppdrag ännu.");
            return;
        }

        // Lista alla uppdrag för användaren
        foreach (var q in user.Quests)
        {
            var status = q.IsCompleted ? "Klar" : "Pågår";
            Console.WriteLine($"{status} - {q.Title} (Deadline: {q.DueDate:d})");
            Console.WriteLine($"   {q.Description}");
        }
    }

    // Markera ett uppdrag som klart
    public static void CompleteQuest(User user)
    {
        Console.WriteLine("Ange titel på uppdraget: ");
        var title = Console.ReadLine()!;
        var quest = user.Quests.Find(q => q.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        if (quest == null)
        {
            Console.WriteLine("Uppdrag hittades inte.");
            return;
        }
        quest.IsCompleted = true;
        Console.WriteLine("Uppdrag markerat som klart!");
    }


    // Uppdatera ett befintligt uppdrag
    public static void UpdateQuest(User user)
    {
        Console.Write("Ange titel på uppdraget att uppdatera: ");
        var title = Console.ReadLine()!;
        var quest = user.Quests.Find(q => q.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        if (quest == null)
        {
            Console.WriteLine("Uppdrag hittades inte.");
            return;
        }

        Console.Write("Ny titel (lämna tom för att behålla nuvarande): ");
        var newTitle = Console.ReadLine()!;
        if (!string.IsNullOrWhiteSpace(newTitle))
            quest.Title = newTitle;

        Console.Write("Ny beskrivning (lämna tom för att behålla nuvarande): ");
        var newDesc = Console.ReadLine()!;
        if (!string.IsNullOrWhiteSpace(newDesc))
            quest.Description = newDesc;

        Console.Write("Ny deadline (yyy-mm-dd, lämna tom för att behålla nuvarande): ");
        var newDeadlineInput = Console.ReadLine()!;
        if (DateTime.TryParse(newDeadlineInput, out var newDeadline))
            quest.DueDate = newDeadline;

        Console.WriteLine("Uppdrag uppdaterat!");
    }

    // Kontrollera om några uppdrag närmar sig deadline och skicka notifikationer
    public static void CheckForUpcomingDeadlines(User user)
    {
        var soonDue = user.Quests
            .Where(q => !q.IsCompleted && q.DueDate > DateTime.Now && q.DueDate <= DateTime.Now.AddHours(24))
            .ToList();

        if (soonDue.Count == 0)
        {
            Console.WriteLine("Inga uppdrag närmar sig deadline inom 24 timmar.");
            return;
        }

        Console.WriteLine("Dessa uppdrag är nära deadline:");
        foreach (var q in soonDue)
        {
            Console.WriteLine($" - {q.Title} (Deadline: {q.DueDate:g})");

            // Skicka SMS-varning
            foreach (var quest in soonDue)
            {
                Console.WriteLine($" - {q.Title} (Deadline: {q.DueDate:g})");
                Notifications.SendQuestDeadlineAlert(user.Username, user.PhoneNumber, q.Title, q.DueDate);
            }
        }
    }

    // Visa en fullständig rapport över användarens uppdrag
    public static void ShowFullQuestReport(User user)
    {
        int activeQuests = user.Quests.Count(q => !q.IsCompleted);
        int completedQuests = user.Quests.Count(q => q.IsCompleted);
        int urgentQuests = user.Quests.Count(q => !q.IsCompleted && q.DueDate <= DateTime.Now.AddHours(24));

        Console.WriteLine("\n=== GUILD REPORT ===");
        Console.WriteLine($"Pågående uppdrag: {activeQuests}");
        Console.WriteLine($"Klara uppdrag: {completedQuests}");
        Console.WriteLine($"Uppdrag nära deadline (inom 24h): {urgentQuests}");

        if (urgentQuests > 0)
        {
            Console.WriteLine("\nDessa uppdrag närmar sig deadline:");
            foreach (var q in user.Quests.Where(q => !q.IsCompleted && q.DueDate <= DateTime.Now.AddHours(24)))
            {
                Console.WriteLine($"- {q.Title} (Deadline: {q.DueDate:g})");
            }
        }
        
        // Sammanfattning av användarens uppdrag
        Console.WriteLine("\nSammanfattning:");
        Console.WriteLine($"Du har {activeQuests} pågående quest{(activeQuests == 1 ? "" : "s")}, " +
                        $"{urgentQuests} måste slutföras snart, " +
                        $"{activeQuests - urgentQuests} är under kontroll, " +
                        $"{completedQuests} är klara.");
    }
}