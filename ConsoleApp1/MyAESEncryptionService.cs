using System;
using System.Security.Cryptography;

namespace ConsoleApp1
{
    public class MyAESEncryptionService
    {
        private byte[] _key;
        private byte[] _iv;
        private AesCryptoServiceProvider _aes;

        public MyAESEncryptionService(string key, string iv)
        {
            _key = Convert.FromBase64String(key);
            _iv = Convert.FromBase64String(iv);
            _aes = new AesCryptoServiceProvider();
            _aes.Key = _key;
            _aes.IV = _iv;
        }

        public MyAESEncryptionService(byte[] key, byte[] iv)
        {
            _key = key;
            _iv = iv;
            _aes = new AesCryptoServiceProvider();
            _aes.Key = _key;
            _aes.IV = _iv;
        }

        public string DecryptMessage(string encryptedMessage)
        {
            //TODO Implement this
            return "";
        }
    }
}