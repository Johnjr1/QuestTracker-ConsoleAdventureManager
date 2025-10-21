using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;

internal class Program
{
    private static Process? _audioProcess;
    private static CancellationTokenSource? _cts;

    private static async Task Main(string[] args)
    {
        try
        {
            // Starta låten i bakgrunden
            _cts = new CancellationTokenSource();
            _ = LoopSongAsync("Resources/song.wav", _cts.Token);

            //Startup sekvens
            ShowStartupSequence();
            
            // Starta menyn
            await Menu.Start();
        }
        finally
        {
            // Stänger av låten så den inte fortsätter spela
            StopSong();
        }
    }

    private static void ShowStartupSequence()
    {
        Console.Clear();

        var title = new FigletText("QUEST GUILD")
            .Color(Color.Yellow)
            .Centered();
        
        var subtitle = new FigletText("ADVENTURE TERMINAL")
            .Color(Color.Blue)
            .Centered();
        
        AnsiConsole.Write(title);
        AnsiConsole.Write(subtitle);
        
        AnsiConsole.Status()
            .Start("Initializing the Guild Halls...", ctx =>
            {
                Thread.Sleep(1000);
                ctx.Status("Awakening the Guild Advisor...");
                Thread.Sleep(1000);
                ctx.Status("Preparing quest scrolls...");
                Thread.Sleep(1000);
                ctx.Status("Lighting the torches...");
                Thread.Sleep(1000);
                ctx.Status("The Guild is ready for your arrival!");
                Thread.Sleep(500);
            });
        
        // Välkomstmeddelande
        var welcomePanel = new Panel(
            "[bold yellow]🌟 Welcome to the Quest Guild Adventure Terminal! 🌟[/]\n\n" +
            "[italic]Where heroes are forged and legends are born...[/]\n" +
            "Your epic journey begins here, brave adventurer!"
        )
        {
            Header = new PanelHeader("[bold green]⚔️ GUILD HALLS OPEN ⚔️[/]", Justify.Center),
            Border = BoxBorder.Double,
            BorderStyle = new Style(Color.Green)
        };
        
        AnsiConsole.Write(welcomePanel);
        AnsiConsole.MarkupLine("\n[grey]Press any key to enter the guild halls...[/]");
        Console.ReadKey(true);
    }

    private static async Task LoopSongAsync(string filePath, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                _audioProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "afplay",
                        Arguments = $"\"{filePath}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                _audioProcess.Start();
                await _audioProcess.WaitForExitAsync(token);
            }
            catch (OperationCanceledException)
            {
                // Stop requested
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error playing song: " + ex.Message);
                await Task.Delay(1000, token); // Retry after a bit
            }
        }
    }

    private static void StopSong()
    {
        try
        {
            _cts?.Cancel();

            if (_audioProcess != null && !_audioProcess.HasExited)
            {
                _audioProcess.Kill();
                _audioProcess.Dispose();
            }

            _cts?.Dispose();
        }
        catch
        {
            // Ignore cleanup exceptions
        }
    }
}
