using Needs.Web;
using Needs.Utils.Descriptions;
using Needs.Utils.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Plat.Devlops
{
    public partial class List : Needs.Web.Sso.Forms.ErpPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            Expression<Func<Needs.Overall.Models.IDevlopNote, bool>> predicate = null;

            string project = Request.QueryString["CsProject"];

            if (!string.IsNullOrWhiteSpace(project))
            {
                Needs.Overall.CsProject csProject;
                if (Enum.TryParse(project, out csProject))
                {
                    predicate = predicate.And(item => item.CsProject == csProject);
                }
                else
                {
                    var arry = EnumUtils.ToEnumNameDictionary<Needs.Overall.CsProject>(item =>
                            item.StartsWith(project, StringComparison.OrdinalIgnoreCase))
                          .Select(item => item.Key).ToArray();

                    predicate = predicate.And(item => arry.Contains(item.CsProject));
                }
            }

            string typeName = Request.QueryString["TypeName"];
            if (!string.IsNullOrWhiteSpace(typeName))
            {
                predicate = predicate.And(item => item.TypeName.StartsWith(typeName));
            }


            IQueryable<Needs.Overall.Models.IDevlopNote> view = Needs.Overall.Devlopers.Currents;
            if (predicate != null)
            {
                view = Needs.Overall.Devlopers.Currents.Where(predicate);
            }

            Response.Paging(view, entity => new
            {
                Context = entity.Context,
                Devloper = entity.Devloper.ToString(),
                ID = entity.ID,
                MethodName = entity.MethodName,
                TypeName = entity.TypeName,
                Number = entity.Number,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                CsProject = entity.CsProject.ToString()
            });
        }
        protected object selects_project()
        {
            IEnumerable<Needs.Overall.CsProject> view = Needs.Overall.Devlopers.Currents.Select(item => item.CsProject)
                .Distinct().ToArray();

            string name_startsWith = Request["name_startsWith"];

            if (!string.IsNullOrWhiteSpace(name_startsWith))
            {
                var arry = Enum.GetNames(typeof(Needs.Overall.CsProject));
                view = view.Where(item =>
                {
                    foreach (var name in Enum.GetNames(typeof(Needs.Overall.CsProject)))
                    {
                        return name.StartsWith(name_startsWith, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                });
            }

            return view.Select(item => new
            {
                id = (int)item,
                name = item.ToString(),
            });
        }
        protected object selects_type()
        {
            Expression<Func<Needs.Overall.Models.IDevlopNote, bool>> predicate = null;


            Needs.Overall.CsProject csProject;
            if (Enum.TryParse(Request.QueryString["CsProject"], out csProject))
            {
                predicate = predicate.And(item => item.CsProject == csProject);
            }



            IQueryable<Needs.Overall.Models.IDevlopNote> view = Needs.Overall.Devlopers.Currents;

            if (predicate != null)
            {
                view = view.Where(predicate);
            }

            var arry = view.Select(item => item.TypeName).Distinct().ToArray();

            return arry.Select(item => new
            {
                id = item,
                name = item,
            });
        }
    }
}