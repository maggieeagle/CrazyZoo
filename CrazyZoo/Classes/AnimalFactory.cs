using CrazyZoo.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazyZoo.Classes
{
    internal class AnimalFactory
    {   public static Animal CreateAnimal(string type, string name, int age, string description, int enclosureId)
        {
            switch (type)
            {
                case "Lion":
                    return new Lion(name, age, description, enclosureId);
                case "Zebra":
                    return new Zebra(name, age, description, enclosureId);
                case "Crocodile":
                    return new Crocodile(name, age, description, enclosureId);
                case "Monkey":
                    return new Monkey(name, age, description, enclosureId);
                case "Owl":
                    return new Owl(name, age, description, enclosureId);
                default:
                    throw new ArgumentException(Resource1.unknownAnimalType + type);
            }
        }
    }
}
