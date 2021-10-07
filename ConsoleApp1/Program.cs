using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using CoMeta.Helpers;

namespace ConsoleApp1
{
    class Program
    {
        private static AuthenticationHelper helper;

        public Program()
        {
        }

        static void Main(string[] args)
        {
            WorkingBackwards();
            //FunWithAESandKeys();
            //PlayingWithMyRSAEncryptionService();
            //playingWithMyAESService();
            //HashPasswordExample("P@$$WORD");
            //funWithTokens();
            //funWithEncryption("Hi Bob, it's Alice (h)");
            //FunWithAsyncEncryption();
        }

        private static void FunWithAESandKeys()
        {
            PasswordToKeyService keyService = new PasswordToKeyService();

            String password = "Password123"; //This is User generated Input (Should never be stored)!
            //The salt is created at User Registration and is stored on the server:
            byte[] salt = CreateRandomSalt(5);
            byte[] key = keyService.GetKey(password, out salt, 256);

            Console.WriteLine("Key: " + Convert.ToBase64String(key));
            Console.WriteLine("Salt: " + Convert.ToBase64String(salt));

            byte[] iv;
            MyAESEncryptionService aesEncryptionService = new MyAESEncryptionService(key, out iv);
            Console.WriteLine("IV: " + Convert.ToBase64String(iv));
            
            String secretText = "This have to work!";
            byte[] result = aesEncryptionService.EncryptMessage(secretText);
            
            Console.WriteLine("Encrypted text: " + Convert.ToBase64String(result));

            String decrypted = aesEncryptionService.DecryptMessage(result);
            Console.WriteLine("Original: " + decrypted);
            
        }

        public static void WorkingBackwards()
        {
            string password = "Password123";
            string salt = "8Eye+X/wPcdTUOhOr2hCbKfPeLnVGAJNzNZKMoLYLD2pGIC4BfADlNMkh0B41JqQG3LTYxIgzfLwGYOjN1+Wkg7Bpm4uwx2eBw7aFUQ8vpx1rQp40FTLwsCTbq2XOmGhgcTl+5lVhkHQrweEYsrzfn5lVY7bOEQcjc480iq1a+BLUn5Mwji2XSwql7FlAxGWzkGXVl9Gq0eREIcSOggzZAe5dK09nUmfAZJ5Wq5h9jOItk3v7l5YE0jEDZByn4flzF0Ub8cYRpso3dG2UeT+/zfBddhkYv2Fx/ZTohAWP1i7jGDYFrctjpAJBHxfd6y8azdrfJ+kHvZ7eLSU1LHz0Q==";
            //Above will generate the key ^
            byte[] key = new PasswordToKeyService().GetKey(password, Convert.FromBase64String(salt), 256);
            
            string iv = "wfLfqUKDlZGQ90Rhl7zv/w==";
            MyAESEncryptionService encryptionService = new MyAESEncryptionService(key, Convert.FromBase64String(iv));
            
            string encryptedText = "wGMoD04DsIFA9fWzRmIDihNeEx4qy6UacEG/+KCcub0=";
            byte[] byteEncryptedText = Convert.FromBase64String(encryptedText);
            string clearText = encryptionService.DecryptMessage(byteEncryptedText);
            Console.WriteLine("The message is: " + clearText);
        }
        
        private static void PlayingWithMyRSAEncryptionService()
        {
            MyRSAEncryptionService encryptionServiceAlice = new MyRSAEncryptionService();

            string alicesPublicKey = encryptionServiceAlice.GetPublicRsaParameters();
            Console.WriteLine("Alice public key: " + alicesPublicKey);

            string alicesPublicAndPrivateKey = encryptionServiceAlice.GetPublicAndPrivateRsaParameters();
            Console.WriteLine("Alices public and private key: " + alicesPublicAndPrivateKey);

            MyRSAEncryptionService encryptionServiceBob = new MyRSAEncryptionService(alicesPublicKey);

            string bobsMessageToAlice = "Hi Alice. I think you are really nice";

            //I encrypt the message from Bob with BOB's service using ALICE's PUBLIC KEY:
            byte[] secret = encryptionServiceBob.encryptMessage(bobsMessageToAlice);

            //I decrypt the message from Bob with ALICE's service using ALICE's PRIVATE KEY:
            string alicesMessageFromBob = encryptionServiceAlice.decryptMessage(secret);

            //I print out the decrypted text:
            Console.WriteLine(alicesMessageFromBob);
        }

        private static void playingWithMyAESService()
        {
            //Create the encryption service using the key and IV:
            //Here I am using the key and IV taken from FaceBook:
            string aKey = "glZXcwfK2eYmfb8drr1ObHn5hXUvl2kXBrOmbvxf8Ow=";
            string aIv = "tx8FgtXX8jCYKQDxBICUlw==";
            MyAESEncryptionService encryptionService = new MyAESEncryptionService(aKey, aIv);

            //Create a text to encrypt:
            String message = "This is a new secret text";
            //Encrypt the secret:
            byte[] secret = encryptionService.EncryptMessage(message);
            //Base64 encode for transfer (URL safe):
            string base64secret = Convert.ToBase64String(secret);

            Console.WriteLine(base64secret);

            //Receiving a secret:
            string aSecret = "8XBmpevEDM5fTwvk+zoi+g==";
            //Base64 decode:
            byte[] byteSecret = Convert.FromBase64String(aSecret);
            //Decrypt:
            string messageTwo = encryptionService.DecryptMessage(byteSecret);
            //Print result:
            Console.WriteLine(messageTwo);
        }

        private static void funWithTokens()
        {
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

            byte[] privKey = rsa.ExportRSAPrivateKey();
            byte[] pubKey = rsa.ExportRSAPublicKey();

            Console.WriteLine("Priv key: " + BitConverter.ToString(privKey));
            Console.WriteLine("Pub  key: " + BitConverter.ToString(pubKey));

            Console.WriteLine("Hit enter to continue:");
            Console.ReadLine();
            Console.WriteLine("Fun with RSA encryption:");
            string text = "This is a secret";
            byte[] byteText = Encoding.UTF8.GetBytes(text);

            byte[] enccryptedBytes = rsa.Encrypt(byteText, RSAEncryptionPadding.Pkcs1);

            string enc = BitConverter.ToString(enccryptedBytes);
            Console.WriteLine("Encrypted data:");
            Console.WriteLine(enc);

            cspp = new CspParameters();
            cspp.KeyContainerName = keyName;
            rsa = new RSACryptoServiceProvider(cspp);

            byte[] clearBytes = rsa.Decrypt(enccryptedBytes, RSAEncryptionPadding.Pkcs1);
            string clear = Encoding.UTF8.GetString(clearBytes);
            Console.WriteLine("Decrypted data (With key container):");
            Console.WriteLine(clear);

            rsa = new RSACryptoServiceProvider();
            int bytesRead;
            rsa.ImportRSAPublicKey(pubKey, out bytesRead);
            int bytesRead2;
            rsa.ImportRSAPrivateKey(privKey, out bytesRead2);
            byte[] cleBytes2 = rsa.Decrypt(enccryptedBytes, RSAEncryptionPadding.Pkcs1);
            string clear2 = Encoding.UTF8.GetString(cleBytes2);
            Console.WriteLine("Decrypted data (With byte[] keys):");
            Console.WriteLine(clear2);

            Directory.CreateDirectory(EncrFolder);
            StreamWriter sw = new StreamWriter(PubKeyFile, false);
            sw.Write(rsa.ToXmlString(false));
            sw.Close();
        }

        public static void funWithEncryption(string plainText)
        {
            //These values cn be taken from the register USER routine:
            byte[] pwd = Encoding.Unicode.GetBytes("p@$$w0rd");
            byte[] salt = CreateRandomSalt(512 / 8);

            // Create a TripleDESCryptoServiceProvider object.
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //AesCryptoServiceProvider aes = new AesCryptoServiceProvider();

            try
            {
                Console.WriteLine("Creating a key with PasswordDeriveBytes...");

                // Create a PasswordDeriveBytes object and then create
                // a TripleDES key from the password and salt.
                PasswordDeriveBytes pdb = new PasswordDeriveBytes(pwd, salt);
                // Create the key and set it to the Key property
                // of the TripleDESCryptoServiceProvider object.
                tdes.Key = pdb.CryptDeriveKey("TripleDES", "SHA512", 0, tdes.IV);

                Console.WriteLine("Operation complete.");
                byte[] encrypted;

                ICryptoTransform encryptor = tdes.CreateEncryptor();
                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }

                        encrypted = msEncrypt.ToArray();
                    }
                }


                string encryptedText = Convert.ToBase64String(encrypted);
                Console.WriteLine("Encrypted: " + encryptedText);

                string clearText;
                //Decryption
                ICryptoTransform decrypter = tdes.CreateDecryptor();
                using (MemoryStream msDecrypt = new MemoryStream(encrypted)) //Remember that encrypted is a byte[]
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decrypter, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            clearText = srDecrypt.ReadToEnd();
                        }
                    }
                }

                Console.WriteLine("Decrypted: " + clearText);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // Clear the buffers
                ClearBytes(pwd);
                ClearBytes(salt);

                // Clear the key.
                tdes.Clear();
                //aes.Clear();
            }

            Console.ReadLine();
        }

        /// <summary>
        /// Generates a random salt value of the specified length.
        /// </summary>
        public static byte[] CreateRandomSalt(int length)
        {
            // Create a buffer
            byte[] randBytes;

            if (length >= 1)
            {
                randBytes = new byte[length];
            }
            else
            {
                randBytes = new byte[1];
            }

            // Create a new RNGCryptoServiceProvider.
            RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();

            // Fill the buffer with random bytes.
            rand.GetBytes(randBytes);

            // return the bytes.
            return randBytes;
        }

        /// <summary>
        /// Clear the bytes in a buffer so they can't later be read from memory.
        /// </summary>
        public static void ClearBytes(byte[] buffer)
        {
            // Check arguments.
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            // Set each byte in the buffer to 0.
            for (int x = 0; x < buffer.Length; x++)
            {
                buffer[x] = 0;
            }
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