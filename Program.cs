using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

internal class Program
{
    private static Process? _audioProcess;
    private static CancellationTokenSource? _cts;

    private static async Task Main(string[] args)
    {
        try
        {
            // Start song loop
            _cts = new CancellationTokenSource();
            _ = LoopSongAsync("Resources/song.wav", _cts.Token);

            // Start your menu
            await Menu.Start();
        }
        finally
        {
            // Stop the song when program exits
            StopSong();
        }
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
