using CrazyZoo;
using CrazyZoo.Properties;


public class Monkey : Animal, ICrazyAction
{
    private readonly string _triggerFood = Resource1.monkeyTriggerFood;

    public Monkey(string name, int age, string description) : base(name, age, description)
    {
    }

    public override string PreferableFood
    {
        get
        {
            return Resource1.monkeyPreferableFood;
        }

    }

    public override string MakeSound()
    {
        return Resource1.monkeySound;
    }

    public override string EatFood(string food)
    {
        string cleanFood = food.Trim().ToLower();
        string cleanTriggerFood = _triggerFood.Trim().ToLower();
        string cleanPreferableFood = PreferableFood.Trim().ToLower();

        if (cleanFood != cleanTriggerFood)
        {
            if (cleanFood != cleanPreferableFood)
            {
                return string.Format(Resource1.ateWithConsequence, cleanFood, RandomConsequence());
            }
            return string.Format(Resource1.ate, cleanFood);
        }
        return ActCrazy(cleanFood);
    }

    public string ActCrazy()
    {
        return Resource1.monkeyDefaultCrazyAction;
    }

    public string ActCrazy(string food)
    {
        return string.Format(Resource1.monkeyFoodCrazyAction, food);
    }
}

