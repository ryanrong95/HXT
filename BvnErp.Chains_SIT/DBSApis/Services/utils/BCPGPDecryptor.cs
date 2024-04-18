using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Utilities.IO;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBSApis.Services
{
    public class BCPGPDecryptor
    {
        private Decrypt m_decrypt = null;
        public BCPGPDecryptor(Decrypt decrypt)
        {
            this.m_decrypt = decrypt;
        }
        public string DecryptMessage(string message)
        {
            byte[] decryptedMsg = null;
            if (m_decrypt.IsVerify)
            {
                //decryptedMsg = DecryptAndVerifyMessage(Encoding.ASCII.GetBytes(message));
                decryptedMsg = DecryptAndVerifyMessage(Encoding.UTF8.GetBytes(message));
            }
            else
            {
                //decryptedMsg = DecryptMessage(Encoding.ASCII.GetBytes(message));
                decryptedMsg = DecryptMessage(Encoding.UTF8.GetBytes(message));
            }
            //return Encoding.ASCII.GetString(decryptedMsg);
            return Encoding.UTF8.GetString(decryptedMsg);
        }
        public byte[] DecryptAndVerifyMessage(byte[] inputData)
        {
            try
            {
                FileStream publicKeyStream = File.Open(m_decrypt.PublicKeyFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                FileStream privateKeyStream = File.Open(m_decrypt.PrivateKeyFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                string passPhrase = m_decrypt.PrivateKeyPassword;

                Stream inputStream = new MemoryStream(inputData);
                inputStream = PgpUtilities.GetDecoderStream(inputStream);
                MemoryStream outputStream = new MemoryStream();

                PgpObjectFactory pgpObjF = new PgpObjectFactory(inputStream);
                PgpEncryptedDataList enc;
                PgpObject obj = pgpObjF.NextPgpObject();
                if (obj is PgpEncryptedDataList)
                {
                    enc = (PgpEncryptedDataList)obj;
                }
                else
                {
                    enc = (PgpEncryptedDataList)pgpObjF.NextPgpObject();
                }
                //
                // find the secret key
                //
                PgpPrivateKey sKey = null;
                PgpPublicKeyEncryptedData pbe = null;
                PgpSecretKeyRingBundle pgpSecRing = new PgpSecretKeyRingBundle(PgpUtilities.GetDecoderStream(privateKeyStream));
                foreach (PgpPublicKeyEncryptedData pked in enc.GetEncryptedDataObjects())
                {
                    sKey = BCPGPUtils.FindPrivateKey(pgpSecRing, pked.KeyId, passPhrase);
                    if (sKey != null)
                    {
                        pbe = pked;
                        break;
                    }
                }
                if (sKey == null)
                    throw new Exception("secret key for message not found.");

                Stream clear = pbe.GetDataStream(sKey);
                PgpObjectFactory pgpFact = new PgpObjectFactory(clear);
                PgpObject message = pgpFact.NextPgpObject();

                PgpOnePassSignatureList onePassSigList = null;
                PgpSignatureList sigList = null;

                while (message != null)
                {
                    if (message is PgpCompressedData)
                    {
                        Console.WriteLine("<<----------Decrypting the compressed payload---------->>");

                        PgpCompressedData compressedData = (PgpCompressedData)message;
                        Stream compDataIn = compressedData.GetDataStream();
                        pgpFact = new PgpObjectFactory(compDataIn);
                        message = pgpFact.NextPgpObject();
                        //this.compressionAlgorithm = compressedData.getAlgorithm();
                    }
                    //
                    if (message is PgpOnePassSignatureList)
                    {
                        onePassSigList = (PgpOnePassSignatureList)message;
                    }
                    else if (message is PgpSignatureList)
                    {
                        sigList = (PgpSignatureList)message;
                    }
                    else if (message is PgpLiteralData)
                    {
                        PgpLiteralData Ld = null;
                        Ld = (PgpLiteralData)message;
                        //Stream output = File.Create(outputpath + "\\" + Ld.FileName);      
                        Stream unc = Ld.GetInputStream();
                        //Streams.PipeAll(unc, output);
                        Streams.PipeAll(unc, outputStream);
                    }
                    else
                    {
                        Console.WriteLine("<----message unknown message type---->");
                    }
                    message = pgpFact.NextPgpObject();
                }

                //Verify message
                byte[] outputBytes = outputStream.ToArray();
                PgpPublicKey pKey = null;
                if (onePassSigList == null || sigList == null)
                {
                    throw new Exception("Poor PGP. Signatures not found.");
                }
                else
                {
                    for (int i = 0; i < onePassSigList.Count; i++)
                    {
                        PgpOnePassSignature ops = onePassSigList[0];
                        PgpPublicKeyRingBundle pKeyRingBundle = new PgpPublicKeyRingBundle(PgpUtilities.GetDecoderStream(publicKeyStream));
                        pKey = pKeyRingBundle.GetPublicKey(ops.KeyId);
                        if (pKey != null)
                        {
                            ops.InitVerify(pKey);
                            ops.Update(outputBytes);
                            PgpSignature signature = sigList[i];
                            if (ops.Verify(signature))
                            {
                                Console.WriteLine("<<----------Verifying the onepass signature using public key---------->>" + m_decrypt.PublicKeyFilePath);

                                foreach (string id in pKey.GetUserIds())
                                {
                                    Console.WriteLine("User Id: " + id);
                                }
                                Console.WriteLine("<------Signature verified------>");
                            }
                            else
                            {
                                Console.WriteLine("<-------Signature verification failed------>");
                            }
                        }
                    }
                }
                //
                if (pbe.IsIntegrityProtected() && !pbe.Verify())
                {
                    Console.WriteLine("Data is integrity protected but integrity is lost.");
                }
                else if (pKey == null)
                {
                    Console.WriteLine("Signature not found");
                }
                return outputStream.ToArray();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public byte[] DecryptMessage(byte[] inputData)
        {
            try
            {
                FileStream publicKeyStream = File.Open(m_decrypt.PublicKeyFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                FileStream privateKeyStream = File.Open(m_decrypt.PrivateKeyFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                string passPhrase = m_decrypt.PrivateKeyPassword;

                Stream inputStream = new MemoryStream(inputData);
                inputStream = PgpUtilities.GetDecoderStream(inputStream);
                MemoryStream outputStream = new MemoryStream();

                PgpObjectFactory pgpObjF = new PgpObjectFactory(inputStream);
                PgpEncryptedDataList enc;
                PgpObject obj = pgpObjF.NextPgpObject();
                if (obj is PgpEncryptedDataList)
                {
                    enc = (PgpEncryptedDataList)obj;
                }
                else
                {
                    enc = (PgpEncryptedDataList)pgpObjF.NextPgpObject();
                }
                //
                // find the secret key
                //
                PgpPrivateKey sKey = null;
                PgpPublicKeyEncryptedData pbe = null;
                PgpSecretKeyRingBundle pgpSecRing = new PgpSecretKeyRingBundle(PgpUtilities.GetDecoderStream(privateKeyStream));
                foreach (PgpPublicKeyEncryptedData pked in enc.GetEncryptedDataObjects())
                {
                    sKey = BCPGPUtils.FindPrivateKey(pgpSecRing, pked.KeyId, passPhrase);
                    if (sKey != null)
                    {
                        pbe = pked;
                        break;
                    }
                }
                if (sKey == null)
                    throw new Exception("secret key for message not found.");

                Stream clear = pbe.GetDataStream(sKey);
                PgpObjectFactory pgpFact = new PgpObjectFactory(clear);
                PgpObject message = pgpFact.NextPgpObject();

                PgpOnePassSignatureList onePassSigList = null;
                PgpSignatureList sigList = null;

                while (message != null)
                {
                    if (message is PgpCompressedData)
                    {
                        Console.WriteLine("<<----------Decrypting the compressed payload---------->>");

                        PgpCompressedData compressedData = (PgpCompressedData)message;
                        Stream compDataIn = compressedData.GetDataStream();
                        pgpFact = new PgpObjectFactory(compDataIn);
                        message = pgpFact.NextPgpObject();
                        //this.compressionAlgorithm = compressedData.getAlgorithm();
                    }
                    //
                    if (message is PgpOnePassSignatureList)
                    {
                        onePassSigList = (PgpOnePassSignatureList)message;
                    }
                    else if (message is PgpSignatureList)
                    {
                        sigList = (PgpSignatureList)message;
                    }
                    else if (message is PgpLiteralData)
                    {
                        PgpLiteralData Ld = null;
                        Ld = (PgpLiteralData)message;
                        //Stream output = File.Create(outputpath + "\\" + Ld.FileName);      
                        Stream unc = Ld.GetInputStream();
                        //Streams.PipeAll(unc, output);
                        Streams.PipeAll(unc, outputStream);
                    }
                    else
                    {
                        Console.WriteLine("<----message unknown message type---->");
                    }
                    message = pgpFact.NextPgpObject();
                }
                //
                if (pbe.IsIntegrityProtected())
                {
                    if (!pbe.Verify())
                    {
                        Console.WriteLine("<-----message failed integrity check------->");
                    }
                    else
                    {
                        Console.WriteLine("<-----message integrity check passed------->");
                    }
                }
                else
                {
                    Console.WriteLine("<--------no message integrity check------>");
                }
                return outputStream.ToArray();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}