using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Usually;

namespace Yahv.CrmPlus.Service.Models.Origins
{
    /// <summary>
    /// 样品明细
    /// </summary>
    public class SampleItem : Yahv.Linq.IUnique
    {
        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public string SampleID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SpnID { get; set; }


        /// <summary>
        /// 
        /// </summary>

        public SampleType SampleType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? Price { get; set; }

        public decimal? Total { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public DateTime ModifyDate { get; set; }

        public Yahv.Underly.AuditStatus AuditStatus { get; set; }

        public string Summary { get; set; }

        #region  拓展字段
        public string SpnName { get; set; }

        public string Brand { get; set; }
        #endregion


        #endregion
        public SampleItem()
        {

            this.CreateDate = this.ModifyDate = DateTime.Now;

        }
        #region 持久化
        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            {

                //添加
                if (!reponsitory.GetTable<Layers.Data.Sqls.PvdCrm.SampleItems>().Any(item => item.SampleID == this.SampleID&&item.SpnID==this.SpnID))
                {
                    this.ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.SampleIt);
                    reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.SampleItems()
                    {
                        ID = this.ID,
                        SampleID = this.SampleID,
                        SpnID = this.SpnID,
                        Type = (int)this.SampleType,
                        Quantity = this.Quantity,
                        Price = this.Price,
                        CreateDate = this.CreateDate,
                        ModifyDate = this.ModifyDate,
                        Status = (int)this.AuditStatus
                    });
                }
                //修改
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvdCrm.SampleItems>(new
                    {
                        SampleID = this.SampleID,
                        SpnID = this.SpnID,
                        Type = (int)this.SampleType,
                        Quantity = this.Quantity,
                        Price = this.Price,
                        CreateDate = this.CreateDate,
                        ModifyDate = this.ModifyDate,
                        Status = (int)this.AuditStatus

                    }, item => item.SampleID == this.SampleID && item.SpnID == this.SpnID);
                }

            }
            this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
        }

        #endregion

        #region 事件
        public event SuccessHanlder EnterSuccess;



        public event SuccessHanlder AbandonSuccess;
        /// <summary>
        /// EnterError
        /// </summary>

        public event ErrorHanlder EnterError;
        #endregion


    }
}

