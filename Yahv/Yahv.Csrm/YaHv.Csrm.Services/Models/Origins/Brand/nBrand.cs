using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;
using YaHv.Csrm.Services.Extends;

namespace YaHv.Csrm.Services.Models.Origins
{
    public class nBrand : Yahv.Linq.IUnique
    {
        public nBrand()
        {
            this.Status = DataStatus.Normal;
        }
        #region 属性
        public string ID { set; get; }

        public string BrandID { set; get; }
        /// <summary>
        /// 品牌名称
        /// </summary>
        public string BrandName { internal set; get; }
        /// <summary>
        /// 品牌简称
        /// </summary>
        public string ShortName { internal set; get; }
        /// <summary>
        /// 品牌中文名称
        /// </summary>
        public string ChineseName { internal set; get; }
        /// <summary>
        /// 企业ID
        /// </summary>
        public string EnterpriseID { set; get; }
        public string EnterpriseName { internal set; get; }
        public DataStatus Status { set; get; }

      
        #endregion


        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;
        /// <summary>
        /// AbandonSuccess
        /// </summary>
        public event SuccessHanlder AbandonSuccess;
        /// <summary>
        /// 已存在
        /// </summary>
        public event ErrorHanlder Reapt;
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.nBrands>().Any(item => item.BrandID == this.BrandID && item.EnterpriseID == this.EnterpriseID && item.Status == (int)DataStatus.Normal))
                {
                    this.Reapt(this, new ErrorEventArgs());
                    return;
                }
                else
                {
                    this.ID = PKeySigner.Pick(PKeyType.nBrand);
                    repository.Insert(new Layers.Data.Sqls.PvbCrm.nBrands
                    {
                        ID = this.ID,
                        BrandID = this.BrandID,
                        EnterpriseID = this.EnterpriseID,
                        Status = (int)this.Status
                    });
                }
            }
            this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
        }
        public void Abandon()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())

            {
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.nBrands>().Any(item => item.ID == this.ID))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.nBrands>(new
                    {
                        Status = (int)DataStatus.Closed
                    }, item => item.ID == this.ID);
                }
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        #endregion
    }
}
