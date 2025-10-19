using CrazyZoo;
using CrazyZoo.Properties;


public class Lion : Animal, ICrazyAction
{
    public DateTime LastFedTime { get; set; } = DateTime.Now;
    public readonly int DigestTime = 20;
    public bool HasActedCrazy = false;

    public Lion(string name, int age, string description) : base(name, age, description)
    {
    }

    public override string PreferableFood
    {
        get
        {
            return Resource1.lionPreferableFood;
        }

    }

    public override string MakeSound()
    {
        return Resource1.lionSound;
    }

    public override string EatFood(string food)
    {
        LastFedTime = DateTime.Now;
        HasActedCrazy = false;

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
        HasActedCrazy = true;
        return string.Format(Resource1.lionCrazyAction, DigestTime);
    }
}

