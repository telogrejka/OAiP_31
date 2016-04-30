using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAiP_Var_31
{
    class Program
    {
        private static BankCollection bankCollection = null;
        private static string fileName = null;
        private static bool isFileModified = false;
        private static string adress;
        private static Int64 depositorsNumber;
        private static decimal depositsSum;
        static void Main(string[] args)
        {
            //CreateTestFile();
            while (true)
            {
                ShowMenu();
            }
        }

        private static void CreateTestFile()
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

        private static void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("Добро пожаловать в <Банки города 1.0>");
            Console.WriteLine();
            Console.WriteLine("Выберите один из пунктов меню:");
            Console.WriteLine("1. Загрузить файл");
            Console.WriteLine("2. Добавить запись в конец файла");
            Console.WriteLine("3. Просмотр всех записей");
            Console.WriteLine("4. Сохранить файл");
            Console.WriteLine("5. Добавить запись перед выбранной записью");
            Console.WriteLine("6. Удалить записи начиная с выбранной");
            Console.WriteLine("7. Рассчет среднего значения");
            Console.WriteLine("8. О программе");
            Console.WriteLine("9. Выход");
            Console.WriteLine();

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.D1:
                    LoadFile();
                    break;
                case ConsoleKey.D2:
                    AddRecord();
                    break;
                case ConsoleKey.D3:
                    Print();
                    break;
                case ConsoleKey.D4:
                    SaveFile();
                    break;
                case ConsoleKey.D5:
                    InsertRecord();
                    break;
                case ConsoleKey.D6:
                    DeleteRecords();
                    break;
                case ConsoleKey.D7:
                    GetAvg();
                    break;
                case ConsoleKey.D8:
                    About();
                    break;
                case ConsoleKey.D9:
                    Exit();
                    break;
                default:
                    break;
            }
        }

        private static void DeleteRecords()
        {
            if (!isLoaded()) return;

            Console.Clear();
            Console.WriteLine("Удаление записей.\n");
            int index = 0;
            EnterIndex(ref index);
            int count = bankCollection.Collection.Count() - index;
            Console.WriteLine(String.Format("Внимание! Будут удалены все записи, начиная с {0}-й. Продолжить? y/n", index));
            if(Console.ReadKey(true).Key == ConsoleKey.Y)
            {
                bankCollection.Collection.RemoveRange(index, count);
                Console.WriteLine("Записи удалены.");
                Print();
            }
        }

        private static void InsertRecord()
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

        private static void EnterIndex(ref int index)
        {
            Console.Write(String.Format("Введите номер записи (от 0 до {0}): ", 
                bankCollection.Collection.Count() - 1));
            try
            {
                index = Convert.ToInt32(Console.ReadLine());
                if(index < 0 || index > bankCollection.Collection.Count() - 1)
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

        private static void SaveFile()
        {
            string fileData = BankCollection.Serialize(bankCollection);
            File.WriteAllText(fileName, fileData);
            Console.WriteLine("Файл {0} сохранен.", fileName);
            Console.ReadLine();
            isFileModified = false;
        }

        private static void AddRecord()
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

        private static void EnterRecordData()
        {
            EnterAdress();
            EnterDepositorsNumber();
            EnterDepositsSum();
        }

        private static void EnterAdress()
        {
            Console.Write("Адрес: ");
            adress = Console.ReadLine();
            if (string.IsNullOrEmpty(adress))
            {
                EnterAdress();
            }
        }

        private static void EnterDepositorsNumber()
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

        private static void EnterDepositsSum()
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

        private static void LoadFile()
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

        private static bool isLoaded()
        {
            if (bankCollection == null)
            {
                Console.WriteLine("Сначала надо загрузить файл.");
                Console.ReadLine();
                return false;
            }
            else return true;
        }

        private static void Print()
        {
            if (!isLoaded()) return;
            PrintLine();
            PrintRow(new string[3] { "Адрес", "Количество вкладчиков", "Сумма вкладов " });
            foreach (var item in bankCollection.Collection)
            {
                PrintLine();
                PrintRow(new string[3] {item.adress, item.depositorsNumber.ToString(), item.depositsSum.ToString()});
            }
            PrintLine();
            Console.ReadKey();
        }

        static int tableWidth = 77;

        static void PrintLine()
        {
            Console.WriteLine(new string('-', tableWidth));
        }

        static void PrintRow(params string[] columns)
        {
            int width = (tableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            Console.WriteLine(row);
        }

        static string AlignCentre(string text, int width)
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

        private static string GetPath()
        {
            Console.Write("Введите путь к файлу: ");
            fileName = Console.ReadLine();
            if (String.IsNullOrEmpty(fileName))
            {
                Console.WriteLine("Путь не указан.");
                GetPath();
            }
            if(!File.Exists(fileName))
            {
                Console.WriteLine("Файл не существует.");
                GetPath();
            }
            return fileName;
        }

        private static void Exit()
        {
            Console.Clear();
            if(isFileModified)
            {
                Console.WriteLine("Сохранить изменения в файле? y/n");
                if(Console.ReadKey(true).Key == ConsoleKey.Y)
                {
                    SaveFile();
                }
            }
            //Console.WriteLine("Вы уверенны что хотите выйти? y/n");
            //if(Console.ReadKey(true).Key == ConsoleKey.Y)
 	        Environment.Exit(0);
        }

        private static void GetAvg()
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

        private static void CalcAvg(string param)
        {
            int startIndex = 0;
            int endIndex = 0;
            EnterRange(ref startIndex, ref endIndex);
            switch(param)
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

        private static void CalcDepositorsSumAVG(int startIndex, int endIndex)
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

        private static void CalcDepositorsNumberAVG(int startIndex, int endIndex)
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

        private static void PrintRange(int startIndex, int endIndex)
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

        private static void EnterRange(ref int startIndex, ref int endIndex)
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
                Console.Write(String.Format("Конец диапазона (от {0} до {1}): ", startIndex ,bankCollection.Collection.Count() - 1));
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

        private static void About()
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
