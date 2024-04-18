using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ExitNoticeFile : IUnique, IPersist, IFulError, IFulSuccess
    {
        public string ID { get; set; }

        public string ExitNoticeID { get; set; }

        public Admin Admin { get; set; }

        public string AdminID { get; set; } = string.Empty;

        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; }

        public Enums.FileType FileType { get; set; }

        public string FileFormat { get; set; }

        /// <summary>
        /// 文件地址
        /// </summary>
        public string URL { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }

        public ExitNoticeFile()
        {
            this.CreateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
        }

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNoticeFiles>().Count(item => item.ID == this.ID);
                    if (count == 0)
                    {
                        this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.ExitNoticeFile);
                        reponsitory.Insert(this.ToLinq());
                    }
                    else
                    {
                        reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                    }
                }
                this.OnEnterSuccess();
            }
            catch (Exception ex)
            {
                this.EnterError(this, new ErrorEventArgs(ex.Message));
            }
        }

        /// <summary>
        /// 去持久化
        /// </summary>
        public void Abandon()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.ExitNoticeFiles>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
                }
                this.OnAbandonSuccess();
            }
            catch (Exception ex)
            {
                this.AbandonError(this, new ErrorEventArgs(ex.Message));
            }
        }

        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
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