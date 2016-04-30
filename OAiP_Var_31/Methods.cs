using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace OAiP_Var_31
{
    public static class Methods
    {
        public static BankCollection bankCollection = null;
        public static string fileName = null;
        public static bool isFileModified = false;
        public static string adress;
        public static Int64 depositorsNumber;
        public static decimal depositsSum;

        public static void CreateTestFile()
        {
            BankCollection bankCollection = new BankCollection();
            bankCollection.Collection.Add(new Bank("Абсолютбанк", 2678, 500100360.16m));
            bankCollection.Collection.Add(new Bank("БелВЭБ", 1610, 326171360.54m));
            bankCollection.Collection.Add(new Bank("Идея Банк", 3830, 779211990.97m));
            bankCollection.Collection.Add(new Bank("Приорбанк", 8124, 1555123125.12m));
            bankCollection.Collection.Add(new Bank("Хоум Кредит Банк", 2519, 14131244.71m));

            string fileData = BankCollection.Serialize(bankCollection);
            Console.WriteLine(fileData);
            File.WriteAllText("brest.xml", fileData);
        }

        public static void DeleteRecords()
        {
            if (!isLoaded()) return;

            Console.Clear();
            Console.WriteLine("Удаление записей.\n");
            int index = 0;
            EnterIndex(ref index);
            int count = bankCollection.Collection.Count() - index;
            Console.WriteLine(String.Format("Внимание! Будут удалены все записи, начиная с {0}-й. Продолжить? y/n", index));
            if (Console.ReadKey(true).Key == ConsoleKey.Y)
            {
                bankCollection.Collection.RemoveRange(index, count);
                Console.WriteLine("Записи удалены.");
                Print();
            }
        }

        public static void InsertRecord()
        {
            if (!isLoaded()) return;

            Console.Clear();
            Console.WriteLine("Вставка записи.\n");
            int index = 0;
            EnterIndex(ref index);

            EnterRecordData();

            bankCollection.Collection.Insert(index, (new Bank(adress, depositorsNumber, depositsSum)));
            Console.WriteLine("Запись успешно добавлена!");
            isFileModified = true;
            Print();
        }

        public static void EnterIndex(ref int index)
        {
            Console.Write(String.Format("Введите номер записи (от 0 до {0}): ",
                bankCollection.Collection.Count() - 1));
            try
            {
                index = Convert.ToInt32(Console.ReadLine());
                if (index < 0 || index > bankCollection.Collection.Count() - 1)
                {
                    Console.WriteLine("Введенный индекс находится за пределами возможного диапазона.");
                    EnterIndex(ref index);
                }
            }
            catch
            {
                Console.WriteLine("Неверный формат.");
                EnterIndex(ref index);
            }
        }

        public static void SaveFile()
        {
            string fileData = BankCollection.Serialize(bankCollection);
            File.WriteAllText(fileName, fileData);
            Console.WriteLine("Файл {0} сохранен.", fileName);
            Console.ReadLine();
            isFileModified = false;
        }

        public static void AddRecord()
        {
            if (!isLoaded()) return;

            Console.Clear();
            Console.WriteLine("Добавление записи\n");

            EnterRecordData();

            bankCollection.Collection.Add(new Bank(adress, depositorsNumber, depositsSum));
            Console.WriteLine("Запись успешно добавлена!");
            isFileModified = true;
            Print();
        }

        public static void EnterRecordData()
        {
            EnterAdress();
            EnterDepositorsNumber();
            EnterDepositsSum();
        }

        public static void EnterAdress()
        {
            Console.Write("Адрес: ");
            adress = Console.ReadLine();
            if (string.IsNullOrEmpty(adress))
            {
                EnterAdress();
            }
        }

        public static void EnterDepositorsNumber()
        {
            Console.Write("Количество вкладчиков: ");
            try
            {
                depositorsNumber = Convert.ToInt64(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("Неверный формат.");
                EnterDepositorsNumber();
            }
        }

        public static void EnterDepositsSum()
        {
            Console.Write("Сумма вкладов: ");
            try
            {
                depositsSum = Convert.ToDecimal(Console.ReadLine(), new CultureInfo("en-US"));
            }
            catch
            {
                Console.WriteLine("Неверный формат.");
                EnterDepositsSum();
            }
        }

        public static void LoadFile()
        {
            Console.Clear();
            GetPath();
            string fileData = File.ReadAllText(fileName);
            bankCollection = BankCollection.Deserialize(fileData);
            if (bankCollection != null)
            {
                Console.WriteLine(String.Format("Файл {0} загружен!\n", fileName));
                isFileModified = false;
                Print();
            }
            else
            {
                Console.WriteLine(String.Format("Ошибка при чтении файла {0}", fileName));
                Console.ReadLine();
            }
        }

        public static bool isLoaded()
        {
            if (bankCollection == null)
            {
                Console.WriteLine("Сначала надо загрузить файл.");
                Console.ReadLine();
                return false;
            }
            else return true;
        }

        public static void Print()
        {
            if (!isLoaded()) return;
            PrintLine();
            PrintRow(new string[3] { "Адрес", "Количество вкладчиков", "Сумма вкладов " });
            foreach (var item in bankCollection.Collection)
            {
                PrintLine();
                PrintRow(new string[3] { item.adress, item.depositorsNumber.ToString(), item.depositsSum.ToString() });
            }
            PrintLine();
            Console.ReadKey();
        }

        private static int tableWidth = 77;

        private static void PrintLine()
        {
            Console.WriteLine(new string('-', tableWidth));
        }

        private static void PrintRow(params string[] columns)
        {
            int width = (tableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            Console.WriteLine(row);
        }

        private static string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }

        public static string GetPath()
        {
            Console.Write("Введите путь к файлу: ");
            fileName = Console.ReadLine();
            if (String.IsNullOrEmpty(fileName))
            {
                Console.WriteLine("Путь не указан.");
                GetPath();
            }
            if (!File.Exists(fileName))
            {
                Console.WriteLine("Файл не существует.");
                GetPath();
            }
            return fileName;
        }

        public static void Exit()
        {
            Console.Clear();
            if (isFileModified)
            {
                Console.WriteLine("Сохранить изменения в файле? y/n");
                if (Console.ReadKey(true).Key == ConsoleKey.Y)
                {
                    SaveFile();
                }
            }
            //Console.WriteLine("Вы уверенны что хотите выйти? y/n");
            //if(Console.ReadKey(true).Key == ConsoleKey.Y)
            Environment.Exit(0);
        }

        public static void GetAvg()
        {
            if (!isLoaded()) return;
            Console.Clear();
            Console.WriteLine("Выберите параметр для рассчета среднего значения:");
            Console.WriteLine("1. Количество вкладчиков");
            Console.WriteLine("2. Сумма вкладов");
            Console.WriteLine("3. Назад");
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.D1:
                    CalcAvg("depositorsNumber");
                    break;

                case ConsoleKey.D2:
                    CalcAvg("depositsSum");
                    break;

                case ConsoleKey.D3:
                    return;

                default:
                    break;
            }
            Console.ReadLine();
        }

        public static void CalcAvg(string param)
        {
            int startIndex = 0;
            int endIndex = 0;
            EnterRange(ref startIndex, ref endIndex);
            switch (param)
            {
                case "depositorsNumber":
                    CalcDepositorsNumberAVG(startIndex, endIndex);
                    break;

                case "depositsSum":
                    CalcDepositorsSumAVG(startIndex, endIndex);
                    break;

                default:
                    break;
            }
        }

        public static void CalcDepositorsSumAVG(int startIndex, int endIndex)
        {
            PrintRange(startIndex, endIndex);
            decimal depositorsSumAVG = 0;
            int count = endIndex - startIndex + 1;
            for (int i = startIndex; i <= endIndex; i++)
            {
                depositorsSumAVG += bankCollection.Collection.ElementAt(i).depositsSum / (decimal)count;
            }
            Console.WriteLine(String.Format("Средняя сумма вкладов: {0}", depositorsSumAVG));
        }

        public static void CalcDepositorsNumberAVG(int startIndex, int endIndex)
        {
            PrintRange(startIndex, endIndex);
            decimal depositorsNumberAVG = 0;
            int count = endIndex - startIndex + 1;
            for (int i = startIndex; i <= endIndex; i++)
            {
                depositorsNumberAVG += bankCollection.Collection.ElementAt(i).depositorsNumber / (decimal)count;
            }
            Console.WriteLine(String.Format("Среднее колчичество вкладчиков: {0}", depositorsNumberAVG));
        }

        public static void PrintRange(int startIndex, int endIndex)
        {
            PrintLine();
            PrintRow(new string[3] { "Адрес", "Количество вкладчиков", "Сумма вкладов " });
            for (int i = startIndex; i <= endIndex; i++)
            {
                PrintLine();
                PrintRow(new string[3] { bankCollection.Collection.ElementAt(i).adress,
                    bankCollection.Collection.ElementAt(i).depositorsNumber.ToString(),
                    bankCollection.Collection.ElementAt(i).depositsSum.ToString() });
            }
            PrintLine();
        }

        public static void EnterRange(ref int startIndex, ref int endIndex)
        {
            try
            {
                Console.Write(String.Format("Начало диапазона (от 0 до {0}): ", bankCollection.Collection.Count() - 1));
                startIndex = Convert.ToInt32(Console.ReadLine());
                if (startIndex < 0 || startIndex > bankCollection.Collection.Count() - 1)
                {
                    Console.WriteLine("Введенный индекс находится за пределами возможного диапазона.");
                    EnterRange(ref startIndex, ref endIndex);
                }
                Console.Write(String.Format("Конец диапазона (от {0} до {1}): ", startIndex, bankCollection.Collection.Count() - 1));
                endIndex = Convert.ToInt32(Console.ReadLine());
                if (endIndex < startIndex || endIndex > bankCollection.Collection.Count() - 1)
                {
                    Console.WriteLine("Введенный индекс находится за пределами возможного диапазона.");
                    EnterRange(ref startIndex, ref endIndex);
                }
            }
            catch
            {
                Console.WriteLine("Неверный формат.");
                EnterRange(ref startIndex, ref endIndex);
            }
        }

        public static void About()
        {
            Console.Clear();
            Console.WriteLine("<Банки города 1.0>");
            Console.WriteLine("Автор: студент группы 569 Наумовец Артем.");
            Console.WriteLine("БрГТУ, 2016.");
            Console.WriteLine();
            Console.WriteLine("Для продолжения нажмите любую клавишу...");
            if (Console.ReadKey(true).Key > 0)
                return;
        }
    }
}