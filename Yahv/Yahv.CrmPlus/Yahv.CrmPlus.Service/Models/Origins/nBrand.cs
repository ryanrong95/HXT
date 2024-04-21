using Layers.Data;
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

namespace Yahv.CrmPlus.Service.Models.Origins
{
    /// <summary>
    /// 品牌关系
    /// </summary>
    public class nBrand : IUnique
    {
        #region 属性
        public string ID { set; get; }
        public string EnterpriseID { set; get; }
        public string EnterpriseName { internal set; get; }
        /// <summary>
        /// 类型：生产，代理
        /// </summary>
        public nBrandType Type { set; get; }
        /// <summary>
        /// 标准品牌ID
        /// </summary>
        public string BrandID { set; get; }
        /// <summary>
        /// 品牌标准名称
        /// </summary>
        public string BrandName { internal set; get; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorID { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { set; get; }
        #endregion
        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;
        /// <summary>
        /// AbandonSuccess
        /// </summary>
        public event AbandonHanlder AbandonSuccess;
        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event ErrorHanlder Repeat;
        #endregion


        #region 持久化
        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            {
                var exist = reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.nBrands>().Where(item => item.Type == (int)this.Type && item.BrandID == this.BrandID && item.EnterpriseID == this.EnterpriseID);
                if (exist.Any())
                {
                    this.Repeat(this, new ErrorEventArgs());
                    return;
                }
                else
                {
                    this.ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.nBrand);
                    reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.nBrands
                    {
                        ID = this.ID,
                        Type = (int)this.Type,
                        BrandID = this.BrandID,
                        EnterpriseID = this.EnterpriseID,
                        CreatorID = this.CreatorID,
                        Summary = this.Summary
                    });
                }
                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }
        public void Abandon()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            {
                reponsitory.Delete<Layers.Data.Sqls.PvdCrm.nBrands>(item => item.ID == this.ID);
            }
        }
        #endregion


        ///// <summary>
        ///// 点燃
        ///// </summary>
        ///// <param name="e">事件参数</param>
        //protected void Fire(EventArgs e)
        //{
        //    if (this.Repeat != null && e is ErrorEventArgs)
        //    {
        //        this.Repeat(this, new ErrorEventArgs());
        //    }
        //    if (this.EnterSuccess != null && e is SuccessEventArgs)
        //    {
        //        this.EnterSuccess(this, e as SuccessEventArgs);
        //    }
        //    if (this.AbandonSuccess != null && e is AbandonedEventArgs)
        //    {
        //        this.AbandonSuccess(this, e as AbandonedEventArgs);
        //    }
        //}
    }

}
