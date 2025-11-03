using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CrazyZoo.Interfaces;

namespace CrazyZoo.Generics
{
    public class Enclosure<T> : IRepository<T> where T : Animal
    {
        public string Name { get; set; }
        public List<T> Items { get; } = new List<T>();

        public event Action<T>? AnimalJoinedInSameEnclosure;
        public event Action<string>? FoodDropped;

        public Enclosure(string name)
        {
            Name = name;
        }

        public void Add(T item)
        {
            if (item == null) return;

            AnimalJoinedInSameEnclosure?.Invoke(item);

            Items.Add(item);

            AnimalJoinedInSameEnclosure += item.OnAnimalJoinedInSameEnclosure;
            FoodDropped += item.OnFoodDropped;
        }

        public void AddSilently(T item)
        {
            if (item == null) return;

            Items.Add(item);

            AnimalJoinedInSameEnclosure += item.OnAnimalJoinedInSameEnclosure;
            FoodDropped += item.OnFoodDropped;
        }

        public void Remove(T item)
        {
            AnimalJoinedInSameEnclosure -= item.OnAnimalJoinedInSameEnclosure;
            FoodDropped -= item.OnFoodDropped;

            item.EatProgressTimer.Stop();

            Items.Remove(item);
        }

        public IEnumerable<T> GetAll()
        {
            return Items;
        }

        public T? Find(Func<T, bool> predicate)
        {
            return Items.FirstOrDefault(predicate);
        }

        public void DropFood(string food)
        {
            FoodDropped?.Invoke(food);
        }
    }
}