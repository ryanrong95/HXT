//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using Yahv.Web.Forms;
//using YaHv.Csrm.Services;
//using YaHv.Csrm.Services.Models.Origins;
//using YaHv.Csrm.Services.Views.Rolls;

//namespace Yahv.Csrm.WebApp.Cbrm.Customsbrokers
//{
//    public partial class Edit : BasePage
//    {
//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (!IsPostBack)
//            {
//                var id = Request.QueryString["id"];
//                var entity = new CustomsbrokersRoll()[id];
//                //下拉
//                //级别
//                selGrade.DataSource = Underly.ExtendsEnum.ToDictionary<Grade>();
//                selGrade.DataTextField = "Value";
//                selGrade.DataValueField = "Key";
//                selGrade.DataBind();
//                if (entity != null)
//                {
//                    this.selGrade.Value = ((int)entity.Grade).ToString();
//                    this.chbOwn.Checked = entity.IsOwn;
//                    this.Model = new
//                    {
//                        entity.ID,
//                        entity.AdminCode,
//                        entity.Name,
//                        entity.DyjCode,
//                        entity.IsOwn,
//                        entity.Grade
//                    };
//                }
//            }
//        }


//        protected void btnSubmit_Click(object sender, EventArgs e)
//        {
//            var entity = new Customsbroker();
//            entity.Name = Request.Form["Name"];
//            entity.Grade = (Grade)int.Parse(selGrade.Value);
//            entity.DyjCode = Request.Form["DyjCode"].Trim();
//            entity.AdminCode = Request.Form["AdminCode"];
//            entity.IsOwn = chbOwn.Checked;
//            entity.EnterSuccess += Clients_EnterSuccess;
//            entity.Enter();
//            //if (!string.IsNullOrEmpty(id))
//            //{
//            //    InquiryID = id;
//            //    model.UpdateSubmit(); //修改并且提交
//            //    Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
//            //                           nameof(Yahv.Systematic.RFQ),
//            //                           Yahv.RFQ.Services.OplogType.InquiryUpdateSubmit.ToString(), "修改询价并提交：" + id, "");

//            //}
//            //else
//            //{
//            //    model.IsFocus = false;
//            //    InquiryID = model.Save();
//            //    Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
//            //                          nameof(Yahv.Systematic.RFQ),
//            //                          Yahv.RFQ.Services.OplogType.InquiryInsertSubmit.ToString(), "新增询价并提交", "");

//            //}

//        }
//        private void Clients_EnterSuccess(object sender, Usually.SuccessEventArgs e)
//        {
//            Easyui.Reload("提示", "保存成功", Yahv.Web.Controls.Easyui.Sign.Info);
//        }
//    }
//}