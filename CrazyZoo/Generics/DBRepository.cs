using CrazyZoo.Classes;
using CrazyZoo.Interfaces;
using CrazyZoo.Properties;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazyZoo.Generics
{
    // repository for animals
    public class DbRepository<T> : IRepository<T> where T : Animal
    {
        public ObservableCollection<T> Items { get; } = new ObservableCollection<T>();

        private readonly string _connStr;

        public DbRepository(string connectionString)
        {
            _connStr = connectionString;
        }

        public void Add(T item)
        {
            using var conn = new SqlConnection(_connStr);
            conn.Open();

            string query = Resource1.dbInsertAnimal;
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue(Resource1.dbNameValue, item.Name);
            cmd.Parameters.AddWithValue(Resource1.dbAgeValue, item.Age);
            cmd.Parameters.AddWithValue(Resource1.dbDescriptionValue, item.Description);
            cmd.Parameters.AddWithValue(Resource1.dbTypeValue, item.Type);
            cmd.Parameters.AddWithValue(Resource1.dbFoodValue, item.PreferableFood);
            cmd.Parameters.AddWithValue(Resource1.dbEnclosureIdValue, item.EnclosureId);

            item.Id = (int)cmd.ExecuteScalar();

            Items.Add(item);
        }

        public void Remove(T item)
        {
            Items.Remove(item);
            using var conn = new SqlConnection(_connStr);
            conn.Open();

            string query = Resource1.dbDeleteAnimal;
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue(Resource1.dbIdValue, item.Id);
            cmd.ExecuteNonQuery();
        }

        public IEnumerable<T> GetAll()
        {
            return Items;
        }

        public T? Find(Func<T, bool> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }
    }
}

