using System;
using System.IO;
using Wallet.Util.Core;

namespace Wallet.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            Util.Core.SimpleWalet walet = new SimpleWalet("107725345683326611836297844809410268066979832523672886309936076812926793365280");

            var tran = walet.Sign("asadagfasdfad", 10, DateTime.Now);

            bool isValid = walet.IsValid(tran);
        }


        private static string GetWalletPath(string walletName)
        {
            return Path.Combine(WalletPath, walletName + ".json");
        }

        static readonly string WalletPath = ".\\.\\Wallets";
    }
}
