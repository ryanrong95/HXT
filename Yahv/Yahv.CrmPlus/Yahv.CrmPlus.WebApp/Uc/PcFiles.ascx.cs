using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Uc
{
    public partial class PcFiles : vUserControl
    {
        //protected Service.Entity.PcFile[] pcFiles;
        //protected Service.Entity.FileDescription[] Files;
        //protected object[] pcFiles;
        protected object[] Files;
        public bool IsPc { get; set; }

        public PcFiles()
        {
            //this.Init+

            //cpage.PreInit += Cpage_PreInit;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (this.IsPc)
                {
                    var files = new Service.Views.PcFilesView().
                    Search(item => item.MainID == Request.QueryString["id"]).
                    ToMyArray().Select(item => new
                    {
                        ID = item.ID,
                        CustomName = item.CustomName,
                        Type = item.Type.GetDescriptions(),
                        item.Url,
                        CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm")
                    });
                    this.Files = files.ToArray();
                }
                else
                {
                    string subid = Request.QueryString["subid"];
                    Expression<Func<Service.Models.FileDescription, bool>> predicate = item => item.EnterpriseID == Request.QueryString["enterpriseid"];

                    if (string.IsNullOrWhiteSpace(subid))
                    {
                        predicate = predicate.And(item => item.SubID == null);
                    }
                    else
                    {
                        predicate = predicate.And(item => item.SubID == subid);
                    }


                    var files = new Service.Views.FilesDescriptionsView().
                    Search(predicate).ToMyArray().Select(item => new
                    {
                        ID = item.ID,
                        CustomName = item.CustomName,
                        item.Url,
                        Type = item.Type.GetDescriptions(),
                        CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm")
                    });
                    this.Files = files.ToArray();
                }

            }
        }

        protected void Delete()
        {
            var id = Request.Form["ID"];
            if (this.IsPc)
            {
                Service.Views.PcFilesView.Delete(id);
                Service.LogsOperating.LogOperating(Erp.Current, id, $"删除PcFiles:{id}");
            }
            else
            {
                Service.Views.FilesDescriptionsView.Delete(id);
                Service.LogsOperating.LogOperating(Erp.Current, id, $"删除FileDes:{id}");
            }
        }
    }
}