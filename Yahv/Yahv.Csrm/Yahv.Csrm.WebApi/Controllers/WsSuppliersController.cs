//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using Yahv.Underly;
//using Yahv.Utils.Converters.Contents;
//using Yahv.Web.Mvc;
//using YaHv.Csrm.Services.Extends;
//using YaHv.Csrm.Services.Models.Origins;
//using YaHv.Csrm.Services.Views.Rolls;

//namespace Yahv.Csrm.WebApi.Controllers
//{
//    /// <summary>
//    /// 代仓储供应商
//    /// </summary>
//    public class WsSuppliersController : ClientController
//    {
//        // GET: WsSuppliers
//        /// <summary>
//        /// 代仓储供应商信息
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult Index()
//        {
//            var json = new JSingle<dynamic>()
//            {
//                code = 200,
//                success = true,
//                data = new WsSuppliersRoll().ToArray().Select(item => new
//                {
//                    ID = item.ID,
//                    Name = item.Enterprise.Name,
//                    item.EnglishName,
//                    item.ChineseName,
//                    Grade = item.Grade,
//                    GradeDesc = item.Grade.GetDescription(),
//                    item.Enterprise.Corporation,
//                    item.Enterprise.Uscc,
//                    item.Enterprise.RegAddress,
//                    Status = item.WsSupplierStatus,
//                    StatusDesc = item.WsSupplierStatus.GetDescription()
//                })
//            };
//            return Json(json, JsonRequestBehavior.AllowGet);
//        }
//        #region 代仓储供应商添加，修改
//        /// <summary>
//        /// 代仓储供应商添加，修改
//        /// </summary>
//        /// <param name="suppliername">供应商名称</param>
//        /// <param name="chinesename">中文名称</param>
//        /// <param name="englishname">英文名称</param>
//        /// <param name="corporation">法人</param>
//        /// <param name="regAddress">注册地址</param>
//        /// <param name="uscc">统一社会信用编码</param>
//        /// <param name="summary">备注</param>
//        /// <param name="grade">等级</param>
//        /// <param name="adminid">添加人ID</param>
//        /// <returns></returns>
//        public ActionResult WsSupplierEnter(string suppliername, string chinesename, string englishname, string corporation, string regAddress, string uscc, string summary, int grade, string adminid)
//        {
//            try
//            {
//                var model = new WsSupplier();
//                model.Enterprise = new Enterprise
//                {
//                    Name = suppliername,
//                    AdminCode = "",
//                    Corporation = corporation,
//                    RegAddress = regAddress,
//                    Uscc = uscc
//                };
//                model.Admin = new Admin
//                {
//                    ID = adminid
//                };
//                model.ChineseName = chinesename;
//                model.EnglishName = englishname;
//                model.Grade = SupplierGrade.Ninth;
//                model.Summary = summary;
//                model.EnterSuccess += WsSupplier_EnterSuccess;
//                model.StatusUnnormal += WsSupplier_NameRepeat;
//                model.Enter();
//                //添加与客户关系
//            }
//            catch
//            {
//                eJson(new JMessage() { code = 100, success = false, data = null });//代码错误
//            }
//            return eJson();
//        }
//        private void WsSupplier_NameRepeat(object sender, Usually.ErrorEventArgs e)
//        {
//            eJson(new JMessage() { code = 300, success = false, data = null });//供应商已存在
//        }

//        private void WsSupplier_EnterSuccess(object sender, Usually.SuccessEventArgs e)
//        {
//            eJson(new JMessage() { code = 200, success = true, data = null });//成功
//        }

//        #endregion

//        #region 供应商删除
//        /// <summary>
//        /// 代仓储供应商删除
//        /// </summary>
//        /// <param name="suppliername">供应商名称</param>
//        /// <param name="clientname">客户名称</param>
//        /// <returns></returns>
//        public ActionResult WsSupplierDelete(string suppliername,string clientname)
//        {
//            //不是删除供应商，删除的应该是与客户的关系
//            var wsclient = new WsClientsRoll()[clientname.MD5()];
//            var wssupplier = new WsSuppliersRoll()[suppliername.MD5()];
//            if (wsclient == null || wssupplier == null)
//            {
//                eJson(new JMessage { code = 300, success = false, data = null });
//            }
//            else
//            {
//                wsclient.DelMapsSupplier(wssupplier.ID);
//                eJson(new JMessage { code = 200, success = true, data = null });
//            }
//            return eJson();
//        }
//        #endregion

//        #region 文件上传：营业执照
//        /// <summary>
//        /// 代仓储供应商的营业执照添加
//        /// </summary>
//        /// <param name="suppliername">供应商名称</param>
//        /// <param name="url">路径</param>
//        /// <param name="type">文件类型</param>
//        /// <param name="fileformat">格式</param>
//        /// <param name="summary">备注</param>
//        /// <param name="filename">文件名称</param>
//        /// <returns></returns>
//        public ActionResult FileEnter(string suppliername, string url, string type, string fileformat, string summary, string filename)
//        {
//            try
//            {
//                var enterprise = new EnterprisesRoll().SingleOrDefault(item => item.Name == suppliername);
//                if (enterprise == null)
//                {
//                    eJson(new JMessage() { code = 310, success = false, data = null });//供应商不存在
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
//                        Admin = new Admin
//                        {
//                            ID = ""
//                        }
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

//        #region 供应商联系人
//        /// <summary>
//        /// 代仓储供应商的联系人
//        /// </summary>
//        /// <param name="suppliername">供应商公司名称</param>
//        /// <returns></returns>
//        public ActionResult Contacts(string suppliername)
//        {
//            var wssupplier = new WsSuppliersRoll().SingleOrDefault(item => item.Enterprise.Name == suppliername);
//            var json = new JSingle<dynamic>()
//            {
//                code = 310,
//                success = false,
//                data = null
//            };
//            if (wssupplier != null)
//            {
//                json.code = 200;
//                json.success = true;
//                json.data = wssupplier.Contacts;
//            }
//            return Json(json, JsonRequestBehavior.AllowGet);
//        }
//        #endregion

//        #region 联系人添加
//        /// <summary>
//        /// 代仓储供应商的联系人添加或修改
//        /// </summary>
//        /// <param name="wssuppliername"></param>
//        /// <param name="contactname"></param>
//        /// <param name="mobile"></param>
//        /// <param name="email"></param>
//        /// <param name="fax"></param>
//        /// <returns></returns>
//        public ActionResult ContactEnter(string wssuppliername, string contactname, string mobile, string email, string fax)
//        {
//            var wssupplier = new WsSuppliersRoll().SingleOrDefault(item => item.Enterprise.Name == wssuppliername);
//            if (wssupplier == null)
//            {
//                eJson(new JMessage() { code = 310, success = false, data = null });//供应商不存在
//            }
//            else
//            {
//                new Contact()
//                {
//                    EnterpriseID = wssupplier.ID,
//                    Name = contactname,
//                    Mobile = mobile,
//                    Tel = mobile,
//                    Email = email,
//                    Fax = fax,
//                    Admin = new Admin
//                    {
//                        ID = ""
//                    }
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

//        #region 发票
//        /// <summary>
//        /// 代仓储供应商的发票
//        /// </summary>
//        /// <param name="suppliername">供应商名称</param>
//        /// <returns></returns>
//        public ActionResult Invoices(string suppliername)
//        {
//            var wssupplier = new WsSuppliersRoll().SingleOrDefault(item => item.Enterprise.Name == suppliername);
//            var json = new JSingle<dynamic>()
//            {
//                code = 310,
//                success = false,
//                data = null
//            };
//            if (wssupplier != null)
//            {
//                json.code = 200;
//                json.success = true;
//                json.data = wssupplier.Invoices;
//            }
//            return Json(json, JsonRequestBehavior.AllowGet);
//        }
//        #endregion

//        #region 发票添加、修改
//        /// <summary>
//        /// 代仓储供应商的发票添加或修改
//        /// </summary>
//        /// <param name="suppliername">供应商名称</param>
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
//        /// <returns></returns>
//        public ActionResult InvoiceEnter(string suppliername, string bank, string bankaddress, string account, string taxperNumber, int invoicetype, string address, string contactname, string mobile, string email, string postzip)
//        {
//            var wssupplier = new WsSuppliersRoll().SingleOrDefault(item => item.Enterprise.Name == suppliername);

//            if (wssupplier == null)
//            {
//                eJson(new JMessage { code = 310, success = false, data = null });//供应商不存在
//            }
//            else
//            {
//                new Invoice
//                {
//                    EnterpriseID = wssupplier.ID,
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
//        /// 代仓储供应商的发票删除
//        /// </summary>
//        /// <param name="id">发票id</param>
//        /// <returns>code:300 供应商不存在，200 成功；success:是否成功；data</returns>
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


//        #region 受益人信息
//        /// <summary>
//        /// 代仓储供应商的受益人信息
//        /// </summary>
//        /// <param name="suppliername">供应商名称</param>
//        /// <returns></returns>
//        public ActionResult Beneficiaries(string suppliername)
//        {
//            var wssupplier = new WsSuppliersRoll().SingleOrDefault(item => item.Enterprise.Name == suppliername);
//            var json = new JSingle<dynamic>()
//            {
//                code = 310,
//                success = false,
//                data = null
//            };
//            if (wssupplier != null)
//            {
//                json.code = 200;
//                json.success = true;
//                json.data = wssupplier.Beneficiaries;
//            }
//            return Json(json, JsonRequestBehavior.AllowGet);
//        }
//        #endregion

//        #region 代仓储客户受益人添加，修改
//        /// <summary>
//        /// 代仓储客户受益人添加，修改
//        /// </summary>
//        /// <param name="suppliername">供应商名称</param>
//        /// <param name="bank">开户行</param>
//        /// <param name="bankaddress">开户行地址</param>
//        /// <param name="account">银行账号</param>
//        /// <param name="swiftcode">银行国际编码</param>
//        /// <param name="contactname">联系人姓名</param>
//        /// <param name="mobile">电话</param>
//        /// <param name="email">邮箱</param>
//        /// <param name="currency">币种</param>
//        /// <param name="adminid">添加人</param>
//        /// <returns></returns>
//        public ActionResult BenficiaryEnter(string suppliername, string bank, string bankaddress, string account, string swiftcode, string contactname, string mobile, string email, string currency, string adminid)
//        {
//            var wssupplier = new SuppliersRoll().SingleOrDefault(item => item.Enterprise.Name == suppliername);
//            if (wssupplier == null)
//            {
//                eJson(new JMessage { code = 310, success = false, data = null });//供应商不存在
//            }
//            else
//            {
//                Currency curren = Currency.Unknown;
//                new Beneficiary
//                {
//                    EnterpriseID = wssupplier.ID,
//                    RealName = wssupplier.Enterprise.Name,
//                    Bank = bank,
//                    BankAddress = bankaddress,
//                    Account = account,
//                    SwiftCode = swiftcode,
//                    Name = contactname,
//                    Mobile = mobile,
//                    Tel = mobile,
//                    Email = email,
//                    District = District.Unknown,
//                    Methord = Methord.TT,
//                    Currency = Enum.TryParse(currency, out curren) ? (Currency)int.Parse(currency) : curren,
//                    Admin = new AdminsAllRoll()[adminid]

//                }.Enter();
//                eJson(new JMessage { code = 200, success = true, data = null });
//            }
//            return eJson();
//        }
//        #endregion

//        #region 代仓储客户受益人删除
//        /// <summary>
//        /// 受益人id
//        /// </summary>
//        /// <param name="id">受益人id</param>
//        /// <returns></returns>
//        public ActionResult BenficiaryDelete(string id)
//        {
//            var benfit = new BeneficiariesRoll().Where(item => item.ID == id);
//            if (benfit == null)
//            {
//                eJson(new JMessage { code = 310, success = false, data = null });
//            }
//            else
//            {
//                benfit.Delete();
//                eJson(new JMessage { code = 200, success = true, data = null });
//            }
//            return eJson();
//        }
//        #endregion

//        #region 到货地址数据
//        /// <summary>
//        /// 代仓储客户的到货地址
//        /// </summary>
//        /// <param name="suppliername">客户名称</param>
//        /// <returns></returns>
//        public ActionResult Consignees(string suppliername)
//        {
//            var wsclient = new WsClientsRoll().SingleOrDefault(item => item.Enterprise.Name == suppliername);
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
//        /// <param name="suppliername">供应商名称</param>
//        /// <param name="title">抬头/门牌</param>
//        /// <param name="address">地址</param>
//        /// <param name="contactname">联系人姓名</param>
//        /// <param name="mobile">手机号、电话</param>
//        /// <param name="email">邮箱</param>
//        /// <param name="postzip">邮编</param>
//        /// <param name="adminid">添加人</param>
//        /// <returns></returns>
//        public ActionResult ConsigneeEnter(string suppliername, string title, string address, string contactname, string mobile, string email, string postzip, string adminid)
//        {
//            var wsclient = new WsSuppliersRoll().SingleOrDefault(item => item.Enterprise.Name == suppliername);
//            if (wsclient == null)
//            {
//                eJson(new JMessage() { code = 310, success = false, data = null });//供应商不存在
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

//        #region 客户与供应商关系
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="clientname"></param>
//        /// <param name="suppliername"></param>
//        /// <param name="adminid"></param>
//        /// <returns></returns>
//        public ActionResult Maps(string clientname, string suppliername, string adminid)
//        {
//            var wsclient = new WsClientsRoll().SingleOrDefault(item => item.Enterprise.Name == clientname);
//            var wssupplier = new WsSuppliersRoll().SingleOrDefault(item => item.Enterprise.Name == suppliername);
//            var admin = new AdminsAllRoll()[adminid];
//            if (wsclient == null || wssupplier == null || admin == null)
//            {
//                eJson(new JMessage() { code = 300, success = false, data = null });//客户或供应商或admin不存在
//            }
//            else
//            {
//                wssupplier.MapsSupplier(wssupplier.ID, admin.ID);
//                eJson(new JMessage() { code = 200, success = true, data = null });
//            }
//            return eJson();
//        }
//        /// <summary>
//        /// 删除客户与供应商的关系
//        /// </summary>
//        /// <param name="clientname">客户名称</param>
//        /// <param name="suppliername">供应商名称</param>
//        /// <returns></returns>
//        public ActionResult delMaps(string clientname, string suppliername)
//        {
//            var wsclient = new WsClientsRoll().SingleOrDefault(item => item.Enterprise.Name == clientname);
//            var wssupplier = new WsSuppliersRoll().SingleOrDefault(item => item.Enterprise.Name == suppliername);
//            if (wsclient == null || wssupplier == null )
//            {
//                eJson(new JMessage() { code = 300, success = false, data = null });//客户或供应商或admin不存在
//            }
//            else
//            {
//                wsclient.DelMapsSupplier(wssupplier.ID);
//                eJson(new JMessage() { code = 200, success = true, data = null });
//            }
//            return eJson();
//        }
//        #endregion

//        #region 供应商与库房的关系

//        #endregion



//    }
//}