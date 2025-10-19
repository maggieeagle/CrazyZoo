using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using CrazyZoo.Properties;

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

        public Animal? NewAnimal { get; private set; }

        private enum AnimalSpecies
        {
            Crocodile,
            Lion,
            Monkey,
            Owl,
            Zebra
        }

        public AddAnimalWindow(Animal? animal = null)
        {
            InitializeComponent();
            AnimalSpecieComboBox.ItemsSource = Enum.GetValues(typeof(AnimalSpecies));
            AnimalSpecieComboBox.SelectedIndex = 0;
            NewAnimal = animal;
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
                    _ => null
                };

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
            return AnimalNameError.Text != "" || AnimalAgeError.Text != "";
        }
        bool FieldIsEmpty()
        {
            return AnimalName.Text == "" || AnimalAge.Text == "";
        }
    }
}
