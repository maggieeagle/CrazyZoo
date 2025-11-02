using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CrazyZoo.Interfaces
{
    public interface IRepository <T> where T : class
    {
        void Add(T item);

        void Remove(T item);

        IEnumerable<T> GetAll();

        T? Find(Func<T, bool> predicate);
    }
}
