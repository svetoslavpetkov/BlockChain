using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wallet.Util;
using BlockChain.Core;

namespace Wallet.UI
{
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

        public Dictionary<string, Action> Commands { get; set; } = new Dictionary<string, Action>() { };

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
            int accountIndex = Input.Int("Enter acount index: ", 0, 9);
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
}
