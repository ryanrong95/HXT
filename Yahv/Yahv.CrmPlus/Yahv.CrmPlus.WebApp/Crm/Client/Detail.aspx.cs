using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.Client
{
    public partial class Detail : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var ID = Request.QueryString["ID"];
                var file = new FilesDescriptionRoll().Where(x => x.EnterpriseID == ID).ToArray();
                this.Model.Licenses = file.Where(x => x.Type == CrmFileType.License).Select(x => new { FileName = x.CustomName, Url = x.Url });
                this.Model.LogoFile = file.FirstOrDefault(x => x.Type == CrmFileType.Logo)?.Url;
                var client = Erp.Current.CrmPlus.Clients[ID];
                //所有人
                this.Model.Top10N =Erp.Current.CrmPlus.MyTops.FirstOrDefault(x => x.ClientID == ID && x.OwnerID == Erp.Current.ID);
                //  this.Model.Owner = new RelationsRoll()[ID].Admin.RealName;
                this.Model.Entity = new
                {
                    client.ID,
                    client.Name,
                    Source = client.SourceDes,
                    ClientTypeValue = client.ClientType,
                    ClientType = client.ClientType.GetDescription(),
                    Status = client.Status.GetDescription(),
                    Vip = client?.Vip == null ? VIPLevel.NonVIP.GetDescription() : client.Vip.GetDescription(),
                    ClientGrade = client?.Grade == null ? ClientGrade.None.GetDescription() : client.ClientGrade.GetDescription(),
                    ProfitRate = client.ProfitRate == null ? 0M : client.ProfitRate,
                    Protector = client.Owner != null ? client.Admin.RealName : client.Owner,
                    IsSpecial = client.IsSpecial ? "是" : "否",
                    IsMajor = client.IsMajor ? "是" : "否",
                    product = client.Industry??"",
                    Place = string.IsNullOrEmpty(client.Place) ? Origin.Unknown.GetDescription() : ((Origin)int.Parse(client.Place)).GetDescription(),
                    District = client?.DistrictDes,
                    EnterPriseStatus = client.Status,
                    Grade = client?.Grade,
                    client.EnterpriseRegister.IsSecret,
                    client.EnterpriseRegister.IsInternational,
                    IsInternationDes = client.EnterpriseRegister.IsInternational ? "是" : "否",
                    client.EnterpriseRegister.Corperation,
                    client.EnterpriseRegister.RegAddress,
                    client.EnterpriseRegister.Uscc,
                    Currency = client.EnterpriseRegister?.Currency == null ? Currency.Unknown.GetDescription() : client.EnterpriseRegister.Currency.GetDescription(),
                    RegistCurrency = client.EnterpriseRegister?.RegistCurrency == null ? Currency.Unknown.GetDescription() : client.EnterpriseRegister.RegistCurrency.GetDescription(),
                    client.EnterpriseRegister.RegistFund,
                    client.EnterpriseRegister.Industry,
                    RegistDate = client.EnterpriseRegister.RegistDate?.ToShortDateString(),
                    client.EnterpriseRegister.BusinessState,
                    client.EnterpriseRegister.WebSite,
                    Nature = client.EnterpriseRegister.Nature,
                    client.EnterpriseRegister.Employees
                };

            }
        }

         
        #region  保护操作
        /// <summary>
        /// 申请保护
        /// </summary>
        /// <returns></returns>
        protected object ApplyProtect()
        {
            var id = Request.Form["id"];
            try
            {
                var entity = Erp.Current.CrmPlus.Clients[id];
                if (entity.Owner == Erp.Current.ID)
                {
                    return new { success = false, message = "已被保护" };
                }
                var tasks = ApplyTasks.All(ApplyTaskType.ClientProtected);
                if (tasks.Any(item => item.MainID == entity.ID
                && item.ApplierID == Erp.Current.ID && item.Status == Underly.ApplyStatus.Waiting))
                {
                    return new { success = false, message = "已申请保护，正在审批中......" };
                }
                (new ApplyTask
                {
                    MainID = id,
                    MainType = MainType.Clients,
                    ApplierID = Erp.Current.ID,
                    ApplyTaskType = ApplyTaskType.ClientProtected,
                }).Enter();
                Service.LogsOperating.LogOperating(Erp.Current, entity.ID, $"{Erp.Current.RealName}申请了该客户：{entity.Name},保护;");
                return new { success = true, data = "", message = "" };
            }
            catch (Exception ex)
            {
                return new { success = false, message = "操作失败：" + ex };
            }


        }


        /// <summary>
        /// 取消保护
        /// </summary>
        /// <returns></returns>
        protected object CancelProtect()
        {
            var id = Request.Form["id"];
            try
            {
                var entity = Erp.Current.CrmPlus.Clients[id];
                entity.Owner = null;
                entity.Enter();
                (new ApplyTask
                {
                    MainID = id,
                    MainType = Underly.MainType.Clients,
                    ApplierID = Erp.Current.ID,
                    ApplyTaskType = Underly.ApplyTaskType.ClientProtected,
                    Status=Underly.ApplyStatus.Cancel
                }).EnterExtend();
                Service.LogsOperating.LogOperating(Erp.Current, entity.ID, $"{Erp.Current.RealName}取消了该客户：{entity.Name},保护;");
                return new { success = true, data = "", message = "" };
            }
            catch (Exception ex)
            {
                return new { success = false, message = "操作失败：" + ex };
            }


        }
        #endregion


        #region 设置Top10 

        /// <summary>
        /// 设置Top10
        /// </summary>
        /// <returns></returns>
        protected object SetTop10()
        {
            var id = Request.Form["id"];
           
            try
            {
                var tops = Erp.Current.CrmPlus.MyTops;
                int count = tops.Count();
                int maxTopOrder = 1;
                if (count > 0 && count < 10)
                {
                    maxTopOrder = tops.OrderByDescending(x => x.TopOrder).FirstOrDefault().TopOrder + 1;
                }
                if (count == 10)
                {
                    return new { success = false, message = "最多可以设置10个客户" };
                }
                var entity = new Yahv.CrmPlus.Service.Models.Origins.MapsTopN();
                entity.TopOrder = maxTopOrder;
                entity.ClientID = id;
                entity.OwnerID = Erp.Current.ID;
                entity.Enter();
                Service.LogsOperating.LogOperating(Erp.Current, entity.ClientID, $"{Erp.Current.RealName} 给客户{entity.OwnerID},设置了Top10;");
                return new { success = true, data = "", message = "" };
            }
            catch (Exception ex)
            {
                return new { success = false, message = "操作失败：" + ex };
            }


        }


        /// <summary>
        /// 取消Top
        /// </summary>
        /// <returns></returns>
        protected object CancelTop10()
        {
            var id = Request.Form["id"];
            try
            {
              
                var entity = Erp.Current.CrmPlus.MyTops.FirstOrDefault(x=>x.ClientID==id&&x.OwnerID==Erp.Current.ID);
                entity.CancelTop10();
                Service.LogsOperating.LogOperating(Erp.Current, entity.ClientID, $"{Erp.Current.RealName}取消了客户：{entity.OwnerID},Top设置;");
                return new { success = true, message = "保存成功" };
            }
            catch (Exception ex)
            {
                return new { success = false, message = "操作失败：" + ex };
            }

        }

        #endregion


    }
}