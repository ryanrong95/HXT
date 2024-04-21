using System;
using System.Linq;
using System.Threading.Tasks;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Controls.Easyui;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm_KQ.SealCertificates
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
                InitData();
            }
        }

        #region 加载数据

        protected void LoadComboBoxData()
        {
            var staffs = Erp.Current.Erm.XdtStaffs
                .Where(item => item.Status == StaffStatus.Normal || item.Status == StaffStatus.Period);
            //员工
            this.Model.StaffData = staffs.Select(item => new
            {
                Value = item.ID,
                Text = item.Name,
            });
            //部门类型
            this.Model.SealCertificateType = ExtendsEnum.ToDictionary<SealCertificateType>().Select(item => new { Value = item.Key, Text = item.Value });
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        private void InitData()
        {
            string id = Request.QueryString["id"];

            if (!string.IsNullOrWhiteSpace(id))
            {
                var seal = Erp.Current.Erm.SealCertificates.SingleOrDefault(item => item.ID == id);
                this.Model.SealModel = seal;
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
                string Type = Request.Form["Type"];
                string ProcessingDate = Request.Form["ProcessingDate"];
                string DueDate = Request.Form["DueDate"];
                string StaffID = Request.Form["StaffID"];

                SealCertificate entity = Erp.Current.Erm.SealCertificates.SingleOrDefault(item => item.ID == id) ?? new SealCertificate();
                entity.Name = name;
                entity.Type = (SealCertificateType)Enum.Parse(typeof(SealCertificateType), Type);
                entity.StaffID = StaffID;
                entity.AdminID = Erp.Current.ID;
                if(!string.IsNullOrEmpty(ProcessingDate))
                {
                    entity.ProcessingDate = Convert.ToDateTime(ProcessingDate);
                }
                if (!string.IsNullOrEmpty(DueDate))
                {
                    entity.DueDate = Convert.ToDateTime(DueDate);
                }
                entity.Enter();


                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }

        #endregion
    }
}