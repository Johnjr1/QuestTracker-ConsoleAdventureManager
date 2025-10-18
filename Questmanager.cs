public static class QuestManager
{
    public static void AddQuest(User user)
    {
        Console.Write("Titel: ");
        var title = Console.ReadLine()!;
        Console.Write("Beskrivning: ");
        var desc = Console.ReadLine()!;
        Console.Write("Deadline (t.ex. 2025-10-20): ");
        DateTime.TryParse(Console.ReadLine(), out var deadline);

        user.Quests.Add(new Quest
        {
            Title = title,
            Description = desc,
            DueDate = deadline
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
        Console.Write("Ange titel på uppdraget: ");
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
}