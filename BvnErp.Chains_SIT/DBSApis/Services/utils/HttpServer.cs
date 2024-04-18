using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DBSApis.Services
{
    public class HttpServer
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static HttpListener listener;
        private Thread listenThread;
        public void Start()
        {
            try
            {
                string asyncCallbackUrl = ConfigurationManager.AppSettings["Async_Callback_Url"];
                string NotificationCallbackUrl = ConfigurationManager.AppSettings["Notification_Callback_Url"];

                listener = new HttpListener();
                //listener.Prefixes.Add("http://localhost:8089/");
                //listener.Prefixes.Add("http://127.0.0.1:8089/");
                listener.Prefixes.Add(asyncCallbackUrl);
                listener.Prefixes.Add(NotificationCallbackUrl);
                listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;

                listener.Start();
                this.listenThread = new Thread(new ParameterizedThreadStart(startlistener));
                this.listenThread.Start();
                log.Info("Listening...");
            }
            catch (Exception ex)
            {
                log.Fatal(ex.ToString());
            }
        }
        public void Stop()
        {
            try
            {
                listener.Stop();
            }
            catch (Exception ex)
            {
                //log.Fatal(ex.ToString());
            }
        }
        private void startlistener(object s)
        {

            while (true)
            {
                ////blocks until a client has connected to the server
                ProcessRequest();
            }
        }


        private void ProcessRequest()
        {
            try
            {
                IAsyncResult result = listener.BeginGetContext(ListenerCallback, listener);
                result.AsyncWaitHandle.WaitOne();
            }
            catch (Exception ex)
            {
                log.Fatal(ex.ToString());
            }
        }

        private void ListenerCallback(IAsyncResult result)
        {
            try
            {
                log.Info("Receiving callback...");
                HttpListenerContext context = listener.EndGetContext(result);
                //Thread.Sleep(1000);

                // Obtain the request object.
                HttpListenerRequest request = context.Request;
                string httpMethod = request.HttpMethod;
                log.Info("Http Method: " + httpMethod);
                string relativeUrl = request.Url.AbsolutePath.Remove(0, 1);
                log.Info("request Url: " + relativeUrl);

                string strVal = "";
                if (String.Equals(httpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                {
                    strVal = request.Url.OriginalString;
                }
                else
                {
                    strVal = new StreamReader(request.InputStream, request.ContentEncoding).ReadToEnd();
                }
                Console.WriteLine("<-----received message----->");
                log.Info(strVal);

                if (Utils.IsValidJson(strVal))
                {
                    // parse json
                    log.Info(strVal);
                }
                else
                {
                    Console.WriteLine("<-----decrypted message----->");
                    string decryptedMsg = DecryptMessage(strVal);
                    log.Info(decryptedMsg);
                }

                log.Info("Sending response...");
                // Obtain a response object.
                HttpListenerResponse response = context.Response;
                // Construct a response.
                //string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
                string responseMsg = "HTTP/ 1.1 200 OK";

                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseMsg);
                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                response.StatusCode = (int)HttpStatusCode.OK;

                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                // You must close the output stream.
                output.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                log.Fatal(ex.ToString());
            }
        }

        private string DecryptMessage(string strMsg)
        {
            log.Info("DecryptMessage...");
            try
            {
                string publicKey = ConfigurationManager.AppSettings["Server_Publickey"];
                string privateKey = ConfigurationManager.AppSettings["Client_Privatekey"];
                string privayeKeyPwd = ConfigurationManager.AppSettings["Client_Privatekey_Password"];

                Decrypt decrypt = new Decrypt();
                decrypt.IsVerify = true;
                decrypt.PublicKeyFilePath = publicKey;
                decrypt.PrivateKeyFilePath = privateKey;
                decrypt.PrivateKeyPassword = privayeKeyPwd;

                BCPGPDecryptor objPgpDecrypt = new BCPGPDecryptor(decrypt);
                string decryptedMsg = objPgpDecrypt.DecryptMessage(strMsg);
                return decryptedMsg;
            }
            catch (Exception ex)
            {
                log.Fatal(ex.ToString());
                return ex.ToString();
            }
        }
        private string EncryptMessage(string strMsg)
        {
            log.Info("EncryptMessage...");
            try
            {
                string publicKey = ConfigurationManager.AppSettings["Server_Publickey"];
                string privateKey = ConfigurationManager.AppSettings["Client_Privatekey"];
                string privayeKeyPwd = ConfigurationManager.AppSettings["Client_Privatekey_Password"];

                Encrypt encrypt = new Encrypt();
                encrypt.IsArmored = true;
                encrypt.IsSigning = true;
                encrypt.CheckIntegrity = true;
                encrypt.PublicKeyFilePath = publicKey;
                encrypt.PrivateKeyFilePath = privateKey;
                encrypt.PrivateKeyPassword = privayeKeyPwd;

                BCPGPEncryptor objPgpEncrypt = new BCPGPEncryptor(encrypt);
                string encryptedMsg = objPgpEncrypt.EncryptMessage(strMsg);
                return encryptedMsg;
            }
            catch (Exception ex)
            {
                log.Fatal(ex.ToString());
                return ex.ToString();
            }
        }
    }
}