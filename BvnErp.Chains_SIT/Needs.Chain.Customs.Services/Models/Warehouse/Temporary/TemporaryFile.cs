using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 暂存文件
    /// </summary>
    public class TemporaryFile : IUnique, IPersist, IFulError, IFulSuccess
    {
        public string ID { get; set; }
        /// <summary>
        /// 暂存通知
        /// </summary>
        public string TemporaryID { get; set; }
       
        public string AdminID { get; set; } 
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 附件类型:营业执照、授权委托书等
        /// </summary>
        public Enums.FileType FileType { get; set;}
        /// <summary>
        /// 附件格式
        /// </summary>
        public string FileFormat { get; set; }

        /// <summary>
        /// 文件地址
        /// </summary>
        public string URL { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        public TemporaryFile()
        {
            this.CreateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
        }
        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.TemporaryFiles>().Count(item => item.ID == this.ID);

                    if (count == 0)
                    {
                        this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.TemporaryFile);
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
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.TemporaryFiles>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }
            this.OnAbandonSuccess();
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

    public class TemporaryFiles : BaseItems<TemporaryFile>
    {
        public TemporaryFiles(IEnumerable<TemporaryFile> enums) : base(enums)
        {
        }

        internal TemporaryFiles(IEnumerable<TemporaryFile> enums, Action<TemporaryFile> action) : base(enums, action)
        {
        }

        public override void Add(TemporaryFile item)
        {
            base.Add(item);
        }
    }
}