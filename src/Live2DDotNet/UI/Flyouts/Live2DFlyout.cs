using Live2DDotNet.Manager;
using Live2DDotNet.Objs;
using Live2DDotNet.UI.Controls.Main;

namespace Live2DDotNet.UI.Flyouts;

/// <summary>
/// Represents a flyout control for Live2D animations and expressions.
/// </summary>
public class Live2DFlyout
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Live2DFlyout"/> class.
    /// </summary>
    /// <param name="live2d">The Live2D renderer.</param>
    public Live2DFlyout(Live2dRender live2d)
    {
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
            };

        // Add questions dynamically
        QnaObj[] qnaList = QnaAudioManager.QnaList;
        int numberOfQuestions = qnaList.Length;
        int startId = qnaList[0].Id;
        for (int i = startId; i < numberOfQuestions; i++)
        {
            int questionNumber = i;
            string qnaQuestion = qnaList[questionNumber].Question;

            // Add a new question to the flyout
            flyoutItems.Add((qnaQuestion, true, () => live2d.StartSpeaking(questionNumber)));
        }

        _ = new FlyoutsControl([.. flyoutItems], live2d);
    }
}
