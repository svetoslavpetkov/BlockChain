using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.UI
{
    public static class Output
    {
        public static void WriteHeading(string message)
        {
            Write(message, ConsoleColor.Blue, ConsoleColor.White);
        }

        public static void WriteError(string message)
        {
            Write(message, ConsoleColor.Red, ConsoleColor.White);
        }

        public static void WriteSuccess(string message)
        {
            Write(message, ConsoleColor.Green, ConsoleColor.White);
        }


        public static void Write(string message, ConsoleColor foreground, ConsoleColor background)
        {
            string surroundingLines = " ".PadRight(message.Length + 2);

            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            Console.WriteLine(surroundingLines);
            Console.WriteLine(" " + message + " ");
            Console.WriteLine(surroundingLines);
            Console.ResetColor();
        }
    }
}
