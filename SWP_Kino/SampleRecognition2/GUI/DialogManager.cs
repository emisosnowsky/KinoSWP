using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using Microsoft.Speech.Recognition.SrgsGrammar;
using System.Xml;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;


namespace GUI
{
    public class ArgumentyRozpoznania : EventArgs
    {
        public float Confidence;
        public string Text;
    }

    public class DialogManager
    {
        public static SpeechRecognitionEngine recognizer;
        XmlDocument vxmlDocument;
        XmlDocument srgsDocument;
        Parser parser;
        Database database;
        Synthesis syn;
        String[] rezerwacjaInfo;
        Boolean pRunnig;
        Task taskDialog;
        private String dataRezerwacji;
        private Boolean ClickFlague;

        public DialogManager()
        {
            syn = new Synthesis();
            rezerwacjaInfo = new String[3];
            database = new Database();
            database.Connect();
            vxmlDocument = new XmlDocument();
            srgsDocument = new XmlDocument();
            parser = new Parser(vxmlDocument);
            System.Globalization.CultureInfo x = new System.Globalization.CultureInfo("pl-PL");
            recognizer = new SpeechRecognitionEngine(x);
            vxmlDocument.Load(@"XmlDocuments\MainDialog.xml");
            pRunnig = true;
            ClickFlague = false;

        }


        public event EventHandler<ArgumentyRozpoznania> WyswietlTekst;

        public void Start()
        {
            recognizer.SetInputToDefaultAudioDevice();

            Action<object> action = (object obj) =>
            {
                while (pRunnig == true)
                {
                    Talk();

                }
            };

            // Create a task but do not start it.
            taskDialog = new Task(action, "alpha");
            
            taskDialog.Start();
            
        }
        public void Talk()
        {
            syn.Speak(parser.ParsePrompt());

            if (parser.GetGrammarPath().Equals(""))
            {
                if (parser.GetFieldName() == "Repertuar")
                {
                    ObsluzRepertuar();
                }
                ObsluzZakonczenie();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Oczekiwanie na odpowiedź użytkownika... ");

                if (parser.GetFieldName() == "CzasRezerwacji")
                {
                    ObsluzCzasRezerwacji();
                }
                else
                {
                    recognizer.LoadGrammar(new Grammar(@parser.GetGrammarPath()));
                    RecognitionResult wynik = recognizer.Recognize();

                    if (wynik == null || wynik.Confidence < 0.5)
                    {
                        syn.Speak(parser.ParseNomatch());
                    }
                    else
                    {
                        ObsluzPrzejscie(wynik);
                    }

                }

            }
            if (parser.GetFieldName() == "WynikRezerwacji")
            {
                WyswietlTekst?.Invoke(this, new ArgumentyRozpoznania() { Text = "WynikRezerwacji", Confidence = 0.9F });
                ObsluzPrzypadki(parser.GetFieldName(), "");
                syn.SayGoodbye();
                pRunnig = false;
            }
        }

        private void ObsluzZakonczenie()
        {
            ObsluzPrzypadki(parser.GetFieldName(), "");
            pRunnig = false;
            WyswietlTekst?.Invoke(this, new ArgumentyRozpoznania() { Text = "Zakonczenie", Confidence = 0.9F });
            syn.SayGoodbye();
        }
        private void ObsluzCzasRezerwacji()
        {
            ObsluzPrzypadki(parser.GetFieldName(), "");
            WyswietlTekst?.Invoke(this, new ArgumentyRozpoznania() { Text = getDate(), Confidence = 1 });
            String pathToXml = parser.GetNext("CzasRezerwacji");
            vxmlDocument.Load(@pathToXml);
        }
        private void ObsluzPrzejscie(RecognitionResult wynik)
        {
            ObsluzPrzypadki(parser.GetFieldName(), wynik.Semantics.Value.ToString());
            WyswietlTekst?.Invoke(this, new ArgumentyRozpoznania() { Text = wynik.Semantics.Value.ToString(), Confidence = wynik.Confidence });
            String pathToXml = parser.GetNext(wynik.Semantics.Value.ToString());
            vxmlDocument.Load(@pathToXml);
        }


        private void ObsluzRepertuar()
        {
            syn.Speak("Dostępne filmy to: ");
            syn.Speak(getFilms());
            syn.SayGoodbye();
        }

        public void GetNextFile(String text)
        {
            String pathToXml = parser.GetNext(text);
            vxmlDocument.Load(@pathToXml);
        }

        public void ObsluzPrzypadki(String fieldName, String text)
        {

            switch (fieldName)
            {
                case "CzasRezerwacji":
                    while (!ClickFlague){}
                    rezerwacjaInfo[0] = dataRezerwacji;

                    break;
                case "RezerwacjaFilm":
                    rezerwacjaInfo[1] = text;
                    break;
                case "LiczbaBiletow":
                    rezerwacjaInfo[2] = text;
                    database.AddReservationToDabase(rezerwacjaInfo[0], Int32.Parse(rezerwacjaInfo[1]), Int32.Parse(rezerwacjaInfo[2]));
                    break;
                case "WynikRezerwacji":
                    String result = getReservation();
                    syn.Speak(result);
                    break;
            }

        }
        

        public void setDate(String date)
        {
            if (!date.Equals(null))
            {
                dataRezerwacji = date;
                System.Console.WriteLine("Nasza data :" + date);
            }
        }

        private String getDate()
        {
            return dataRezerwacji;
        }

        public void changeClickFlagState()
        {
            ClickFlague = true;
        }

        public String getFilms()
        {
            StringBuilder films = new StringBuilder();
            foreach (String s in database.SelectAllFilms())
            {
                films.Append(s.ToString());
                films.Append("\n");
            }
            return films.ToString();
        }

        public String getReservation()
        {
            String bilet = "";
            switch (rezerwacjaInfo[2])
            {
                case "1":
                    bilet = " bilet ";
                    break;
                case "5":
                    bilet = " biletów ";
                    break;
                default:
                    bilet = " bilety ";
                    break;
            }

            return "Zarezerwowałeś " + rezerwacjaInfo[2] + bilet + " na " + rezerwacjaInfo[0] + "r. na film " + database.GetFilm(Int32.Parse(rezerwacjaInfo[1])) + ".";


        }

        static void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            String userInput = string.Format("ROZPOZNANO: {0}\n\tCONFIDENCE: {1}", e.Result.Text, e.Result.Confidence);
            System.Diagnostics.Debug.WriteLine(userInput);
            Console.WriteLine("WYSŁUCHANO: " + e.Result.Text);
            //obsluz(e.Result.Text);

        }
    }

}
