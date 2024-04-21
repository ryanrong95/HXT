using System;
using System.Linq;
using Yahv.Erm.Services;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Utils.Validates;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm.WageItems
{
    /// <summary>
    /// 编辑页面
    /// </summary>
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var id = Request.QueryString["id"];
                this.Model.Data = Alls.Current.WageItems[id];
                this.Model.Type = ExtendsEnum.ToDictionary<WageItemType>().Select(item => new { value = item.Key, text = item.Value });
                this.Model.IsImport = ExtendsEnum.ToDictionary<IsCalc>().Select(item => new { value = item.Key, text = item.Value });
            }
        }

        /// <summary>
        /// 数据保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            var id = Request.QueryString["id"];
            var wageitem = Alls.Current.WageItems[id] ?? new Services.Models.Origins.WageItem();
            wageitem.Name = Request.Form["Name"].Trim();
            wageitem.AdminID = Erp.Current.ID;
            wageitem.InputerId = Request.Form["InputerId"];
            wageitem.Type = (WageItemType)int.Parse(Request.Form["Type"]);
            if (!string.IsNullOrEmpty(Request.Form["CalcOrder"]))
            {
                wageitem.CalcOrder = int.Parse(Request.Form["CalcOrder"]);
            }
            wageitem.IsImport = Request.Form["IsImport"].IsNumber() ? Convert.ToBoolean(int.Parse(Request.Form["IsImport"])) : Convert.ToBoolean(Request.Form["IsImport"]);
            wageitem.Formula = Request.Form["Formula"];
            wageitem.Description = Request.Form["Description"];
            wageitem.OrderIndex = 1;

            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "工资项管理",
                string.IsNullOrWhiteSpace(id) ? $"新增工资项：{wageitem.Name}" : $"修改资项：{wageitem.Name}", wageitem.Json());

            wageitem.EnterError += Wageitem_EnterError;
            wageitem.EnterSuccess += Wageitem_EnterSuccess;
            wageitem.Enter();

            
        }

        /// <summary>
        /// 保存成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Wageitem_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }

        /// <summary>
        /// 保存失败触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Wageitem_EnterError(object sender, Usually.ErrorEventArgs e)
        {
            Easyui.Alert("操作提示", e.Message, Web.Controls.Easyui.Sign.Error);
        }

        /// <summary>
        /// 获取录入人
        /// </summary>
        /// <returns></returns>
        protected object getAdmins()
        {
            return Alls.Current.Admins.Where(item => item.Status == AdminStatus.Normal).Select(item => new
            {
                text = item.RealName + "(" + item.RoleName + ")",
                value = item.ID,
            });
        }
    }
}