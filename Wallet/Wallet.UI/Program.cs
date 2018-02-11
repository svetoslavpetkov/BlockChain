using System;
using System.IO;

namespace Wallet.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            Wallet.Util.Wallet wallet = new Util.Wallet("12345_qwe", GetWalletPath("initial"));
            string address = wallet.GetAddress(1);
            string privatekey = wallet.GetPrivateKey(1);
        }


        private static string GetWalletPath(string walletName)
        {
            return Path.Combine(WalletPath, walletName + ".json");
        }

        static readonly string WalletPath = ".\\.\\Wallets";
    }
}
