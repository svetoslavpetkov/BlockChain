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
            new KeyValuePair<string, Action>("open",OpenWallet),
            new KeyValuePair<string, Action>("recover",RecoverWallet),
            new KeyValuePair<string, Action>("list-wallets",GetWallets)

        });

        private static void RecoverWallet()
        {
            FullWallet wallet = null;
            try
            {
                string mnemonic = Input.String("Enter mnemonic: ");
                string password = Input.Password();
                string walletName = Input.String("Enter new wallet name");

                wallet = FullWallet.Recover(mnemonic, password, GetWalletPath(walletName), walletName);

                Output.WriteSuccess("Wallet recovered");
                Input.EnterKey();
            }
            catch (Exception ex)
            {
                Output.WriteError(ex.Message);
                Input.EnterKey();
            }

            if (wallet != null)
            {
                WalletHandler walletHandler = new WalletHandler(wallet, blockChainClient);
            }
        }

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
            if (args.Length > 0)
            {
                blockChainClient.NodeUrl = args[0];
            }

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

        
        public static void GetWallets()
        {
            DirectoryInfo di = new DirectoryInfo(WalletPath);
            var walletFiles = di.GetFiles("*.json");

            if (walletFiles.Length == 0)
            {
                Output.WriteError("No wallets found");
                Input.EnterKey();
                return;
            }

            string headerName = "Wallet name";
            string headerDateCreated = "Date created";
            string format = "{0,-20}   {1,-15}";
            Console.WriteLine(format, headerName, headerDateCreated);
            Console.WriteLine("-------------------------------------------------------------------");
            foreach (var file in walletFiles.OrderBy(f=> f.Name))
            {
                Console.WriteLine(format, file.Name, file.CreationTime.ToString());
            }

            Console.WriteLine();
            Input.EnterKey();
        }

        private static string GetWalletPath(string walletName)
        {
            return Path.Combine(WalletPath, walletName + ".json");
        }

        static readonly string WalletPath = ".\\.\\Wallets";
    }


    

    

   


    public class ExitCommandException : Exception
    {

    }
}
