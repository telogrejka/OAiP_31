using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OAiP_Var_31
{
    [Serializable]
    public class BankCollection
    {
        [XmlArray("Collection"), XmlArrayItem("Item")]
        public List<Bank> Collection { get; set; }

        public BankCollection()
        {
            Collection = new List<Bank>();
        }

        public static string Serialize(BankCollection data)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(BankCollection));
                StringWriter stringWriter = new StringWriter();
                xmlSerializer.Serialize(stringWriter, data);
                return stringWriter.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("При сохранении файла возникла ошибка: " + ex.Message);
            }
            return null;
         }

        public static BankCollection Deserialize(string serializedData)
        {
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(BankCollection));
                var stringReader = new StringReader(serializedData);
                return (BankCollection)xmlSerializer.Deserialize(stringReader);
            }
            catch(Exception ex)
            {
                Console.WriteLine("При загрузке файла возникла ошибка: " + ex.Message);
            }
            return null;
        }
    }
}
