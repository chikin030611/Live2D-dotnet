using System.Media;

namespace Live2DDotNet.Avatar;

public static class AudioPlayer
{
    static readonly SoundPlayer player = new();

    public static void PlayAudio(string path)
    {
        try
        {
            player.SoundLocation = path;
            player.Load();
            player.Play();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error playing audio: {ex.Message}");
        }
    }

    public static void StopAudio() {
        try
        {
            player.Stop();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error stopping audio: {ex.Message}");
        }
    }
}
