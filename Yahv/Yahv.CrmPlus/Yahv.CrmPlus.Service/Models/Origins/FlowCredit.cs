using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;

namespace YaHv.CrmPlus.Services.Models.Origins
{

    public class FlowCredit : IUnique
    {
        public FlowCredit()
        {
            this.CreateDate = DateTime.Now;
        }
        #region 属性
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { set; get; }
        /// <summary>
        /// 授信方
        /// </summary>
        public string MakerID { set; get; }
        /// <summary>
        /// 接收方
        /// </summary>
        public string TakerID { set; get; }
        /// <summary>
        /// 类型
        /// </summary>
        public CreditType Type { set; get; }
        /// <summary>
        /// 业务
        /// </summary>
        public ConductType Conduct { set; get; }
        /// <summary>
        /// 科目
        /// </summary>
        public string Subject { set; get; }
        /// <summary>
        /// 分类
        /// </summary>
        public string Catalog { set; get; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Price { set; get; }
        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { set; get; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorID { set; get; }
        /// <summary>
        /// 创建人
        /// </summary>
        public Admin Creator { set; get; }
        #endregion

        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;

        public event ErrorHanlder EnterError;
        /// <summary>
        /// AbandonSuccess
        /// </summary>
        public event SuccessHanlder AbandonSuccess;
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    this.ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.Flows);
                    repository.Insert(new Layers.Data.Sqls.PvdCrm.FlowCredits
                    {
                        ID = this.ID,
                        TakerID = this.TakerID,
                        MakerID = this.MakerID,
                        Conduct = (int)ConductType.Trade,
                        Type = (int)this.Type,
                        Currency = (int)this.Currency,
                        Price = this.Price,
                        Catalog = this.Catalog,
                        Subject = this.Subject,
                        Summary = this.Summary,
                        CreateDate = this.CreateDate,
                        CreatorID = this.CreatorID
                    });
                }
                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }
        #endregion
    }


}
