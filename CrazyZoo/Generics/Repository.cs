using CrazyZoo.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace CrazyZoo.Generics
{
    public class Repository<T> : IRepository<T> where T : Animal
    {
        public ObservableCollection<T> Items { get; } = new ObservableCollection<T>();

        public void Add(T item)
        {
            Items.Add(item);
        }

        public void Remove(T item)
        {
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
    }
}
