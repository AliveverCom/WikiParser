using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;

namespace KnowedgeBox.WinApp
{
    public class GSpeaker
    {
        public const string DefaultVoiceName = "Microsoft Zira Desktop";

        public static SpeechSynthesizer DfrSpeaker;

        static GSpeaker()
        {
            DfrSpeaker = CreateSpeaker_US();

        }

        public static SpeechSynthesizer CreateSpeaker_US()
        {
            var speaker = new SpeechSynthesizer();
            var voiceList = speaker.GetInstalledVoices();

            speaker.SelectVoice("Microsoft Zira Desktop");
            speaker.Rate = 2;

            return speaker;

        }//CreateSpeaker_US()

        public static SpeechSynthesizer CreateSpeaker_Cn()
        {
            var speaker = new SpeechSynthesizer();
            var voiceList = speaker.GetInstalledVoices();

            speaker.SelectVoice("Microsoft Huihui Desktop");
            speaker.Rate = 5;

            return speaker;

        }//CreateSpeaker_US()

        public static void Speak_CN(string _txt)
        {
            CreateSpeaker_Cn().Speak(_txt);
        }

        public static void Speak_US(string _txt)
        {
            CreateSpeaker_US().Speak(_txt);
        }

        public static void Speak(string _txt)
        {
            DfrSpeaker.Speak(_txt);
        }

    }//class GSpeaker
}
