using CrazyZoo;
using CrazyZoo.Properties;
using CrazyZoo.Interfaces;

public class Monkey : Animal, ICrazyAction
{
    private readonly string _triggerFood = Resource1.monkeyTriggerFood;

    public Monkey(string name, int age, string description, int enclosureId) : base(name, age, description, enclosureId)
    {
        timeBetweenBites = 1;
        bitesUntilFull = 15;
        setEatProgressTimer();
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

    public override void OnAnimalJoinedInSameEnclosure(Animal animal)
    {
        Log(string.Format(Resource1.monkeyOnAnimalJoinedInSameEnclosureAction, animal.Name));
    }

    public override void OnFoodDropped(string food)
    {
        string cleanFood = food.Trim().ToLower();
        FoodDropped = cleanFood;
    }
}

