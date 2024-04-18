using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.AdvanceMoney.AdvanceMoneyApply
{
    public partial class Add : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 提交
        /// </summary>
        protected void Submit()
        {
            try
            {
                string applyID = Guid.NewGuid().ToString("N");
                string clientID = Request.Form["ClientID"];
                string clientCode = Request.Form["ClientCode"];
                string clientName = Request.Form["ClientName"];
                string advanceAmount = Request.Form["AdvanceAmount"];
                string limitDay = Request.Form["LimitDay"];
                string interestRate = Request.Form["InterestRate"];
                string overdueInterestRate = Request.Form["OverdueInterestRate"];
                string files = Request.Form["Files"].Replace("&quot;", "'");
                string summary = Request.Form["Summary"];

                var FileList = files.JsonTo<List<dynamic>>();

                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                foreach (var item in FileList)
                {
                    #region 关联付汇委托书上传中心

                    var entity = new CenterFileDescription();
                    entity.AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                    entity.ApplicationID = applyID;
                    entity.Type = 1;
                    entity.Url = Convert.ToString(item.VirtualPath).Replace(@"/", @"\");
                    entity.Status = FileDescriptionStatus.Normal;
                    entity.CreateDate = DateTime.Now;
                    entity.CustomName = item.Name;

                    DateTime liunxStart = new DateTime(1970, 1, 1);
                    var linuxtime = (DateTime.Now - liunxStart).Ticks;
                    string topID = "F" + linuxtime;

                    new CenterFilesTopView().Insert(entity, topID);

                    #endregion
                }

                //保存垫资申请
                Needs.Ccs.Services.Models.AdvanceMoneyApplyModel advanceMoneyApplyFiles = new Needs.Ccs.Services.Models.AdvanceMoneyApplyModel
                {
                    ID = applyID,
                    ClientID = clientID,
                    Amount = Convert.ToDecimal(advanceAmount),
                    AmountUsed = 0,
                    Status = Needs.Ccs.Services.Enums.AdvanceMoneyStatus.RiskAuditing,
                    LimitDays = Convert.ToInt32(limitDay),
                    InterestRate = Convert.ToDecimal(interestRate),
                    OverdueInterestRate = Convert.ToDecimal(overdueInterestRate),
                    AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = summary,
                };

                Needs.Ccs.Services.Models.AdvanceMoneyApplyLogModel advanceMoneyApplyLogs = new Needs.Ccs.Services.Models.AdvanceMoneyApplyLogModel()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    ApplyID = applyID,
                    Status = Needs.Ccs.Services.Enums.AdvanceMoneyStatus.RiskAuditing,
                    AdminID = admin.ID,
                    CreateDate = DateTime.Now,
                    Summary = "业务员【" + admin.RealName + "】提交了垫资申请；备注：" + summary,
                };

                //保存 Begin

                advanceMoneyApplyFiles.Enter();

                advanceMoneyApplyLogs.Enter();

                //保存 End

                Response.Write((new { success = true, message = "提交成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "提交失败：" + ex.Message }).Json());
            }
        }

        //上传文件
        protected void UploadFile()
        {
            try
            {
                List<dynamic> fileList = new List<dynamic>();
                IList<HttpPostedFile> files = System.Web.HttpContext.Current.Request.Files.GetMultiple("uploadFile");
                if (files.Count > 0 )
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        //处理附件
                        HttpPostedFile file = files[i];
                        if (file.ContentLength != 0)
                        {
                            //文件保存
                            string fileName = files[i].FileName.ReName();

                            //创建文件目录
                            FileDirectory fileDic = new FileDirectory(fileName);
                            fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.AdvanceMoney);
                            fileDic.CreateDataDirectory();
                            file.SaveAs(fileDic.FilePath);

                            fileList.Add(new
                            {
                                Name = file.FileName,
                                FileType = FileType.OriginalInvoice.GetDescription(),
                                FileFormat = file.ContentType,
                                VirtualPath = fileDic.VirtualPath,
                                Url = fileDic.FileUrl,
                            });
                        }
                    }
                }
                if (fileList.Count() == 0)
                {
                    Response.Write((new { success = false, data = new { } }).Json());
                }
                else
                {
                    Response.Write((new { success = true, data = fileList }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "上传失败：" + ex.Message }).Json());
            }
        }

        //验证客户编号
        protected void CheckClientCode()
        {
            try
            {
                string ClientCode = Request.Form["ClientCode"];
                if (!string.IsNullOrEmpty(ClientCode))
                {
                    var clientIds = new ClientAdminsView().Where(t => t.Type == ClientAdminType.ServiceManager && t.Admin.ID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID && t.Type!=ClientAdminType.StorageServiceManager).Select(t => t.ClientID).ToList();

                    var client = new CheckClientCodeView().Where(t => t.ClientCode == ClientCode && clientIds.Contains(t.ClientID)).Select(t => new { t.ClientName, t.ClientID }).FirstOrDefault();
                    if (client == null)
                    {
                        //当前报关客户不是有效客户，请重新输入！
                        Response.Write((new { success = false, message = "当前报关客户不是有效客户，请重新输入！" }).Json());
                    }
                    else
                    {
                        var clientName = new CheckClientIDView().Where(t => t.ClientID == client.ClientID).FirstOrDefault();

                        if (clientName != null)
                        {
                            Response.Write((new { success = false, message = "当前报关客户存在非【作废】状态垫资申请,不能再次申请！" }).Json());
                        }
                        else
                        {
                            Response.Write((new { success = true, data = client, message = "提交成功" }).Json());
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "验证失败：" + ex.Message }).Json());
            }
        }
    }
}