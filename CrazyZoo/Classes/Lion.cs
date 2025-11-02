using CrazyZoo;
using CrazyZoo.Properties;
using CrazyZoo.Interfaces;

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

    public override void OnAnimalJoinedInSameEnclosure(Animal animal)
    {
        string log;
        if (animal is Lion lion)
        {
            log = $"doesn't stand {lion.Name} on his territory.";
        }
        else if (animal is Zebra zebra)
        {
            log = $"is figuring out how to eat {zebra.Name}.";
        }
        else  if (animal is Monkey monkey)
        {
            log = $"is sure it was better without {monkey.Name}.";
        }
        else log = $"is not interested in {animal.Name} and continue basking in the sun.";
        Log(log);
    }

    public override void OnFoodDropped(string food)
    {
        string cleanFood = food.Trim().ToLower();
        Log($"ate {cleanFood}.");
    }
}

