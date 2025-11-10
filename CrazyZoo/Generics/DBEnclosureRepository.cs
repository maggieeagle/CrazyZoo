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
    // repository for enclosures
    public class DbEnclosureRepository : IRepository<Enclosure<Animal>>
    {
        public ObservableCollection<Enclosure<Animal>> Items { get; } = new ObservableCollection<Enclosure<Animal>>();

        private readonly string _connStr;

        public DbEnclosureRepository(string connectionString)
        {
            _connStr = connectionString;
        }

        public void Add(Enclosure<Animal> enclosure)
        {
            using var conn = new SqlConnection(_connStr);
            conn.Open();

            // add enclosure
            string insertEnclosure = Resource1.dbInsertEnclosure;
            using var cmd = new SqlCommand(insertEnclosure, conn);
            cmd.Parameters.AddWithValue(Resource1.dbNameValue, enclosure.Name);
            enclosure.Id = (int)cmd.ExecuteScalar();

            Items.Add(enclosure);
        }
        public void Remove(Enclosure<Animal> enclosure)
        {
            using var conn = new SqlConnection(_connStr);
            conn.Open();

            // remove enclosure
            string deleteEnclosure = Resource1.dbDeleteEnclosure;
            using var cmd = new SqlCommand(deleteEnclosure, conn);
            cmd.Parameters.AddWithValue(Resource1.dbEnclosureIdValue, enclosure.Id);
            cmd.ExecuteNonQuery();

            Items.Remove(enclosure);
        }
        public IEnumerable<Enclosure<Animal>> GetAll()
        {
            return Items;
        }

        public Enclosure<Animal>? Find(Func<Enclosure<Animal>, bool> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

    }
}
