using Spectre.Console;

public static class QuestManager
{

    // Lägg till ett nytt uppdrag
    public static async Task AddQuest(User user)
    {
        Console.Clear();

        var questPanel = new Panel(
            "[bold yellow]📜 QUEST ACCEPTANCE CHAMBER 📜[/]\n" +
            "[italic]Choose How You Wish To Receive Your New Quest...[/]"
        )
        {
            Header = new PanelHeader("[bold cyan]⚔️ NEW QUEST REGISTRATION ⚔️[/]", Justify.Center),
            Border = BoxBorder.Double,
            BorderStyle = new Style(Color.Blue)
        };

        AnsiConsole.Write(questPanel);

        // Fråga om chat ska hjälpa till att skapa uppdraget
        var aiChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("[bold magenta]Do You Want The Guild Advisor To Give You a Quest?[/]")
            .AddChoices(new[] {
                "[bold purple]🧙‍♂️  Yes, Accept The Quest From The Guild Advisor[/]",
                "[bold green]✍️  No, Choose Your Own Quest[/]"
            }));

        Quest? newQuest = null;

        if (aiChoice == "[bold purple]🧙‍♂️  Yes, Accept The Quest From The Guild Advisor[/]")
        {
            // Använd AI för att generera uppdrag
            AnsiConsole.MarkupLine("[bold cyan]Write a Quest Title To The Guild Advisor:[/]");
            Console.Write("Write a Quest Title ");
            var title = Console.ReadLine()!;
            newQuest = await GuildAdvisorAI.GenerateQuestFromTitle(title); // Anropa AI-tjänsten

            if (newQuest == null)
            {
                AnsiConsole.MarkupLine("[red]❌ The Guild Advisor's Magic Has Failed. Create Your Own Quest Instead...[/]");
            }
            else
            {
                // Lägg till det genererade uppdraget till användarens lista
                user.Quests.Add(newQuest);

                var advisorSuccessPanel = new Panel(
                    $"[bold green]✨ The Guild Advisor Has Choosen a Quest For You! ✨[/]\n\n" +
                    $"[bold]Titel:[/] {newQuest.Title}\n" +
                    $"[bold]Beskrivning:[/] {newQuest.Description}\n" +
                    $"[bold]Prioritet:[/] {newQuest.Priority}\n" +
                    $"[bold]Deadline:[/] {newQuest.DueDate:d}"
                )
                {
                    Header = new PanelHeader("[bold green]📜 QUEST CREATED SUCCESSFULLY 📜[/]", Justify.Center),
                    Border = BoxBorder.Double,
                    BorderStyle = new Style(Color.Green)
                };

                AnsiConsole.Write(advisorSuccessPanel);
                AnsiConsole.MarkupLine("[bold green]✅ QUEST CREATED SUCCESSFULLY 📜[/]");
                AnsiConsole.MarkupLine("\n[grey]Press any key to return to the guild hall...[/]");
                Console.ReadKey();
                return;
            }
        }

        // Manuell skapelse av uppdrag
        AnsiConsole.MarkupLine("[bold yellow]📝 Create your own quest:[/]");
        AnsiConsole.MarkupLine("[yellow]Titel: ");
        var manualTitle = Console.ReadLine()!;
        AnsiConsole.MarkupLine("[yellow]Description: ");
        var desc = Console.ReadLine()!;
        AnsiConsole.MarkupLine("[yellow]Deadline (yyyy-mm-dd): ");
        DateTime.TryParse(Console.ReadLine(), out var deadline);
        AnsiConsole.MarkupLine("[yellow]Priority (Low, Medium, High): ");
        var priority = Console.ReadLine()!;

        user.Quests.Add(new Quest
        {
            Title = manualTitle,
            Description = desc,
            DueDate = deadline == default ? DateTime.Now.AddDays(3) : deadline,
            Priority = string.IsNullOrWhiteSpace(priority) ? "Medium" : priority
        });

        var successPanel = new Panel(
            "[bold green]✨ Quest Created Successfully! ✨[/]\n\n" +
            $"[bold]Titel:[/] {manualTitle}\n" +
            $"[bold]Beskrivning:[/] {desc}\n" +
            $"[bold]Prioritet:[/] {priority}\n" +
            $"[bold]Deadline:[/] {(deadline == default ? DateTime.Now.AddDays(3) : deadline):d}"
        )
        {
            Header = new PanelHeader("[bold green]📜 QUEST CREATED SUCCESSFULLY 📜[/]", Justify.Center),
            Border = BoxBorder.Double,
            BorderStyle = new Style(Color.Green)
        };

        AnsiConsole.Write(successPanel);
        AnsiConsole.MarkupLine("[bold green]✅ QUEST CREATED SUCCESSFULLY 📜[/]");
        AnsiConsole.MarkupLine("\n[grey]Press any key to return to the guild hall...[/]");
        Console.ReadKey();
        return;
    }

    // Visa alla uppdrag som tillhör en användare
    public static void ShowQuests(User user)
    {
        Console.Clear();

        var questLogPanel = new Panel(
            "[bold yellow]📜 QUEST LOG 📜[/]\n" +
            "[italic]Your Heroic Journey Awaits...[/]"
        )
        {
            Header = new PanelHeader("[bold cyan]🗺️ ADVENTURER'S QUEST LOG 🗺️[/]", Justify.Center),
            Border = BoxBorder.Double,
            BorderStyle = new Style(Color.Blue)
        };

        AnsiConsole.Write(questLogPanel);

        if (user.Quests.Count == 0)
        {
            var emptyPanel = new Panel(
                "[bold yellow]🌟 No Quests Yet, Brave Adventurer! 🌟[/]\n\n" +
                "[italic]Your Heroic Journey Begins When You Accept Your First Quest...[/]"
            )
            {
                Header = new PanelHeader("[bold yellow]📜 QUEST LOG EMPTY 📜[/]", Justify.Center),
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Yellow)
            };

            AnsiConsole.Write(emptyPanel);
            AnsiConsole.MarkupLine("You Have No Quests");
            return;
        }

        // Skapa en tabell för att visa uppdragen
        var questTable = new Table()
            .AddColumn(new TableColumn("[bold]Status[/]").Centered())
            .AddColumn(new TableColumn("[bold]Quest Title[/]").LeftAligned())
            .AddColumn(new TableColumn("[bold]Priority[/]").Centered())
            .AddColumn(new TableColumn("[bold]Deadline[/]").Centered());

        questTable.Border = TableBorder.Double;
        questTable.BorderStyle = new Style(Color.Grey);

        // Lista alla uppdrag för användaren
        foreach (var q in user.Quests)
        {
            var status = q.IsCompleted ? "[bold green]✅ Completed[/]" : "[bold yellow]⚔️ Current[/]";
            var priorityColor = q.Priority switch
            {
                "High" => "red",
                "Medium" => "yellow",
                "Low" => "green",
                _ => "white"
            };
            var priorityIcon = q.Priority switch
            {
                "High" => "🔥",
                "Medium" => "⚡",
                "Low" => "🌱",
                _ => "📋"
            };

            questTable.AddRow(
                status,
                $"[bold]{q.Title}[/]\n[dim]{q.Description}[/]",
                $"[{priorityColor}]{priorityIcon} {q.Priority}[/]",
                $"[bold]{q.DueDate:d}[/]"
            );
        }

        AnsiConsole.Write(questTable);
    }

    // Markera ett uppdrag som klart
    public static void CompleteQuest(User user)
    {
        AnsiConsole.MarkupLine("[bold yellow]Write The Title Of The Quest You Want To Complete: ");
        var title = Console.ReadLine()!;
        var quest = user.Quests.Find(q => q.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        if (quest == null)
        {
            Console.WriteLine("No Quest Found.");
            return;
        }
        quest.IsCompleted = true;
        Console.WriteLine("The Quest Has Been Completed!");
    }


    // Uppdatera ett befintligt uppdrag
    public static void UpdateQuest(User user)
    {
        if (user.Quests.Count == 0)
        {
            Console.WriteLine("No Quests To Update.");
            return;
        }

        //Visar alla uppdrag först
        AnsiConsole.MarkupLine("\n[bold green]Your Current Quests:[/]");
        ShowQuests(user);

        // Fråga vilket uppdrag som ska uppdateras
        AnsiConsole.MarkupLine("\n[bold yellow]Write The Title Of The Quest You Want To Update: [/]");
        var title = Console.ReadLine()!;
        var quest = user.Quests.Find(q => q.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        if (quest == null)
        {
            Console.WriteLine("The Quest Could Not Be Found....");
            return;
        }

        // Uppdatera titeln
        Console.Write("New title (leave blank to keep current): ");
        var newTitle = Console.ReadLine()!;
        if (!string.IsNullOrWhiteSpace(newTitle))
            quest.Title = newTitle;

        // Uppdatera beskrivningen
        Console.Write("New Description (leave blank to keep current): ");
        var newDesc = Console.ReadLine()!;
        if (!string.IsNullOrWhiteSpace(newDesc))
            quest.Description = newDesc;

        // Uppdatera deadline
        Console.Write("New Deadline (yyyy-mm-dd, leave blank to keep current): ");
        var newDeadlineInput = Console.ReadLine()!;
        if (DateTime.TryParse(newDeadlineInput, out var newDeadline))
            quest.DueDate = newDeadline;

        // Uppdatera prioritet
        Console.Write("New Priority (Low, Medium, High, leave blank to keep current): ");
        var newPriority = Console.ReadLine()!;
        if (!string.IsNullOrWhiteSpace(newPriority))
            quest.Priority = newPriority;

        Console.WriteLine("The Quest Has Been Updated");
    }


    // Kontrollera om några uppdrag närmar sig deadline och skicka notifikationer
    public static void CheckForUpcomingDeadlines(User user)
    {
        var soonDue = user.Quests
            .Where(q => !q.IsCompleted && q.DueDate > DateTime.Now && q.DueDate <= DateTime.Now.AddHours(24))
            .ToList();

        if (soonDue.Count == 0)
        {
            Console.WriteLine("No Quests Are Approaching Their Deadline Within 24 Hours.");
            return;
        }

        Console.WriteLine("Quests Approaching Deadline:");
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
        int underControl = activeQuests - urgentQuests;

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Grey)
            .AddColumn("[bold blue]Quest Type[/]")
            .AddColumn("[bold yellow]Count[/]")
            .AddColumn("[bold white]Description[/]");

        // Add data rows
        table.AddRow("🔥 [bold green]Active[/]", $"{activeQuests}", "Currently in progress");
        table.AddRow("⚠️ [bold red]Urgent[/]", $"{urgentQuests}", "Due within 24 hours!");
        table.AddRow("🕊 [bold yellow]Under Control[/]", $"{underControl}", "Safe and manageable");
        table.AddRow("🏁 [blue]Completed[/]", $"{completedQuests}", "Successfully finished");

        // Render the table
        AnsiConsole.Write(table);
    }
}