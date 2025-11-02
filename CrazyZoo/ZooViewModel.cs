using CrazyZoo.Properties;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.ComponentModel;
using Prism.Commands; // to use DelegateCommand
using CrazyZoo.Interfaces;
using CrazyZoo.Generics;

// add new animal
namespace CrazyZoo
{
    public class ZooViewModel : INotifyPropertyChanged
    {
        public struct Log
        {
            public Animal Animal { get; }
            public string Information { get; }

            public Log(Animal animal, string information)
            {
                Animal = animal;
                Information = information;
            }
        }

        public Repository<Animal> AnimalsRepository { get; } = new Repository<Animal>();
        public ObservableCollection<Enclosure<Animal>> Enclosures { get; } = new ObservableCollection<Enclosure<Animal>>();

        public DelegateCommand MakeSoundCommand { get; }
        public DelegateCommand ValidateFoodCommand { get; }
        public DelegateCommand FeedCommand { get; }
        public DelegateCommand AddAnimalCommand { get; }
        public DelegateCommand RemoveAnimalCommand { get; }

        private Animal? _selected;
        public Animal? SelectedAnimal
        {
            get => _selected;
            set { _selected = value;
                OnPropertyChanged(nameof(SelectedAnimal));
                AnimalFood = _selected?.PreferableFood ?? string.Empty;
                Enclosure<Animal> enclosure = Enclosures.FirstOrDefault(e => e.Items.Contains(SelectedAnimal));
                SelectedAnimalEnclosureName = enclosure?.Name ?? "This animal walks freely";
            }
        }

        private string? _selectedAnimalEnclosureName;
        public string? SelectedAnimalEnclosureName
        {
            get => _selectedAnimalEnclosureName;
            set
            {
                _selectedAnimalEnclosureName = value;
                OnPropertyChanged(nameof(SelectedAnimalEnclosureName));
            }
        }

        public ObservableCollection<Log> Logs { get; } = new ObservableCollection<Log>();

        private string _animalFood = "";
        public string? AnimalFood
        {
            get => _animalFood;
            set
            {
                _animalFood = value != null ? value : "";
                OnPropertyChanged(nameof(AnimalFood));
                ValidateFood();
            }
        }

        private string _animalFoodError = "";
        public string AnimalFoodError
        {
            get => _animalFoodError;
            set { _animalFoodError = value;
                OnPropertyChanged(nameof(AnimalFoodError)); }
        }

        private ObservableCollection<string> _statistics = new ObservableCollection<string>();
        public ObservableCollection<string> Statistics
        {
            get => _statistics;
            set
            {
                _statistics = value;
                OnPropertyChanged(nameof(Statistics));
            }
        }

        private DispatcherTimer feedCheckTimer;
        private Queue<char> keyBuffer = new Queue<char>();

        public ZooViewModel()
        {
            MakeSoundCommand = new DelegateCommand(MakeSound);
            ValidateFoodCommand = new DelegateCommand(() => ValidateFood());
            FeedCommand = new DelegateCommand(() => Feed());
            AddAnimalCommand = new DelegateCommand(() => AddAnimal());
            RemoveAnimalCommand = new DelegateCommand(() => RemoveAnimal(), () => SelectedAnimal != null);

            Animal lion = new Lion(Resource1.lionName, 2, Resource1.lionDescription);
            Animal zebra = new Zebra(Resource1.zebraName, 4, Resource1.zebraDescription);
            Animal crocodile = new Crocodile(Resource1.crocodileName, 3, Resource1.crocodileDescription);
            Animal monkey = new Monkey(Resource1.monkeyName, 3, Resource1.monkeyDescription);
            Animal owl = new Owl(Resource1.owlName, 10, Resource1.owlDescription);

            AnimalsRepository.Add(lion);
            AnimalsRepository.Add(zebra);
            AnimalsRepository.Add(crocodile);
            AnimalsRepository.Add(monkey);
            AnimalsRepository.Add(owl);

            foreach (var animal in AnimalsRepository.GetAll())
            {
                animal.LogGenerated += log => Logs.Insert(0, new Log(animal, log));
            }

            Enclosure<Animal> savannaVoljeer = new Enclosure<Animal>("Savanna");
            Enclosure<Animal> tropicalVoljeer = new Enclosure<Animal>("Tropics");
            Enclosure<Animal> forestVoljeer = new Enclosure<Animal>("Forest");
            Enclosures.Add(savannaVoljeer);
            Enclosures.Add(tropicalVoljeer);
            Enclosures.Add(forestVoljeer);

            savannaVoljeer.AddSilently(lion);
            savannaVoljeer.AddSilently(zebra);
            tropicalVoljeer.AddSilently(crocodile);
            tropicalVoljeer.AddSilently(monkey);
            forestVoljeer.AddSilently(owl);

            SelectedAnimal = AnimalsRepository.GetAll().FirstOrDefault();
            setFeedingTimer();
            RecalculateStatistics();

        }

        // timer for lions feed time
        private void setFeedingTimer()
        {
            feedCheckTimer = new DispatcherTimer();
            feedCheckTimer.Interval = TimeSpan.FromSeconds(1);
            feedCheckTimer.Tick += FeedCheckTimer_Tick;
            feedCheckTimer.Start();
        }

        private void FeedCheckTimer_Tick(object sender, EventArgs e)
        {
            foreach (var animal in AnimalsRepository.GetAll())
            {
                if (animal is Lion lion)
                {
                    if ((DateTime.Now - lion.LastFedTime).TotalSeconds > lion.DigestTime && lion.HasActedCrazy == false)
                    {
                        Logs.Insert(0, new Log(lion, lion.ActCrazy()));
                    }
                }
            }
        }

        // keys for owl wisdoms
        public void FollowKeysPressed(KeyEventArgs e)
        {
            char pressedChar = KeyToChar(e.Key);

            if (pressedChar != '\0')
            {
                if (keyBuffer.Count == 3)
                    keyBuffer.Dequeue();

                keyBuffer.Enqueue(char.ToLower(pressedChar));

                if (new string(keyBuffer.ToArray()) == Resource1.owlCrazyActionTrigger)
                {
                    foreach (var animal in AnimalsRepository.GetAll())
                    {
                        if (animal is Owl owl)
                        {
                            Logs.Insert(0, new Log(owl, owl.ActCrazy()));
                        }
                    }
                    keyBuffer.Clear();
                }
            }
        }

        public char KeyToChar(Key key)
        {
            if (key >= Key.A && key <= Key.Z)
            {
                return (char)('a' + (key - Key.A));
            }
            return '\0'; // non-letter keys ignored
        }

        public void MakeSound()
        {
            if (SelectedAnimal == null) return;
            string sound = SelectedAnimal.MakeSound();
            Logs.Insert(0, new Log(SelectedAnimal, $"\"{sound}\""));
        }

        public void ValidateFood()
        {
            if (String.IsNullOrEmpty(AnimalFood))
            {
                AnimalFoodError = Resource1.foodEmptyStringError;
            }
            else
            {
                AnimalFoodError = String.Empty;
            }
        }

        public void Feed()
        {
            if (SelectedAnimal == null || String.IsNullOrEmpty(AnimalFood) || !String.IsNullOrEmpty(AnimalFoodError)) return;
            Logs.Insert(0, new Log(SelectedAnimal, SelectedAnimal.EatFood(AnimalFood)));
        }

        public void AddAnimal()
        {

            var addWindow = new AddAnimalWindow(Enclosures);
            bool? result = addWindow.ShowDialog();

            Animal newAnimal = addWindow.NewAnimal;
            Enclosure<Animal> selectedEnclosure = addWindow.SelectedEnclosure;

            if (result == false || newAnimal == null || selectedEnclosure == null) return;

            AnimalsRepository.Add(newAnimal);
            selectedEnclosure.Add(newAnimal);
            SelectedAnimal = newAnimal;

            foreach (var animal in AnimalsRepository.GetAll())
            {
                if (animal is Crocodile crocodile && animal != newAnimal)
                {
                    Logs.Insert(0, new Log(crocodile, crocodile.ActCrazy(newAnimal)));
                }
            }

            RecalculateStatistics();
        }

        public void RemoveAnimal()
        {
            if (SelectedAnimal == null) return;
            AnimalsRepository.Remove(SelectedAnimal);

            SelectedAnimal = AnimalsRepository.GetAll().FirstOrDefault(); // FirstOrDefault in Repository class
            RecalculateStatistics();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged(string n) 
            => PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(n));

        private void RecalculateStatistics()
        {
            int crocodileNumber = AnimalsRepository.GetAll().Count(a => a.Type == "Crocodile");
            double crocodileAverageAge = AnimalsRepository.GetAll().Where(a => a.Type == "Crocodile").Average(a => a.Age);
            int lionNumber = AnimalsRepository.GetAll().Count(a => a.Type == "Lion");
            double lionAverageAge = AnimalsRepository.GetAll().Where(a => a.Type == "Lion").Average(a => a.Age);
            int monkeyNumber = AnimalsRepository.GetAll().Count(a => a.Type == "Monkey");
            double monkeyAverageAge = AnimalsRepository.GetAll().Where(a => a.Type == "Monkey").Average(a => a.Age);
            int owlNumber = AnimalsRepository.GetAll().Count(a => a.Type == "Owl");
            double owlAverageAge = AnimalsRepository.GetAll().Where(a => a.Type == "Owl").Average(a => a.Age);
            int zebraNumber = AnimalsRepository.GetAll().Count(a => a.Type == "Zebra");
            double zebraAverageAge = AnimalsRepository.GetAll().Where(a => a.Type == "Zebra").Average(a => a.Age);

            _statistics.Clear();
            _statistics.Add($"Crocodiles: {crocodileNumber} (avg {crocodileAverageAge} ages)");
            _statistics.Add($"Lions: {lionNumber} (avg {lionAverageAge} ages)");
            _statistics.Add($"Monkeys: {monkeyNumber} (avg {monkeyAverageAge} ages)");
            _statistics.Add($"Owls: {owlNumber} (avg {owlAverageAge} ages)");
            _statistics.Add($"Zebras: {zebraNumber} (avg {zebraAverageAge} ages)");
        }
    }
}