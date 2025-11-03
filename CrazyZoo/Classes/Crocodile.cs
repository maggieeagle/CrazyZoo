using CrazyZoo;
using CrazyZoo.Interfaces;
using CrazyZoo.Properties;
using System.Windows.Threading;

public class Crocodile : Animal, ICrazyAction
{
    public Crocodile(string name, int age, string description) : base(name, age, description)
    {
        timeBetweenBites = 2;
        bitesUntilFull = 2;
        setEatProgressTimer();
    }

    public override string PreferableFood
    {
        get
        {
            return Resource1.crocodilePreferableFood;
        }

    }

    public override string MakeSound()
    {
        return Resource1.crocodileSound;
    }

    public string ActCrazy()
    {
        return Resource1.crocodileDefaultCrazyAction;
    }

    public string ActCrazy(Animal animal)
    {
        if (animal is Crocodile crocodile)
        {
            return string.Format(Resource1.crocodileInterestedAction, animal.Name);
        }
        return string.Format(Resource1.crocodileIndifferentAction, animal.Name);
    }

    public override void OnAnimalJoinedInSameEnclosure(Animal animal)
    {
        string log;
        if (animal is Crocodile crocodile)
        {
            log = string.Format(Resource1.crocodileOnCrocodileJoinedInSameEnclosureAction, crocodile.Name);
        }
        else log = Resource1.crocodileOnAnimalJoinedInSameEnclosureAction;
        Log(log);
    }

    public override void OnFoodDropped(string food)
    {
        string cleanFood = food.Trim().ToLower();
        FoodDropped = cleanFood;
    }
}

