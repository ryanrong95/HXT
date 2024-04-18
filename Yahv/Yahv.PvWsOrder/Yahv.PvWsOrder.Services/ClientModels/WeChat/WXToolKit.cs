using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.ClientModels
{
    public class WXToolKit
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"> </param>
        /// <param name="encryptedData"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string DecodeEncryptedData(string sessionKey, string encryptedData, string iv)
        {
            var aesKey = Convert.FromBase64String(sessionKey);
            var aesIV = Convert.FromBase64String(iv);

            var result = AES_Decrypt(encryptedData, aesIV, aesKey);
            var resultStr = Encoding.UTF8.GetString(result);
            return resultStr;
        }

        private static byte[] decode2(byte[] decrypted)
        {
            int pad = (int)decrypted[decrypted.Length - 1];
            if (pad < 1 || pad > 32)
            {
                pad = 0;
            }
            byte[] res = new byte[decrypted.Length - pad];
            Array.Copy(decrypted, 0, res, 0, decrypted.Length - pad);
            return res;
        }


        private static byte[] AES_Decrypt(String Input, byte[] Iv, byte[] Key)
        {
            //#if NET45
            RijndaelManaged aes = new RijndaelManaged();
            //#else
            //SymmetricAlgorithm aes = Aes.Create();
            //#endif
            aes.KeySize = 128;//原始：256
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = Key;
            aes.IV = Iv;
            var decrypt = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] xBuff = null;

            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                    {

                        byte[] xXml = Convert.FromBase64String(Input);
                        byte[] msg = new byte[xXml.Length + 32 - xXml.Length % 32];
                        Array.Copy(xXml, msg, xXml.Length);
                        cs.Write(xXml, 0, xXml.Length);
                    }

                    xBuff = decode2(ms.ToArray());
                }
            }
            catch (System.Security.Cryptography.CryptographicException)
            {

                using (var ms = new MemoryStream())
                {
                    var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write);
                    {
                        byte[] xXml = Convert.FromBase64String(Input);
                        byte[] msg = new byte[xXml.Length + 32 - xXml.Length % 32];
                        Array.Copy(xXml, msg, xXml.Length);
                        cs.Write(xXml, 0, xXml.Length);
                    }
                    //cs.Dispose();
                    xBuff = decode2(ms.ToArray());
                }
            }
            return xBuff;
        }

    }
}
