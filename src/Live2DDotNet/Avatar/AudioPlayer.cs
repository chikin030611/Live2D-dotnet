using System.Media;

namespace Live2DDotNet.Avatar;

public static class AudioPlayer
{
    public static void PlayAudio(string path)
    {
        try
        {
            using (SoundPlayer player = new SoundPlayer(path))
            {
                player.Load();
                player.Play();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error playing audio: {ex.Message}");
        }
    }
}
