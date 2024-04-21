using System;
using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Usually;

namespace Yahv.Erm.Services.Models.Origins
{
    /// <summary>
    /// 月账单
    /// </summary>
    public class PayBill : IUnique
    {
        #region 事件

        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// EnterError
        /// </summary>
        public event ErrorHanlder EnterError;

        /// <summary>
        /// AbandonSuccess
        /// </summary>
        public event SuccessHanlder AbandonSuccess;

        #endregion

        #region 属性
        /// <summary>
        /// 唯一码 ：'员工编号'+ '-' + [DateIndex] 
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 员工ID
        /// </summary>
        public string StaffID { get; set; }

        /// <summary>
        /// 封账日期
        /// </summary>
        public DateTime? ClosedData { get; set; }

        /// <summary>
        /// 日期序数(例：201901 (表月份))
        /// </summary>
        public string DateIndex { get; set; }

        /// <summary>
        /// 所属公司ID
        /// </summary>
        public string EnterpriseID { get; set; }

        /// <summary>
        /// 岗位ID
        /// </summary>
        public string PostionID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreaetDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? UpdateDate { get; set; }

        /// <summary>
        /// 状态(考核,封账,已发放)
        /// </summary>
        public PayBillStatus Status { get; set; }

        /// <summary>
        /// 员工编码
        /// </summary>
        public string StaffCode { get; set; }

        #endregion

        #region 持久化
        /// <summary>
        /// 添加/修改
        /// </summary>
        public void Enter()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                //添加
                if (!repository.ReadTable<PayBills>().Any(a => a.ID == this.ID))
                {
                    this.ID = this.StaffCode + "-" + this.DateIndex;

                    repository.Insert(new PayBills()
                    {
                        Status = (int)(PayBillStatus.Check),
                        ID = this.ID,
                        StaffID = this.StaffID,
                        ClosedData = this.ClosedData,
                        CreaetDate = DateTime.Now,
                        DateIndex = this.DateIndex,
                        PostionID = this.PostionID,
                        EnterpriseID = this.EnterpriseID,
                    });
                }
                else
                {
                    repository.Update<PayBills>(new
                    {
                        UpdateDate = DateTime.Now,
                        ClosedData = this.ClosedData,
                        Status = (int)this.Status,
                    }, a => a.ID == this.ID);
                }


                //操作成功
                if (this != null && EnterSuccess != null)
                    this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        /// <summary>
        /// 封账
        /// </summary>
        public void Closed(string dateIndex)
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                repository.Command($"update PayBills set Status={(int)PayBillStatus.Closed} where dateIndex='{dateIndex}'");
                //repository.Update<Layers.Data.Sqls.PvbErm.PayBills>(new
                //{
                //    Status = PayBillStatus.Closed
                //}, item => item.DateIndex == dateIndex);
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }


        #endregion
    }
}