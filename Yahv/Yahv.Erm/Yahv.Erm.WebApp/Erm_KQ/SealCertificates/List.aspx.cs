using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Common;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Models.Rolls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Application = Yahv.Erm.Services.Models.Origins.Application;
using ApplicationType = Yahv.Erm.Services.ApplicationType;

namespace Yahv.Erm.WebApp.Erm_KQ.SealCertificates
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected object data()
        {
            var seals = Erp.Current.Erm.SealCertificates.Where(GetExpression()).AsEnumerable();
            var linq = seals.Select(t => new
            {
                t.ID,
                t.Name,
                Type = t.Type.GetDescription(),
                ProcessingDate = t.ProcessingDate == null ? "--" : ((DateTime)t.ProcessingDate).ToString("yyyy-MM-dd"),
                DueDate = t.DueDate == null ? "--" : ((DateTime)t.DueDate).ToString("yyyy-MM-dd"),
                Staff = t.Staff?.Name,
            });
            return linq;
        }

        protected void Delete()
        {
            try
            {
                string id = Request.Form["ID"];
                var seal = Erp.Current.Erm.SealCertificates.FirstOrDefault(item => item.ID == id);
                seal.Abandon();

                Response.Write((new { success = true, message = "删除成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "删除失败:" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 查询条件
        /// </summary>
        /// <returns></returns>
        private Expression<Func<SealCertificate, bool>> GetExpression()
        {
            Expression<Func<SealCertificate, bool>> predicate = item => true;

            string name = Request.QueryString["name"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Name.Contains(name));
            }

            return predicate;
        }
    }
}