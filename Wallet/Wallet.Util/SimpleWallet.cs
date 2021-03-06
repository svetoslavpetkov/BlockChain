﻿using BlockChain.Core;
using System;
using System.Linq;

namespace Wallet.Util
{
    public class SimpleWallet
    {
        private ITransactionSigner Signer = new TransactionSigner();
        private ICryptoUtil CryptoUtil = new CryptoUtil();

        public string PrivateKey { get; set; }


        private string _address = null;
        public string Address
        {
            get
            {
                if (_address == null)
                {
                    _address = GetAddress();
                }
                return _address;
            }
        }

        public SimpleWallet(byte[] privateKey)
        {
            PrivateKey = CryptoUtil.GetHexString(privateKey);
        }

        public SimpleWallet(string privateKey)
        {
            PrivateKey = privateKey;
        }

        public Transaction Sign(string recipientAddress, ulong value)
        {
            DateTime signDate = DateTime.Now;
            Transaction signedTransaction = Signer.Sign(PrivateKey, recipientAddress, value, signDate);

            return signedTransaction;
        }

        public static SimpleWallet GenerateNewWallet()
        {
            string privateKeyString = new CryptoUtil().GetRandomPrivateKey(); // must be 16?

            return new SimpleWallet(privateKeyString);
        }

        public static SimpleWallet GenerateNewWallet(string menmonic,string password)
        {
            string privateKey = new CryptoUtil().RecoverPrivateKey(menmonic + password);

            return new SimpleWallet(privateKey);
        }

        public string GetAddress()
        {
            string address = Signer.CalculateAddress(PrivateKey);
            return address;
        }
       
        public string GetPublicKey()
        {
            return CryptoUtil.GetPublicKeyCompressed(PrivateKey);
        }
    }
}
