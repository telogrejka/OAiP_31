using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OAiP_Var_31
{
    /// <summary>
    /// Класс содержит процедуры реализующие запросы.
    /// </summary>
    public static class Methods
    {
        private static BankCollection bankCollection = null;
        private static string fileName = null;
        private static bool isFileModified = false;
        private static int tableWidth = Console.WindowWidth - 3;

        /// <summary>
        /// Создание тестового файла содержащего массив банков.
        /// </summary>
        public static void CreateTestFile()
        {
            BankCollection bankCollection = new BankCollection();
            bankCollection.Collection.Add(new Bank("Абсолютбанк", 267, 5000.16m));
            bankCollection.Collection.Add(new Bank("БелВЭБ", 161, 3261.54m));
            bankCollection.Collection.Add(new Bank("Идея Банк", 380, 7792.97m));
            bankCollection.Collection.Add(new Bank("Приорбанк", 824, 15551.12m));
            bankCollection.Collection.Add(new Bank("Хоум Кредит Банк", 2519, 1413.71m));

            string fileData = BankCollection.Serialize(bankCollection);
            File.WriteAllText("brest.xml", fileData);
        }

        /// <summary>
        /// Выводит главное меню.
        /// </summary>
        public static void PrintMainMenu()
        {
            Console.Clear();
            PrintAppInfo();
            Console.WriteLine("Выберите один из пунктов меню:");
            Console.WriteLine("1. Загрузить файл");
            Console.WriteLine("2. Добавить запись в конец файла");
            Console.WriteLine("3. Просмотр всех записей");
            Console.WriteLine("4. Сохранить файл");
            Console.WriteLine("5. Добавить запись перед выбранной записью");
            Console.WriteLine("6. Удалить записи начиная с выбранной");
            Console.WriteLine("7. Расчет среднего значения");
            Console.WriteLine("8. О программе");
            Console.WriteLine("9. Выход");
            Console.WriteLine();
        }

        /// <summary>
        /// Загрузка информации из текстового файла в массив указателей на записи.
        /// </summary>
        public static void LoadFile()
        {
            Console.Clear();
            GetPath();
            string fileData = File.ReadAllText(fileName);
            bankCollection = BankCollection.Deserialize(fileData);
            if (bankCollection != null)
            {
                Console.WriteLine(string.Format("Файл {0} загружен!\n", fileName));
                isFileModified = false;
                Print();
            }
            else
            {
                Console.WriteLine(string.Format("Ошибка при чтении файла {0}", fileName));
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Ввод пути к файлу.
        /// </summary>
        /// <returns>Путь к файлу.</returns>
        private static string GetPath()
        {
            Console.Write("Введите путь к файлу: ");
            fileName = Console.ReadLine();
            if (string.IsNullOrEmpty(fileName))
            {
                Console.WriteLine("Путь не указан.");
                return GetPath();
            }
            if (!File.Exists(fileName))
            {
                Console.WriteLine("Файл не существует.");
                return GetPath();
            }
            return fileName;
        }

        /// <summary>
        /// Добавление новых элементов в конец коллекции.
        /// </summary>
        public static void AddRecord()
        {
            if (!isLoaded()) return;
            Console.Clear();
            Console.WriteLine("Добавление записи\n");
            Bank bank = EnterBankData();
            bankCollection.Collection.Add(bank);
            Console.WriteLine("Запись успешно добавлена!");
            isFileModified = true;
            Print();
        }

        /// <summary>
        /// Просмотр элементов массива.
        /// </summary>
        /// <param name="start">Начало диапазона. По умолчанию 0.</param>
        /// <param name="end">Конец диапазона. По умолчанию равен количеству элементов массива.</param>
        public static void Print(int start = 0, int end = -1)
        {
            if (!isLoaded()) return;

            if (start < 0)
                start = 0;
            if (start > bankCollection.Collection.Count)
                start = bankCollection.Collection.Count;
            if (end < 0 || end > bankCollection.Collection.Count)
                end = bankCollection.Collection.Count;

            PrintHeader();
            for (int i = start; i < end; i++)
            {
                PrintRow(i.ToString(),
                    bankCollection.Collection.ElementAt(i).adress,
                    bankCollection.Collection.ElementAt(i).depositorsNumber.ToString(),
                    bankCollection.Collection.ElementAt(i).depositsSum.ToString());
                PrintLine();
            }
            Console.ReadKey();
        }

        /// <summary>
        /// Печать шапки таблицы.
        /// </summary>
        private static void PrintHeader()
        {
            PrintLine();
            PrintRow("№", "Адрес", "Количество вкладчиков", "Сумма вкладов");
            PrintLine();
        }

        /// <summary>
        /// Сохранение файла.
        /// </summary>
        public static void SaveFile()
        {
            if (!isLoaded()) return;
            string fileData = BankCollection.Serialize(bankCollection);
            File.WriteAllText(fileName, fileData);
            Console.WriteLine("Файл {0} сохранен.", fileName);
            isFileModified = false;
            Console.ReadLine();
        }

        /// <summary>
        /// Вставка нового элемента перед выбранным элементом.
        /// </summary>
        public static void InsertRecord()
        {
            if (!isLoaded()) return;
            Console.Clear();
            Console.WriteLine("Вставка записи.\n");
            int index = EnterIndex();
            Bank bank = EnterBankData();
            bankCollection.Collection.Insert(index, bank);
            Console.WriteLine("Запись успешно добавлена!");
            isFileModified = true;
            Print();
        }

        /// <summary>
        /// Удаление элементов, начиная от выбранного.
        /// </summary>
        public static void DeleteRecords()
        {
            if (!isLoaded()) return;
            Console.Clear();
            Console.WriteLine("Удаление записей.\n");
            int index = EnterIndex();
            int count = bankCollection.Collection.Count() - index;
            Console.WriteLine(string.Format("Внимание! Будут удалены все записи, начиная с {0}-й. Продолжить? y/n", index));
            if (Console.ReadKey(true).Key == ConsoleKey.Y)
            {
                bankCollection.Collection.RemoveRange(index, count);
                isFileModified = true;
                Console.WriteLine("Записи удалены.");
                Print();
            }
        }

        /// <summary>
        /// Выбор параметра для которого необходимо расчитать среднее значение.
        /// </summary>
        public static void GetAvg()
        {
            if (!isLoaded()) return;
            PrintAVGMenu();
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
        }

        /// <summary>
        /// Вывод меню расчета среднего значения.
        /// </summary>
        private static void PrintAVGMenu()
        {
            Console.Clear();
            Console.WriteLine("Выберите параметр для расчета среднего значения:");
            Console.WriteLine("1. Количество вкладчиков");
            Console.WriteLine("2. Сумма вкладов");
            Console.WriteLine("3. Назад");
        }

        /// <summary>
        /// Вызов соответствующего метода для вычисления среднего.
        /// </summary>
        /// <param name="param">Поле, по которому будет вычисляться среднеее.</param>
        private static void CalcAvg(string param)
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

        /// <summary>
        /// Ввод с клавиатуры номера записи.
        /// </summary>
        /// <param name="index">Номер записи.</param>
        private static int EnterIndex()
        {
            Console.Write(string.Format("Введите номер записи, перед которой будет вставлена новая (от 0 до {0}): ",
                bankCollection.Collection.Count() - 1));
            int index = 0;
            try
            {
                index = Convert.ToInt32(Console.ReadLine());
                if (index < 0 || index > bankCollection.Collection.Count() - 1)
                {
                    Console.WriteLine("Введенный индекс находится за пределами возможного диапазона.");
                    return EnterIndex();
                }
                return index;
            }
            catch
            {
                Console.WriteLine("Неверный формат.");
                return EnterIndex();
            }
        }

        /// <summary>
        /// Ввод данных для одной записи.
        /// </summary>
        private static Bank EnterBankData()
        {
            string adress = EnterAdress();
            long depositorsNumber = EnterDepositorsNumber();
            decimal depositsSum = EnterDepositsSum();
            return new Bank(adress, depositorsNumber, depositsSum);
        }

        /// <summary>
        /// Ввод адреса.
        /// </summary>
        private static string EnterAdress()
        {
            Console.Write("Адрес: ");
            string adress = Console.ReadLine();
            if (string.IsNullOrEmpty(adress))
            {
                return EnterAdress();
            }
            else
            {
                return adress;
            }
        }

        /// <summary>
        /// Ввод количества вкладчиков.
        /// </summary>
        private static long EnterDepositorsNumber()
        {
            long depositorsNumber = 0;
            Console.Write("Количество вкладчиков: ");
            try
            {
                depositorsNumber = Convert.ToInt64(Console.ReadLine());
                return depositorsNumber;
            }
            catch
            {
                Console.WriteLine("Неверный формат.");
                return EnterDepositorsNumber();
            }
        }

        /// <summary>
        /// Ввод суммы вкладов.
        /// </summary>
        private static decimal EnterDepositsSum()
        {
            decimal depositsSum = 0m;
            Console.Write("Сумма вкладов: ");
            try
            {
                depositsSum = Convert.ToDecimal(Console.ReadLine(), new CultureInfo("en-US"));
                return depositsSum;
            }
            catch
            {
                Console.WriteLine("Неверный формат.");
                return EnterDepositsSum();
            }
        }

        /// <summary>
        /// Проверка загружен ли файл в программу.
        /// </summary>
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

        /// <summary>
        /// Печать линии.
        /// </summary>
        private static void PrintLine()
        {
            Console.WriteLine(new string('-', tableWidth));
        }

        /// <summary>
        /// Печать строки.
        /// </summary>
        private static void PrintRow(string index, string column1, string column2, string column3)
        {
            int columnWidth = (tableWidth - 3) / 3;
            int indexWidth = bankCollection.Collection.Count.ToString().Length;

            string row = string.Format("|{0}|{1}|{2}|{3}|",
                AlignCentre(index, indexWidth),
                AlignCentre(column1, columnWidth),
                AlignCentre(column2, columnWidth),
                AlignCentre(column3, columnWidth - indexWidth));

            Console.WriteLine(row);
        }

        /// <summary>
        /// Выравнивание текста по центру столбца.
        /// </summary>
        /// <param name="text">Текст.</param>
        /// <param name="width">Ширина столбцов таблицы.</param>
        /// <returns></returns>
        public static string AlignCentre(string text, int width)
        {
            if (text.Length > width)
            {
                text = text.Substring(0, width - 3) + "...";
            }

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }

        /// <summary>
        /// Вычисление среднего значения суммы вкладов по заданному диапазону.
        /// </summary>
        /// <param name="startIndex">Начало диапазона.</param>
        /// <param name="endIndex">Конец диапазона.</param>
        private static void CalcDepositorsSumAVG(int startIndex, int endIndex)
        {
            decimal depositorsSumAVG = 0;
            int count = endIndex - startIndex + 1;
            for (int i = startIndex; i <= endIndex; i++)
            {
                depositorsSumAVG += bankCollection.Collection.ElementAt(i).depositsSum / count;
            }
            Console.WriteLine(string.Format("\nСредняя сумма вкладов: {0:0.00}", depositorsSumAVG));
            Print(startIndex, endIndex + 1);
        }

        /// <summary>
        /// Вычисление среднего олчичество вкладчиков по заданному диапазону.
        /// </summary>
        /// <param name="startIndex">Начало диапазона.</param>
        /// <param name="endIndex">Конец диапазона.</param>
        private static void CalcDepositorsNumberAVG(int startIndex, int endIndex)
        {
            decimal depositorsNumberAVG = 0;
            int count = endIndex - startIndex + 1;
            for (int i = startIndex; i <= endIndex; i++)
            {
                depositorsNumberAVG += bankCollection.Collection.ElementAt(i).depositorsNumber / (decimal)count;
            }
            Console.WriteLine(string.Format("\nСреднее количество вкладчиков: {0:0.00}", depositorsNumberAVG));
            Print(startIndex, endIndex + 1);
        }

        /// <summary>
        /// Ввод с клавиатуры начала и конца диапазона.
        /// </summary>
        /// <param name="startIndex">Начало диапазона.</param>
        /// <param name="endIndex">Конец диапазона.</param>
        private static void EnterRange(ref int startIndex, ref int endIndex)
        {
            try
            {
                Console.Write(string.Format("Начало диапазона (от 0 до {0}): ", bankCollection.Collection.Count() - 1));
                startIndex = Convert.ToInt32(Console.ReadLine());
                if (startIndex < 0 || startIndex > bankCollection.Collection.Count() - 1)
                {
                    Console.WriteLine("Введенный индекс находится за пределами возможного диапазона.");
                    EnterRange(ref startIndex, ref endIndex);
                }
                Console.Write(string.Format("Конец диапазона (от {0} до {1}): ", startIndex, bankCollection.Collection.Count() - 1));
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

        /// <summary>
        /// Вывод информации о программе.
        /// </summary>
        public static void About()
        {
            Console.Clear();
            PrintAppInfo();
            Console.WriteLine("Автор: студент группы 569 Наумовец Артем.");
            Console.WriteLine("БрГТУ, 2016.");
            Console.WriteLine();
            Console.WriteLine("Для продолжения нажмите любую клавишу...");
            if (Console.ReadKey(true).Key > 0)
                return;
        }

        /// <summary>
        /// Выводит название и версию программы.
        /// </summary>
        private static void PrintAppInfo()
        {
            string appInfo = string.Format("<{0} {1}>",
                Assembly.GetExecutingAssembly().GetName().Name,
                Assembly.GetExecutingAssembly().GetName().Version);
            Console.SetCursorPosition((Console.WindowWidth - appInfo.Length) / 2, Console.CursorTop);
            Console.WriteLine(appInfo);
            Console.WriteLine();
        }

        /// <summary>
        /// Выход из программы.
        /// </summary>
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
            Environment.Exit(0);
        }
    }
}