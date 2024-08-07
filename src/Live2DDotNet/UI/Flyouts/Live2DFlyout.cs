using Live2DDotNet.Avatar;
using Live2DDotNet.UI.Controls.Main;

namespace Live2DDotNet.UI.Flyouts;

public class Live2DFlyout
{
    public Live2DFlyout(Live2dRender live2d)
    {
        QnAController controller = new QnAController(live2d);

        var flyoutItems = new List<(string, bool, Action)>
        {
            ("Play animation", true, () =>
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
            ("Toggle Emote", true, () =>
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
            ("Speak intro", true, () => controller.StartLive2dSpeaking(0))
        };

        // Add questions dynamically
        QnAMapper qnAMapper = new QnAMapper();
        int numberOfQuestions = qnAMapper.GetNumOfQuestions() - 1;
        for (int i = 1; i <= numberOfQuestions; i++)
        {
            int questionNumber = i; // Capture the loop variable
            flyoutItems.Add(($"Question {questionNumber}", true, () => controller.StartLive2dSpeaking(questionNumber)));
        }

        _ = new FlyoutsControl([.. flyoutItems], live2d);
    }
}
