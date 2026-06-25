using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicServiceDBTools.Extensions
{
    public static class LogExt
    {

        public static void Warning(string text, params object[] args)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[WRN]: {text}", args);
            Console.ForegroundColor = color;
        }

        public static void Error(Exception ex)
        {
            var exMessage = ex.ToString();
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERR]: {exMessage}");
            Console.ForegroundColor = color;
        }

        public static void Error(string text, params object[] args)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[WRN]: {text}", args);
            Console.ForegroundColor = color;
        }

        public static void Info(string text, params object[] args)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"[WRN]: {text}", args);
            Console.ForegroundColor = color;
        }

        public static void Headline(string text, params object[] args)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[WRN]: {text}", args);
            Console.ForegroundColor = color;
        }

    }
}
