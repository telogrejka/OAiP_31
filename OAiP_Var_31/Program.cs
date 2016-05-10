using System;

namespace OAiP_Var_31
{
    internal class Program
    {
        /// <summary>
        /// Точка входа в программу
        /// </summary>
        /// <param name="args">Список аргументов командной строки</param>
        private static void Main(string[] args)
        {
            //Methods.CreateTestFile();
            while (true)
            {
                ShowMenu();
            }
        }

        /// <summary>
        /// Главное меню программы.
        /// </summary>
        private static void ShowMenu()
        {
            //Methods.CreateTestFile();
            Methods.PrintMainMenu();

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.D1:
                    Methods.LoadFile();
                    break;

                case ConsoleKey.D2:
                    Methods.AddRecord();
                    break;

                case ConsoleKey.D3:
                    Methods.Print();
                    break;

                case ConsoleKey.D4:
                    Methods.SaveFile();
                    break;

                case ConsoleKey.D5:
                    Methods.InsertRecord();
                    break;

                case ConsoleKey.D6:
                    Methods.DeleteRecords();
                    break;

                case ConsoleKey.D7:
                    Methods.GetAvg();
                    break;

                case ConsoleKey.D8:
                    Methods.About();
                    break;

                case ConsoleKey.D9:
                    Methods.Exit();
                    break;

                default:
                    break;
            }
        }
    }
}