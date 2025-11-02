using CrazyZoo;
using CrazyZoo.Properties;
using CrazyZoo.Interfaces;

public class Zebra : Animal, ICrazyAction
{
    private const int portionsLimit = 5;
    private int foodPortions = 0;
    public Zebra(string name, int age, string description) : base(name, age, description)
    {
    }

    public override string PreferableFood
    {
        get
        {
            return Resource1.zebraPreferableFood;
        }

    }

    public override string MakeSound()
    {
        return Resource1.zebraSound;
    }

    public override string EatFood(string food)
    {
        string cleanFood = food.Trim().ToLower();
        string cleanPreferableFood = PreferableFood.Trim().ToLower();

        foodPortions += 1;
        if (foodPortions < portionsLimit)
        {
            if (cleanFood != cleanPreferableFood)
            {
                return string.Format(Resource1.ateWithConsequence, cleanFood, RandomConsequence());
            }
            return string.Format(Resource1.ate, cleanFood);
        }
        return string.Format(Resource1.zebraIsFull, Name, ActCrazy());
    }

    public string ActCrazy()
    {
        return Resource1.zebraCrazyAction;
    }

    public override void OnAnimalJoinedInSameEnclosure(Animal animal)
    {
        string log;
        if (animal is Zebra zebra)
        {
            log = $"is showing flower to {zebra.Name}.";
        }
        else
        {
            log = $"is not sure it is a good idea to become friends with {animal.Name}.";
        }
        Log(log);
    }

    public override void OnFoodDropped(string food)
    {
        string cleanFood = food.Trim().ToLower();
        Log($"ate {cleanFood}.");
    }
}

