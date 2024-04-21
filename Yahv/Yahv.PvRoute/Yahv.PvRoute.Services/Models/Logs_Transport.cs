using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Usually;

namespace Yahv.PvRoute.Services.Models
{
    /// <summary>
    /// 运输日志
    /// </summary>
    public class Logs_Transport : IUnique
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
        /// 面单编号
        /// </summary>
        public string FaceOrderID { get; set; }

        /// <summary>
        /// 信息说明
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 接口通讯数据，可空
        /// </summary>
        public string Json { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 实际收货人
        /// </summary>
        public string ConsigneeID { get; set; }
        #endregion

        #region 其它属性

        /// <summary>
        /// 实际收货人名字
        /// </summary>
        public string ConsigneeName { get; set; }
        public TransportConsignee TransportConsignee { get; set; }
        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvRouteReponsitory>.Create())
            {
                //添加,满足根据ID查不到值 并且满足 根据面单编号和对应的信息说明查不到值的时候去添加
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvRoute.Logs_Transports>().Any(item => item.ID == this.ID) && !reponsitory.ReadTable<Layers.Data.Sqls.PvRoute.Logs_Transports>().Any(item => item.FaceOrderID == this.FaceOrderID && item.Summary == this.Summary))
                {
                    this.ID = PKeySigner.Pick(Yahv.PvRoute.Services.Enums.PKeyType.LogsTransports);
                    var now = DateTime.Now;
                    reponsitory.Insert(new Layers.Data.Sqls.PvRoute.Logs_Transports()
                    {
                        ID = this.ID,
                        FaceOrderID = this.FaceOrderID,
                        Summary = this.Summary,
                        CreateDate = now,
                        Json = this.Json,
                        Phone = this.Phone,
                        Contact = this.Contact,
                        ConsigneeID = this.ConsigneeID
                    });
                }
                //修改
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvRoute.Logs_Transports>(new
                    {
                        FaceOrderID = this.FaceOrderID,
                        Summary = this.Summary,
                        Json = this.Json,
                        Phone = this.Phone,
                        Contact = this.Contact,
                        ConsigneeID = this.ConsigneeID
                    }, item => item.ID == this.ID|| (item.FaceOrderID == this.FaceOrderID && item.Summary == this.Summary));
                }

                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }
        #endregion


    }

}
