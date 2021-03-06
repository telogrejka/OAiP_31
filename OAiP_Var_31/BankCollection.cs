﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace OAiP_Var_31
{
    /// <summary>
    /// Коллекция элеметов банка.
    /// </summary>
    [Serializable]
    public class BankCollection
    {
        [XmlArray("Collection"), XmlArrayItem("Item")]
        public List<Bank> Collection { get; set; }

        public BankCollection()
        {
            Collection = new List<Bank>();
        }

        /// <summary>
        /// Сериализация массива банков.
        /// </summary>
        /// <param name="data">Массив банков.</param>
        /// <returns>Массив банков в формате XML.</returns>
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

        /// <summary>
        /// Десериализация массива банков.
        /// </summary>
        /// <param name="serializedData">Массив банков в формате XML.</param>
        /// <returns>Массив банков.</returns>
        public static BankCollection Deserialize(string serializedData)
        {
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(BankCollection));
                var stringReader = new StringReader(serializedData);
                return (BankCollection)xmlSerializer.Deserialize(stringReader);
            }
            catch (Exception ex)
            {
                Console.WriteLine("При загрузке файла возникла ошибка: " + ex.Message);
            }
            return null;
        }
    }
}