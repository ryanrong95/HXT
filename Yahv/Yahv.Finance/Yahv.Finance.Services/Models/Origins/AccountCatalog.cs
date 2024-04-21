using System;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Usually;

namespace Yahv.Finance.Services.Models.Origins
{
    /// <summary>
    /// 收付款类型
    /// </summary>
    public class AccountCatalog : IUnique
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

        #region 属性
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        public string FatherID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

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

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                //添加
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.AccountCatalogs>().Any(item => item.ID == this.ID))
                {
                    this.ID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.AccCatType);
                    reponsitory.Insert(new Layers.Data.Sqls.PvFinance.AccountCatalogs()
                    {
                        ID = this.ID,
                        Name = this.Name,
                        FatherID = this.FatherID,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                        CreatorID = this.CreatorID,
                        ModifierID = this.CreatorID,
                        Status = (int)GeneralStatus.Normal,
                    });
                }
                //修改
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvFinance.AccountCatalogs>(new
                    {
                        Name = this.Name,
                        ModifyDate = DateTime.Now,
                        ModifierID = this.ModifierID,
                        Status = (int)this.Status,
                    }, item => item.ID == this.ID);
                }

                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        public void Abandon(string id)
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                if (reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.AccountCatalogs>().Any(item => item.ID == id))
                {
                    reponsitory.Update<Layers.Data.Sqls.PvFinance.AccountCatalogs>(new
                    {
                        Status = GeneralStatus.Closed,
                    }, item => item.ID == id);
                }
            }
        }
        #endregion
    }
}
