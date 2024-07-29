using System;
using System.IO;
using System.Media;
using System.Collections.Generic;
using ColorMC.Gui.UI.Controls.Main;
using Silk.NET.SDL;

namespace ColorMC.Gui.UI.Flyouts;

public class Live2DFlyout
{   
    private void PlayIntroAudio()
    {
        try
        {
            // Get the base directory of the application
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Construct the relative path to the audio file
            string audioFilePath = Path.Combine(baseDirectory, "Resource", "Audio", "intro.wav");

            using (SoundPlayer player = new SoundPlayer(audioFilePath))
            {
                player.Load();
                player.Play();
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., file not found, format issues)
            Console.WriteLine($"Error playing audio: {ex.Message}");
        }
    }

    public Live2DFlyout(Live2dRender live2d)
    {
        _ = new FlyoutsControl(
        [
            (App.Lang("Live2dControl.Flyouts.Text1"), true, ()=>
            {
                var list = live2d.GetMotions();
                if (list.Count != 0)
                {
                    var list1 = new List<(string, bool, Action)>();
                    list.ForEach(item =>
                    {
                        list1.Add((item, true, () => live2d.PlayMotion(item)));
                    });
                    _ = new FlyoutsControl([.. list1], live2d);
                }
            }),
            (App.Lang("Live2dControl.Flyouts.Text2"), true, ()=>
            {
                var list = live2d.GetExpressions();
                if (list.Count != 0)
                {
                    var list1 = new List<(string, bool, Action)>();
                    list.ForEach(item =>
                    {
                        list1.Add((item, true, () => live2d.PlayExpression(item)));
                    });
                    _ = new FlyoutsControl([.. list1], live2d);
                }
            }),
            ("Speak intro", true, ()=>
            {
                PlayIntroAudio();
            })
        ], live2d);
    }
}
