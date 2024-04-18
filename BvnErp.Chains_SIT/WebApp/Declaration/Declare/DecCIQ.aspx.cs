using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Declare
{
    public partial class DecCIQ : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected object getDropdownlist()
        {
            string value = Request.Form["value"];
            return Needs.Wl.Admin.Plat.AdminPlat.BaseOrgCodes.OrderBy(item => item.Code).Where(item => item.Code.Contains(value)).Take(10).Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name });
        }

        protected void LoadData()
        {
            this.Model.OrgCodes = Needs.Wl.Admin.Plat.AdminPlat.BaseOrgCodes.OrderBy(item => item.Code).Take(10).Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name }).Json();
            this.Model.CorrelationReason = Needs.Wl.Admin.Plat.AdminPlat.BaseCorrelationReason.OrderBy(item => item.Code).Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name }).Json();
            this.Model.OrigBoxFlag = "[{\"Value\":\"\" ,\"Text\":\" \"},{\"Value\":1,\"Text\":\"是\"},{\"Value\":0,\"Text\":\"否\"}]";
            

            string ID = Request.QueryString["ID"];
            this.Model.ID = ID;
            if (!string.IsNullOrEmpty(ID))
            {
                var head = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.DecHeads[ID];
                if (head != null)
                {
                    this.Model.DecHead = new
                    {
                        head.ID,
                        head.IsInspection,
                        head.IsQuarantine,
                        head.OrgCode,
                        head.VsaOrgCode,
                        head.InspOrgCode,
                        head.PurpOrgCode,
                        head.DespDate,
                        head.BLNo,
                        head.CorrelationNo,
                        head.CorrelationReasonFlag,
                        head.OrigBoxFlag,
                        head.SpecDeclFlag,
                        head.UseOrgPersonCode,
                        head.UseOrgPersonTel,
                        head.DomesticConsigneeEname,
                        head.OverseasConsignorCname,
                        head.OverseasConsignorAddr,
                        CmplDschrgDt = head.CmplDschrgDt==null?"":head.CmplDschrgDt.Insert(4,"-").Insert(7,"-"),
                        head.RequestCerts
                    }.Json();
                }
                else
                {
                    this.Model.DecHead = new { }.Json();
                }
                
            }
            else
            {
                this.Model.DecHead = "";
            }

        }

        /// <summary>
        /// 证书列表
        /// </summary>
        protected void data()
        {
            var appCerts = Needs.Wl.Admin.Plat.AdminPlat.BaseAppCertCode.OrderBy(item => item.Code);

            Func<Needs.Ccs.Services.Models.BaseAppCertCode, object> convert = item => new
            {
                AppCertCode = item.Code,
                AppCertCodeName = item.Name,
                ApplOri = 1,
                ApplCopyQuan = 2
            };

            Response.Write(new
            {
                rows = appCerts.Select(convert).ToArray(),
                total = appCerts.Count()
            }.Json());
        }

        /// <summary>
        /// 保存检验检疫信息
        /// </summary>
        protected void Save()
        {

            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            dynamic model = Model.JsonTo<dynamic>();

            var head = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.DecHeads[(string)model.ID];
            head.OrgCode = model.OrgCodeID;
            head.VsaOrgCode = model.VsaOrgCodeID;
            head.InspOrgCode = model.InspOrgCodeID;
            head.PurpOrgCode = model.PurpOrgCodeID;
            head.DespDate = ((string)model.DespDate).Replace("-", "");
            head.BLNo = model.BLNo;
            head.CorrelationNo = model.CorrelationNo;
            head.CorrelationReasonFlag = model.CorrelationReasonFlagID;
            head.OrigBoxFlag = model.OrigBoxFlagID;
            head.SpecDeclFlag = model.SpecDeclFlag;
            head.UseOrgPersonCode = model.UseOrgPersonCode;
            head.UseOrgPersonTel = model.UseOrgPersonTel;
            head.DomesticConsigneeEname = model.DomesticConsigneeEname;
            head.OverseasConsignorCname = model.OverseasConsignorCname;
            head.OverseasConsignorAddr = model.OverseasConsignorAddr;
            head.CmplDschrgDt = ((string)model.CmplDschrgDt).Replace("-", "");

            foreach (var cert in model.Cert)
            {
                var c = new Needs.Ccs.Services.Models.DecRequestCert {
                    DeclarationID = model.ID,
                    AppCertCode = cert.AppCertCode,
                    ApplOri = cert.ApplOri,
                    ApplCopyQuan = cert.ApplCopyQuan
                };
                c.Enter();
            }

            head.EnterError += DecHead_EnterError;
            head.EnterSuccess += DecHead_EnterSuccess;
            head.Enter();
        }

        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecHead_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write(new { success = false, message = e.Message });
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecHead_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功", ID = e.Object }).Json());
        }
    }
}