using System;
using System.Xml.Serialization;

namespace OAiP_Var_31
{
    /// <summary>
    /// Элемент банка.
    /// </summary>
    [Serializable]
    public class Bank
    {
        /// <summary>
        /// Адрес
        /// </summary>
        [XmlElement("Adress")]
        public string adress { get; set; }

        /// <summary>
        /// Количество вкладчиков
        /// </summary>
        [XmlElement("depositorsNumber")]
        public long depositorsNumber { get; set; }

        /// <summary>
        ///  Сумма вкладов
        /// </summary>
        [XmlElement("depositsSum")]
        public decimal depositsSum { get; set; }

        public Bank(string _adress, long _depositorsNumber, decimal _depositsSum)
        {
            adress = _adress;
            depositorsNumber = _depositorsNumber;
            depositsSum = _depositsSum;
        }

        public Bank()
        {
        }
    }
}