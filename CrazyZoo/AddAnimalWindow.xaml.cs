using CrazyZoo.Classes;
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

        public Animal? NewAnimal { get; private set; }
        public Enclosure<Animal>? SelectedEnclosure { get; private set; }

        private ObservableCollection<Enclosure<Animal>>? Enclosures { get; }

        private enum AnimalSpecies
        {
            Crocodile,
            Lion,
            Monkey,
            Owl,
            Zebra
        }

        public AddAnimalWindow(ObservableCollection<Enclosure<Animal>> enclosures, Animal? animal = null)
        {
            InitializeComponent();
            AnimalSpecieComboBox.ItemsSource = Enum.GetValues(typeof(AnimalSpecies));
            AnimalSpecieComboBox.SelectedIndex = 0;
            VoljeerComboBox.ItemsSource = enclosures;
            VoljeerComboBox.DisplayMemberPath = Resource1.name;
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
                AnimalNameError.Text = string.Empty;
            }
        }

        private void AnimalAgeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AnimalAgeError.Text = string.Empty;
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
                    AnimalAgeError.Text = Properties.Resource1.ageMustBeLessThenError + string.Format(Resource1.emptyString, maxAge);
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
                AddAnimalFormError.Text = Resource1.incorrectValuesError;
            }
            else if (FieldIsEmpty())
            {
                AddAnimalFormError.Text = Resource1.emptyFieldsError;
            }
            else
            {
                AddAnimalFormError.Text = string.Empty;

                string specie = string.Empty;
                if (AnimalSpecieComboBox.SelectedItem is AnimalSpecies specieEnum)
                {
                    specie = specieEnum.ToString();
                }
                string name = AnimalName.Text.Trim();
                int age = Int32.Parse(AnimalAge.Text.Trim());
                string description = AnimalDescription.Text.Trim();

                SelectedEnclosure = VoljeerComboBox.SelectedItem as Enclosure<Animal>;

                NewAnimal = AnimalFactory.CreateAnimal(specie, name, age, description, SelectedEnclosure.Id);

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
