using System;
using System.Linq;
using System.Threading.Tasks;
using Yahv.Erm.Services;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm.PayBills
{
    public partial class Closed : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 数据查询
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            //获取当前所有工资项
            var wageitems = Alls.Current.WageItems.Where(item => item.Type == WageItemType.Normal);

            return new
            {
                rows = wageitems.OrderBy(t => t.OrderIndex).Select(item => new
                {
                    item.ID,
                    item.Name,
                }),
            };
        }

        /// <summary>
        /// 保存事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string dateIndex = Request.QueryString["dateIndex"];

                if (string.IsNullOrWhiteSpace(dateIndex))
                {
                    Easyui.Alert("操作提示", "封账日期不能为空!", Web.Controls.Easyui.Sign.Error);
                    return;
                }
                string adminId = Erp.Current.ID;

                Task.Run(() =>
                {
                    Alls.Current.PayBills.Close(dateIndex.Replace("-", ""), hNames.Value, adminId);
                });

                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "工资录入",
                    $"封账：{dateIndex}", string.Empty);

                Easyui.Dialog.Close("操作成功!", Web.Controls.Easyui.AutoSign.Success);

            }
            catch (System.Threading.ThreadAbortException ex)
            {
                //忽略
            }
            catch (Exception ex)
            {
                Easyui.Reload("操作提示", ex.Message, Web.Controls.Easyui.Sign.Error);
            }
        }

        #region api

        protected object closed()
        {
            try
            {
                string dateIndex = Request.QueryString["dateIndex"];
                string names = Request.QueryString["names"];

                if (string.IsNullOrWhiteSpace(dateIndex))
                {
                    return new { code = 400, msg = "封账日期不能为空" };
                }
                string adminId = Erp.Current.ID;

                Task.Run(() =>
                {
                    Alls.Current.PayBills.Close(dateIndex.Replace("-", ""), names, adminId);
                });
            }
            catch (System.Threading.ThreadAbortException ex)
            {
                //忽略
            }
            catch (Exception ex)
            {
                return new { code = 500, msg = "出现异常!" + ex.Message };
            }

            return new { code = 200, msg = "操作成功!" };
        }
        #endregion
    }
}