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
        public Int64 depositorsNumber { get; set; }

        /// <summary>
        ///  Сумма вкладов
        /// </summary>
        [XmlElement("depositsSum")]
        public decimal depositsSum { get; set; }

        public Bank(string _adress, Int64 _depositorsNumber, decimal _depositsSum)
        {
            this.adress = _adress;
            this.depositorsNumber = _depositorsNumber;
            this.depositsSum = _depositsSum;
        }

        public Bank()
        {
        }
    }
}