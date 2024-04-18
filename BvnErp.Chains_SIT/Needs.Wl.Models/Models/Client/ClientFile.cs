using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using System;
using System.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 客户文件
    /// </summary>
    public class ClientFile : ModelBase<Layer.Data.Sqls.ScCustoms.ClientFiles, ScCustomsReponsitory>, IUnique, IPersist
    {
        public string ClientID { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public Enums.FileType FileType { get; set; }

        /// <summary>
        /// 文件格式
        /// </summary>
        public string FileFormat { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 上传人
        /// </summary>
        public Admin Admin { get; set; }

        public ClientFile()
        {

        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public override void Enter()
        {
            var count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientFiles>().Where(item => item.ClientID == this.ClientID && item.FileType == (int)this.FileType && item.Status == (int)Enums.Status.Normal);

            if (count.Count() == 0)
            {
                this.ID = Guid.NewGuid().ToString("N");
                this.Reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ClientFiles
                {
                    ID = this.ID,
                    ClientID = this.ClientID,
                    AdminID = this.Admin.ID,
                    Name = this.Name,
                    FileType = (int)this.FileType,
                    FileFormat = this.FileFormat,
                    Url = this.Url,
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    Summary = this.Summary
                });
            }
            else
            {
                //存在有效附件
                var file = count.FirstOrDefault();

                //附件不一致
                if (file.Url != this.Url)
                {
                    //失效附件
                    file.Status = (int)Enums.Status.Delete;
                    this.Reponsitory.Update(file, t => t.ID == file.ID);

                    //新增附件
                    this.ID = Guid.NewGuid().ToString("N");
                    this.Reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ClientFiles
                    {
                        ID = this.ID,
                        ClientID = this.ClientID,
                        AdminID = this.Admin.ID,
                        Name = this.Name,
                        FileType = (int)this.FileType,
                        FileFormat = this.FileFormat,
                        Url = this.Url,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        Summary = this.Summary
                    });
                }
            }
            this.OnEnter();
        }

        virtual public void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 去持久化
        /// </summary>
        public override void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientFiles>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }
            this.OnAbandonSuccess();
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}