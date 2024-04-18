using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class CostApplyFile : IUnique, IPersistence, IFulError, IFulSuccess
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// CostApplyID
        /// </summary>
        public string CostApplyID { get; set; } = string.Empty;

        /// <summary>
        /// AdminID
        /// </summary>
        public string AdminID { get; set; } = string.Empty;

        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 文件类型（单据等）
        /// </summary>
        public Enums.CostApplyFileTypeEnum FileType { get; set; }

        /// <summary>
        /// 文件格式
        /// </summary>
        public string FileFormat { get; set; } = string.Empty;

        /// <summary>
        /// URL
        /// </summary>
        public string URL { get; set; } = string.Empty;

        /// <summary>
        /// Status
        /// </summary>
        public Enums.Status Status { get; set; }

        /// <summary>
        /// CreateDate
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Summary
        /// </summary>
        public string Summary { get; set; }



        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.CostApplyFiles>(
                    new
                    {
                        Status = (int)Enums.Status.Delete
                    }, item => item.ID == this.ID);
            }
            this.OnAbandonSuccess();
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CostApplyFiles>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.CostApplyFiles>(new Layer.Data.Sqls.ScCustoms.CostApplyFiles
                    {
                        ID = this.ID,
                        CostApplyID = this.CostApplyID,
                        AdminID = this.AdminID,
                        Name = this.Name,
                        FileType = (int)this.FileType,
                        FileFormat = this.FileFormat,
                        URL = this.URL,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        Summary = this.Summary,
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.CostApplyFiles>(new
                    {
                        CostApplyID = this.CostApplyID,
                        AdminID = this.AdminID,
                        Name = this.Name,
                        FileType = (int)this.FileType,
                        FileFormat = this.FileFormat,
                        URL = this.URL,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        Summary = this.Summary,
                    }, item => item.ID == this.ID);
                }
            }
            this.OnEnterSuccess();
        }

        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

    }
}
