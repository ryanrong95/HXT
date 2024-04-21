using System;
using System.Linq;
using Yahv.Underly;
using Yahv.Web.Forms;
using YaHv.Csrm.Services;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Crm.Clients
{
    public partial class Edit1 : BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var clientid = Request.QueryString["id"];
                this.Model.Entity = new ClientsRoll()[clientid];
                init();
            }
        }
        void init()
        {
            //客户级别
            this.Model.Grade = ExtendsEnum.ToArray<ClientGrade>().Select(item => new
            {
                value = (int)item,
                text = item.GetDescription()
            });
            //地区类型
            this.Model.AreaType = ExtendsEnum.ToArray<AreaType>().Select(item => new
            {
                value = (int)item,
                text = item.GetDescription()
            });
            //客户类型
            this.Model.ClientType = ExtendsEnum.ToArray<ClientType>().Select(item => new
            {
                value = (int)item,
                text = item.GetDescription()
            });
            //合作类型
            this.Model.CooperType = ExtendsEnum.ToArray<CooperType>().Select(item => new
            {
                value = (int)item,
                text = item.GetDescription()
            });
            //客户Vip等级
            this.Model.VipRank = ExtendsEnum.ToArray<VIPLevel>().Select(item => new
            {
                value = (int)item,
                text = (int)item == 0 ? "非Vip" : item.GetDescription()
            });
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var id = Request.QueryString["id"];
            var entity = Erp.Current.Crm.Clients[id] ?? new TradingClient();

            entity.Nature = (ClientType)int.Parse(Request["Nature"]);
            entity.AreaType = (AreaType)int.Parse(Request["Type"]);
            string taxpernumber = Request.Form["TaxperNumber"].Trim();
            string corporation = Request["Corporation"].Trim();
            string regAddress = Request["RegAddress"].Trim();
            // string uscc = Request["Uscc"].Trim();
            entity.TaxperNumber = string.IsNullOrWhiteSpace(taxpernumber) ? "" : taxpernumber;
            string dyjcode = Request["DyjCode"].Trim();
            entity.DyjCode = string.IsNullOrWhiteSpace(dyjcode) ? "" : dyjcode;
            string admincode = Request["AdminCode"].Trim();
            //0:非VIP
            var vip = int.Parse(Request.Form["radio_vipRank"]);
            entity.Vip = (Underly.VIPLevel)vip;

            ///国家或地区
            entity.Place = Request["Origin"];
            //重点客户
            entity.Major = Request["Major"] != null;

            entity.Enterprise = new Enterprise
            {
                Name = Request.Form["Name"].Trim(),
                AdminCode = string.IsNullOrWhiteSpace(admincode) ? "" : admincode,
                Corporation = corporation,
                RegAddress = regAddress,
                Uscc = taxpernumber
            };
            entity.saleid = Erp.Current.IsSuper ? "" : Erp.Current.ID;
            if (string.IsNullOrEmpty(id))
            {
                //entity.CompanyID = Request["txt_InternalCompany"];
                //录入人
                entity.CreatorID = Yahv.Erp.Current.ID;
                entity.StatusUnnormal += Entity_StatusUnnormal;

            }
            else
            {
                //可编辑级别
                entity.Grade = (ClientGrade)int.Parse(Request["Grade"]);
            }
            entity.EnterSuccess += Clients_EnterSuccess;
            entity.Enter();

        }

        private void Entity_StatusUnnormal(object sender, Usually.ErrorEventArgs e)
        {
            var entity = sender as TradingClient;
            Easyui.Reload("提示", "客户已存在，客户状态：" + entity.ClientStatus.GetDescription(), Yahv.Web.Controls.Easyui.Sign.Warning);
            //var Company = new CompaniesRoll()[entity.CompanyID];
            //Easyui.Reload("提示", "客户和" + Company.Enterprise.Name + "已进行合作", Yahv.Web.Controls.Easyui.Sign.Warning);
        }

        private void Clients_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as TradingClient;
            //CooperType coopertype = CooperType.None;
            //CooperType type;
            //if (Enum.TryParse(Request.QueryString["type"], out type))
            //{
            //    coopertype = type;
            //}
            //if (!string.IsNullOrEmpty(Request["txt_InternalCompany"]) && type != CooperType.None)
            //{
            //    entity.CooperBinding(Request["txt_InternalCompany"], (CooperType)int.Parse(Request["selCooperType"]));
            //    Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
            //                            nameof(Yahv.Systematic.Crm),
            //                           "AdminBinding", "客户" + entity.ID + "绑定" + Erp.Current.ID, "合作业务" + type.GetDescription());
            //}
            //操作日志
            if (string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                         nameof(Yahv.Systematic.Crm),
                                        "ClientInsert", "新增客户：" + entity.Enterprise.ID + ",销售公司ID：" + entity.CompanyID, "");
            }
            else
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                         nameof(Yahv.Systematic.Crm),
                                        "ClientUpdate", "修改客户：" + entity.Enterprise.ID, "");
            }
            //Easyui.Alert("提示", "保存成功", Yahv.Web.Controls.Easyui.Sign.Info, true, Web.Controls.Easyui.Method.Window);
            Easyui.Window.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
        protected object CheckEnterprise()
        {
            string clientname = Request["clientname"];
            string companyid = Request["companyid"];

            if (!string.IsNullOrWhiteSpace(clientname))
            {
                var clients = new TradingClientsRoll().FirstOrDefault(item => item.Enterprise.Name == clientname);
                if (clients == null)
                {
                    return new { success = true, code = 1 };
                }
                else if (!string.IsNullOrWhiteSpace(companyid))
                {
                    bool havecompany = clients.Sales.Any(item => item.Company.ID == companyid);
                    return new
                    {
                        code = havecompany ? 2 : 0,
                        success = !havecompany,
                        message = havecompany ? "销售公司已被占用，请选择其他公司" : ""
                    };
                }
                else
                {
                    return new { code = 1, success = true };
                }
            }
            else
            {
                return new { code = 0, success = false, message = "请输入客户名称" };
            }
        }

        protected object Origin()
        {
            return ExtendsEnum.ToArray<Origin>().Select(item => new
            {
                value = (int)item,
                text = item.GetDescription()
            });
        }
    }
}