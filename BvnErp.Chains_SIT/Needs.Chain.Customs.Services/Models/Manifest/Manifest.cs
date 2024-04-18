using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Utils.Serializers;
using Needs.Ccs.Services.Hanlders;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 舱单
    /// </summary>
    public class Manifest : Voyage, IUnique, IPersist
    {
        #region 属性

        /// <summary>
        /// 承运人代码
        /// </summary>
        public string CarrierCode { get; set; }

        /// <summary>
        /// 运输工具代理企业代码  默认：  4位关区号+9位组织机构代码
        /// </summary>
        public string TransAgentCode { get; set; }

        /// <summary>
        /// 传输企业备案关区
        /// </summary>
        public string CustomMaster { get; set; }

        /// <summary>
        /// 企业代码 18位统一社会信用代码
        /// </summary>
        public string UnitCode { get; set; }

        /// <summary>
        /// 舱单传输人名称 公司名称
        /// </summary>
        public string MsgRepName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string AdditionalInformation { get; set; }

        /// <summary>
        /// 拓展，舱单报文中使用
        /// </summary>
        public string ClientID
        {
            get
            {
               return PurchaserContext.Current.ClientID;
            }
        }


        #endregion

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;


        public Manifest()
        {
            this.CreateTime = DateTime.Now;
        }

        public new void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Manifests>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
            }
            this.OnEnter();
        }

        virtual protected void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}