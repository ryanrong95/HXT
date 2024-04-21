//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using Yahv.Underly;
//using Yahv.Utils.Converters.Contents;
//using Yahv.Utils.Serializers;
//using Yahv.Web.Mvc;
//using YaHv.Csrm.Services.Extends;
//using YaHv.Csrm.Services.Models.Origins;
//using YaHv.Csrm.Services.Views.Rolls;

//namespace Yahv.Csrm.WebApi.Controllers
//{
//    /// <summary>
//    /// 代仓储客户
//    /// </summary>
//    public class WsClientsController : ClientController
//    {
//        // GET: WsClients
//        /// <summary>
//        /// 代仓储客户信息
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult Index()
//        {
//            var client = new WsClientsRoll().ToArray();
//            var json = new JSingle<dynamic>()
//            {
//                code = 200,
//                success = true,
//                data = new WsClientsRoll().ToArray().Select(item => new
//                {
//                    ID = item.ID,
//                    Name = item.Enterprise.Name,
//                    Grade = item.Grade,
//                    GradeDesc = item.Grade.GetDescription(),
//                    item.Enterprise.Corporation,
//                    item.Enterprise.Uscc,
//                    item.Enterprise.RegAddress,
//                    item.EnterCode,
//                    item.CustomsCode,
//                    Status = item.WsClientStatus,
//                    StatusDesc = item.WsClientStatus.GetDescription(),
//                    ServiceManager = item.ServiceManager == null ? null : item.ServiceManager.ID,
//                    ServiceManagerRealName = item.ServiceManager == null ? null : item.ServiceManager.RealName,
//                    Merchandiser = item.Merchandiser == null ? null : item.Merchandiser.ID,
//                    MerchandiserRealName = item.Merchandiser == null ? null : item.Merchandiser.RealName,
//                    //BusinessLicense = item.BusinessLicense
//                })
//            };
//            return Json(json, JsonRequestBehavior.AllowGet);
//        }

//        #region 代仓储客户添加，修改
//        /// <summary>
//        /// 代仓储客户添加，修改
//        /// </summary>
//        /// <param name="clientname">客户名称</param>
//        /// <param name="corporation">法人</param>
//        /// <param name="regAddress">注册地址</param>
//        /// <param name="uscc">统一社会信用编码</param>
//        /// <param name="customcode">海关编码</param>
//        /// <param name="entercode">入仓号</param>
//        /// <param name="summary">备注</param>
//        /// <param name="grade">客户等级</param>
//        /// <param name="contactname"></param>
//        /// <param name="mobile"></param>
//        /// <param name="tel">手机号</param>
//        /// <param name="email"></param>
//        /// <param name="fax"></param>
//        /// <param name="adminid">添加人ID</param>
//        /// <returns></returns>
//        public ActionResult WsClientEnter(string clientname, string corporation, string regAddress, string uscc, string customcode, string entercode, string summary, int grade, string contactname, string mobile, string tel, string email, string fax, string adminid)
//        {
//            try
//            {
//                var model = new WsClient();
//                model.Enterprise = new Enterprise
//                {
//                    Name = clientname,
//                    AdminCode = "",
//                    Corporation = corporation,
//                    RegAddress = regAddress,
//                    Uscc = uscc
//                };
//                model.Admin = new AdminsAllRoll()[adminid];
//                model.Grade = (ClientGrade)grade;
//                model.CustomsCode = customcode;
//                model.Grade = ClientGrade.None;
//                model.EnterCode = entercode;
//                model.Summary = summary;
//                model.EnterSuccess += WsClient_EnterSuccess;
//                model.StatusUnnormal += WsClient_NameRepeat;
//                model.Enter();
//                contactadd(clientname, contactname, mobile, tel, email, fax, adminid);
//            }
//            catch
//            {
//                eJson(new JMessage() { code = 100, success = false, data = null });//代码错误
//            }
//            return eJson();
//        }
//        void contactadd(string wsclientname, string contactname, string mobile, string tel, string email, string fax, string adminid)
//        {
//            var wsclient = new WsClientsRoll().SingleOrDefault(item => item.Enterprise.Name == wsclientname);
//            if (wsclient == null)
//            {
//                eJson(new JMessage() { code = 310, success = false, data = null });//客户不存在
//            }
//            else
//            {
//                new Contact()
//                {
//                    EnterpriseID = wsclient.ID,
//                    Name = contactname,
//                    Mobile = mobile,
//                    Tel = tel,
//                    Email = email,
//                    Fax = fax,
//                    Admin = new AdminsAllRoll()[adminid],
//                    IsDefault = true
//                }.Enter();
//            }
//        }
//        private void WsClient_NameRepeat(object sender, Usually.ErrorEventArgs e)
//        {
//            eJson(new JMessage() { code = 300, success = false, data = null });//客户已存在
//        }

//        private void WsClient_EnterSuccess(object sender, Usually.SuccessEventArgs e)
//        {
//            eJson(new JMessage() { code = 200, success = true, data = null });//成功
//        }

//        #endregion


//        #region 添加客户业务员、跟单员
//        /// <summary>
//        /// 指派业务员或跟单员
//        /// </summary>
//        /// <param name="clientname">客户名称</param>
//        /// <param name="serviceManagerID">业务员ID</param>
//        /// <param name="merchandiserID">跟单员ID</param>
//        /// <returns></returns>
//        public ActionResult Assign(string clientname, string serviceManagerID, string merchandiserID)
//        {
//            var wsclient =new WsClientsRoll()[clientname.MD5()];
//            if (wsclient == null)
//            {
//                eJson(new JMessage { code = 310, success = false, data = null });
//            }
//            //指派业务员
//            if (!string.IsNullOrWhiteSpace(serviceManagerID))
//            {
//                wsclient.Assin(serviceManagerID);
//                eJson(new JMessage { code = 200, success = true, data = null });
//            }
//            //指派跟单员
//            if (!string.IsNullOrWhiteSpace(merchandiserID))
//            {
//                wsclient.Assin(merchandiserID);
//                eJson(new JMessage { code = 200, success = true, data = null });
//            }
//            return eJson();
//        }
//        #endregion

//        #region 客户删除
//        /// <summary>
//        /// 代仓储客户删除
//        /// </summary>
//        /// <param name="clientname">客户名称</param>
//        /// <returns></returns>
//        public ActionResult WsClientDelete(string clientname)
//        {
//            var entity = new WsClientsRoll().SingleOrDefault(item => item.Enterprise.Name == clientname);
//            if (entity == null)
//            {
//                eJson(new JMessage() { code = 310, success = false, data = null });//客户不存在
//            }
//            else
//            {
//                entity.AbandonSuccess += WsClient_AbandonSuccess;
//                entity.Abandon();
//            }
//            return eJson();
//        }

//        private void WsClient_AbandonSuccess(object sender, Usually.SuccessEventArgs e)
//        {
//            eJson(new JMessage() { code = 200, success = true, data = null });//成功
//        }
//        #endregion

//        #region 文件上传：营业执照
//        /// <summary>
//        /// 代仓储客户的营业执照添加
//        /// </summary>
//        /// <param name="clientname">客户名称</param>
//        /// <param name="filename">文件名称</param>
//        /// <param name="url">路径</param>
//        /// <param name="type">文件类型</param>
//        /// <param name="fileformat">格式</param>
//        /// <param name="summary">备注</param>
//        /// <param name="adminid">上传人</param>
//        /// <returns></returns>
//        public ActionResult FileEnter(string clientname, string filename, string url, string type, string fileformat, string summary, string adminid)
//        {
//            try
//            {
//                var enterprise = new WsClientsRoll()[clientname.MD5()];
//                if (enterprise == null)
//                {
//                    eJson(new JMessage() { code = 310, success = false, data = null });//客户不存在
//                }
//                else
//                {
//                    var entity = new FileDescription()
//                    {
//                        EnterpriseID = enterprise.ID,
//                        Name = filename,
//                        Url = url,
//                        Type = (FileType)int.Parse(type),
//                        FileFormat = fileformat,
//                        Summary = summary,
//                        Admin = new AdminsAllRoll()[adminid]
//                    };
//                    entity.EnterSuccess += File_EnterSuccess;
//                }

//            }
//            catch
//            {
//                eJson(new JMessage() { code = 100, success = false, data = null });
//            }
//            return eJson();
//        }

//        private void File_EnterSuccess(object sender, Usually.SuccessEventArgs e)
//        {
//            eJson(new JMessage() { code = 200, success = true, data = null });//成功
//        }
//        #endregion

//        #region 客户联系人
//        /// <summary>
//        /// 代仓储客户的联系人
//        /// </summary>
//        /// <param name="clientname">客户公司名称</param>
//        /// <returns></returns>
//        //public ActionResult Contacts(string clientname)
//        //{
//        //    var wsclient = new WsClientsRoll().SingleOrDefault(item => item.Enterprise.Name == clientname);
//        //    var json = new JSingle<dynamic>()
//        //    {
//        //        code = 310,
//        //        success = false,
//        //        data = null
//        //    };
//        //    if (wsclient != null)
//        //    {
//        //        json.code = 200;
//        //        json.success = true;
//        //        json.data = wsclient.Contacts;
//        //    }
//        //    return Json(json, JsonRequestBehavior.AllowGet);
//        //}
//        #endregion

//        #region 联系人添加
//        /// <summary>
//        /// 代仓储客户的联系人添加或修改
//        /// </summary>
//        /// <param name="wsclientname"></param>
//        /// <param name="contactname"></param>
//        /// <param name="mobile"></param>
//        /// <param name="tel">手机号</param>
//        /// <param name="email"></param>
//        /// <param name="fax"></param>
//        /// <param name="adminid">添加人</param>
//        /// <returns></returns>
//        public ActionResult ContactEnter(string wsclientname, string contactname, string mobile, string tel, string email, string fax, string adminid)
//        {
//            var wsclient = new WsClientsRoll().SingleOrDefault(item => item.Enterprise.Name == wsclientname);
//            if (wsclient == null)
//            {
//                eJson(new JMessage() { code = 310, success = false, data = null });//客户不存在
//            }
//            else
//            {
//                new Contact()
//                {
//                    EnterpriseID = wsclient.ID,
//                    Name = contactname,
//                    Mobile = mobile,
//                    Tel = tel,
//                    Email = email,
//                    Fax = fax,
//                    Admin = new AdminsAllRoll()[adminid]
//                }.Enter();
//            }
//            return eJson();
//        }

//        #endregion

//        #region 联系人删除
//        /// <summary>
//        /// 删除联系人
//        /// </summary>
//        /// <param name="id">联系人id</param>
//        /// <returns></returns>
//        public ActionResult ContactDelete(string id)
//        {
//            var contact = new ContactsRoll()[id];

//            if (contact == null)
//            {
//                eJson(new JMessage() { code = 310, success = false, data = null });//联系人不存在
//            }
//            else
//            {
//                contact.Abandon();
//                eJson(new JMessage() { code = 200, success = true, data = null });
//            }
//            return View();
//        }
//        #endregion


//        #region 到货地址数据
//        /// <summary>
//        /// 代仓储客户的到货地址
//        /// </summary>
//        /// <param name="clientname">客户名称</param>
//        /// <returns></returns>
//        public ActionResult Consignees(string clientname)
//        {
//            var wsclient = new WsClientsRoll().SingleOrDefault(item => item.Enterprise.Name == clientname);
//            var json = new JSingle<dynamic>()
//            {
//                code = 310,
//                success = false,
//                data = null
//            };
//            if (wsclient != null)
//            {
//                json.code = 200;
//                json.success = true;
//                json.data = wsclient.Consignees;
//            }
//            return Json(json, JsonRequestBehavior.AllowGet);
//        }
//        #endregion

//        #region 到货地址添加,修改
//        /// <summary>
//        /// 到货地址添加,修改
//        /// </summary>
//        /// <param name="clientname">客户名称</param>
//        /// <param name="title">抬头/门牌</param>
//        /// <param name="address">地址</param>
//        /// <param name="contactname">联系人姓名</param>
//        /// <param name="mobile">手机号、电话</param>
//        /// <param name="email">邮箱</param>
//        /// <param name="postzip">邮编</param>
//        /// <param name="adminid">添加人</param>
//        /// <returns></returns>
//        public ActionResult ConsigneeEnter(string clientname, string title, string address, string contactname, string mobile, string email, string postzip, string adminid)
//        {
//            var wsclient = new WsClientsRoll().SingleOrDefault(item => item.Enterprise.Name == clientname);
//            if (wsclient == null)
//            {
//                eJson(new JMessage() { code = 310, success = false, data = null });//客户不存在
//            }
//            else
//            {
//                new Consignee
//                {
//                    EnterpriseID = wsclient.ID,
//                    Title = title,
//                    Address = address,
//                    Name = contactname,
//                    Mobile = mobile,
//                    Email = email,
//                    Tel = mobile,
//                    Postzip = postzip,
//                    Admin = new AdminsAllRoll()[adminid]
//                }.Enter();
//                eJson(new JMessage { code = 200, success = true, data = null });
//            }
//            return View();
//        }
//        #endregion

//        #region 到货地址删除
//        /// <summary>
//        /// 到货地址id
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        public ActionResult ConsigneeDelete(string id)
//        {
//            var consignee = new ConsigneesRoll().Where(item => item.ID == id);
//            if (consignee == null)
//            {
//                eJson(new JMessage() { code = 310, success = false, data = null });
//            }
//            else
//            {
//                consignee.Delete();
//                eJson(new JMessage() { code = 200, success = true, data = null });
//            }
//            return eJson();
//        }

//        #endregion

//        #region 发票
//        /// <summary>
//        /// 代仓储客户的发票
//        /// </summary>
//        /// <param name="clientname">客户名称</param>
//        /// <returns></returns>
//        public ActionResult Invoices(string clientname)
//        {
//            var wsclient = new WsClientsRoll().SingleOrDefault(item => item.Enterprise.Name == clientname);
//            var json = new JSingle<dynamic>()
//            {
//                code = 310,
//                success = false,
//                data = null
//            };
//            if (wsclient != null)
//            {
//                json.code = 200;
//                json.success = true;
//                json.data = wsclient.Invoice;
//            }
//            return Json(json, JsonRequestBehavior.AllowGet);
//        }
//        #endregion


//        #region 发票添加、修改
//        /// <summary>
//        /// 代仓储客户的发票添加或修改
//        /// </summary>
//        /// <param name="clientname">客户名称</param>
//        /// <param name="bank">银行</param>
//        /// <param name="bankaddress">银行地址</param>
//        /// <param name="account">银行账号</param>
//        /// <param name="taxperNumber">纳税人识别号</param>
//        /// <param name="invoicetype">发票类型</param>
//        /// <param name="address">地址</param>
//        /// <param name="contactname">联系人姓名</param>
//        /// <param name="mobile">手机号</param>
//        /// <param name="email">邮箱</param>
//        /// <param name="postzip">邮编</param>
//        /// <param name="adminid">添加人</param>
//        /// <returns></returns>
//        public ActionResult InvoiceEnter(string clientname, string bank, string bankaddress, string account, string taxperNumber, int invoicetype, string address, string contactname, string mobile, string email, string postzip, string adminid)
//        {
//            var wsclient = new WsClientsRoll().SingleOrDefault(item => item.Enterprise.Name == clientname);

//            if (wsclient == null)
//            {
//                eJson(new JMessage { code = 310, success = false, data = null });//客户不存在
//            }
//            else
//            {
//                new Invoice
//                {
//                    EnterpriseID = wsclient.ID,
//                    Bank = bank,
//                    BankAddress = bankaddress,
//                    Account = account,
//                    TaxperNumber = taxperNumber,
//                    Type = (InvoiceType)invoicetype,
//                    Address = address,
//                    Name = contactname,
//                    Mobile = mobile,
//                    Tel = mobile,
//                    Email = email,
//                    Postzip = postzip
//                }.Enter();
//                eJson(new JMessage { code = 200, success = true, data = null });
//            }
//            return eJson();
//        }
//        #endregion

//        #region 发票删除
//        /// <summary>
//        /// 代仓储客户的发票删除
//        /// </summary>
//        /// <param name="id">发票id</param>
//        /// <returns>code:300 客户不存在，200 成功；success:是否成功；data</returns>
//        public ActionResult InvoiceDelete(string id)
//        {
//            var invoice = new InvoicesRoll().Where(item => item.ID == id);
//            if (invoice == null)
//            {
//                eJson(new JMessage { code = 310, success = false, data = null });
//            }
//            else
//            {
//                invoice.Delete();
//                eJson(new JMessage { code = 200, success = true, data = null });
//            }

//            return eJson();
//        }
//        #endregion

//        #region Xdt网站用户信息
//        /// <summary>
//        /// 网站用户信息
//        /// </summary>
//        /// <param name="clientname">客户名称</param>
//        /// <returns></returns>
//        public ActionResult SiteUsers(string clientname)
//        {
//            var wsclient = new WsClientsRoll()[clientname.MD5()];
//            var json = new JSingle<dynamic>()
//            {
//                code = 310,
//                success = false,
//                data = null
//            };
//            if (wsclient != null)
//            {
//                json.code = 200;
//                json.success = true;
//                json.data = wsclient.SiteUsers;
//            }
//            return Json(json, JsonRequestBehavior.AllowGet);
//        }
//        #endregion

//        #region Xdt网站用户添加、编辑
//        //public ActionResult SiteuserEnter()
//        //{

//        //}
//        #endregion

//        #region Xdt网站用户删除

//        #endregion

//        #region 客户与供应商关系

//        #endregion


//        #region 客户与库房的关系

//        #endregion

//    }
//}