using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Web;

namespace DBSApis.Services
{
    public class BCPGPUtils
    {

        #region Public Key
        public static PgpPublicKey ReadPublicKey(string publicKeyPath)
        {
            using (Stream keyIn = File.OpenRead(publicKeyPath))
            using (Stream inputStream = PgpUtilities.GetDecoderStream(keyIn))
            {
                PgpPublicKeyRingBundle publicKeyRingBundle = new PgpPublicKeyRingBundle(inputStream);
                PgpPublicKey foundKey = GetFirstPublicKey(publicKeyRingBundle);
                if (foundKey != null)
                    return foundKey;
            }
            throw new ArgumentException("No encryption key found in public key ring.");
        }

        public static PgpPublicKey GetFirstPublicKey(PgpPublicKeyRingBundle publicKeyRingBundle)
        {

            foreach (PgpPublicKeyRing kRing in publicKeyRingBundle.GetKeyRings())
            {
                PgpPublicKey key = kRing.GetPublicKeys()
                    .Cast<PgpPublicKey>()
                    .Where(k => k.IsEncryptionKey)
                    .FirstOrDefault();

                if (key != null)
                    return key;
            }
            return null;
        }
        #endregion

        #region Secret Key
        public static PgpSecretKey ReadSecretKey(string privateKeyPath)
        {
            using (Stream keyIn = File.OpenRead(privateKeyPath))
            using (Stream inputStream = PgpUtilities.GetDecoderStream(keyIn))
            {
                PgpSecretKeyRingBundle secretKeyRingBundle = new PgpSecretKeyRingBundle(inputStream);
                PgpSecretKey foundKey = GetFirstSecretKey(secretKeyRingBundle);
                if (foundKey != null)
                    return foundKey;
            }
            throw new ArgumentException("Can't find signing key in key ring.");
        }

        /// <summary>
        /// Return the first key we can use to encrypt.
        /// Note: A file can contain multiple keys (stored in "key rings")
        /// </summary>
        public static PgpSecretKey GetFirstSecretKey(PgpSecretKeyRingBundle secretKeyRingBundle)
        {
            foreach (PgpSecretKeyRing kRing in secretKeyRingBundle.GetKeyRings())
            {
                PgpSecretKey key = kRing.GetSecretKeys().Cast<PgpSecretKey>().Where(k => k.IsSigningKey).FirstOrDefault();

                if (key != null)
                    return key;
            }
            return null;
        }
        #endregion
        #region Private Key
        public static PgpPrivateKey ReadPrivateKey(PgpSecretKey secretKey, string passPhrase)
        {
            PgpPrivateKey privateKey = secretKey.ExtractPrivateKey(passPhrase.ToCharArray());
            if (privateKey != null)
                return privateKey;

            throw new ArgumentException("No private key found in secret key.");

        }
        public static PgpPrivateKey FindPrivateKey(PgpSecretKeyRingBundle pgpSecKeyRing, long secKeyId, string passPhrase)
        {
            PgpSecretKey pgpSecKey = pgpSecKeyRing.GetSecretKey(secKeyId);
            if (pgpSecKey == null)
                return null;

            return pgpSecKey.ExtractPrivateKey(passPhrase.ToCharArray());
        }
        #endregion
    }
}