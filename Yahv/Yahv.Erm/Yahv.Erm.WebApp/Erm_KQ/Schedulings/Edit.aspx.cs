using System;
using System.Linq;
using System.Threading.Tasks;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Utils.Serializers;
using Yahv.Web.Controls.Easyui;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm_KQ.Schedulings
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
        /// <summary>
        /// 加载数据
        /// </summary>
        private void InitData()
        {
            string id = Request.QueryString["id"];

            if (!string.IsNullOrWhiteSpace(id))
            {
                var scheduling = Erp.Current.Erm.Schedulings[id];

                this.Model = scheduling;
            }
        }
        #endregion

        #region 功能按钮

        protected void Save()
        {
            try
            {
                string id = Request.Form["ID"];
                string name = Request.Form["Name"];
                string IsMain = Request.Form["IsMain"];
                var schedulings = Erp.Current.Erm.Schedulings;
                if (schedulings.Where(item => item.ID != id).Any(item => item.Name == name))
                {
                    throw new Exception("班别名称不能重复!");
                }
                var staffs = Alls.Current.Staffs.Where(item => item.SchedulingID == id);
                if (staffs.Count() != 0 && IsMain == "false")
                {
                    throw new Exception("班别已经被分配给员工，不能修改主班别类型!");
                }

                string AmStartTime = Request.Form["AmStartTime"];
                string AmEndTime = Request.Form["AmEndTime"];
                string PmStartTime = Request.Form["PmStartTime"];
                string PmEndTime = Request.Form["PmEndTime"];
                string DomainValue = Request.Form["DomainValue"];
                string Summary = Request.Form["Summary"];

                TimeSpan? amStart, amEnd;
                TimeSpan pmStart, pmEnd;
                if (string.IsNullOrEmpty(AmStartTime))
                {
                    amStart = null;
                }
                else
                {
                    amStart = TimeSpan.Parse(AmStartTime);
                }
                if (string.IsNullOrEmpty(AmEndTime))
                {
                    amEnd = null;
                }
                else
                {
                    amEnd = TimeSpan.Parse(AmEndTime);
                }
                TimeSpan.TryParse(PmStartTime, out pmStart);
                TimeSpan.TryParse(PmEndTime, out pmEnd);

                Scheduling entity = schedulings[id] ?? new Scheduling();
                entity.Name = name;
                entity.AmStartTime = amStart;
                entity.AmEndTime = amEnd;
                entity.PmStartTime = pmStart;
                entity.PmEndTime = pmEnd;
                entity.DomainValue = int.Parse(DomainValue);
                entity.IsMain = IsMain == "true" ? true : false;
                entity.Summary = Summary;

                entity.Enter();


                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                string message = "空间名：" + ex.Source + "；" + '\n' +
                    "方法名：" + ex.TargetSite + '\n' +
                    "故障点：" + ex.StackTrace.ToString() + '\n' +
                    "错误提示：" + ex.Message;
                Response.Write((new { success = false, message = "保存失败：" + message }).Json());
            }
        }

        #endregion
    }
}