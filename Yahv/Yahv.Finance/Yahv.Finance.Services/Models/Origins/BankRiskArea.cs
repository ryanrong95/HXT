using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;

namespace Yahv.Finance.Services.Models.Origins
{
    public class BankRiskArea : IUnique
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

        #endregion

        #region 数据库属性

        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 银行ID
        /// </summary>
        public string BankID { get; set; }

        /// <summary>
        /// 国家及地区
        /// </summary>
        public string District { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string ModifierID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { get; set; }

        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                //添加
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.BankRiskAreas>().Any(item => item.ID == this.ID))
                {
                    this.ID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.BankRiskArea);
                    var now = DateTime.Now;
                    reponsitory.Insert(new Layers.Data.Sqls.PvFinance.BankRiskAreas()
                    {
                        ID = this.ID,
                        BankID = this.BankID,
                        District = this.District,
                        CreatorID = this.CreatorID,
                        ModifierID = this.ModifierID,
                        CreateDate = now,
                        ModifyDate = now,
                        Status = (int)GeneralStatus.Normal,
                    });
                }
                //修改
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvFinance.BankRiskAreas>(new
                    {
                        BankID = this.BankID,
                        District = this.District,
                        ModifierID = this.ModifierID,
                        ModifyDate = DateTime.Now,
                    }, item => item.ID == this.ID);
                }

                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }

        #endregion

        /// <summary>
        /// 启用
        /// </summary>
        public void Enable()
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                reponsitory.Update<Layers.Data.Sqls.PvFinance.BankRiskAreas>(new
                {
                    ModifyDate = DateTime.Now,
                    Status = (int)GeneralStatus.Normal,
                }, item => item.ID == this.ID);
            }
        }

        /// <summary>
        /// 停用
        /// </summary>
        public void Disable()
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                reponsitory.Update<Layers.Data.Sqls.PvFinance.BankRiskAreas>(new
                {
                    ModifyDate = DateTime.Now,
                    Status = (int)GeneralStatus.Closed,
                }, item => item.ID == this.ID);
            }
        }

    }
}
