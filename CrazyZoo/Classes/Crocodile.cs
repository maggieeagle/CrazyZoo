using CrazyZoo;
using CrazyZoo.Properties;
using CrazyZoo.Interfaces;

public class Crocodile : Animal, ICrazyAction
{
    public Crocodile(string name, int age, string description) : base(name, age, description)
    {
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
            log = $"is swimming to become friends with {crocodile.Name}.";
        }
        else log = "\"zzz...\"";
        Log(log);
    }

    public override void OnFoodDropped(string food)
    {
        string cleanFood = food.Trim().ToLower();
        Log($"ate {cleanFood}.");
    }
}

