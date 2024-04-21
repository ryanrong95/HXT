using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.BasicInfo.Persons
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var id = Request.QueryString["id"];
                var entity = new Person();

                if (!string.IsNullOrEmpty(id))
                {
                    entity = Yahv.Erp.Current.Finance.Persons[id];
                }

                this.Model.Data = entity;
                var genders = ExtendsEnum.ToDictionary<GenderType>().Select(item => new { text = item.Value, value = item.Key });
                this.Model.Genders = genders;
            }
        }

        #region 提交保存

        protected object Submit()
        {
            var json = new JMessage() { success = true, data = "提交成功!" };

            var id = Request.Form["ID"];
            Person entity = null;

            try
            {
                var realName = Request.Form["RealName"];
                var gender = Request.Form["Gender"];
                var iDCard = Request.Form["IDCard"];
                var mobile = Request.Form["Mobile"];

                var persons = Erp.Current.Finance.Persons;

                if (string.IsNullOrWhiteSpace(id))
                {
                    entity = new Person()
                    {
                        RealName = realName,
                        Gender = (GenderType)int.Parse(gender),
                        IDCard = iDCard,
                        Mobile = mobile,
                        CreatorID = Erp.Current.ID,
                    };
                }
                else
                {
                    entity = persons[id];

                    entity.RealName = realName;
                    entity.Gender = (GenderType)int.Parse(gender);
                    entity.IDCard = iDCard;
                    entity.Mobile = mobile;
                    entity.ModifyDate = DateTime.Now;
                    entity.ModifierID = Erp.Current.ID;
                }

                entity.Enter();
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.人员管理, Services.Oplogs.GetMethodInfo(), string.IsNullOrWhiteSpace(id) ? "新增" : "修改", entity.Json());
            }
            catch (Exception ex)
            {
                json.success = false;
                json.data = "添加失败!" + ex.Message;
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.人员管理, Services.Oplogs.GetMethodInfo(), (string.IsNullOrWhiteSpace(id) ? "新增" : "修改") + "失败!", new { entity, ex = ex.ToString() }.Json());
                return json;
            }

            return json;
        }

        #endregion
    }
}