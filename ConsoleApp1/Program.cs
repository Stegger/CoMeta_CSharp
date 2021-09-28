using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using CoMeta.Helpers;

namespace ConsoleApp1
{
    class Program
    {
        private static AuthenticationHelper helper;
        
        static void Main(string[] args)
        {
            //HashPasswordExample("P@$$WORD");
            
            // Create a byte array with random values. This byte array is used
            // to generate a key for signing JWT tokens.
            Byte[] secretBytes = new byte[40];
            Random rand = new Random();
            rand.NextBytes(secretBytes);
            helper = new AuthenticationHelper(secretBytes);

            string password = "Pa$$w0rd";
            byte[] passwordHash;
            byte[] salt;
            helper.CreatePasswordHash(password, out passwordHash, out salt);

            string pwHash = BitConverter.ToString(passwordHash);
            string pwSalt = BitConverter.ToString(salt);
            
            Console.WriteLine("Hash: " + pwHash);
            Console.WriteLine("Salt: " + pwSalt);
            
            if (helper.VerifyPasswordHash(password, passwordHash, salt))
            {
                Console.WriteLine("We have a match");
            }
            else
            {
                Console.WriteLine("It doesn't match");
            }
            
        }

        public static void FunWithAsyncEncryption()
        {
            // Declare CspParmeters and RsaCryptoServiceProvider
            // objects with global scope of your Form class.
            CspParameters cspp = new CspParameters();
            RSACryptoServiceProvider rsa;

            // Path variables for source, encryption, and
            // decryption folders. Must end with a backslash.
            const string EncrFolder = @"c:\CoMeta\Encrypt\";
            const string DecrFolder = @"c:\CoMeta\Decrypt\";
            const string SrcFolder = @"c:\CoMeta\docs\";

            // Public key file
            const string PubKeyFile = @"c:\CoMeta\encrypt\rsaPublicKey.txt";

            // Key container name for
            // private/public key value pair.
            const string keyName = "Key02";
            // Stores a key pair in the key container.
            cspp.KeyContainerName = keyName;
            rsa = new RSACryptoServiceProvider(cspp);
            rsa.PersistKeyInCsp = true;
            string strKey = "";
            if (rsa.PublicOnly == true)
                strKey = "Key: " + cspp.KeyContainerName + " - Public Only";
            else
                strKey = "Key: " + cspp.KeyContainerName + " - Full Key Pair";

            Console.Out.WriteLine(strKey);
            
            Directory.CreateDirectory(EncrFolder);
            StreamWriter sw = new StreamWriter(PubKeyFile, false);
            sw.Write(rsa.ToXmlString(false));
            sw.Close();
        }
        
        public static void HashPasswordExample(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                // Send a sample text to hash.  
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                // Get the hashed string.  
                var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                // Print the string.   
                Console.WriteLine(hash);
            }
        }
        
        
    }
}