using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Yahv.Services;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApi.Controllers
{
    public class ShencController : ClientController
    {
        public Enterprise Company = new CompaniesRoll()["DBAEAB43B47EB4299DD1D62F764E6B6A"].Enterprise;
        JMessage result = new JMessage { };
        // GET: Shenc
        public ActionResult Index()
        {

            return View();
        }
        [HttpPost]
        public ActionResult ForIC([System.Web.Http.FromBody]Models.ShencModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Enterprise.Name))
                {
                    return eJson(new JMessage { code = 100, success = false, data = "企业名称缺失" });
                }
                else
                {
                    Enterprise enterprise = new EnterprisesRoll().FirstOrDefault(item => item.Name == model.Enterprise.Name);


                    #region client客户

                    if (model.Enterprise.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))//说明是用户注册
                    {
                        var wsclient = new WsClientsRoll().FirstOrDefault(item => item.Enterprise.Name == model.Enterprise.Name);
                        if (wsclient != null)
                        {
                            return eJson(new JMessage { code = 400, success = false, data = $"{model.Enterprise.Name},客户企业已存在，不能注册" });
                        }
                        #region Enterprise
                        if (enterprise == null)
                        {
                            enterprise = new Enterprise
                            {
                                Name = model.Enterprise.Name
                            };
                            enterprise.Enter();
                        }
                        #endregion

                        #region SiteUser
                        if (model.SiteUser != null)
                        {
                            if (new SiteUsersRoll().Any(item => item.UserName == model.SiteUser.UserName))
                            {
                                return eJson(new JMessage { code = 300, success = false, data = "用户名已存在" });
                            }
                            var siteuser = new SiteUsersXdtRoll(enterprise).FirstOrDefault(item => item.UserName == model.SiteUser.UserName) ?? new SiteUserXdt();
                            siteuser = siteuser ?? new SiteUserXdt();
                            siteuser.Enterprise = enterprise;
                            siteuser.EnterpriseID = enterprise.ID;
                            siteuser.UserName = model.SiteUser.UserName;
                            siteuser.Password = model.SiteUser.Password.StrToMD5();
                            siteuser.RealName = model.SiteUser.RealName;
                            siteuser.Mobile = model.SiteUser.Mobile;
                            siteuser.Email = model.SiteUser.Email;
                            siteuser.IsMain = model.SiteUser.IsMain;
                            siteuser.IsDeclaretion = false;
                            siteuser.IsStorageService = false;
                            //siteuser.UserNameRepeat += Siteuser_UserNameRepeat;
                            siteuser.EnterSuccess += Siteuser_EnterSuccess;
                            siteuser.Enter();
                        }
                        #endregion

                        #region wsclient
                        wsclient = wsclient ?? new WsClient();
                        enterprise.Uscc = model.Enterprise.Uscc;
                        enterprise.Corporation = model.Enterprise.Corporation;
                        enterprise.RegAddress = model.Enterprise.RegAddress;
                        wsclient.Company = this.Company;
                        wsclient.Enterprise = enterprise;
                        wsclient.CustomsCode = model.CustomsCode;
                        wsclient.Grade = ClientGrade.Ninth;
                        wsclient.Nature = ClientType.Unknown;
                        wsclient.ServiceType = ServiceType.Unknown;
                        wsclient.IsDeclaretion = false;
                        wsclient.IsStorageService = false;
                        wsclient.StorageType = WsIdentity.Unknown;
                        wsclient.CreatorID = "";
                        wsclient.ChargeWHType = ChargeWHType.Charge;
                        wsclient.EnterSuccess += Wsclient_EnterSuccess;
                        wsclient.Enter();
                        #endregion

                    }

                    else//完善信息，可修改企业名称
                    {
                        var wsclient = new WsClientsRoll().FirstOrDefault(item => item.Enterprise.Name == model.Enterprise.Name);
                        if (model.Enterprise.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))
                        {
                            return eJson(new JMessage { code = 600, success = false, data = "企业名字不能以reg-开头" });
                        }
                        if (wsclient == null)
                        {
                            return eJson(new JMessage { code = 500, success = false, data = "客户企业不存在" });
                        }
                        wsclient = wsclient ?? new WsClient();
                        wsclient = wsclient ?? new WsClient();
                        enterprise.Uscc = model.Enterprise.Uscc;
                        enterprise.Corporation = model.Enterprise.Corporation;
                        enterprise.RegAddress = model.Enterprise.RegAddress;
                        wsclient.Company = this.Company;
                        wsclient.Enterprise = enterprise;
                        wsclient.CustomsCode = model.CustomsCode;
                        wsclient.CreatorID = "";
                        wsclient.EnterSuccess += Wsclient_EnterSuccess;
                        wsclient.Enter();
                    }

                    #endregion

                    #region contact联系人

                    //添加联系人
                    if (model.Contact != null)
                    {
                        if (model.Enterprise.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))
                        {
                            return eJson(new JMessage { code = 600, success = false, data = "企业名字不能以reg-开头" });
                        }
                        //var contact = new WsContactsRoll(enterprise);
                        string mobile = model.Contact.Mobile;
                        new WsContact
                        {
                            Enterprise = enterprise,
                            Type = ContactType.Online,//线上联系人
                            Name = string.IsNullOrWhiteSpace(model.Contact.Name) ? "" : model.Contact.Name,
                            Mobile = mobile,
                            Tel = model.Contact.Tel,
                            Email = model.Contact.Email,
                            Status = YaHv.Csrm.Services.Status.Normal,
                            CreatorID = string.IsNullOrWhiteSpace(model.Contact.CreatorID) ? "" : model.Contact.CreatorID,
                            EnterpriseID = enterprise.ID,
                            IsDefault = true
                        }.Enter();

                    }
                    #endregion

                    #region 异步调用Jss通接口

                    var entity = new
                    {
                        Company = enterprise.Name,
                        UserName = model.SiteUser?.UserName,
                        Password = model.SiteUser?.Password.StrToMD5(),
                        Contacts = model.SiteUser?.RealName,
                        Mobile = model.SiteUser?.Mobile,
                        CustomsCode = model.CustomsCode,
                        Uscc = model.Enterprise?.Uscc,
                        Corporate = model.Enterprise?.Corporation,
                        Address = model.Enterprise?.RegAddress,
                        Tel = model.Contact?.Tel,
                        Email = model.Contact?.Email,

                    }.Json();

                    string response = "";
                    // response = Commons.HttpPostRaw(Commons.UnifyApiUrl + "/loginClient", entity);
                    Task t1 = new Task(() =>
                    {
                        try
                        {
                            response = Commons.HttpPostRaw(Commons.UnifyApiUrl + "/loginClient", entity);
                        }
                        catch (Exception ex)
                        {
                            eJson(new JMessage { code = 400, success = false, data = "XDT接口调用失败" + ex });
                        }

                    });
                    t1.Start();
                    Task.WaitAll(t1);
                    Dictionary<string, string> ss = response.JsonTo<Dictionary<string, string>>();
                    if (ss?["code"] != "0")
                    {
                        return eJson(new JMessage { code = 400, success = false, data = "XDT接口异常," + ss["desc"] });
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                eJson(new JMessage
                {
                    code = 300,
                    success = false,
                    data = ex.Message
                });
            }
            return eJson();
        }
        #region 
        private void Siteuser_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            eJson(new JMessage { code = 200, success = true, data = "用户保存成功" });
        }

        private void Wsclient_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            eJson(new JMessage { code = 200, success = true, data = "保存成功" });
        }

        //private void Businesslicense_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        //{
        //    eJson(new JMessage { code = 200, success = true, data = "营业执照保存成功" });
        //}

        //private void Siteuser_UserNameRepeat(object sender, Usually.ErrorEventArgs e)
        //{
        //    eJson(new JMessage { code = 300, success = false, data = "用户名已存在" });
        //}
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EnterpriseRename()
        {
            try
            {
                string OldName = Request["OldName"];
                string NewName = Request["NewName"];
                if (!OldName.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))
                {
                    return eJson(new JMessage { code = 500, success = false, data = "企业名称不能修改" });
                }
                if (NewName.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))
                {
                    return eJson(new JMessage { code = 600, success = false, data = "企业名称不规范" });
                }
                bool result = false;
                var oldwsclient = new WsClientsRoll().FirstOrDefault(item => item.Enterprise.Name == OldName);

                if (oldwsclient == null)
                {
                    return eJson(new JMessage { code = 100, success = false, data = "企业客户不存在" });
                }
                else
                {
                    var newclient = new WsClientsRoll().FirstOrDefault(item => item.Enterprise.Name == NewName);
                    var enterprise = new EnterprisesRoll().FirstOrDefault(item => item.Name == NewName);
                    if (newclient != null)//客户存在
                    {
                        return eJson(new JMessage { code = 300, success = false, data = "客户已存在" });

                    }
                    else if (enterprise != null)//客户部存在，企业存在
                    {
                        //把oldwsclient的企业ID和SiteUser的企业ID修改为enterprise.ID
                        result = oldwsclient.UpdateEnterpriseID(enterprise.ID);
                        if (!result)
                        {
                            return eJson(new JMessage { code = 400, success = false, data = "内部错误" });
                        }
                        eJson(new JMessage { code = 200, success = true, data = enterprise.ID });
                    }
                    else
                    {
                        //修改企业名称
                        result = oldwsclient.Enterprise.UpdateName(NewName);
                        if (!result)
                        {
                            return eJson(new JMessage { code = 400, success = false, data = "内部错误" });
                        }
                        eJson(new JMessage { code = 200, success = true, data = oldwsclient.ID });
                    }
                }
                if (result)
                {
                    //向jss同步
                    string apiurl = System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"];
                    //if (!string.IsNullOrWhiteSpace(apiurl))
                    //{
                    //    var ss = Yahv.Utils.Http.ApiHelper.Current.JPost($"{apiurl}/clients/ModifyCompanyName", new { OldName = OldName, NewName = NewName });
                    //}
                    string response = "";
                    Task t1 = new Task(() =>
                {
                    try
                    {
                        response = Yahv.Utils.Http.ApiHelper.Current.JPost($"{apiurl}/clients/ModifyCompanyName?OldName={OldName}&NewName={NewName}");
                    }
                    catch (Exception ex)
                    {
                        eJson(new JMessage { code = 400, success = false, data = "XDT接口调用失败" + ex });
                    }

                });
                    t1.Start();
                    Task.WaitAll(t1);
                }
            }
            catch (Exception ex)
            {
                return eJson(new JMessage { code = 400, success = false, data = "内部错误，" + ex });
            }
            return eJson();
        }


    }
}