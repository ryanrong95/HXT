using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBSApis.Services
{
    public class Decrypt
    {
        private bool isVerify;
        private string publicKeyFilePath;
        private string privateKeyFilePath;
        private string privateKeyPassword;
        public bool IsVerify
        {
            get { return isVerify; }
            set { isVerify = value; }
        }
        public string PublicKeyFilePath
        {
            get { return publicKeyFilePath; }
            set { publicKeyFilePath = value; }
        }
        public string PrivateKeyFilePath
        {
            get { return privateKeyFilePath; }
            set { privateKeyFilePath = value; }
        }
        public string PrivateKeyPassword
        {
            get { return privateKeyPassword; }
            set { privateKeyPassword = value; }
        }
    }
}