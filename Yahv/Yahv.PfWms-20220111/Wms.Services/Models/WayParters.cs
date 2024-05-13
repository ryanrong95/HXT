using Layers.Data;
using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Extends;
using Yahv.Linq;
using Yahv.Linq.Persistence;
using Yahv.Usually;

namespace Wms.Services.Models
{
    public class WayParters :  IUnique, IPersisting
    {

        #region 事件
        //Enter成功
        public event SuccessHanlder WayParterSuccess;
        //Enter失败
        public event ErrorHanlder WayParterFailed;
        #endregion

        #region 属性

        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 公司联系人
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 收/发 地区
        /// </summary>
        public string Place { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 联系人电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        public string Zipcode { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; }

        #endregion

        #region 方法

        public void Enter()
        {
            try
            {
                using (var repository = new PvWmsRepository())
                {
                    if (string.IsNullOrWhiteSpace(this.ID))
                    {
                        this.ID = PKeySigner.Pick(PkeyType.Waybills);
                        this.CreateDate = DateTime.Now;
                        repository.Insert(this.ToLinq());
                    }
                    else
                    {
                        repository.Update(this.ToLinq(), item => item.ID == this.ID);
                    }
                }
                this.WayParterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
            catch
            {
                this.WayParterFailed?.Invoke(this, new ErrorEventArgs("Failed"));
            }
            
        }

        public void Abandon()
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
