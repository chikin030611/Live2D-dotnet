using System;
using System.IO;
using System.Media;
using System.Collections.Generic;
using ColorMC.Gui.UI.Controls.Main;
using Silk.NET.SDL;
using ColorMC.Gui.AudioPlayer;
using Live2DCSharpSDK.App;

namespace ColorMC.Gui.UI.Flyouts;

public class Live2DFlyout
{
    private void PlayAudio(Live2dRender live2d, int qnum)
    {
        // Parameter: location of audio file
        // Find file path of audio according question number
        QnAMapper qnAMapper = new QnAMapper();
        string path = qnAMapper.GetAudioFilePath(qnum);

        // TODO: find model.lappwavfilehandler
        live2d.StartSpeaking(path);
    }

    public Live2DFlyout(Live2dRender live2d)
    {
        var flyoutItems = new List<(string, bool, Action)>
        {
            (App.Lang("Live2dControl.Flyouts.Text1"), true, () =>
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
            (App.Lang("Live2dControl.Flyouts.Text2"), true, () =>
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
            ("Speak intro", true, () => PlayAudio(live2d, 0))
        };

        // Add questions dynamically
        QnAMapper qnAMapper = new QnAMapper();
        int numberOfQuestions = qnAMapper.GetNumOfQuestions() - 1;
        for (int i = 1; i <= numberOfQuestions; i++)
        {
            int questionNumber = i; // Capture the loop variable
            flyoutItems.Add(($"Question {questionNumber}", true, () => PlayAudio(live2d, questionNumber)));
        }

        _ = new FlyoutsControl([.. flyoutItems], live2d);
    }
}
