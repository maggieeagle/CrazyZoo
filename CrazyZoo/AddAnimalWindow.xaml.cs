using CrazyZoo.Generics;
using CrazyZoo.Properties;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace CrazyZoo
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class AddAnimalWindow : Window
    {
        const int maxAge = 150;
        const string crocodile = "Crocodile";
        const string lion = "Lion";
        const string monkey = "Monkey";
        const string owl = "Owl";
        const string zebra = "Zebra";
        const string horse = "Horse";
        const string hedgehog = "Hedgehog";
        const string kiwi = "Kiwi";


        public Animal? NewAnimal { get; private set; }
        public Enclosure<Animal>? SelectedEnclosure { get; private set; }

        private ObservableCollection<Enclosure<Animal>>? Enclosures { get; }

        private enum AnimalSpecies
        {
            Crocodile,
            Lion,
            Monkey,
            Owl,
            Zebra,
            Horse,
            Kiwi,
            Hedgehog
        }

        public AddAnimalWindow(ObservableCollection<Enclosure<Animal>> enclosures, Animal? animal = null)
        {
            InitializeComponent();
            AnimalSpecieComboBox.ItemsSource = Enum.GetValues(typeof(AnimalSpecies));
            AnimalSpecieComboBox.SelectedIndex = 0;
            VoljeerComboBox.ItemsSource = enclosures;
            VoljeerComboBox.DisplayMemberPath = "Name";
            if (enclosures.Count > 0) VoljeerComboBox.SelectedIndex= 0;
            NewAnimal = animal;
            Enclosures = enclosures;
        }

        private void AnimalNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string name = AnimalName.Text;
            if (!IsLetter(name))
            {
                AnimalNameError.Text = Properties.Resource1.enterLettersError;
            } 
            else if (name.Length < 3)
            {
                AnimalNameError.Text = Properties.Resource1.animalNameIsTooShortError;
            }
            else if (name.Length > 50)
            {
                AnimalNameError.Text = Properties.Resource1.animalNameIsTooLongError;
            }
            else
            {
                AnimalNameError.Text = "";
            }
        }

        private void AnimalAgeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AnimalAgeError.Text = "";
            try
            {
                int time = Int32.Parse(AnimalAge.Text);

                if (time <= 0)
                {
                    AnimalAgeError.Text = Properties.Resource1.ageIsNegativeError;
                    return;
                }
                else if (time > maxAge)
                {
                    AnimalAgeError.Text = Properties.Resource1.ageMustBeLessThenError + $"{maxAge}";
                    return;
                }
            }
            catch (FormatException)
            {
                AnimalAgeError.Text = Properties.Resource1.enterDigitsError;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ErrorsInForm())
            {
                AddAnimalFormError.Text = Properties.Resource1.incorrectValuesError;
            }
            else if (FieldIsEmpty())
            {
                AddAnimalFormError.Text = Properties.Resource1.emptyFieldsError;
            }
            else
            {
                AddAnimalFormError.Text = "";

                string specie = "";
                if (AnimalSpecieComboBox.SelectedItem is AnimalSpecies specieEnum)
                {
                    specie = specieEnum.ToString();
                }
                string name = AnimalName.Text.Trim();
                int age = Int32.Parse(AnimalAge.Text.Trim());
                string description = AnimalDescription.Text.Trim();

                NewAnimal = specie switch
                {
                    crocodile => new Crocodile(name, age, description),
                    lion => new Lion(name, age, description),
                    monkey => new Monkey(name, age, description),
                    owl => new Owl(name, age, description),
                    zebra => new Zebra(name, age, description),
                    horse => new Horse(name, age, description),
                    hedgehog => new Hedgehog(name, age, description),
                    kiwi => new Kiwi(name, age, description),
                    _ => null
                };

                SelectedEnclosure = Enclosures?[VoljeerComboBox.SelectedIndex];

                if (NewAnimal == null)
                {
                    MessageBox.Show(Resource1.animalTypeNotSupportedError);
                    return;
                }

                this.DialogResult = true;
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }

        bool IsLetter(string input)
        {
            return Regex.IsMatch(input, Resource1.isLetterRegex);
        }

        bool ErrorsInForm()
        {
            return !String.IsNullOrEmpty(AnimalNameError.Text) || !String.IsNullOrEmpty(AnimalAgeError.Text);
        }
        bool FieldIsEmpty()
        {
            return String.IsNullOrEmpty(AnimalName.Text) || String.IsNullOrEmpty(AnimalAge.Text);
        }
    }
}
