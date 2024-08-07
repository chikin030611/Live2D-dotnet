using System;
using System.IO;
using System.Media;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
