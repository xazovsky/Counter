using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Maui.Storage;

namespace Counter
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Counter> Counters { get; set; } = new ObservableCollection<Counter>();

        private string filePath = Path.Combine(FileSystem.AppDataDirectory, "counters.txt");

        public MainPage()
        {
            InitializeComponent();
            CounterList.ItemsSource = Counters;
            LoadCounters();
        }
        private void LoadCounters()
        {
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    var parts = line.Split(';');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int value))
                    {
                        Counters.Add(new Counter { Name = parts[0], Value = value });
                    }
                }
            }
        }
        private void SaveCounters()
        {
            var lines = Counters.Select(counter => $"{counter.Name};{counter.Value}");
            File.WriteAllLines(filePath, lines);
        }
        private void OnAddCounterClicked(object sender, EventArgs e)
        {
            var counterName = CounterNameEntry.Text;
            int initialValue = 0;
            if (!string.IsNullOrWhiteSpace(InitialValueEntry.Text))
            {
                if (!int.TryParse(InitialValueEntry.Text, out initialValue))
                {
                    DisplayAlert("Błąd", "Wprowadź prawidłową wartość.", "OK");
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(counterName))
            {
                Counters.Add(new Counter { Name = counterName, Value = initialValue });
                CounterNameEntry.Text = string.Empty; // Czyści pole po dodaniu licznika
                InitialValueEntry.Text = string.Empty; // Czyści pole po dodaniu licznika
                SaveCounters();
            }
            else
            {
                DisplayAlert("Błąd", "Wprowadź nazwę.\nhttps://www.youtube.com/watch?v=dQw4w9WgXcQ", "OK");
            }
        }
        private void OnPlusClicked(object sender, EventArgs e)
        {
            var counter = (sender as Button).CommandParameter as Counter;
            counter.Value++;
            SaveCounters();
        }
        private void OnMinusClicked(object sender, EventArgs e)
        {
            var counter = (sender as Button).CommandParameter as Counter;
            counter.Value--;
            SaveCounters();
        }
    }
    public class Counter : INotifyPropertyChanged
    {
        private int _value;

        public string Name { get; set; }
        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
