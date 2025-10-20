public static class QuestManager
{
    public static void AddQuest(User user)
    {
        Console.WriteLine("Titel: ");
        var title = Console.ReadLine()!;
        Console.WriteLine("Beskrivning: ");
        var desc = Console.ReadLine()!;
        Console.WriteLine("Deadline (t.ex. 2025-10-20): ");
        DateTime.TryParse(Console.ReadLine(), out var deadline);
        Console.WriteLine("Prioritet (Low, Medium, High): ");
        var priority = Console.ReadLine()!;

        user.Quests.Add(new Quest
        {
            Title = title,
            Description = desc,
            DueDate = deadline,
            Priority = string.IsNullOrWhiteSpace(priority) ? "Medium" : priority
        });

        Console.WriteLine("Uppdrag tillagt!");
    }

    public static void ShowQuests(User user)
    {
        if (user.Quests.Count == 0)
        {
            Console.WriteLine("Inga uppdrag ännu.");
            return;
        }

        foreach (var q in user.Quests)
        {
            var status = q.IsCompleted ? "✅ Klar" : "🕓 Pågår";
            Console.WriteLine($"{status} - {q.Title} (Deadline: {q.DueDate:d})");
            Console.WriteLine($"   {q.Description}");
        }
    }

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

        Console.Write("Ny deadline (t.ex. 2025-10-20, lämna tom för att behålla nuvarande): ");
        var newDeadlineInput = Console.ReadLine()!;
        if (DateTime.TryParse(newDeadlineInput, out var newDeadline))
            quest.DueDate = newDeadline;

        Console.WriteLine("Uppdrag uppdaterat!");
    }
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

        Console.WriteLine("\nSammanfattning:");
        Console.WriteLine($"Du har {activeQuests} pågående quest{(activeQuests == 1 ? "" : "s")}, " +
                        $"{urgentQuests} måste slutföras snart, " +
                        $"{activeQuests - urgentQuests} är under kontroll, " +
                        $"{completedQuests} är klara.");
    }
}