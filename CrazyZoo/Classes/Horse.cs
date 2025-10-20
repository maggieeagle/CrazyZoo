using CrazyZoo.Properties;
using System.Text.RegularExpressions;

namespace CrazyZoo.Classes
{
    internal class Horse:Animal, ICrazyAction
    {
        public Horse(string name, int age, string description) : base(name, age, description) { }

        public override string PreferableFood
        {
            get
            {
                return Resource1.horsePreferableFood;
            }

        }

        public override string MakeSound()
        {
            return Resource1.horseSound;
        }

        public override string EatFood(string food)
        {
            Regex horseCrazyFoodRegex = new Regex(Resource1.horseCrazyFoodPattern, RegexOptions.IgnoreCase);

            string cleanFood = food.Trim().ToLower();
            string cleanPreferableFood = PreferableFood.Trim().ToLower();

            if (horseCrazyFoodRegex.IsMatch(cleanFood))
            {
                return this.ActCrazy();
            }

            if (cleanFood != cleanPreferableFood)
            {
                return string.Format(Resource1.ateWithConsequence, cleanFood, RandomConsequence());
            }
            return string.Format(Resource1.ate, cleanFood);
        }

        public string ActCrazy()
        {
            return Resource1.horseCrazyAction;
        }
    }
}
