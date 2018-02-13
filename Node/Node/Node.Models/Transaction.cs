﻿using Node.Domain.Exceptions;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Node.Domain
{
    public static class Crypto
    {
        public static String Sha256(String value)
        {
            StringBuilder hashResult = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Byte[] result = hash.ComputeHash(Encoding.UTF8.GetBytes(value));

                foreach (Byte b in result)
                    hashResult.Append(b.ToString("x2"));
            }

            return hashResult.ToString();
        }
    }

    public class Transaction
    {
        public string From { get; private set; }
        public string To { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime DateCreated { get; private set; }
        public string SenderPublicKey { get; private set; }
        public string[] Signature { get; private set; }
        public string Hash { get; private set; }

        public int MinedInIndex { get; private set; }
        public bool TranserSuccessful { get; private set; }

        public Transaction(string from, string to, decimal amount, DateTime creation, string senderPyblicKey,
            string[] signature, string hash)
        {
            From = from;
            To = to;
            Amount = amount;
            DateCreated = creation;
            SenderPublicKey = senderPyblicKey;
            Signature = signature;
            Hash = hash;
        }

        public Transaction(string from, string to, decimal amount, DateTime creation, string senderPyblicKey,
            string[] signature) : this(from, to,amount,creation,senderPyblicKey,signature, null)
        {
            Hash = CalculateHash();
        }

        public void Validate()
        {
            string hash = CalculateHash();

            if (hash != Hash)
                throw new TransactionNotValidException("Transaction not Valid! Tranascion is chnaged by middle man");
        }

        public bool VerifySenderSignature()
        {
            if (string.IsNullOrWhiteSpace(SenderPublicKey))
                throw new TransactionNotValidException("Sender signature is not valid");

            return true;
        }

        private string CalculateHash()
        {
            TransactionHash hashObject = new TransactionHash(From, To, Amount, DateCreated, SenderPublicKey, Signature);
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(hashObject);
            string hash = Crypto.Sha256(json);

            return hash;

        }
    }
}
