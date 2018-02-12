using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;

namespace GUI
{
    public class Synthesis
    {
        public static SpeechSynthesizer engTTS;

        public Synthesis()
        {
            engTTS = new SpeechSynthesizer();

            engTTS.SetOutputToDefaultAudioDevice();
            engTTS.StateChanged += new EventHandler<StateChangedEventArgs>(EngTTS_StateChanged);
        }

        public void Speak(String text)
        {
            engTTS.Speak(text);
        }

        public void SayGoodbye()
        {
            engTTS.Speak("Do zobaczenia");
        }

        static void EngTTS_StateChanged(object sender, StateChangedEventArgs e)
        {
            // TODO : implement action when Speaker is saying sth
        }

    }
}
