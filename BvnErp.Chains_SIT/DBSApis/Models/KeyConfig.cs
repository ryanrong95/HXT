using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBSApis.Models
{
    public class KeyConfig
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public string PrivayeKeyPwd { get; set; }
        public string KeyId { get; set; }
        public string OrgId { get; set; }
        public string swiftBic { get; set; }
    }
}