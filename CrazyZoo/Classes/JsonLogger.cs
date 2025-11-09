using CrazyZoo.Interfaces;
using CrazyZoo.Properties;
using System;
using System.IO;
using System.Text.Json;
using static CrazyZoo.ZooViewModel;

namespace CrazyZoo.Classes
{

    public class JsonLogger : ILogger
    {
        private readonly string filePath = Path.Combine(Resource1.pathBack, Resource1.pathBack, Resource1.pathBack, Resource1.filenameLogJson);

        public JsonLogger()
        {
            if (File.Exists(filePath)) File.Delete(filePath);
            File.WriteAllText(filePath, Resource1.emptyListJson);
        }

        public void Log(Log log)
        {
            // read existing logs
            var existingJson = File.ReadAllText(filePath);
            var logs = JsonSerializer.Deserialize<List<object>>(existingJson) ?? new List<object>();

            var logView = new {
                log.Animal,
                log.Information,
                Timestamp = DateTime.Now
            };

            logs.Add(logView);

            // write updated logs
            File.WriteAllText(filePath, JsonSerializer.Serialize(logs, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
        }
    }
}
