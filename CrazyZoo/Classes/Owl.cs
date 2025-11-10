using CrazyZoo;
using CrazyZoo.Properties;
using CrazyZoo.Interfaces;

public enum OwlLocation
{
    OnTree,
    OnShelf,
    OnFence,
    InNest
}

public class Owl : Animal, IFlyable, ICrazyAction
{
    private OwlLocation Location { get; set; } = OwlLocation.OnTree;

    private readonly string[] wisdoms =
    {
        Resource1.owlWisdom1,
        Resource1.owlWisdom2,
        Resource1.owlWisdom3,
        Resource1.owlWisdom4,
        Resource1.owlWisdom5
    };

    public Owl(string name, int age, string description, int enclosureId) : base(name, age, description, enclosureId)
    {
        timeBetweenBites = 1;
        bitesUntilFull = 3;
        setEatProgressTimer();
    }

    public override string PreferableFood
    {
        get
        {
            return Resource1.owlPreferableFood;
        }

    }

    public override string MakeSound()
    {
        return Resource1.owlSound;
    }

    public void Fly()
    {
        Random random = new Random();
        int number = random.Next(Enum.GetNames(typeof(OwlLocation)).Length);

        Location = (OwlLocation)number;
    }

    public string ActCrazy()
    {
        Fly();
        Random random = new Random();
        string wisdom = wisdoms[random.Next(wisdoms.Length)];

        return string.Format(Resource1.owlCrazyAction, FormatLocation(Location), wisdom);
    }

    private static string FormatLocation(OwlLocation location)
    {
        return string.Concat(
            location.ToString()
                .Select((c, i) => i > 0 && char.IsUpper(c) ? " " + char.ToLower(c) : c.ToString().ToLower())
        );
    }

    public override void OnAnimalJoinedInSameEnclosure(Animal animal)
    {
        string newName = String.Format(Resource1.owlNewAnimalName, animal.Name);
        string action = String.Format(Resource1.owlOnAnimalJoinedInSameEnclosureAction, animal.Name, newName);
        animal.Name = newName;
        Log(action);
    }

    public override void OnFoodDropped(string food)
    {
        string cleanFood = food.Trim().ToLower();
        FoodDropped = cleanFood;
    }
}

