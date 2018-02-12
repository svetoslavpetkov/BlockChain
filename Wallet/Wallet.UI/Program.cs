using System;
using System.IO;
using Wallet.Util.Core;

namespace Wallet.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            Util.Core.SimpleWalet walet = new SimpleWalet("E9873D79C6D87DC0FB6A5778633389F4453213303DA61F20BD67FC233AA33262");

            //E9873D79C6D87DC0FB6A5778633389_SAMPLE_PRIVATE_KEY_DO_NOT_IMPORT_F4453213303DA61F20BD67FC233AA33262
            // our generated privete key 107725345683326611836297844809410268066979832523672886309936076812926793365280
            //"0435839402B35555B48000000295B3226E8D4FC878C1BDE087A65341F558294B0F9D1B1D59EB88B31F510F5CCE00677FC1457CA004949520FB171539144044722815B377FAFF747778CD805EFAE329890C65""
            //"tprv8emgGE8KBmFaqpmi9Rmh3fkwUBQhmMJwoofsNiWCvUW94DY36QwhGNEFe5qjzfdmHyAkqWACGnXDjM2JkGHVDoCAfQ2Z7uXZdNFJBQEK6ax"

            Console.WriteLine($"My address:{walet.GetAddress()}");
            var tran = walet.Sign("asadagfasdfad", 12);

            ITransactionValidator nodeSimulator = new TransactionValidator();
            bool isValid =  nodeSimulator.IsValid(tran);
        }


        private static string GetWalletPath(string walletName)
        {
            return Path.Combine(WalletPath, walletName + ".json");
        }

        static readonly string WalletPath = ".\\.\\Wallets";
    }
}
