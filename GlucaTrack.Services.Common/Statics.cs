using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Resources;
using System.Security.Cryptography;
using System.Xml.Serialization;
using System.Diagnostics;
using GlucaTrack.Communication;

namespace GlucaTrack.Services.Common
{
    public class Statics
    {
        public static string baseFilepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GlucaTrack");
        public static DeviceInfo deviceFound = null;

        public static string serviceName = "GlucaTrack Detector";

        public static void SaveSettingsFile(Settings settings)
        {
            Directory.CreateDirectory(Statics.baseFilepath);

            //write saved file
            var xmlSerial = new XmlSerializer(typeof(Settings));
            using (var sw = new StringWriter())
            {
                xmlSerial.Serialize(sw, settings);

                using (StreamWriter stream = new StreamWriter(Path.Combine(Statics.baseFilepath, "glucatrack.sav"), false, Encoding.UTF8))
                {
                    stream.Write(Convert.ToBase64String(Encoding.UTF8.GetBytes(sw.ToString())));
                }
            }
        }
        public static Settings ReadSettingsFile()
        {
            return ReadSettingsFile(Path.Combine(Common.Statics.baseFilepath, "glucatrack.sav"));
        }
        public static Settings ReadSettingsFile(string path)
        {
            //read saved settings file
            if (File.Exists(path))
            {
                //file exists
                using (TextReader reader = new StreamReader(path, Encoding.UTF8, true))
                {
                    var xmlSerial = new XmlSerializer(typeof(Settings));
                    using (var sr = new StringReader(Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()))))
                    {
                        return (Settings)xmlSerial.Deserialize(sr);
                    }
                }
            }
            else
            {
                return new Settings();
            }
        }
    }

    public static class StringCipher
    {
        // This constant string is used as a "salt" value for the PasswordDeriveBytes function calls.
        // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
        // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        private const string initVector = "tx81geji350v89u7";

        // This constant is used to determine the keysize of the encryption algorithm.
        private const int keysize = 256;

        /// <summary>
        /// Encrypt a string using AES 256bit encryption.
        /// </summary>
        /// <param name="plainText">The original string.</param>
        /// <returns>The encrypted string.</returns>
        public static string Encrypt(string plainText, bool useDefaultKey = false)
        {
            if (String.IsNullOrEmpty(plainText))
            {
                throw new ArgumentNullException("The string which needs to be encrypted can not be null.");
            }

            string key = (useDefaultKey) ? "E441699041D7434797DBBF1493A5C15A" : Properties.Settings.Default["Encrypt_Key" + DateTime.Now.Day.ToString()].ToString();
            
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(key, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Padding = PaddingMode.PKCS7;
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    byte[] cipherTextBytes = memoryStream.ToArray();
                    return Convert.ToBase64String(cipherTextBytes);
                }
            }
        }

        /// <summary>
        /// Decrypt a string using AES 256bit encryption.
        /// </summary>
        /// <param name="cipherText">The encrypted string.</param>
        /// <returns>The original string.</returns>
        public static string Decrypt(string cipherText, bool useDefaultKey = false)
        {
            if (String.IsNullOrEmpty(cipherText))
            {
                throw new ArgumentNullException
                   ("The string which needs to be decrypted can not be null.");
            }

            string key = (useDefaultKey) ? "E441699041D7434797DBBF1493A5C15A" : Properties.Settings.Default["Encrypt_Key" + DateTime.Now.Day.ToString()].ToString();

            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(key, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Padding = PaddingMode.PKCS7;
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                    int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                    return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                }
            }
        }

        /// <summary>
        /// Encrypt a string.
        /// </summary>
        /// <param name="originalString">The original string.</param>
        /// <returns>The encrypted string.</returns>
        /// <exception cref="ArgumentNullException">This exception will be 
        /// thrown when the original string is null or empty.</exception>
        [Obsolete("This method is obsolete; use Encrypt instead")]
        public static string DES_Encrypt(string originalString)
        {
            if (String.IsNullOrEmpty(originalString))
            {
                throw new ArgumentNullException ("The string which needs to be encrypted can not be null.");
            }
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                cryptoProvider.CreateEncryptor(ASCIIEncoding.ASCII.GetBytes("ZeroCool"), ASCIIEncoding.ASCII.GetBytes("ZeroCool")), CryptoStreamMode.Write);
            StreamWriter writer = new StreamWriter(cryptoStream);
            writer.Write(originalString);
            writer.Flush();
            cryptoStream.FlushFinalBlock();
            writer.Flush();
            return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
        }

        /// <summary>
        /// Decrypt a crypted string.
        /// </summary>
        /// <param name="cryptedString">The crypted string.</param>
        /// <returns>The decrypted string.</returns>
        /// <exception cref="ArgumentNullException">This exception will be thrown 
        /// when the crypted string is null or empty.</exception>
        [Obsolete("This method is obsolete; use Decrypt instead")]
        public static string DES_Decrypt(string cryptedString)
        {
            if (String.IsNullOrEmpty(cryptedString))
            {
                throw new ArgumentNullException
                   ("The string which needs to be decrypted can not be null.");
            }
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream
                    (Convert.FromBase64String(cryptedString));
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                cryptoProvider.CreateDecryptor(ASCIIEncoding.ASCII.GetBytes("ZeroCool"), ASCIIEncoding.ASCII.GetBytes("ZeroCool")), CryptoStreamMode.Read);
            StreamReader reader = new StreamReader(cryptoStream);
            return reader.ReadToEnd();
        }
    }

    public class Errors
    {
        public static void Error(Exception ex)
        {
            //error in windows application
        }

        public static void ServiceError(Exception ex)
        {
            //error in background service
            EventLog el = new EventLog("Application");
            el.Source = Statics.serviceName;
            string sep = "======================================";
            el.WriteEntry(ex.Message + Environment.NewLine + sep + Environment.NewLine + (ex.InnerException != null ? ex.InnerException.Message + Environment.NewLine : string.Empty) + sep + Environment.NewLine + ex.StackTrace, EventLogEntryType.Error);
        }
    }
}
