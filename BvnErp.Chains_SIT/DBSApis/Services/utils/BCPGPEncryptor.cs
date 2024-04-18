using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Security;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBSApis.Services
{
    public class BCPGPEncryptor
    {
        private Encrypt m_encrypt = null;
        private const int BufferSize = 0x10000; // should always be power of 2
        public BCPGPEncryptor(Encrypt encrypt)
        {
            this.m_encrypt = encrypt;
        }
        public string EncryptMessage(string message)
        {
            if (m_encrypt.IsSigning)
            {
                //byte[] encryptedMsg = EncryptAndSignMessage(Encoding.ASCII.GetBytes(message));
                //return Encoding.ASCII.GetString(encryptedMsg);
                byte[] encryptedMsg = EncryptAndSignMessage(Encoding.UTF8.GetBytes(message));
                return Encoding.UTF8.GetString(encryptedMsg);
            }
            else
                return "Without Signing not supported";
        }
        public byte[] EncryptAndSignMessage(byte[] inputData)
        {
            try
            {
                Stream inputStream = new MemoryStream(inputData);
                MemoryStream bOut = new MemoryStream();
                Stream outputStream = bOut;
                if (m_encrypt.IsArmored)
                {
                    outputStream = new ArmoredOutputStream(outputStream);
                }

                //Console.WriteLine("<----------Encrypting the input file using public key---------->" + m_encrypt.PublicKeyFilePath);

                PgpEncryptedDataGenerator encGenerator = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Aes256, new SecureRandom());
                encGenerator.AddMethod(m_encrypt.PublicKey);
                Stream encryptedOut = encGenerator.Open(outputStream, new byte[BufferSize]);

                PgpCompressedDataGenerator compGenerator = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);
                Stream compressedOut = compGenerator.Open(encryptedOut);

                //Console.WriteLine("<----------Generating OnePass Signature for signing using private Key---------->" + m_encrypt.PrivateKeyFilePath);

                PgpSecretKey secretKey = BCPGPUtils.ReadSecretKey(m_encrypt.PrivateKeyFilePath);
                PgpPrivateKey signingKey = BCPGPUtils.ReadPrivateKey(secretKey, m_encrypt.PrivateKeyPassword);
                PublicKeyAlgorithmTag tag = secretKey.PublicKey.Algorithm;
                PgpSignatureGenerator sigGenerator = new PgpSignatureGenerator(tag, HashAlgorithmTag.Sha256);
                sigGenerator.InitSign(PgpSignature.BinaryDocument, signingKey);

                //Console.WriteLine("<----------Compressing the encrypted data---------->");

                foreach (string userId in secretKey.PublicKey.GetUserIds())
                {
                    PgpSignatureSubpacketGenerator spGenerator = new PgpSignatureSubpacketGenerator();
                    spGenerator.SetSignerUserId(false, userId);
                    sigGenerator.SetHashedSubpackets(spGenerator.Generate());
                }

                //Console.WriteLine("<----------Signing the message after encryption---------->");

                sigGenerator.GenerateOnePassVersion(false).Encode(compressedOut);

                PgpLiteralDataGenerator litDataGenerator = new PgpLiteralDataGenerator();
                Stream literalOut = litDataGenerator.Open(compressedOut, PgpLiteralData.Binary, "filename", DateTime.UtcNow, new byte[BufferSize]);

                int len = 0;
                byte[] buf = new byte[BufferSize];
                //inputStream.Seek(0, SeekOrigin.Begin);
                while ((len = inputStream.Read(buf, 0, buf.Length)) > 0)
                {
                    literalOut.Write(buf, 0, len);
                    sigGenerator.Update(buf, 0, len);
                }
                inputStream.Close();
                //literalOut.Close();
                litDataGenerator.Close();
                sigGenerator.Generate().Encode(compressedOut);
                //compressedOut.Close();
                compGenerator.Close();
                //encryptedOut.Close();
                encGenerator.Close();
                if (m_encrypt.IsArmored)
                {
                    outputStream.Close();
                }
                //Console.WriteLine("<----------Generated Encrypted and Signed Message successfully---------->");
                return bOut.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in encrypt and sign : " + ex);
            }
        }
        public byte[] EncryptData(byte[] inputData)
        {
            try
            {
                Stream inputStream = new MemoryStream(inputData);
                MemoryStream bOut = new MemoryStream();
                Stream outputStream = bOut;
                if (m_encrypt.IsArmored)
                {
                    outputStream = new ArmoredOutputStream(outputStream);
                }

                Console.WriteLine("<----------Encrypting the input file using public key---------->" + m_encrypt.PublicKeyFilePath);

                PgpEncryptedDataGenerator encGenerator = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Aes256, new SecureRandom());
                encGenerator.AddMethod(m_encrypt.PublicKey);
                Stream encryptedOut = encGenerator.Open(outputStream, new byte[BufferSize]);

                PgpCompressedDataGenerator compGenerator = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);
                Stream compressedOut = compGenerator.Open(encryptedOut);

                PgpLiteralDataGenerator litDataGenerator = new PgpLiteralDataGenerator();
                Stream literalOut = litDataGenerator.Open(compressedOut, PgpLiteralData.Binary, "filename", DateTime.UtcNow, new byte[BufferSize]);

                int len = 0;
                byte[] buf = new byte[BufferSize];
                //inputStream.Seek(0, SeekOrigin.Begin);
                while ((len = inputStream.Read(buf, 0, buf.Length)) > 0)
                {
                    literalOut.Write(buf, 0, len);
                }
                inputStream.Close();
                //literalOut.Close();
                litDataGenerator.Close();
                //compressedOut.Close();
                compGenerator.Close();
                //encryptedOut.Close();
                encGenerator.Close();
                if (m_encrypt.IsArmored)
                {
                    outputStream.Close();
                }
                Console.WriteLine("<----------Generated Encrypted Message successfully---------->");
                return bOut.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in encrypt and sign : " + ex);
            }
        }
    }
}