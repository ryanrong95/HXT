using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBSApis.Services
{
    public class Encrypt
    {
        private bool isArmored;
        private bool isSigning;
        private bool checkIntegrity;

        private string publicKeyFilePath;
        private string privateKeyFilePath;
        private string privateKeyPassword;

        private PgpPublicKey publicKey;
        private PgpPrivateKey privateKey;
        private PgpSecretKey secretKey;

        public bool IsArmored
        {
            get { return isArmored; }
            set { isArmored = value; }
        }
        public bool IsSigning
        {
            get { return isSigning; }
            set { isSigning = value; }
        }
        public bool CheckIntegrity
        {
            get { return checkIntegrity; }
            set { checkIntegrity = value; }
        }
        public string PublicKeyFilePath
        {
            get { return publicKeyFilePath; }
            set { publicKeyFilePath = value; }
        }
        public string PrivateKeyFilePath
        {
            get { return privateKeyFilePath; }
            set
            {
                privateKeyFilePath = value;
                publicKey = BCPGPUtils.ReadPublicKey(publicKeyFilePath);
            }
        }
        public string PrivateKeyPassword
        {
            get { return privateKeyPassword; }
            set { privateKeyPassword = value; }
        }
        public PgpPublicKey PublicKey
        {
            get { return publicKey; }
            set { publicKey = value; }
        }
        public PgpPrivateKey PrivateKey
        {
            get { return privateKey; }
            set { privateKey = value; }
        }
        public PgpSecretKey SecretKey
        {
            get { return secretKey; }
            set { secretKey = value; }
        }
    }
}