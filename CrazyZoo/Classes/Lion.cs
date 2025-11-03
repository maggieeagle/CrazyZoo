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
        timeBetweenBites = 10;
        bitesUntilFull = 10;
        setEatProgressTimer();
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
            log = string.Format(Resource1.lionOnLionJoinedInSameEnclosureAction, lion.Name);
        }
        else if (animal is Zebra zebra)
        {
            log = string.Format(Resource1.lionOnZebraJoinedInSameEnclosureAction, zebra.Name);
        }
        else if (animal is Monkey monkey)
        {
            log = string.Format(Resource1.lionOnMonkeyJoinedInSameEnclosureAction, monkey.Name);
        }
        else log = string.Format(Resource1.lionOnAnimalJoinedInSameEnclosureAction, animal.Name);
        Log(log);
    }

    public override void OnFoodDropped(string food)
    {
        string cleanFood = food.Trim().ToLower();
        FoodDropped = cleanFood;
    }
}

