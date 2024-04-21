using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Finance.Services.Enums;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.BasicInfo.Banks
{
    public partial class BankRiskArea : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string BankID = Request.QueryString["BankID"];
            this.Model.BankID = BankID;

            Dictionary<string, string> dicDistricts = new Dictionary<string, string>();
            foreach (Origin item in Enum.GetValues(typeof(Origin)))
            {
                dicDistricts.Add(item.ToString(), item.GetDescription());
            }
            this.Model.Districts = dicDistricts.Select(item => new { value = item.Key, text = item.Value });
        }

        protected object data()
        {
            string BankID = Request.QueryString["BankID"];

            using (var query1 = Erp.Current.Finance.BankRiskAreas)
            {
                var view = query1;

                view = view.SearchByBankID(BankID);

                return view.ToMyPage().Json();
            }
        }

        protected void Add()
        {
            Services.Models.Origins.BankRiskArea bankRiskArea = null;
            try
            {
                string BankID = Request.Form["BankID"];
                string Districts = Request.Form["Districts"];

                Origin flag;
                if (!Enum.TryParse<Origin>(Districts, true, out flag))
                {
                    Response.Write((new { success = false, message = "请正确选择地区", }).Json());
                    return;
                }

                var bankRiskAreas = Erp.Current.Finance.BankRiskAreas;
                var exist = bankRiskAreas.Where(t => t.BankID == BankID && t.District == Districts).FirstOrDefault();
                if (exist != null)
                {
                    Response.Write((new { success = false, message = "该地区已添加", }).Json());
                    return;
                }

                bankRiskArea = new Services.Models.Origins.BankRiskArea()
                {
                    BankID = BankID,
                    District = Districts,
                    CreatorID = Erp.Current.ID,
                    ModifierID = Erp.Current.ID,
                };

                bankRiskArea.Enter();
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.银行管理, Services.Oplogs.GetMethodInfo(), "风险地区-新增", bankRiskArea.Json());
                Response.Write((new { success = true, message = "添加成功", }).Json());
            }
            catch (Exception ex)
            {
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.银行管理, Services.Oplogs.GetMethodInfo(), "风险地区-新增 异常!", new { bankRiskArea, exception = ex.ToString() }.Json());
                Response.Write((new { success = false, message = $"提交异常!{ex.Message}", }).Json());
            }
        }

        protected void deleteBankRiskArea()
        {
            Services.Models.Origins.BankRiskArea bankRiskArea = null;
            try
            {
                string BankRiskAreaID = Request.Form["BankRiskAreaID"];

                bankRiskArea = Erp.Current.Finance.BankRiskAreas.Where(t => t.ID == BankRiskAreaID).FirstOrDefault();
                if (bankRiskArea == null)
                {
                    Response.Write((new { success = false, message = "已删除", }).Json());
                    return;
                }

                bankRiskArea.Disable();
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.银行管理, Services.Oplogs.GetMethodInfo(), "风险地区-删除", bankRiskArea.Json());
                Response.Write((new { success = true, message = "删除成功", }).Json());
            }
            catch (Exception ex)
            {
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.银行管理, Services.Oplogs.GetMethodInfo(), "风险地区-删除 异常!", new { bankRiskArea, exception = ex.ToString() }.Json());
                Response.Write((new { success = false, message = $"发生异常!{ex.Message}", }).Json());
            }
        }

    }
}