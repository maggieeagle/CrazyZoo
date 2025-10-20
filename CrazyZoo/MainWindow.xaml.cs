using CrazyZoo.Classes;
using CrazyZoo.Properties;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace CrazyZoo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private struct Log
        {
            public Animal Animal { get; }
            public string Information { get; }

            public Log(Animal animal, string information)
            {
                Animal = animal;
                Information = information;
            }
        }

        private ObservableCollection<Animal> animals;
        private ObservableCollection<Log> logs = new ObservableCollection<Log>();
        private DispatcherTimer feedCheckTimer;
        private DispatcherTimer hedgehogVisitTimer;

        private Queue<char> keyBuffer = new Queue<char>();

        public MainWindow()
        {
            InitializeComponent();
            FillData();
            setFeedingTimer();
            setHedgehogVisitTimer();
            this.KeyDown += MainWindow_KeyDown; // listen at keys on window
            AnimalList.ItemsSource = animals;
            AnimalList.SelectedItem = animals[0];
            Logs.ItemsSource = logs;
        }

        private void setHedgehogVisitTimer()
        {
            hedgehogVisitTimer = new DispatcherTimer();
            hedgehogVisitTimer.Interval = TimeSpan.FromSeconds(1);
            hedgehogVisitTimer.Tick += HedgehogVisit_Tick;
            hedgehogVisitTimer.Start();
        }

        private void HedgehogVisit_Tick(object sender, EventArgs e)
        {
            var localTime = DateTime.Now;

            if (localTime.Hour >= 0 && localTime.Hour < 6)
            {
                foreach (var animal in animals)
                {
                    if (animal is Hedgehog hedgehog)
                        logs.Insert(0, new Log(hedgehog, hedgehog.ActCrazy()));
                }
            }
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
            foreach (var animal in animals)
            {
                if (animal is Lion lion)
                {
                    if ((DateTime.Now - lion.LastFedTime).TotalSeconds > lion.DigestTime && lion.HasActedCrazy == false)
                    {
                        logs.Insert(0, new Log(lion, lion.ActCrazy()));
                    }
                }
            }
        }



        // keys for owl wisdoms
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            char pressedChar = KeyToChar(e.Key);

            if (pressedChar != '\0')
            {
                if (keyBuffer.Count == 3)
                    keyBuffer.Dequeue();

                keyBuffer.Enqueue(char.ToLower(pressedChar));

                if (new string(keyBuffer.ToArray()) == Resource1.owlCrazyActionTrigger)
                {
                    foreach (var animal in animals)
                    {
                        if (animal is Owl owl)
                        {
                            logs.Insert(0, new Log(owl, owl.ActCrazy()));
                        }
                    }
                    keyBuffer.Clear();
                }
            }
        }

        private char KeyToChar(Key key)
        {
            if (key >= Key.A && key <= Key.Z)
            {
                return (char)('a' + (key - Key.A));
            }
            return '\0'; // non-letter keys ignored
        }

        private void FillData()
        {
            animals = new ObservableCollection<Animal>
            {
                new Lion(Resource1.lionName, 2, Resource1.lionDescription),
                new Zebra(Resource1.zebraName, 4, Resource1.zebraDescription),
                new Crocodile(Resource1.crocodileName, 3, Resource1.crocodileDescription),
                new Monkey(Resource1.monkeyName, 3, Resource1.monkeyDescription),
                new Owl(Resource1.owlName, 10, Resource1.owlDescription),
            };
        }

        private void MakeSoundButton_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalList.SelectedItem is Animal selectedAnimal)
            {
                string sound = selectedAnimal.MakeSound();
                logs.Insert(0, new Log(selectedAnimal, $"\"{sound}\""));
            }
        }

        private void FoodTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(AnimalFood.Text))
            {
                AnimalFoodError.Text = Resource1.foodEmptyStringError;
            }
            else
            {
                AnimalFoodError.Text = "";
            }
        }

        private void FeedButton_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(AnimalFoodError.Text))
            {
                return;
            }

            if (AnimalList.SelectedItem is Animal selectedAnimal)
            {
                string food = AnimalFood.Text;
                logs.Insert(0, new Log(selectedAnimal, selectedAnimal.EatFood(food)));
            }
        }

        private void AddAnimalButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddAnimalWindow();
            bool? result = addWindow.ShowDialog();

            Animal newAnimal = addWindow.NewAnimal;

            if (result == true && newAnimal != null)
            {
                animals.Add(newAnimal);
                AnimalList.SelectedItem = newAnimal;
                foreach (var animal in animals)
                {
                    if (animal is Crocodile crocodile && animal != newAnimal)
                    {
                        logs.Insert(0, new Log(crocodile, crocodile.ActCrazy(newAnimal)));
                    }
                }
            }
        }

        private void RemoveAnimalButton_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalList.SelectedItem is Animal selectedAnimal)
            {
                animals.Remove(selectedAnimal);
                if (animals.Count > 0)
                {
                    AnimalList.SelectedItem = animals[0];
                }
                else
                {
                    AnimalNameValue.Text = "";
                    AnimalAgeValue.Text = "";
                    AnimalDescriptionValue.Text = "";
                    AnimalFood.Text = "";
                }
            }
        }

        private void AnimalListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AnimalList.SelectedItem is Animal selectedAnimal)
            {
                AnimalNameValue.Text = selectedAnimal.Name;
                AnimalAgeValue.Text = selectedAnimal.Age.ToString();
                AnimalDescriptionValue.Text = selectedAnimal.Description;
                AnimalFood.Text = selectedAnimal.PreferableFood;
            }
        }
    }
}