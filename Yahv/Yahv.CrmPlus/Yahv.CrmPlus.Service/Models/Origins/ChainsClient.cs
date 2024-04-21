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
using YaHv.CrmPlus.Services.Models.Origins;

namespace Yahv.CrmPlus.Service.Models.Origins
{
    public class ChainsClient : IUnique
    {
        public ChainsClient()
        { }
        #region 属性
        public string ID { set; get; }
        /// <summary>
        /// 客户企业信息
        /// </summary>
        public Enterprise Enterprise { set; get; }
        /// <summary>
        /// 客户企业信息
        /// </summary>
        public EnterpriseRegister EnterpriseRegister { set; get; }
        public bool? Vip { set; get; }
        /// <summary>
        /// 等级
        /// </summary>
        public int? Grade { set; get; }
        /// <summary>
        /// 客户性质
        /// </summary>
        public ClientType Nature { set; get; }
        /// <summary>
        /// 服务类型
        /// </summary>
        public ServiceType ServiceType { set; get; }
        /// <summary>
        /// 国家
        /// </summary>
        public string District { set; get; }
        /// <summary>
        /// 海关编码
        /// </summary>
        public string CustomCode { set; get; }
        /// <summary>
        /// 入仓号
        /// </summary>
        public string WsCode { set; get; }
        /// <summary>
        /// 业务员ID
        /// </summary>
        public string OwnerID { set; get; }
        public Admin Owner { internal set; get; }
        /// <summary>
        /// 跟单员ID
        /// </summary>
        public string TrackerID { set; get; }
        public Admin Tracker { internal set; get; }
        /// <summary>
        /// 引荐人
        /// </summary>
        public string ReferrerID { set; get; }
        public Admin Referrer { internal set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { internal set; get; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { internal set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public ApprovalStatus Status { set; get; }
        #endregion;

        public List<CallFile> Lisences { set; get; }
        public CallFile HkLisences { set; get; }

        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// EnterError
        /// </summary>
        public event ErrorHanlder EnterError;       
        //public event SuccessHanlder AbandonSuccess;
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            {

            }
        }
        #endregion
    }
}
