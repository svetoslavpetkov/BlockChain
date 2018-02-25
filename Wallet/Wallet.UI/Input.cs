using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wallet.UI
{
    public static class Input
    {
        public static string Command(string label, IEnumerable<string> commands)
        {
            string result = "";
            bool validInput = false;
            while (!validInput)
            {
                result = "";
                Console.Write(label + " " + string.Join(',', commands) + ": ");
                var pressedKey = Console.ReadKey(true);
                while (pressedKey.Key != ConsoleKey.Enter)
                {
                    if (pressedKey.Key == ConsoleKey.Tab)
                    {
                        var commandsMathc = commands.SingleOrDefault(c => c.StartsWith(result));
                        if (commandsMathc != null)
                        {
                            Console.Write(commandsMathc.Substring(result.Length));
                            result = commandsMathc;
                        }
                    }
                    else if (pressedKey.Key == ConsoleKey.Backspace)
                    {
                        if (result.Length == 0)
                        {
                            result = string.Empty;
                        }
                        else if (result.Length == 1)
                        {
                            result = string.Empty;
                            Console.Write("\b\0\b");
                        }
                        else
                        {
                            Console.Write("\b\0\b");
                            result = result.Substring(0, result.Length - 1);
                        }
                    }
                    else
                    {
                        result += pressedKey.KeyChar;
                        Console.Write(pressedKey.KeyChar);
                    }

                    pressedKey = Console.ReadKey(true);
                }

                if (commands.Any(c => c.ToLower() == result.ToLower()))
                {
                    break;
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            return result.ToLower();
        }

        public static string String(string label, Func<string, bool> validation = null)
        {
            string result = string.Empty;

            while (result == string.Empty)
            {
                Console.Write(label);
                result = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(result) ||
                    (validation != null && !validation(result))
                    )
                {
                    Output.WriteError("Input is invvalid");
                    result = string.Empty;
                }
            }


            return result;
        }

        public static string Password(string message = "Enter password:")
        {
            Console.Write(message);
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                ConsoleKeyInfo cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }

                if (cki.Key == ConsoleKey.Backspace)
                {
                    if (sb.Length > 0)
                    {
                        Console.Write("\b\0\b");
                        sb.Length--;
                    }

                    continue;
                }

                Console.Write('$');
                sb.Append(cki.KeyChar);
            }

            return sb.ToString();
        }

        public static uint Uint(string message, uint rangeFrom = uint.MinValue, uint rangeTo = uint.MaxValue)
        {
            uint result = 0;

            string input = String(message, x => { return uint.TryParse(x, out result) && result >= rangeFrom && result <= rangeTo; });

            return uint.Parse(input);
        }

        public static int Int(string message, int rangeFrom = 0, int rangeTo = int.MaxValue)
        {
            int result = 0;

            string input = String(message, x => { return int.TryParse(x, out result) && result >= rangeFrom && result <= rangeTo; });

            return int.Parse(input);
        }

        public static decimal Decimal(string message)
        {
            decimal result = 0m;

            string input = String(message, x => { return decimal.TryParse(x, out result); });

            return decimal.Parse(input);
        }

        public static void EnterKey(string message = "Press [Enter] key to continue")
        {
            Console.WriteLine(message);
            Console.ReadLine();
        }
    }
}
