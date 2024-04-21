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
    /// 运输日志收货人
    /// </summary>
    public class TransportConsignee:IUnique
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
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 手机：推送货物动态，物流等信息给收货人的邮箱活手机
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 邮箱：推送货物动态，物流等信息给收货人的邮箱活手机
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvRouteReponsitory>.Create())
            {
                //添加
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvRoute.TransportConsignees>().Any(item => item.ID == this.ID))
                {
                    this.ID = PKeySigner.Pick(Yahv.PvRoute.Services.Enums.PKeyType.TransportConsignees);
                    var now = DateTime.Now;
                    reponsitory.Insert(new Layers.Data.Sqls.PvRoute.TransportConsignees()
                    {
                        ID = this.ID,
                        Name = this.Name,
                        Phone = this.Phone,
                        Mobile = this.Mobile,
                        Email = this.Email,
                        Address = this.Address,
                        CreateDate = now

                    });
                }
                //修改
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvRoute.TransportConsignees>(new
                    {
                        Name = this.Name,
                        Phone = this.Phone,
                        Mobile = this.Mobile,
                        Email = this.Email,
                        Address = this.Address
                    }, item => item.ID == this.ID);
                }

                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }
        #endregion

    }
}
