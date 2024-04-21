using System;
using System.Linq;
using System.Linq.Expressions;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Forms;
using YaHv.Csrm.Services;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Rolls;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Crm.Companies
{
    public partial class Selected_Companies : ClientPage
    {
        protected CooperType CooperType;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //下拉绑定
                cboCooperType.DataSource = ExtendsEnum.ToDictionary<CooperType>();
                cboCooperType.DataTextField = "Value";
                cboCooperType.DataValueField = "Key";
                cboCooperType.DataBind();
                this.Model = new { ClientID = Request.QueryString["id"], CooperType = Request.QueryString["type"] };
            }
        }
        protected object data()
        {
            var clientid = Request.QueryString["id"];
            CooperType type;
            if (Enum.TryParse(Request.QueryString["type"], out type))
            {
                this.CooperType = type;
                var arry = type.GetHasFlag(CooperType.None);
            }
            Expression<Func<Cooperater, bool>> predicate = item => true;
            var name = Request["s_name"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Enterprise.Name.Contains(name));
            }
            //var query = new ClientsRoll()[clientid].Cooperaters[type].Where(predicate);
            //return new
            //{
            //    rows = query.OrderBy(item => item.Enterprise.Name).ToArray().Select(item => new
            //    {
            //        item.Company.ID,
            //        item.Company.Enterprise.Name,
            //        item.Company.Enterprise.AdminCode,
            //        Range = item.Company.Range.GetDescription(),
            //        Type = item.Company.Type.GetDescription(),
            //        item.Company.Enterprise.District,
            //        CooperType = item.CooperType.GetDescription(),
            //        Status = item.Company.CompanyStatus.GetDescription()
            //    })
            //};
            return null;
        }
        /// <summary>
        /// 绑定合作关系
        /// </summary>
        protected void Binding()
        {
            var companyid = Request["id"];
            var id = Request["clientid"];
            CooperType type = (CooperType)int.Parse(Request["type"]);
            var entity = new ClientsRoll()[id];
            if (entity != null)
            {
                entity.CooperBinding(companyid, type);
                if (string.IsNullOrEmpty(companyid))
                {
                    Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                             nameof(Yahv.Systematic.Crm),
                                            "CooperBinding", "内部公司" + companyid + "与客户" + id + "合作", "合作类型:" + type.GetDescription());
                }
            }
        }

        /// <summary>
        /// 删除某业务
        /// </summary>
        protected void Delete()
        {
            var companyids = Request["ids"];
            var id = Request["clientid"];
            CooperType type = (CooperType)int.Parse(Request["type"]);
            var entity = new ClientsRoll()[id];
            if (entity != null)
            {
                entity.CooperUnbind(companyids.Split(','), type);
                if (string.IsNullOrEmpty(companyids))
                {
                    Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                             nameof(Yahv.Systematic.Crm),
                                            "CooperUnbind", "内部公司：" + companyids + "取消与客户" + id + "合作：", "取消的合作类型:" + type.GetDescription());
                }
            }
        }
    }
}