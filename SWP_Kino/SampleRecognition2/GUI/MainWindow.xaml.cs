using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace GUI
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    /// 
    delegate void DelegateWyswietl(string tekst, float confidence);

    public partial class MainWindow : Window
    {
        
        DialogManager manager;
        public MainWindow()
        {
                InitializeComponent();
                manager = new DialogManager();
                manager.WyswietlTekst += Manager_WyswietlTekst;
                ObslugaGUI += WyswietlWynik;
        }

        DelegateWyswietl ObslugaGUI;

        void WyswietlWynik(string tekst, float confidence)
        {
            txtTest.Text = string.Format("ROZPOZNANO: {0}\nCONFIDENCE: {1}", tekst, confidence);
            if (tekst == "Dokonać rezerwacji")
            {
                CalendarBox.Visibility = Visibility.Visible;
            }
            if (confidence == 1)
            {
                TextBox2.Visibility = Visibility.Visible;
                Seanse.Visibility = Visibility.Visible;
                Zarezerwowano.Visibility = ~Visibility.Visible;
                TextBox2.Text = manager.getFilms();
            }
            if (tekst == "Informacje o repertuarze")
            {
                TextBox2.Visibility = Visibility.Visible;
                Seanse.Visibility = Visibility.Visible;
                TextBox2.Text = manager.getFilms();
            }

            if(tekst == "WynikRezerwacji")
            {
                TextBox2.Visibility = Visibility.Visible;
                Seanse.Visibility = ~Visibility.Visible;
                Zarezerwowano.Visibility = Visibility.Visible;
                TextBox2.Text = manager.getReservation();
            }
            if (tekst == "Zakonczenie")
            {
                Close();
            }

        }


        public void Manager_WyswietlTekst(object sender, ArgumentyRozpoznania e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, ObslugaGUI, e.Text, e.Confidence);
        }

        private void txtTest_Loaded(object sender, RoutedEventArgs e)
        {

            manager.Start();
            CalendarBox.Visibility = ~Visibility.Visible;
            Seanse.Visibility = ~Visibility.Visible;
            Zarezerwowano.Visibility = ~Visibility.Visible;
            TextBox2.Visibility = ~Visibility.Visible;

        }


        private void CalendarBox_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get reference.
            var calendar = sender as Calendar;

            calendar.Visibility = ~Visibility.Visible;


            //Calendar.VisibilityProperty.= false;


            // ... See if a date is selected.
            if (calendar.SelectedDate.HasValue)
            {
                // ... Display SelectedDate in Title.
                DateTime date = calendar.SelectedDate.Value;
                //this.Title = date.ToShortDateString();
                Console.WriteLine("Wybrana data: " + date.ToLongDateString());

                manager.setDate(date.ToShortDateString());
                manager.changeClickFlagState();


            }
        }

        private void TextBlock_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = "The text contents of this TextBlock.";
        }

        private void CalendarBox_DisplayModeChanged(object sender, CalendarModeChangedEventArgs e)
        {
            var calendar = sender as Calendar;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            Close();
        }
    }
}
