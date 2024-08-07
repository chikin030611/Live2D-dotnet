using Live2DDotNet.UI.Controls.Main;
namespace Live2DDotNet.Avatar;

public class QnAController
{
    Live2dRender live2d;

    public QnAController(Live2dRender live2d)
    {
        this.live2d = live2d;
    }

    public void StartLive2dSpeaking(int qnum)
    {
        QnAMapper qnAMapper = new QnAMapper();
        string path = qnAMapper.GetAudioFilePath(qnum);

        AudioPlayer.PlayAudio(path);
        live2d.StartSpeaking(path);
    }
}
