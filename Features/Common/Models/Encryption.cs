using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class Encryption {
        public static string KeyString;
        public static byte[] iv;
        public static byte[] Key {
            get { return System.Convert.FromBase64String(KeyString); }
        }


        public static string EncryptData(string text) {
            var buffer = Encoding.UTF8.GetBytes(text);
            return Encryption.EncryptData(buffer);
        }
        public static string EncryptData(byte[] dataToEncrypt) {
            if (Key == null || Key.Length == 0) {
                throw new Exception("No Key was specified");
            }

            using (var algorithm = Aes.Create()) {
                algorithm.GenerateIV();
                algorithm.Key = Key;
                ICryptoTransform encryptor = algorithm.CreateEncryptor();
                byte[] encryptedBytes = encryptor.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
                byte[] bytes = algorithm.IV.Concat(encryptedBytes).ToArray();
                return System.Convert.ToBase64String(bytes);
            }
        }
        public static byte[] DecryptData(string dataToDecrypt) {
            byte[] dataToDecryptBytes = System.Convert.FromBase64String(dataToDecrypt);

            using (var algorithm = Aes.Create()) {
                var iv = dataToDecryptBytes.Take(16).ToArray();
                var dataBytes = dataToDecryptBytes.Skip(16).ToArray();

                algorithm.IV = iv;
                algorithm.Key = Key;
                ICryptoTransform decryptor = algorithm.CreateDecryptor();
                byte[] outBlock = decryptor.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
                return outBlock;
            }
        }
        public static string DecryptString(string dataToDecrypt) {
            if(dataToDecrypt == null)
                return "";
            return Encoding.UTF8.GetString(Encryption.DecryptData(dataToDecrypt));
        }
    }
}