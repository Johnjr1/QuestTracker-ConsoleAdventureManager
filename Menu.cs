using Spectre.Console;
public static class Menu
{

    // Startmeny för Quest Tracker
    public static async Task Start()
    {
        while (true)
        {
            Console.Clear(); // Clear console för bättre läsbarhet

            // Header panel
            var headerPanel = new Panel(
                "[bold yellow]⚔️ QUEST GUILD TERMINAL ⚔️[/]\n" +
                "[italic]Where heroes are forged and legends are born...[/]"
            )
            {
                Header = new PanelHeader("[bold cyan]🏰 GUILD HALLS 🏰[/]", Justify.Center),
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Blue)
            };

            AnsiConsole.Write(headerPanel);

            // Start menyn med styling
            var menu = new SelectionPrompt<string>()
                .Title("[bold magenta]🏰 Choose your destiny, brave adventurer:[/]")
                .AddChoices(new[] {
                    "[bold green]⚔️  Register hero[/]",
                    "[bold blue]🔑  Log in[/]",
                    "[bold red]🚪  Exit[/]"
                })
                .HighlightStyle(new Style(Color.White, decoration: Decoration.Bold));

            var choice = AnsiConsole.Prompt(menu);

            switch (choice)
            {
                case "[bold green]⚔️  Register hero[/]":
                    User.Register();
                    break;
                case "[bold blue]🔑  Log in[/]":
                    if (User.Login()) await HeroMenu();
                    break;
                case "[bold red]🚪  Exit[/]":
                    ShowFarewellMessage();
                    return;
            }
        }
    }

    private static void ShowFarewellMessage()
    {
        Console.Clear();
        var farewellPanel = new Panel(
            "[bold yellow]May your adventures be legendary, brave soul![/]\n\n" +
            "[italic]The Guild awaits your return...[/]"
        )
        {
            Header = new PanelHeader("[bold red]👋 Farewell, Adventurer! 👋[/]", Justify.Center),
            Border = BoxBorder.Double,
            BorderStyle = new Style(Color.Red)
        };

        AnsiConsole.Write(farewellPanel);
        AnsiConsole.MarkupLine("\n[grey]Press any key to exit the realm...[/]");
        Console.ReadKey(true);
    }

    // Hjälte-meny efter inloggning
    private static async Task HeroMenu()
    {
        var user = User.LoggedInUser!;
        while (true)
        {
            Console.Clear(); // Clear console för bättre läsbarhet

            // Guild header med styling
            var guildHeader = new Panel(
                $"[bold yellow]🏰 GUILD MENY — {user.Username.ToUpper()} 🏰[/]\n" +
                $"[italic]Welcome back, brave adventurer! Your quests await...[/]"
            )
            {
                Header = new PanelHeader("[bold cyan]⚔️ ADVENTURER'S DASHBOARD ⚔️[/]", Justify.Center),
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Blue)
            };

            AnsiConsole.Write(guildHeader);

            // Visa sammanfattning av uppdrag
            var activeQuests = user.Quests.Count(q => !q.IsCompleted);
            var completedQuests = user.Quests.Count(q => q.IsCompleted);
            var urgentQuests = user.Quests.Count(q => !q.IsCompleted && q.DueDate <= DateTime.Now.AddHours(24));

            var statusTable = new Table()
                .AddColumn(new TableColumn("[bold green]Active Quests[/]").Centered())
                .AddColumn(new TableColumn("[bold yellow]Completed[/]").Centered())
                .AddColumn(new TableColumn("[bold red]Urgent[/]").Centered())
                .AddRow($"[green]{activeQuests}[/]", $"[yellow]{completedQuests}[/]", $"[red]{urgentQuests}[/]");

            statusTable.Border = TableBorder.Rounded;
            statusTable.BorderStyle = new Style(Color.Grey);

            AnsiConsole.Write(statusTable);

            // Huvudmenyval med styling
            var menu = new SelectionPrompt<string>()
                .Title("[bold magenta]🎯 What shall you do, brave hero?[/]")
                .AddChoices(new[] {
                    "[bold green]📜  Add a Quest[/]",
                    "[bold blue]🗺️  Show Your Quests[/]",
                    "[bold yellow]✅  Complete a Quest[/]",
                    "[bold cyan]✏️  Update a Quest[/]",
                    "[bold purple]🧙‍♂️  Guild Advisor (AI)[/]",
                    "[bold yellow]📊  Guild report[/]",
                    "[bold red]🚪  Log out[/]"
                });

            var choice = AnsiConsole.Prompt(menu);

            switch (choice)
            {
                case "[bold green]📜  Add a Quest[/]":
                    await QuestManager.AddQuest(user);
                    break;
                case "[bold blue]🗺️  Show Your Quests[/]":
                    QuestManager.ShowQuests(user);
                    AnsiConsole.MarkupLine("\n[grey]Press any key to exit the realm...[/]");
                    Console.ReadKey(true);
                    break;
                case "[bold yellow]✅  Complete a Quest[/]":
                    QuestManager.CompleteQuest(user);
                    AnsiConsole.MarkupLine("\n[grey]Press any key to exit the realm...[/]");
                    Console.ReadKey(true);
                    break;
                case "[bold cyan]✏️  Update a Quest[/]":
                    QuestManager.UpdateQuest(user);
                    AnsiConsole.MarkupLine("\n[grey]Press any key to exit the realm...[/]");
                    Console.ReadKey(true);
                    break;
                case "[bold purple]🧙‍♂️  Guild Advisor (AI)[/]":
                    await ConsultGuildAdvisor(user);
                    break;
                case "[bold yellow]📊  Guild report[/]":
                    QuestManager.ShowFullQuestReport(user);
                    AnsiConsole.MarkupLine("\n[grey]Press any key to exit the realm...[/]");
                    Console.ReadKey(true);
                    break;
                case "[bold red]🚪  Log out[/]":
                    User.Logout();
                    return;
            }
        }
    }

    private static async Task ConsultGuildAdvisor(User user)
    {
        Console.Clear();

        var advisorPanel = new Panel(
            "[bold purple]🧙‍♂️ The Guild Advisor awaits your request...[/]\n" +
            "[italic]Speak your quest title, and I shall craft an epic adventure for you![/]"
        )
        {
            Header = new PanelHeader("[bold purple]🔮 GUILD ADVISOR CHAMBER 🔮[/]", Justify.Center),
            Border = BoxBorder.Double,
            BorderStyle = new Style(Color.Purple)
        };

        AnsiConsole.Write(advisorPanel);

        Console.Write("Choose a Quest Title: ");
        var title = Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(title))
        {
            AnsiConsole.MarkupLine("[red]The advisor requires a quest title to work their magic![/]");
            AnsiConsole.MarkupLine("\n[grey]Press any key to return to the guild hall...[/]");
            Console.ReadKey(true);
            return;
        }

        // Laddar AI-processen med statusindikator
        AnsiConsole.Status()
            .Start("The Guild Advisor is consulting ancient tomes...", ctx =>
            {
                Thread.Sleep(2000);
            });

        var generatedQuest = await GuildAdvisorAI.GenerateQuestFromTitle(title);

        if (generatedQuest != null)
        {
            var questPanel = new Panel(
                $"[bold green]✨ The Guild Advisor has crafted your quest! ✨[/]\n\n" +
                $"[bold]Titel:[/] {generatedQuest.Title}\n" +
                $"[bold]Beskrivning:[/] {generatedQuest.Description}\n" +
                $"[bold]Prioritet:[/] {generatedQuest.Priority}\n" +
                $"[bold]Deadline:[/] {generatedQuest.DueDate:d}"
            )
            {
                Header = new PanelHeader("[bold green]📜 NEW QUEST CREATED 📜[/]", Justify.Center),
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Green)
            };

            AnsiConsole.Write(questPanel);

            // Lägg till i användarens quest-lista
            user.Quests.Add(generatedQuest);
            Console.WriteLine("\nThe Quest Has Been Added to Your Quest Log!");
        }
        else
        {
            Console.WriteLine("AI Could Not Create The Quest, Try Again...");
        }

        AnsiConsole.MarkupLine("\n[grey]Press any key to return to the guild hall...[/]");
        Console.ReadKey(true);
    }
}