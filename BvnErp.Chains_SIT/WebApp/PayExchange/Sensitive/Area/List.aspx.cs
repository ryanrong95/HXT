using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.PayExchange.Sensitive.Area
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        protected void LoadData()
        {
            Dictionary<string, string> dicAreaType = new Dictionary<string, string>();
            dicAreaType.Add(Needs.Ccs.Services.Enums.PayExchangeSensitiveAreaType.Forbid.GetHashCode().ToString(), 
                            Needs.Ccs.Services.Enums.PayExchangeSensitiveAreaType.Forbid.GetDescription());
            dicAreaType.Add(Needs.Ccs.Services.Enums.PayExchangeSensitiveAreaType.Sensitive.GetHashCode().ToString(),
                            Needs.Ccs.Services.Enums.PayExchangeSensitiveAreaType.Sensitive.GetDescription());
            this.Model.AreaType = dicAreaType.Select(item => new { item.Key, item.Value }).Json();
        }

        /// <summary>
        /// 敏感地区列表数据
        /// </summary>
        protected void data()
        {
            var view = new Needs.Wl.Models.Views.PayExchangeSensitiveAreasView();
            view.AllowPaging = false;
            view.OrderBy = "Type";
            view.OrderBy = "Name";

            int recordCount = view.RecordCount;
            var areas = view.ToList();

            Func<Needs.Wl.Models.PayExchangeSensitiveArea, object> convert = area => new
            {
                AreaID = area.ID,
                AreaTypeDes = area.Type.GetDescription(),
                AreaTypeCode = area.Type.GetHashCode(),
                AreaName = area.Name,
            };

            Response.Write(new
            {
                rows = areas.Select(convert).ToArray(),
                total = recordCount,
            }.Json());
        }

        /// <summary>
        /// 新增/编辑敏感地区
        /// </summary>
        protected void EditArea()
        {
            string areaID = Request.Form["ID"];
            string areaName = Request.Form["Name"];
            string areaType = Request.Form["Type"];

            areaName = areaName.Trim();

            var theArea = new Needs.Wl.Models.Views.PayExchangeSensitiveAreasView()[areaID];
            if (theArea == null)
            {
                theArea = new Needs.Wl.Models.PayExchangeSensitiveArea()
                {
                    ID = System.Guid.NewGuid().ToString("N"),
                    Type = (Needs.Wl.Models.Enums.PayExchangeSensitiveAreaType)int.Parse(areaType),
                    Name = areaName,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                };
            }
            else
            {
                theArea.Name = areaName;
                theArea.Type = (Needs.Wl.Models.Enums.PayExchangeSensitiveAreaType)int.Parse(areaType);
                theArea.UpdateDate = DateTime.Now;
            }

            theArea.Enter();

            Response.Write((new { success = true, message = "保存成功" }).Json());
        }

        /// <summary>
        /// 删除敏感区域
        /// </summary>
        protected void DeleteArea()
        {
            string areaID = Request.Form["ID"];
            var theArea = new Needs.Wl.Models.Views.PayExchangeSensitiveAreasView()[areaID];
            theArea.Abandon();

            Response.Write((new { success = true, message = "删除成功" }).Json());
        }

    }
}