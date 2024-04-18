using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 代理订单附件
    /// </summary>
    [Serializable]
    public class MainOrderFile : IUnique, IPersist, IFulError, IFulSuccess
    {
        #region 属性
        public string ID { get; set; }

        public string MainOrderID { get; set; }
     
        /// <summary>
        /// 后台管理员
        /// </summary>
        public Admin Admin { get; set; }

        public string AdminID { get; set; }

        public string UserID { get; set; }

        /// <summary>
        /// 平台用户
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public Enums.FileType FileType { get; set; }

        /// <summary>
        /// 中心文件类型
        /// </summary>
        public Needs.Ccs.Services.Models.ApiModels.Files.FileType Type { get; set; }
        /// <summary>
        /// 新加的ERMAdminID 
        /// </summary>
        public string ErmAdminID { get; set; }

        /// <summary>
        /// 文件格式
        /// </summary>
        public string FileFormat { get; set; }

        /// <summary>
        /// 文件地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 附件审核状态：待审核、已审核
        /// </summary>
        public Enums.OrderFileStatus FileStatus { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }

        /// <summary>
        /// 是否是旧数据0 是 1 否
        /// </summary>
        public int DataType { get; set; }
        #endregion

        public MainOrderFile()
        {
            this.FileStatus = Enums.OrderFileStatus.Auditing;
            this.Status = Enums.Status.Normal;
            this.CreateDate = DateTime.Now;
        }

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;
        public event SuccessHanlder DeleteSuccess;

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MainOrderFiles>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    //主键ID（OrderFile +8位年月日+6位流水号）
                    this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderFile);
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
            }

            this.OnEnterSuccess();
        }

        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.MainOrderFiles>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }

            this.OnAbandonSuccess();
        }

        public void Delete()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.MainOrderFiles>(item => item.ID == this.ID);
            }

            this.OnDeletedSuccess();
        }

        virtual protected void OnDeletedSuccess()
        {
            if(this!=null && this.DeleteSuccess != null)
            {
                this.DeleteSuccess(this,new SuccessEventArgs(this.ID));
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
