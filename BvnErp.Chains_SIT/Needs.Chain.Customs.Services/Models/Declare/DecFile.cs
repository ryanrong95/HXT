using Needs.Ccs.Services.Enums;
using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 报关单附件
    /// </summary>
    [Serializable]
    public class DecFile : IUnique, IPersist
    {
        #region 属性
        private string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.DecHeadID, this.FileType, this.Url).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        public string DecHeadID { get; set; }

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

        public int  FileTypeInt { get; set; }

        public FileType FileType { get; set; }

        public string FileFormat { get; set; }

        public string Url { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }
        #endregion

        public DecFile()
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
            this.OnEnter();

            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        virtual public void OnEnter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //var count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeadFiles>().Where(item => item.DecHeadID == this.DecHeadID && item.FileType == this.FileTypeInt);
                //if (count.Count() == 0)
                //{
                //    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.DecHeadFiles
                //    {
                //        ID = this.ID,
                //        DecHeadID = this.DecHeadID,
                //        AdminID = this.AdminID,
                //        Name = this.Name,
                //        FileType = this.FileTypeInt,
                //        FileFormat = this.FileFormat,
                //        Url = this.Url,
                //        Status = (int)this.Status,
                //        CreateDate = this.CreateDate,
                //        Summary = this.Summary
                //    });
                //}

                //允许重复上传
                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.DecHeadFiles
                {
                    ID = this.ID,
                    DecHeadID = this.DecHeadID,
                    AdminID = this.AdminID,
                    Name = this.Name,
                    FileType = this.FileTypeInt,
                    FileFormat = this.FileFormat,
                    Url = this.Url,
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    Summary = this.Summary
                });
            }
        }
    }
}