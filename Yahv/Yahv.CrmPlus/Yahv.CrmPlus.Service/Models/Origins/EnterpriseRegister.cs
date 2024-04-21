using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Extends;
using Yahv.CrmPlus.Service.Views.Origins;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Serializers;

namespace Yahv.CrmPlus.Service.Models.Origins
{
    public class EnterpriseRegister : IUnique,IMyCloneable
    {
        #region  属性
        public string ID { get; set; }
        /// <summary>
        /// 是否高校或涉密,只填写统一社会信用代码
        /// </summary>
        public bool IsSecret { get; set; }
        /// <summary>
        /// 是否国际(要填写币种，国家地址，注册地址)
        /// </summary>
        public bool IsInternational { get; set; }
        /// <summary>
        /// 法人
        /// </summary>
        /// <remarks>
        /// 把所有第三方可以获取的接口数据都表remark标识：接口存在\接口不存在
        /// </remarks>
        public string Corperation { get; set; }
        /// <summary>
        /// 注册地址
        /// </summary>
        public string RegAddress { get; set; }
        /// <summary>
        /// 统一社会信用编码
        /// </summary>
        public string Uscc { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public Currency? Currency { get; set; }
        /// <summary>
        /// 注册资金
        /// </summary>

        public string RegistFund { get; set; }
        /// <summary>
        /// 注册币种
        /// </summary>
        public Currency RegistCurrency { get; set; }
        /// <summary>
        /// 所属行业
        /// </summary>
        public string Industry { get; set; }

        /// <summary>
        /// 注册日期
        /// </summary>
        public DateTime? RegistDate { get; set; }
        /// <summary>
        /// 营业状态
        /// </summary>
        public string BusinessState { get; set; }
        /// <summary>
        /// 员工数量
        /// </summary>

        public int? Employees { get; set; }
        /// <summary>
        /// 网站
        /// </summary>
        public string WebSite { get; set; }
        /// <summary>
        /// 企业性质
        /// </summary>
        public string Nature { get; set; }

        public string Summary { get; set; }

        #endregion
        public EnterpriseRegister()
        {
           
        }

        #region 持久化

        public void Enter()
        {
            using (var reponsitory =  new PvdCrmReponsitory())
            {
                if (!reponsitory.GetTable<Layers.Data.Sqls.PvdCrm.EnterpriseRegisters>().Any(x => x.ID == this.ID))
                {
                    //this.ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.EnterpriseRegister);
                    reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.EnterpriseRegisters()
                    {
                        ID = this.ID,//企业ID
                        IsSecret = this.IsSecret,
                        IsInternational = this.IsInternational,
                        Corperation = this.Corperation,
                        RegAddress = this.RegAddress,
                        Uscc = this.Uscc,
                        Currency = (int?)this.Currency,
                        RegistFund = this.RegistFund,
                        RegistCurrency = (int)this.RegistCurrency,
                        Industry = this.Industry,
                        RegistDate = this.RegistDate,
                        Summary = this.Summary,
                        BusinessState = this.BusinessState,
                        Employees = this.Employees,
                        WebSite = this.WebSite,
                        Nature = this.Nature,
                    });
                }
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvdCrm.EnterpriseRegisters>(new
                    {
                        ID = this.ID,
                        IsSecret = this.IsSecret,
                        IsInternational = this.IsInternational,
                        Corperation = this.Corperation,
                        RegAddress = this.RegAddress,
                        Uscc = this.Uscc,
                        Currency = (int?)this.Currency,
                        RegistFund = this.RegistFund,
                        RegistCurrency = (int)this.RegistCurrency,
                        Industry = this.Industry,
                        RegistDate = this.RegistDate,
                        Summary = this.Summary,
                        BusinessState = this.BusinessState,
                        Employees = this.Employees,
                        WebSite = this.WebSite,
                        Nature = this.Nature
                    }, item => item.ID == this.ID);

                }

                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }

        }

        #endregion

        #region 事件

        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// EnterError
        /// </summary>
        public event ErrorHanlder EnterError;


        public event SuccessHanlder AbandonSuccess;
        #endregion


        #region Clone
        public object Clone()
        {
            return this.ToLinq() as object;
        }

        public object Clone(bool isCloneDb)
        {
            var json = this.Json();
            if (isCloneDb)
            {
                return this.Clone();
            }
            else
            {
                return json.JsonTo<EnterpriseRegister>();
            }
        }
        #endregion
    }
}
