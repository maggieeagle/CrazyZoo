using  CrazyZoo.Properties;

namespace CrazyZoo
{
    public abstract class Animal
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public string Description { get; set; }

        public abstract string PreferableFood { get; }
        public string Type
        {
            get
            {
                return this.GetType().Name;
            }
        }

        protected Animal(string name, int age, string description)
        {
            Name = name;
            Age = age;
            if (description != null && description != "")
            {
                Description = description;
            } else
            {
                Description = Describe();
            }
        }

        public virtual string Describe()
        {
            return string.Format(Resource1.describeDefault, Name, Age);
        }

        public abstract string MakeSound();
        public virtual string EatFood(string food)
        {
            string cleanFood = food.Trim().ToLower();
            string cleanPreferableFood = PreferableFood.Trim().ToLower();
            if (cleanFood != cleanPreferableFood) {
                return string.Format(Resource1.ateWithConsequence, cleanFood, RandomConsequence());
            }
            return string.Format(Resource1.ate, cleanFood);
        }

        protected static string RandomConsequence()
        {
            Random random = new Random();
            int number = random.Next(0, 3);
            string consequence;

            switch (number)
            {
                case 0:
                    consequence = Resource1.consequenceDissatisfied;
                    break;
                case 1:
                    consequence = Resource1.consequenceFeelsBad;
                    break;
                default:
                    consequence = Resource1.consequenceWantsMore;
                    break;

            }
            return consequence;
        }
    }
}
