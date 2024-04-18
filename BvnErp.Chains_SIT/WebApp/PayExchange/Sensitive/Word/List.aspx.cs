using Needs.Linq;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.PayExchange.Sensitive.Word
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
            this.Model.AreaID = Request.QueryString["AreaID"];
        }

        /// <summary>
        /// 关键词列表数据
        /// </summary>
        protected void data()
        {
            string areaID = Request.QueryString["AreaID"];

            var predicate = PredicateBuilder.Create<Needs.Wl.Models.PayExchangeSensitiveWord>();
            predicate = predicate.And(item => item.AreaID == areaID);

            var view = new Needs.Wl.Models.Views.PayExchangeSensitiveWordsView();
            view.AllowPaging = false;
            view.Predicate = predicate;
            view.OrderBy = "Content";

            int recordCount = view.RecordCount;
            var words = view.ToList();

            Func<Needs.Wl.Models.PayExchangeSensitiveWord, object> convert = word => new
            {
                WordID = word.ID,
                WordContent = word.Content,
            };

            Response.Write(new
            {
                rows = words.Select(convert).ToArray(),
                total = recordCount,
            }.Json());
        }

        /// <summary>
        /// 新增/编辑关键词
        /// </summary>
        protected void EditWord()
        {
            string wordID = Request.Form["ID"];
            string areaID = Request.Form["AreaID"];
            string wordContent = Request.Form["Content"];

            wordContent = wordContent.Trim();

            var theWord = new Needs.Wl.Models.Views.PayExchangeSensitiveWordsView()[wordID];
            if (theWord == null)
            {
                theWord = new Needs.Wl.Models.PayExchangeSensitiveWord()
                {
                    ID = System.Guid.NewGuid().ToString("N"),
                    AreaID = areaID,
                    Content = wordContent,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                };
            }
            else
            {
                theWord.Content = wordContent;
                theWord.UpdateDate = DateTime.Now;
            }

            theWord.Enter();

            Response.Write((new { success = true, message = "保存成功" }).Json());
        }

        /// <summary>
        /// 删除关键词
        /// </summary>
        protected void DeleteWord()
        {
            string wordID = Request.Form["ID"];
            var theWord = new Needs.Wl.Models.Views.PayExchangeSensitiveWordsView()[wordID];
            theWord.Abandon();

            Response.Write((new { success = true, message = "删除成功" }).Json());
        }

    }
}