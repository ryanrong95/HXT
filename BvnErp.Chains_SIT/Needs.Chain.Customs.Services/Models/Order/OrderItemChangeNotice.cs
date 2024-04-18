using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Linq;
using Needs.Ccs.Services.Enums;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 订单项产品变更通知
    /// </summary>
    public class OrderItemChangeNotice : IUnique, IPersist, IFulError, IFulSuccess
    {
        #region 属性
        private string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.OrderItemID, this.Type, this.TriggerSource).MD5();
            }
            set { id = value; }
        }

        public string OrderItemID { get; set; }
        public string OrderID { get; set; }
        /// <summary>
        /// 库房分拣员（变更产地/品牌）
        /// </summary>
        public string SorterID { get; set; }
        public Admin Sorter { get; set; }
      
        public string ProductID { get; set; }
        public string ClientCode { get; set; }
        public string CompanyName { get; set; }

        public string ProductName { get; set; }

        public string ProductModel { get; set; }


        /// <summary>
        /// 20190826新添加的字段
        /// </summary>
        public string OldValue { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public  string NewValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsSplited { get; set; }
        
        /// <summary>
        /// 品牌变更/产地变更
        /// </summary>
        public OrderItemChangeType Type { get; set; }
        /// <summary>
        /// 已处理/未处理
        /// </summary>
        public ProcessState ProcessState { get; set; }
        public Enums.Status Status { get; set; }

        /// <summary>
        /// 当前归类是否已经锁定
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// 锁定人
        /// </summary>
        public Admin Locker { get; set; }

        /// <summary>
        /// 锁定时间
        /// </summary>
        public DateTime? LockDate { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 引发来源
        /// </summary>
        public TriggerSource TriggerSource { get; set; }


        #endregion

        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;

        public OrderItemChangeNotice()
        {
            this.IsSplited = false;
            this.Status = Enums.Status.Normal;
           // this.EnterSuccess += OrderItemChange_Log;
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //产地和品牌
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemChangeNotices>()
                    .Count(item => item.OrderItemID == this.OrderItemID && item.Type == (int)this.Type && item.TriggerSource == (int)this.TriggerSource);

                if (count == 0)
                {
                    this.CreateDate = DateTime.Now;
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    //分拣 更新未处理 分拣人变更
                    if (this.ProcessState == ProcessState.UnProcess)
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItemChangeNotices>(
                            new {ProcessStatus = (int) ProcessState.UnProcess,NewValue=this.NewValue, UpdateDate = DateTime.Now, AdminID =this.Sorter.ID},
                            item => item.ID == this.ID);
                    }
                    else
                    {//归类更新成已处理  sorter 不更新
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItemChangeNotices>(
                            new { ProcessStatus = (int)ProcessState.Processed, UpdateDate = DateTime.Now },
                            item => item.ID == this.ID);
                        //reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                    }
                   
                }
            }
        }


        virtual protected void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        /// <summary>
        /// 去持久化
        /// </summary>
        public virtual void Abandon()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItemChangeNotices>(
                        new
                        {
                            UpdateDate = DateTime.Now,
                            Status = Enums.Status.Delete
                        }, item => item.ID == this.ID);
                }
                this.OnAbandonSuccess();
            }
            catch (Exception ex)
            {
                this.AbandonError(this, new ErrorEventArgs(ex.Message));
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
        /// <summary>
        /// 成功之后做日志记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void OrderItemChange_Log(object sender, SuccessEventArgs e)
        //{
        //    var orderItemChange = (OrderItemChangeNotice)e.Object;
        //    orderItemChange.Log(orderItemChange.Sorter, "[" + orderItemChange.Sorter.RealName + "]做了+["+ (Enums.OrderItemChangeType)orderItemChange.Type+"]的操作！");

        //}
    }
}
