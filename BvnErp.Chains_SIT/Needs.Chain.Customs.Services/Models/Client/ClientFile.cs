using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 客户文件
    /// </summary>
    public class ClientFile : IUnique, IPersist
    {
        public string ID { get; set; }
        public string ClientID { get; set; }

        string adminID;
        public string AdminID
        {
            get
            {
                return this.adminID ?? this.Admin?.ID;
            }
            set
            {
                this.adminID = value;
            }
        }

        public Admin Admin { get; set; }

        public string Name { get; set; }

        public Enums.FileType FileType { get; set; }

        public string FileFormat { get; set; }

        public string Url { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }

        public ClientFile()
        {
            this.CreateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientFiles>().Where(item => item.ClientID == this.ClientID && item.FileType == (int)this.FileType && item.Status == (int)Enums.Status.Normal);

                if (count.Count() == 0)
                {
                    //this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.Form);
                    this.ID = Guid.NewGuid().ToString("N");
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ClientFiles
                    {
                        ID = this.ID,
                        ClientID = this.ClientID,
                        AdminID = this.AdminID,
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
                        reponsitory.Update(file, t => t.ID == file.ID);

                        //新增附件
                        this.ID = Guid.NewGuid().ToString("N");
                        reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ClientFiles
                        {
                            ID = this.ID,
                            ClientID = this.ClientID,
                            AdminID = this.AdminID,
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
        public void Abandon()
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
