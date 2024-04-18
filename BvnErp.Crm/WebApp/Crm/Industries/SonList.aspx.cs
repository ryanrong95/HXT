using Needs.Utils.Serializers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Industries
{
    /// <summary>
    /// 行业子类展示页面
    /// </summary>
    public partial class SonList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var al = new ArrayList();
                al.Add(new { value = 1, text = "终端市场分组" });
                al.Add(new { value = 2, text = "终端市场分组子类" });
                al.Add(new { value = 3, text = "终端市场分组子子类" });
                this.Model.TypeData = al.Json();
            }
        }
        protected void data()
        {
            var type = Request.QueryString["Type"];
            var fatherID = Request.QueryString["fatherID"];
            if (string.IsNullOrEmpty(type) || type == "1")
            {
                var data = Needs.Erp.ErpPlot.Current.ClientSolutions.MyIndustries;
                var industry = from ins in data
                               join ins2 in data on ins.ID equals ins2.FatherID
                               where ins.ID == fatherID
                               orderby ins.Name, ins2.Name
                               select new
                               {
                                   FatherName = ins.Name,
                                   ins2.ID,
                                   ins2.Name,
                                   ins2.EnglishName
                               };
                this.Paging(industry);
            }
            else if (type == "2")
            {
                var data = Needs.Erp.ErpPlot.Current.ClientSolutions.MyIndustries;
                var industry = from ins in data
                               join ins2 in data on ins.ID equals ins2.FatherID
                               join ins3 in data on ins2.ID equals ins3.FatherID
                               where ins.ID == fatherID
                               orderby ins.Name, ins2.Name, ins3.Name
                               select new
                               {
                                   FatherName = ins.Name + "/" + ins2.Name,
                                   ins3.ID,
                                   ins3.Name,
                                   ins3.EnglishName
                               };
                this.Paging(industry);
            }
            else if (type == "3")
            {
                var data = Needs.Erp.ErpPlot.Current.ClientSolutions.MyIndustries;
                var industry = from ins in data
                               join ins2 in data on ins.ID equals ins2.FatherID
                               join ins3 in data on ins2.ID equals ins3.FatherID
                               join ins4 in data on ins3.ID equals ins4.FatherID
                               where ins.ID == fatherID
                               orderby ins.Name, ins2.Name, ins3.Name, ins4.Name
                               select new
                               {
                                   FatherName = ins.Name + "/" + ins2.Name + "/" + ins3.Name,
                                   ins4.ID,
                                   ins4.Name,
                                   ins4.EnglishName
                               };
                this.Paging(industry);
            }
        }
    }
}