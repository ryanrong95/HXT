//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using Yahv.Linq.Extends;
//using Yahv.Underly;
//using YaHv.Csrm.Services;
//using YaHv.Csrm.Services.Models.Origins;
//using YaHv.Csrm.Services.Views.Rolls;

//namespace Yahv.Csrm.WebApp.Cbrm.Customsbrokers
//{
//    /// <summary>
//    /// 报关公司列表
//    /// </summary>
//    public partial class List : BasePage
//    {
//        protected void Page_Load(object sender, EventArgs e)
//        {

//        }
//        protected object data()
//        {
//            Expression<Func<Customsbroker, bool>> predicate = item => true;
//            var name = Request["s_name"];
//            if (!string.IsNullOrWhiteSpace(name))
//            {
//                predicate = predicate.And(item => item.Name.Contains(name) || item.AdminCode.Contains(name) || item.DyjCode.Contains(name));
//            }
//            var query = new CustomsbrokersRoll().Where(predicate);
//            return new
//            {
//                rows = query.OrderBy(item => item.Name).ToArray().Select(item => new
//                {
//                    item.ID,
//                    item.Name,
//                    item.AdminCode,
//                    item.Grade,
//                    item.DyjCode,
//                    IsOwn = item.IsOwn ? "是" : "否"
//                })
//            };
//        }

//        protected void del()
//        {
//            var id = Request.Form["id"];
//            var entity = new CustomsbrokersRoll()[id];
//            entity.Abandon();
//        }
//    }
//}