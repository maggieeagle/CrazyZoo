using CrazyZoo.Properties;

namespace CrazyZoo.Classes
{
    internal class Kiwi : Animal, ICrazyAction
    {
        public Kiwi(string name, int age, string description) : base(name, age, description) { }

        public override string PreferableFood
        {
            get
            {
                return Resource1.kiwiPreferableFood;
            }

        }

        public override string MakeSound()
        {
            return Resource1.kiwiSound;
        }

        public override string EatFood(string food)
        {
            string cleanFood = food.Trim().ToLower();
            string cleanPreferableFood = PreferableFood.Trim().ToLower();
            if (cleanFood != cleanPreferableFood)
            {
                return string.Format(Resource1.ateWithConsequence, cleanFood, RandomConsequence());
            }
            return this.ActCrazy();
        }

        public string ActCrazy()
        {
            return Resource1.kiwiCrazyAction;
        }
    }
}

