using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Yahv.Erm.Services.Common;
using Yahv.Erm.Services.Extends;
using Yahv.Erm.Services.Models.Rolls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Erm.WebApp.Erm_KQ.Application_Resignation
{
    public partial class Approval_HR : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitData();
            }
        }

        #region 加载数据
        /// <summary>
        /// 加载数据
        /// </summary>
        private void InitData()
        {
            string id = Request.QueryString["id"];
            var entity = JsonConvert.DeserializeObject<Resignation>(Erp.Current.Erm.Applications[id].Context);
            entity.ID = id;
            this.Model.Data = entity;
            this.Model.FileType = ExtendsEnum.ToArray<FileType>().Select(item => new { value = (int)item, text = item.GetDescription() }).Where(item => item.value > 2200 && item.value < 2300 || item.value == 2003);
        }

        /// <summary>
        /// 加载文件
        /// </summary>
        /// <returns></returns>
        protected object LoadFile()
        {
            string ApplicationID = Request.QueryString["id"];
            var files = new Services.Views.ApplicationFileAlls(ApplicationID).AsEnumerable();
            var linq = files.Select(t => new
            {
                ID = t.ID,
                CustomName = t.CustomName,
                FileTypeDec = ((FileType)t.Type).GetDescription(),
                Type = t.Type,
                CreateDate = t.CreateDate?.ToString("yyyy-MM-dd"),
                Creater = t.AdminID,
                Url = FileDirectory.ServiceRoot + t.Url,
            });
            return linq;
        }

        /// <summary>
        /// 审批日志
        /// </summary>
        /// <returns></returns>
        protected object LoadLogs()
        {
            var id = Request.QueryString["ID"];
            var logs = Erp.Current.Erm.Logs_ApplyVoteSteps.Where(item => item.ApplicationID == id);

            var data = logs.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                AdminID = item.Admin.RealName,
                Status = item.Status.GetDescription(),
                Summary = item.Summary,
                item.VoteStepName,
            });
            return new
            {
                rows = data,
                total = data.Count(),
            };
        }
        #endregion

        #region 功能函数

        /// <summary>
        /// 审批通过
        /// </summary>
        /// <returns></returns>
        protected object Submit()
        {
            JMessage json = new JMessage() { success = true, data = "审批成功!" };

            try
            {
                string id = Request.Form["ID"];
                string Argument = Request.Form["Argument"];
                DateTime resignationDate = Convert.ToDateTime(Request.Form["ResignationDate"]);

                //文件信息
                var files = Request.Form["files"].Replace("&quot;", "'").Replace("amp;", "");
                var fileList = files.JsonTo<List<Yahv.Services.Models.CenterFileDescription>>();

                if (fileList.All(item => item.Type != (int)FileType.ResignationApplication))
                {
                    json.success = false;
                    json.data = "请您上传[离职申请表]附件!";
                    return json;
                }
                if (fileList.All(item => item.Type != (int)FileType.ResignationHandover))
                {
                    json.success = false;
                    json.data = "请您上传[离职交接表]附件!";
                    return json;
                }
                //审批日期不能早于当天
                if (resignationDate.Date > DateTime.Now.Date)
                {
                    json.success = false;
                    json.data = "您不能在离职日期之前审批!";
                    return json;
                }

                var entity = Erp.Current.Erm.ApplicationsRoll[id];
                entity.Fileitems = fileList;

                //离职申请信息
                var model = entity.Context.JsonTo<Resignation>();
                model.ResignationDate = DateTime.Parse(Request.Form["ResignationDate"]);

                entity.Context = model.Json();
                entity.Enter();

                //审批通过
                entity.CurrentVoteStep.Status = Services.ApprovalStatus.Agree;
                entity.CurrentVoteStep.Summary = Argument;
                entity.Approval(true);
            }
            catch (Exception ex)
            {
                json.success = false;
                json.data = $"提交异常!{ex.Message}";
            }
            return json;
        }

        /// <summary>
        /// 驳回
        /// </summary>
        /// <returns></returns>
        protected object Reject()
        {
            JMessage json = new JMessage() { success = true, data = "驳回成功!" };

            try
            {
                string id = Request.Form["ID"];
                string Argument = Request.Form["Argument"];

                //文件信息
                var files = Request.Form["files"].Replace("&quot;", "'").Replace("amp;", "");
                var fileList = files.JsonTo<List<Yahv.Services.Models.CenterFileDescription>>();

                var entity = Erp.Current.Erm.ApplicationsRoll[id];
                entity.Fileitems = fileList;

                //离职申请信息
                var model = entity.Context.JsonTo<Resignation>();
                model.ResignationDate = DateTime.Parse(Request.Form["ResignationDate"]);
                entity.Context = model.Json();
                entity.Enter();


                //审批通过
                entity.CurrentVoteStep.Status = Services.ApprovalStatus.Reject;
                entity.CurrentVoteStep.Summary = Argument;
                entity.Approval(false);
            }
            catch (Exception ex)
            {
                json.success = false;
                json.data = $"提交异常!{ex.Message}";
            }
            return json;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        protected void UploadFile()
        {
            try
            {
                string fileType = Request.Form["fileType"];

                List<dynamic> fileList = new List<dynamic>();
                IList<HttpPostedFile> files = HttpContext.Current.Request.Files.GetMultiple("uploadFile");
                if (files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        //处理附件
                        HttpPostedFile file = files[i];
                        if (file.ContentLength != 0)
                        {
                            FileDirectory dic = new FileDirectory(file.FileName, (FileType)Enum.Parse(typeof(FileType), fileType));
                            dic.AdminID = Erp.Current.ID;
                            dic.Save(file);
                            fileList.Add(new
                            {
                                ID = dic.uploadResult.FileID,
                                CustomName = dic.FileName,
                                FileName = dic.uploadResult.FileName,
                                FileType = dic.FileType,
                                FileTypeDec = dic.FileType.GetDescription(),
                                Url = dic.uploadResult.Url,
                            });
                        }
                    }
                }
                Response.Write((new { success = true, data = fileList }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导入失败：" + ex.Message }).Json());
            }
        }
        #endregion
    }
}