using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DBSApis.Models
{
    public class ApiService
    {
        static object locker = new object();
        static ApiService current;

        private KeyConfig keyConfigs;

        public KeyConfig KeyConfigs
        {
            get
            {
                if (keyConfigs == null)
                {
                    this.keyConfigs = new KeyConfig();
                }
                return this.keyConfigs;
            }
            set
            {
                this.keyConfigs = value;
            }
        }

        private UrlConfig urlConfigs;

        public UrlConfig UrlConfigs
        {
            get
            {
                if (this.urlConfigs == null)
                {
                    this.urlConfigs = new UrlConfig();
                }
                return this.urlConfigs;
            }
            set
            {
                this.urlConfigs = value;
            }
        }

        private ApiService()
        {       
            string fileServerUrl = ConfigurationManager.AppSettings["FileServerUrl"];
            string keyUrl = ConfigurationManager.AppSettings["KeyUrl"];
            string publicKey = ConfigurationManager.AppSettings["Server_Publickey"];
            string privateKey = ConfigurationManager.AppSettings["Client_Privatekey"];
            string privayeKeyPwd = ConfigurationManager.AppSettings["Client_Privatekey_Password"];
            string keyId = ConfigurationManager.AppSettings["Api_KeyId"];
            string orgId = ConfigurationManager.AppSettings["Api_OrgId"];    
            string swiftBic = ConfigurationManager.AppSettings["Api_SwiftBic"];

            this.keyConfigs = new KeyConfig();
            keyConfigs.PublicKey = fileServerUrl+ keyUrl+ publicKey;
            keyConfigs.PrivateKey = fileServerUrl + keyUrl + privateKey;
            keyConfigs.PrivayeKeyPwd = privayeKeyPwd;
            keyConfigs.KeyId = keyId;
            keyConfigs.OrgId = orgId;
            keyConfigs.swiftBic = swiftBic;

            string ApiServerUrl = ConfigurationManager.AppSettings["ApiServerUrl"];
            string ABEUrl = ConfigurationManager.AppSettings["ABEUrl"];
            string FXPricingUrl = ConfigurationManager.AppSettings["FXPricingUrl"];
            string FXBookingUrl = ConfigurationManager.AppSettings["FXBookingUrl"];
            string AREUrl = ConfigurationManager.AppSettings["AREUrl"];
            string ACTUrl = ConfigurationManager.AppSettings["ACTUrl"];
            string CNAPSUrl = ConfigurationManager.AppSettings["CNAPSUrl"];
            string TTUrl = ConfigurationManager.AppSettings["TTUrl"];

            this.urlConfigs = new UrlConfig();
            urlConfigs.ApiServerUrl = ApiServerUrl;
            urlConfigs.ABEUrl = ABEUrl;
            urlConfigs.FXPricingUrl = FXPricingUrl;
            urlConfigs.FXBookingUrl = FXBookingUrl;
            urlConfigs.AREUrl = AREUrl;
            urlConfigs.ACTUrl = ACTUrl;
            urlConfigs.CNAPSUrl = CNAPSUrl;
            urlConfigs.TTUrl = TTUrl;
           
        }

        public static ApiService Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new ApiService();
                        }
                    }
                }
                return current;
            }
        }
    }
}