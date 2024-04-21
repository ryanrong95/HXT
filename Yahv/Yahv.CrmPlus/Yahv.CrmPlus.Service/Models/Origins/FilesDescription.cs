using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.CrmPlus.Service.Models.Origins
{
    public class CallFile
    {
        public string FileName { set; get; }
        string url { set; get; }
        public string CallUrl
        {
            get { return this.url; }
            set
            {
                this.url = value.Replace(Models.FileHost.Web, "").Trim();
            }
        }
    }
    public class FilesDescription : Yahv.Linq.IUnique
    {
        public FilesDescription()
        {
            this.Status = DataStatus.Normal;
            this.CreateDate = DateTime.Now;
        }
        public string ID { get; set; }

        public string EnterpriseID { get; set; }
        public string SubID { set; get; }
        /// <summary>
        /// 文件类型
        /// </summary>

        public CrmFileType Type { set; get; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string CustomName { set; get; }
        /// <summary>
        /// url
        /// </summary>
        public string Url { set; get; }
        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { set; get; }
        /// <summary>
        /// 创建人真实姓名
        /// </summary>
        public string CreatorName { internal set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { internal set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public DataStatus Status { set; get; }


        #region 持久化
        public void Enter()
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                if (this.Type == CrmFileType.Logo)
                {
                    repository.Update<Layers.Data.Sqls.PvdCrm.FilesDescription>(new
                    {
                        Status = (int)DataStatus.Closed
                    }, item => item.EnterpriseID == this.EnterpriseID);

                }
                repository.Insert(new Layers.Data.Sqls.PvdCrm.FilesDescription
                {
                    ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.File),
                    EnterpriseID = this.EnterpriseID,
                    SubID = this.SubID,
                    CustomName = this.CustomName,
                    Url = this.Url,
                    Summary = this.Summary,
                    Type = (int)this.Type,
                    CreateDate = this.CreateDate,
                    CreatorID = this.CreatorID,
                    Status = (int)this.Status,
                });
            }
        }
        #endregion
    }
}
