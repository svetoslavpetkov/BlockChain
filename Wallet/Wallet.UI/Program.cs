using BlockChain.Core;
using System;
using System.Collections.Generic;
using System.IO;
using Wallet.Util;

namespace Wallet.UI
{
    class Program
    {

        private static Dictionary<string, Action> Commands = new Dictionary<string, Action>(new List<KeyValuePair<string, Action>>() {
            new KeyValuePair<string, Action>("create",CreateWallet),
            new KeyValuePair<string, Action>("exit",ExitApplication)
        });

        private class ExitCommandException : Exception
        {

        }

        private static void ExitApplication()
        {
            throw new ExitCommandException();
        }

        private static void CreateWallet()
        {
            var wallet = Wallet.Util.SimpleWallet.GenerateNewWallet();

            Console.Clear();
            Console.WriteLine($"New wallet generated. Keep your private key !");
            Console.WriteLine($"PrivateKey: {wallet.PrivateKey}");
            Console.WriteLine($"Address: {wallet.GetAddress()}");
            Console.WriteLine($"Address: {wallet.GetPublicKey()}");

            Console.WriteLine();
            Console.WriteLine("Press any key");
            Console.ReadLine();
        }

        static void Main(string[] args)
        {

            //var shaTheirs = new TransactionSigner().Sha("marion ladder needle peace rodent stock tribune whiskey ski job wave unit");
            //var shaOurs = new TransactionSigner().CalcSHA256("marion ladder needle peace rodent stock tribune whiskey ski job wave unit");


            SimpleWallet walet = new SimpleWallet("E9873D79C6D87DC0FB6A5778633389F4453213303DA61F20BD67FC233AA33262");

            //027e755df9dbf072945d376c3f2a5a0dd4fe08a6773975c43734027695f96b9171
            //E9873D79C6D87DC0FB6A5778633389F4453213303DA61F20BD67FC233AA33262
            // our generated privete key 107725345683326611836297844809410268066979832523672886309936076812926793365280
            //"0435839402B35555B48000000295B3226E8D4FC878C1BDE087A65341F558294B0F9D1B1D59EB88B31F510F5CCE00677FC1457CA004949520FB171539144044722815B377FAFF747778CD805EFAE329890C65""
            //"tprv8emgGE8KBmFaqpmi9Rmh3fkwUBQhmMJwoofsNiWCvUW94DY36QwhGNEFe5qjzfdmHyAkqWACGnXDjM2JkGHVDoCAfQ2Z7uXZdNFJBQEK6ax"

            Console.WriteLine($"My address:{walet.GetAddress()}");
            var tran = walet.Sign("asadagfasdfad", 12);

            ITransactionValidator nodeSimulator = new TransactionValidator();
            bool isValid =  nodeSimulator.IsValid(tran);

            string path = "C:\\Users\\svetoslav.petkov\\Source\\Repos\\BlockChain\\Wallet\\Wallet.UI\\Wallet.json";
            string pass = "12345_qwe";

            //FullWallet.GenerateNewWallet(path, pass);

            var fullWallet = new FullWallet("name",path, pass);
            var accounts = fullWallet.GetAccounts();

            tran = accounts[0].Sign("asfasdfasdfasdfdasasdffasdsd", 5000000);
            isValid = nodeSimulator.IsValid(tran);

            tran = accounts[1].Sign("asfasdfasdfasdasfasddfdasasdffasdsd", 1000);
            isValid = nodeSimulator.IsValid(tran);

            tran = accounts[2].Sign("asfasdfasdfasdasfasddfdasasdffasdsd", 1000);
            isValid = nodeSimulator.IsValid(tran);
        }


        private static string ReadStringFromConsole(string label, Func<bool> validation = null)
        {
            string result = string.Empty;

            while (result == string.Empty)
            {
                Console.Write(label);
                result = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(result) || 
                    (validation != null)
                    )
                {

                }
            }


            return result;
        }



        private static string GetWalletPath(string walletName)
        {
            return Path.Combine(WalletPath, walletName + ".json");
        }

        static readonly string WalletPath = ".\\.\\Wallets";
    }
}
