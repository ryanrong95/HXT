using DBSApis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBSApis.Services
{
    public class DecryptMessage
    {
        public KeyConfig Config { get; set; }
        public string Message { get; set; }

        public DecryptMessage(KeyConfig keyConfig, string message)
        {
            this.Config = keyConfig;
            this.Message = message;
        }

        public string Decrypt()
        {
            try
            {
                Decrypt decrypt = new Decrypt();
                decrypt.IsVerify = true;
                decrypt.PublicKeyFilePath = Config.PublicKey;
                decrypt.PrivateKeyFilePath = Config.PrivateKey;
                decrypt.PrivateKeyPassword = Config.PrivayeKeyPwd;

                BCPGPDecryptor objPgpDecrypt = new BCPGPDecryptor(decrypt);
                string decryptedMessage = objPgpDecrypt.DecryptMessage(Message);

                return decryptedMessage;
            }
            catch
            {
                return Needs.Ccs.Services.Models.DBSConstConfig.DBSConstError.Error002;
            }
        }
    }
}