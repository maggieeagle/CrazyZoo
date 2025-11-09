using CrazyZoo.Interfaces;
using System.IO;
using System.Xml.Linq;
using static CrazyZoo.ZooViewModel;
using CrazyZoo.Properties;

namespace CrazyZoo.Classes
{
    public class XmlLogger : ILogger
    {
        private readonly string filePath = Path.Combine(Resource1.pathBack, Resource1.pathBack, Resource1.pathBack, Resource1.filenameLogXml);

        public XmlLogger()
        {
            if (File.Exists(filePath)) File.Delete(filePath);

            // create root element, save to file
            new XDocument(new XElement(Resource1.rootElementXml)).Save(filePath);
        }

        public void Log(Log log)
        {
            var doc = XDocument.Load(filePath);

            var logElement = new XElement(Resource1.logElementXml,
                new XElement(Resource1.animalElementXml, string.Format(Resource1.log, log.Animal.Type, log.Animal.Name)),
                new XElement(Resource1.informationElementXml, log.Information),
                new XElement(Resource1.timestampElementXml, DateTime.Now)
            );

            doc.Root!.Add(logElement);

            doc.Save(filePath);
        }
    }
}
