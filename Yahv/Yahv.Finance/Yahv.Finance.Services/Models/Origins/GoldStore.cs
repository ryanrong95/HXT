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
    public class GoldStore : IUnique
    {
        #region 事件

        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder AddSuccess;

        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder Updateuccess;

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
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 是否特殊
        /// </summary>
        public bool IsSpecial { get; set; }

        /// <summary>
        /// 所属人ID
        /// </summary>
        public string OwnerID { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 修改人ID
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
        /// 状态 启用、停用
        /// </summary>
        public GeneralStatus Status { get; set; }

        #endregion

        #region 其它属性

        /// <summary>
        /// 所属人姓名
        /// </summary>
        public string OwnerName { get; set; }

        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string CreatorName { get; set; }

        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                //添加
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.GoldStores>().Any(item => item.ID == this.ID))
                {
                    this.ID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.GoldStore);
                    var now = DateTime.Now;
                    reponsitory.Insert(new Layers.Data.Sqls.PvFinance.GoldStores()
                    {
                        ID = this.ID,
                        Name = this.Name,
                        Summary = this.Summary,
                        IsSpecial = this.IsSpecial,
                        OwnerID = this.OwnerID,
                        CreatorID = this.CreatorID,
                        ModifierID = this.CreatorID,
                        CreateDate = now,
                        ModifyDate = now,
                        Status = (int)GeneralStatus.Normal,
                    });

                    this.AddSuccess?.Invoke(this, new SuccessEventArgs(this));
                }
                //修改
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvFinance.GoldStores>(new
                    {
                        Name = this.Name,
                        Summary = this.Summary,
                        OwnerID = this.OwnerID,
                        ModifierID = this.ModifierID,
                        ModifyDate = DateTime.Now,
                    }, item => item.ID == this.ID);

                    this.Updateuccess?.Invoke(this, new SuccessEventArgs(this));
                }


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
                reponsitory.Update<Layers.Data.Sqls.PvFinance.GoldStores>(new
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
                reponsitory.Update<Layers.Data.Sqls.PvFinance.GoldStores>(new
                {
                    ModifyDate = DateTime.Now,
                    Status = (int)GeneralStatus.Closed,
                }, item => item.ID == this.ID);
            }
        }

    }
}
