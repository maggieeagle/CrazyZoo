using CrazyZoo.Classes;
using CrazyZoo.Generics;
using CrazyZoo.Interfaces;
using CrazyZoo.Properties;
using Microsoft.Extensions.DependencyInjection;
using Prism.Commands; // to use DelegateCommand
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using static CrazyZoo.ZooViewModel;

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

        public struct RecordItem
        {
            public string Type { get; }
            public int Count { get; }
            public double Average { get; }

            public RecordItem(string type, int count, double average)
            {
                Type = type;
                Count = count;
                Average = average;
            }
        }

        public ObservableCollection<Enclosure<Animal>> Enclosures=> _enclosuresRepository.Items;
        public ObservableCollection<Animal> Animals => _animalsRepository.Items;

        public DelegateCommand DropFoodCommand { get; }
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
                Enclosure<Animal> enclosure = FindSelectedAnimalEnclosure();
                SelectedAnimalEnclosureName = enclosure?.Name ?? Resource1.animalWalksFreely;
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

        private string _animalFood = String.Empty;
        public string? AnimalFood
        {
            get => _animalFood;
            set
            {
                _animalFood = value != null ? value : String.Empty;
                OnPropertyChanged(nameof(AnimalFood));
                ValidateFood();
            }
        }

        private string _animalFoodError = String.Empty;
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
        private List<RecordItem> records;

        private readonly ILogger _logger;
        private IRepository<Animal> _animalsRepository;
        private IRepository<Enclosure<Animal>> _enclosuresRepository;

        public ZooViewModel(ILogger logger, IRepository<Animal> animalRepository, IRepository<Enclosure<Animal>> enclosureRepository)
        {
            _logger = logger;
            _animalsRepository = animalRepository;
            _enclosuresRepository = enclosureRepository;

            Animal lion = new Lion(Resource1.lionName, 2, Resource1.lionDescription, 1);
            Animal zebra = new Zebra(Resource1.zebraName, 4, Resource1.zebraDescription, 1);
            Animal crocodile = new Crocodile(Resource1.crocodileName, 3, Resource1.crocodileDescription, 2);
            Animal monkey = new Monkey(Resource1.monkeyName, 3, Resource1.monkeyDescription, 2);
            Animal owl = new Owl(Resource1.owlName, 10, Resource1.owlDescription, 3);

            Enclosure<Animal> savannaVoljeer = new Enclosure<Animal>(Resource1.savannaVoljeerName);
            Enclosure<Animal> tropicalVoljeer = new Enclosure<Animal>(Resource1.tropicsVoljeerName);
            Enclosure<Animal> forestVoljeer = new Enclosure<Animal>(Resource1.forestVoljeerName);

            _enclosuresRepository.Add(savannaVoljeer);
            _enclosuresRepository.Add(tropicalVoljeer);
            _enclosuresRepository.Add(forestVoljeer);

            _animalsRepository.Add(lion);
            _animalsRepository.Add(zebra);
            _animalsRepository.Add(crocodile);
            _animalsRepository.Add(monkey);
            _animalsRepository.Add(owl);

            savannaVoljeer.AddSilently(lion);
            savannaVoljeer.AddSilently(zebra);
            tropicalVoljeer.AddSilently(crocodile);
            tropicalVoljeer.AddSilently(monkey);
            forestVoljeer.AddSilently(owl);

            /*foreach (var enclosure in _enclosuresRepository.GetAll())
            {
                Enclosures.Add(enclosure);
            }
            /*foreach (var animal in _animalsRepository.GetAll())
            {
                _animalsRepository.Add(animal);
            }*/
            
            DropFoodCommand = new DelegateCommand(DropFood);
            MakeSoundCommand = new DelegateCommand(MakeSound);
            ValidateFoodCommand = new DelegateCommand(() => ValidateFood());
            FeedCommand = new DelegateCommand(() => Feed());
            AddAnimalCommand = new DelegateCommand(() => AddAnimal());
            RemoveAnimalCommand = new DelegateCommand(() => RemoveAnimal(), () => SelectedAnimal != null);

            foreach (var animal in _animalsRepository.GetAll())
            {
                animal.LogGenerated += log => {
                    if (_animalsRepository.GetAll().Contains(animal)) {
                        Logs.Insert(0, new Log(animal, log));
                        _logger.Log(new Log(animal, log));

                    }
                };
            }

            SelectedAnimal = _animalsRepository.GetAll().FirstOrDefault();
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
            foreach (var animal in _animalsRepository.GetAll())
            {
                if (animal is Lion lion)
                {
                    if ((DateTime.Now - lion.LastFedTime).TotalSeconds > lion.DigestTime && lion.HasActedCrazy == false && lion.FoodDropped == string.Empty)
                    {
                        Logs.Insert(0, new Log(lion, lion.ActCrazy()));
                        _logger.Log(new Log(lion, lion.ActCrazy()));
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
                    foreach (var animal in _animalsRepository.GetAll())
                    {
                        if (animal is Owl owl)
                        {
                            Logs.Insert(0, new Log(owl, owl.ActCrazy()));
                            _logger.Log(new Log(owl, owl.ActCrazy()));
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

        public void DropFood()
        {
            FindSelectedAnimalEnclosure()?.DropFood(AnimalFood);
        }

        public void MakeSound()
        {
            if (SelectedAnimal == null) return;
            string sound = SelectedAnimal.MakeSound();
            Logs.Insert(0, new Log(SelectedAnimal, string.Format(Resource1.sound, sound)));
            _logger.Log(new Log(SelectedAnimal, string.Format(Resource1.sound, sound)));
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
            _logger.Log(new Log(SelectedAnimal, SelectedAnimal.EatFood(AnimalFood)));
        }

        public void AddAnimal()
        {

            var addWindow = new AddAnimalWindow(Enclosures);
            bool? result = addWindow.ShowDialog();

            Animal newAnimal = addWindow.NewAnimal;
            Enclosure<Animal> selectedEnclosure = addWindow.SelectedEnclosure;

            if (result == false || newAnimal == null || selectedEnclosure == null) return;

            _animalsRepository.Add(newAnimal);
            selectedEnclosure.Add(newAnimal);
            SelectedAnimal = newAnimal;

            foreach (var animal in _animalsRepository.GetAll())
            {
                if (animal is Crocodile crocodile && animal != newAnimal)
                {
                    Logs.Insert(0, new Log(crocodile, crocodile.ActCrazy(newAnimal)));
                    _logger.Log(new Log(crocodile, crocodile.ActCrazy(newAnimal)));
                }
            }

            RecalculateStatistics();
        }

        public void RemoveAnimal()
        {
            if (SelectedAnimal == null) return;

            FindSelectedAnimalEnclosure()?.Remove(SelectedAnimal);
            _animalsRepository.Remove(SelectedAnimal);

            SelectedAnimal = _animalsRepository.GetAll().FirstOrDefault();
            RecalculateStatistics();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged(string n) 
            => PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(n));

        private void RecalculateStatistics()
        {
            _statistics.Clear();
            
            records = _animalsRepository.GetAll().GroupBy(a => a.Type).Select(g => new RecordItem(g.Key, g.Count(), g.Average(a => a.Age))).ToList();

            foreach (var record in records)
            {
                _statistics.Add(String.Format(Resource1.statisticsRecord, record.Type, record.Count, FormatValue(record.Average)));
            }

            Animal oldestAnimal = _animalsRepository.GetAll().OrderByDescending(a => a.Age).FirstOrDefault();

            if (oldestAnimal != null)
            {
                _statistics.Add(String.Format(Resource1.statisticsRecordOldest, oldestAnimal.Type, oldestAnimal.Name, oldestAnimal.Age));
            }

            Enclosure<Animal> mostFilledEnclosure = Enclosures.OrderByDescending(e => e.Items.Count()).FirstOrDefault();

            if (mostFilledEnclosure != null)
            {
                _statistics.Add(String.Format(Resource1.statisticsRecordLargestVoljeer, mostFilledEnclosure.Name));
            }
        }

        private Enclosure<Animal> FindSelectedAnimalEnclosure()
        {
            if (SelectedAnimal == null) return null;

            Enclosure<Animal> enclosure = Enclosures.FirstOrDefault(e => e.Id == SelectedAnimal.EnclosureId);
            if (enclosure == null || AnimalFood == null) return null;
            return enclosure;
        }

        private string FormatValue(double value)
        {
            if ((int)value == value) return value.ToString();
            return value.ToString(Resource1.formatTwoDigitsAfterZero);

        }
    }
}