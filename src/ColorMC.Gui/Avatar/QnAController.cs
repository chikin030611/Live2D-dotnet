using ColorMC.Gui.AudioPlayer;
using ColorMC.Gui.UI.Controls.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorMC.Gui.Avatar;

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

        AudioPlayer.AudioPlayer.PlayAudio(path);
        live2d.StartSpeaking(path);
    }
}
