using BlockChain.Core;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Wallet.Util;
using System.Text;

namespace Wallet.UI
{
    class Program
    {
        private static IBlockChainClient blockChainClient = new BlockChainClient();

        private static Dictionary<string, Action> Commands = new Dictionary<string, Action>(new List<KeyValuePair<string, Action>>() {
            new KeyValuePair<string, Action>("create",CreateWallet),
            new KeyValuePair<string, Action>("exit",ExitApplication),
            new KeyValuePair<string, Action>("open",OpenWallet)
        });

        private static void OpenWallet()
        {
            try
            {
                string walletName = Input.String("Enter wallet name ");
                string password = Input.Password();

                FullWallet wallet = new FullWallet(walletName, GetWalletPath(walletName), password);

                WalletHandler walletHandler = new WalletHandler(wallet, blockChainClient);

                walletHandler.Run();
            }
            catch (Exception ex)
            {
                Output.WriteError(ex.Message);
                Input.EnterKey();
            }

        }

        private static void ExitApplication()
        {
            throw new ExitCommandException();
        }

        private static void CreateWallet()
        {
            Console.Clear();
            var walletName = Input.String("Enter walletName: ");
            string password = "1";
            string repeatPassword = "2";
            while (password != repeatPassword)
            {
                password = Input.Password();
                repeatPassword = Input.Password("Enter password again: ");
                if (password != repeatPassword)
                {
                    Console.WriteLine("PAsswords do not match");
                }
            }

            var mnemonic = FullWallet.GenerateNewWallet(GetWalletPath(walletName), password);
            Console.WriteLine("Please keep mnemonic safe: " + mnemonic);
            Console.WriteLine();

            var wallet = new FullWallet(walletName, GetWalletPath(walletName), password);

            WalletHandler.PrintWalletAccounts(wallet, blockChainClient);

            Console.WriteLine();
            Console.WriteLine("Press any key");
            Console.ReadLine();
        }

        static void Main(string[] args)
        {

            //var shaTheirs = new TransactionSigner().Sha("marion ladder needle peace rodent stock tribune whiskey ski job wave unit");
            //var shaOurs = new TransactionSigner().CalcSHA256("marion ladder needle peace rodent stock tribune whiskey ski job wave unit");


            //SimpleWallet walet = new SimpleWallet("E9873D79C6D87DC0FB6A5778633389F4453213303DA61F20BD67FC233AA33262");

            //027e755df9dbf072945d376c3f2a5a0dd4fe08a6773975c43734027695f96b9171
            //E9873D79C6D87DC0FB6A5778633389F4453213303DA61F20BD67FC233AA33262
            // our generated privete key 107725345683326611836297844809410268066979832523672886309936076812926793365280
            //"0435839402B35555B48000000295B3226E8D4FC878C1BDE087A65341F558294B0F9D1B1D59EB88B31F510F5CCE00677FC1457CA004949520FB171539144044722815B377FAFF747778CD805EFAE329890C65""
            //"tprv8emgGE8KBmFaqpmi9Rmh3fkwUBQhmMJwoofsNiWCvUW94DY36QwhGNEFe5qjzfdmHyAkqWACGnXDjM2JkGHVDoCAfQ2Z7uXZdNFJBQEK6ax"

            //Console.WriteLine($"My address:{walet.GetAddress()}");
            //var tran = walet.Sign("asadagfasdfad", 12);

            //ITransactionValidator nodeSimulator = new TransactionValidator();
            //bool isValid =  nodeSimulator.IsValid(tran);

            //string path = "C:\\Users\\svetoslav.petkov\\Source\\Repos\\BlockChain\\Wallet\\Wallet.UI\\Wallet.json";
            //string pass = "12345_qwe";

            ////FullWallet.GenerateNewWallet(path, pass);

            //var fullWallet = new FullWallet("name",path, pass);
            //var accounts = fullWallet.GetAccounts();

            //tran = accounts[0].Sign("asfasdfasdfasdfdasasdffasdsd", 5000000);
            //isValid = nodeSimulator.IsValid(tran);

            //tran = accounts[1].Sign("asfasdfasdfasdasfasddfdasasdffasdsd", 1000);
            //isValid = nodeSimulator.IsValid(tran);

            //tran = accounts[2].Sign("asfasdfasdfasdasfasddfdasasdffasdsd", 1000);
            //isValid = nodeSimulator.IsValid(tran);
            try
            {
                while (true)
                {
                    Console.Clear();
                    string command = Input.Command("Enter command", Commands.Select(c => c.Key).ToList());
                        //Input.String("Enter command: " + string.Join(',' ,Commands.Select(c=> c.Key).ToList()) + ": ",
                        //x => { return Commands.ContainsKey(x.ToLower()); }
                        //);
                    //execute the command
                    Commands[command.ToLower()]();
                }
            }
            catch (ExitCommandException ex)
            {
                Console.WriteLine("Exiting. Press any key to close");
                Input.EnterKey();
            }
        }

        /*
        public static List<string> GetWallets()
        {
            DirectoryInfo di = new DirectoryInfo(WalletPath);

            return di.GetFiles
        }*/

        private static string GetWalletPath(string walletName)
        {
            return Path.Combine(WalletPath, walletName + ".json");
        }

        static readonly string WalletPath = ".\\.\\Wallets";
    }


    public class WalletHandler
    {
        public static void PrintWalletAccounts(FullWallet wallet, IBlockChainClient blockChainClient)
        {
            var accounts = wallet.GetAccounts();
            ulong totalBallance = 0;
            for (int i = 0; i < accounts.Count; i++)
            {
                Console.WriteLine($"**** Account  {i}");
                Console.WriteLine($"PrivateKey: " + accounts[i].PrivateKey);
                Console.WriteLine($"Address: " + accounts[i].Address);

                ulong ballance = blockChainClient.GetBalance(accounts[i].Address);
                totalBallance += ballance;

                Console.WriteLine($"Ballance: " + ballance.GetFormattedTokens());
            }
        }

        public FullWallet Wallet { get; set; }

        public IBlockChainClient BlockChainClient { get; set; }

        public Dictionary<string, Action> Commands { get; set; } = new Dictionary<string, Action>() {};

        private void Accounts()
        {
            throw new NotImplementedException();
        }

        public WalletHandler(FullWallet wallet, IBlockChainClient blockChainClient)
        {
            Wallet = wallet;
            BlockChainClient = blockChainClient;
            //new KeyValuePair<string, Action>("accounts",Accounts)
            Commands.Add("accounts", ViewAccounts);
            Commands.Add("send", Send);
            Commands.Add("close", CloseWallet);
        }

        private void CloseWallet()
        {
            throw new ExitCommandException();
        }

        private void Send()
        {
            int accountIndex = Input.Int("Enter acount index: ", 0 , 9);
            string receiverAddress = Input.String("Receiver address: ");
            ulong amount = Input.Decimal("Enter value to send: ").ToTokens();

            Console.WriteLine("Processing ... ");

            var account = Wallet.GetAccounts()[accountIndex];
            var transaction = account.Sign(receiverAddress, amount);
            ulong ballance = BlockChainClient.GetBalance(account.Address);

            var result = BlockChainClient.SendTransaction(transaction);

            if (result)
            {
                Output.WriteSuccess("Money send to blockchain");
            }
            else
            {
                Output.WriteError("Error sending the money");
            }
            Input.EnterKey();
        }

        private void ViewAccounts()
        {
            PrintWalletAccounts(Wallet, BlockChainClient);
            Input.EnterKey();
        }

        public void Run()
        {
            try
            {
                while (true)
                {
                    Console.Clear();
                    Output.WriteHeading($"Open wallet: {Wallet.Name}");
                    Console.WriteLine();
                    Console.WriteLine();
                    string command = Input.Command("Enter command", Commands.Select(c => c.Key).ToList());
                    //Input.String("Enter command: " + string.Join(',', Commands.Select(c => c.Key).ToList()) + ": ",
                    //x => { return Commands.ContainsKey(x.ToLower()); }
                    //);
                    //execute the command
                    Commands[command.ToLower()]();
                }
            }
            catch (ExitCommandException ex)
            {
                Console.WriteLine("Closign wallet");
                Input.EnterKey();
            }
        }
    }

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

    public static class Input
    {
        public static string Command(string label, IEnumerable<string> commands)
        {
            string result = "";
            bool validInput = false;
            while (!validInput)
            {
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
                    else if(pressedKey.Key == ConsoleKey.Backspace)
                    {
                        Console.Write("\b\0\b");
                        result = result.Substring(0, result.Length - 1);
                    }
                    else
                    {
                        result += pressedKey.KeyChar;
                        Console.Write(pressedKey.KeyChar);
                    }

                    pressedKey = Console.ReadKey(true);
                }

                if (commands.Any(c=> c.ToLower() == result.ToLower()))
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

            string input = String(message, x => { return uint.TryParse(x, out result) && result >= rangeFrom && result<= rangeTo; });

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

        public static void EnterKey(string message= "Press [Enter] key to continue")
        {
            Console.WriteLine(message);
            Console.ReadLine();
        }
    }


    public class ExitCommandException : Exception
    {

    }
}
