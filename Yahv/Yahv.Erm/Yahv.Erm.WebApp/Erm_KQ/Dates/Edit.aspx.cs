using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Web.Controls.Easyui;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm_KQ.Dates
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitData();
            }
        }

        #region 加载数据
        private void InitData()
        {
            string id = Request.QueryString["id"];

            if (string.IsNullOrWhiteSpace(id))
            {
                return;
            }

            var entity = Erp.Current.Erm.SchedulesPublic[id];
            if (entity == null || string.IsNullOrWhiteSpace(entity.ID))
            {
                throw new ArgumentNullException($"未找到详细信息!");
            }

            this.Model.Data = entity;

            this.Model.Methods = ExtendsEnum.ToDictionary<ScheduleMethod>().Select(item => new { text = item.Value, value = item.Key });        //日程安排方式
            this.Model.Names = ExtendsEnum.ToDictionary<CalendarType>().Select(item => new { text = item.Value, value = item.Value });        //日期类型
            this.Model.Froms = ExtendsEnum.ToDictionary<ScheduleFrom>().Select(item => new { text = item.Value, value = item.Key });        //日程安排来源
            this.Model.Regions = Erp.Current.Erm.RegionsAc.Where(item => item.FatherID != null).Select(item => new { text = item.Name, value = item.ID });       //区域
            this.Model.Schedulings = Erp.Current.Erm.Schedulings.Select(item => new { text = item.Name, value = item.ID });
            this.Model.DateString = entity.Date.ToString("yyyy-MM-dd");
        }
        #endregion

        #region 功能函数
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string id = Request.QueryString["id"];

                if (string.IsNullOrWhiteSpace(id))
                {
                    Easyui.Alert("操作提示", "ID不能为空!", Sign.Error);
                    return;
                }

                var entity = Erp.Current.Erm.SchedulesPublic[id];

                if (string.IsNullOrWhiteSpace(entity?.ID))
                {
                    Easyui.Alert("操作提示", "未找到详细信息!", Sign.Error);
                    return;
                }

                entity.Method = (ScheduleMethod)int.Parse(Request.Form["Method"]);
                entity.Name = Request.Form["Name"];
                entity.From = (ScheduleFrom)int.Parse(Request.Form["From"]);
                //entity.RegionID = Request.Form["RegionID"];
                //entity.ShiftID = Request.Form["ShiftID"];
                entity.SchedulingID = Request.Form["SchedulingID"];

                if (!string.IsNullOrWhiteSpace(Request.Form["SalaryMultiple"]))
                {
                    entity.SalaryMultiple = decimal.Parse(Request.Form["SalaryMultiple"]);
                }
                else
                {
                    entity.SalaryMultiple = 1;
                }

                entity.Enter();
                Easyui.Dialog.Close("操作成功!", Web.Controls.Easyui.AutoSign.Success);
            }
            catch (Exception ex)
            {
                Easyui.Alert("操作提示", $"提交异常!{ex.Message}", Sign.Error);
            }
        }
        #endregion
    }
}