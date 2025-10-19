using CrazyZoo;
using CrazyZoo.Properties;
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
}

