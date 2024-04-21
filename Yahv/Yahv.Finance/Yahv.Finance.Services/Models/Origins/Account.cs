using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Finance.Services.Enums;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Usually;

namespace Yahv.Finance.Services.Models.Origins
{
    public class Account : IUnique
    {
        #region 事件

        /// <summary>
        /// AddSuccess
        /// </summary>
        public event SuccessHanlder AddSuccess;

        /// <summary>
        /// UpdateSuccess
        /// </summary>
        public event SuccessHanlder UpdateSuccess;

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
        /// 户名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// 帐号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 账户性质
        /// </summary>
        public NatureType NatureType { get; set; }

        /// <summary>
        /// 管理类型
        /// </summary>
        public ManageType ManageType { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 开户行
        /// </summary>
        public string OpeningBank { get; set; }

        /// <summary>
        /// 开户行地址
        /// </summary>
        public string BankAddress { get; set; }

        /// <summary>
        /// 国家及地区
        /// </summary>
        public string District { get; set; }

        /// <summary>
        /// SwiftCode
        /// </summary>
        public string SwiftCode { get; set; }

        /// <summary>
        /// 开户时间
        /// </summary>
        public DateTime? OpeningTime { get; set; }

        /// <summary>
        /// 有无u盾
        /// </summary>
        public bool IsHaveU { get; set; }

        /// <summary>
        /// 行号
        /// </summary>
        public string BankNo { get; set; }

        /// <summary>
        /// 帐户管理人
        /// </summary>
        public string OwnerID { get; set; }

        /// <summary>
        /// 所在金库
        /// </summary>
        public string GoldStoreID { get; set; }

        /// <summary>
        /// 企业ID
        /// </summary>
        public string EnterpriseID { get; set; }

        /// <summary>
        /// 个人ID
        /// </summary>
        public string PersonID { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }

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

        /// <summary>
        /// 来源
        /// </summary>
        public AccountSource Source { get; set; }

        /// <summary>
        /// 是否为虚拟账户
        /// </summary>
        public bool IsVirtual { get; set; }

        /// <summary>
        /// 大赢家账户名称
        /// </summary>
        public string DyjShortName { get; set; }
        #endregion

        #region 其它属性

        /// <summary>
        /// 金库名称
        /// </summary>
        public string GoldStoreName { get; set; }

        /// <summary>
        /// 企业信息
        /// </summary>
        public Enterprise Enterprise { get; set; }

        /// <summary>
        /// 创建人名称
        /// </summary>
        public string CreatorName { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public decimal? Balance { get; set; }

        /// <summary>
        /// 是否为承兑户
        /// </summary>
        public bool IsAcceptance { get; set; }
        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                //添加
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.Accounts>().Any(item => item.ID == this.ID))
                {
                    this.ID = this.ID ?? PKeySigner.Pick(PKeyType.Account);

                    reponsitory.Insert(new Layers.Data.Sqls.PvFinance.Accounts()
                    {
                        ID = this.ID,
                        Name = this.Name,
                        Code = this.Code,
                        NatureType = (int)this.NatureType,
                        ManageType = (int)this.ManageType,
                        Currency = (int)this.Currency,
                        BankName = this.BankName,
                        OpeningBank = this.OpeningBank,
                        BankAddress = this.BankAddress,
                        District = this.District,
                        SwiftCode = this.SwiftCode,
                        OpeningTime = this.OpeningTime,
                        IsHaveU = this.IsHaveU,
                        BankNo = this.BankNo,
                        OwnerID = this.OwnerID,
                        GoldStoreID = this.GoldStoreID,
                        EnterpriseID = this.EnterpriseID,
                        PersonID = this.PersonID,
                        CreatorID = this.CreatorID,
                        ModifierID = this.CreatorID,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                        Status = (int)GeneralStatus.Normal,
                        ShortName = this.ShortName,
                        Summary = this.Summary,
                        Source = (int)this.Source,
                        IsVirtual = this.IsVirtual,
                    });

                    this.AddSuccess?.Invoke(this, new SuccessEventArgs(this));
                }
                //修改
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvFinance.Accounts>(new
                    {
                        Name = this.Name,
                        ShortName = this.ShortName,
                        Code = this.Code,
                        NatureType = (int)this.NatureType,
                        ManageType = (int)this.ManageType,
                        Currency = (int)this.Currency,
                        BankName = this.BankName,
                        OpeningBank = this.OpeningBank,
                        BankAddress = this.BankAddress,
                        District = this.District,
                        SwiftCode = this.SwiftCode,
                        OpeningTime = this.OpeningTime,
                        IsHaveU = this.IsHaveU,
                        BankNo = this.BankNo,
                        OwnerID = this.OwnerID,
                        GoldStoreID = this.GoldStoreID,
                        EnterpriseID = this.EnterpriseID,
                        PersonID = this.PersonID,
                        ModifierID = this.ModifierID,
                        ModifyDate = DateTime.Now,
                        Summary = this.Summary,
                        Source = (int)this.Source,
                        IsVirtual = this.IsVirtual,
                    }, item => item.ID == this.ID);

                    this.UpdateSuccess?.Invoke(this, new SuccessEventArgs(this));
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
                reponsitory.Update<Layers.Data.Sqls.PvFinance.Accounts>(new
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
                reponsitory.Update<Layers.Data.Sqls.PvFinance.Accounts>(new
                {
                    ModifyDate = DateTime.Now,
                    Status = (int)GeneralStatus.Closed,
                }, item => item.ID == this.ID);
            }
        }

    }
}
