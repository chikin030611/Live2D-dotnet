using System;
using System.IO;
using System.Media;
using System.Collections.Generic;
using ColorMC.Gui.UI.Controls.Main;
using Silk.NET.SDL;
using ColorMC.Gui.AudioPlayer;
using Live2DCSharpSDK.App;
using ColorMC.Gui.Avatar;

namespace ColorMC.Gui.UI.Flyouts;

public class Live2DFlyout
{
    public Live2DFlyout(Live2dRender live2d)
    {
        QnAController controller = new QnAController(live2d);

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
            ("Speak intro", true, (Action)(() => controller.StartLive2dSpeaking(0)))
        };

        // Add questions dynamically
        QnAMapper qnAMapper = new QnAMapper();
        int numberOfQuestions = qnAMapper.GetNumOfQuestions() - 1;
        for (int i = 1; i <= numberOfQuestions; i++)
        {
            int questionNumber = i; // Capture the loop variable
            flyoutItems.Add(($"Question {questionNumber}", true, (Action)(() => controller.StartLive2dSpeaking(questionNumber))));
        }

        _ = new FlyoutsControl([.. flyoutItems], live2d);
    }
}
