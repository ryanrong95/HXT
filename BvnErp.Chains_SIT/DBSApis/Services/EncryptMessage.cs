using DBSApis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBSApis.Services
{
    public class EncryptMessage
    {
        public KeyConfig Config { get; set; }
        public string Message { get; set; }

        public EncryptMessage(KeyConfig keyConfig,string message)
        {
            this.Config = keyConfig;
            this.Message = message;
        }

        public string Encrypt()
        {
            try
            {
                Encrypt encrypt = new Encrypt();
                encrypt.IsArmored = true;
                encrypt.IsSigning = true;
                encrypt.CheckIntegrity = true;
                encrypt.PublicKeyFilePath = Config.PublicKey;
                encrypt.PrivateKeyFilePath = Config.PrivateKey;
                encrypt.PrivateKeyPassword = Config.PrivayeKeyPwd;

                BCPGPEncryptor objPgpEncrypt = new BCPGPEncryptor(encrypt);
                string encryptedMessage = objPgpEncrypt.EncryptMessage(Message);

                return encryptedMessage;
            }
            catch(Exception ex)
            {
                //return ConstConfig.ConstError.Error001;
                throw new Exception(ex.Message);
            }           
        }
    }
}