//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Yahv.Utils.Converters.Contents;
//using YaHv.Csrm.Services.Models.Origins;
//using YaHv.Csrm.Services.Views.Rolls;

//namespace ShencLibrary
//{
//    public class DccSiteUser
//    {
//        string enterpriseid;
//        /// <summary>
//        /// 唯一码
//        /// </summary>
//        /// <chenhan>保障全局唯一</chenhan>
//        private string EnterpriseID
//        {
//            get
//            {
//                if (this.EnterpriseName == "个人承运商")
//                {
//                    return this.enterpriseid ?? "Personal";
//                }
//                else if (this.EnterpriseName == "芯达通物流部")
//                {
//                    return this.enterpriseid ?? "XdtPCL";
//                }
//                else
//                {
//                    return this.enterpriseid ?? this.EnterpriseName.MD5();
//                }
//            }
//            set
//            {
//                this.enterpriseid = value;
//            }
//        }
//        public string UserName { set; get; }
//        string pws;
//        public string Password {
//            get
//            {
//                    return this.pws.StrToMD5();
                
//            }
//            set
//            {
//                this.pws = value;
//            }
//        }
//        /// <summary>
//        /// 海关编码
//        /// </summary>
//        public string CustomsCode { set; get; }
//        public string EnterpriseName { set; get; }
//        public string RealName { set; get; }
//        public string Mobile { set; get; }
//        public string Tel { set; get; }
//        public string Email { set; get; }

//        public void Enter()
//        {
//            if (new SiteUsersRoll().Any(item => item.UserName == this.UserName))
//            {
//                return;//用户名已存在
//            }
//            else
//            {
//                var enterprise = new EnterprisesRoll()[this.EnterpriseID] ?? new Enterprise { ID = this.EnterpriseID, Name = this.EnterpriseName };
//                #region WsClient
//                var wsclient = new WsClientsRoll()[this.EnterpriseID];
//                if (wsclient == null)
//                {
//                    wsclient = new WsClient
//                    {
//                        Enterprise = enterprise,
//                        CustomsCode = CustomsCode
//                    };
//                }
//                else
//                {
//                    wsclient.CustomsCode = this.CustomsCode;
//                }
//                wsclient.Enter();
//                #endregion

//                #region SiteUser
//                var xdtuser = new SiteUsersXdtRoll(enterprise).FirstOrDefault(item=>item.UserName==this.UserName);
//                xdtuser.Enterprise = enterprise;
//                xdtuser.EnterpriseID = enterprise.ID;
//                xdtuser.UserName = this.UserName;
//                xdtuser.Password = this.Password;
//                xdtuser.RealName = this.RealName;
//                xdtuser.Mobile = this.Mobile;
//                xdtuser.Email = this.Email;
//                xdtuser.Enter();
//                #endregion
                
//            }
            

//        }
//    }
//}
