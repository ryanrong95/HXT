using System;
using System.Linq;
using Yahv.Erm.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm.Admins
{
    public partial class PositionEdit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var adminid = Request.QueryString["id"];
                var leagueids = Alls.Current.Admins.AdminPositions(adminid);
                this.Model.leagueids = leagueids;
                this.Model.treedata = new LeaguesTree(Category.Work).Tree();
            }
        }


        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                var adminid = Request.QueryString["id"];
                var ids = this.hidids.Value.Split(',');
                var positions = new LeaguesRoll().Where(item => ids.Contains(item.ID)).ToArray();
                Alls.Current.Admins.BindingPosition(adminid, positions);

                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "管理员管理", $"给【{adminid}】分配职位", positions.Json());
            }
            catch (Exception ex)
            {
                Easyui.Alert("操作提示", ex.Message, Web.Controls.Easyui.Sign.Error);
            }
            //Easyui.Reload("操作提示", "操作成功!", Web.Controls.Easyui.Sign.None);
            Easyui.Dialog.Close("操作成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}