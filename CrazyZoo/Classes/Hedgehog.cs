using CrazyZoo.Properties;

namespace CrazyZoo.Classes
{
    internal class Hedgehog : Animal, ICrazyAction
    {
        public Hedgehog(string name, int age, string description) : base(name, age, description) { }

        public override string PreferableFood
        {
            get
            {
                return Resource1.hedgehogPreferableFood;
            }

        }

        public override string MakeSound()
        {
            return Resource1.hedgehogSound;
        }

        public override string EatFood(string food)
        {
            string cleanFood = food.Trim().ToLower();
            string cleanPreferableFood = PreferableFood.Trim().ToLower();
            if (cleanFood != cleanPreferableFood)
            {
                return string.Format(Resource1.ateWithConsequence, cleanFood, RandomConsequence());
            }
            return string.Format(Resource1.ate, cleanFood);
        }

        public string ActCrazy()
        {
            return string.Format(Resource1.hedgehogCrazyAction, this.Name);
        }
    }
}
