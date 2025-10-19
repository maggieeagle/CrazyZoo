using CrazyZoo;
using CrazyZoo.Properties;


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
}

