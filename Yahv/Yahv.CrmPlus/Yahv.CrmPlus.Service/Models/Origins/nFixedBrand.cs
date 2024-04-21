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

namespace Yahv.CrmPlus.Service.Models.Origins
{
    public class nFixedBrand : Yahv.Linq.IUnique
    {
        public nFixedBrand()
        {

        }
        #region 属性
        public string ID { set; get; }
        /// <summary>
        /// 企业ID
        /// </summary>
        public string EnterpriseID { set; get; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { set; get; }
        /// <summary>
        /// 是否限制出货
        /// </summary>
        public bool? IsProhibited { set; get; }
        /// <summary>
        /// 有无折扣
        /// </summary>
        public bool? IsDiscounted { set; get; }
        /// <summary>
        /// 是否推广促销
        /// </summary>
        public bool? IsPromoted { set; get; }
        /// <summary>
        /// 是否优势
        /// </summary>
        public bool? IsAdvantaged { set; get; }
        public string Summary { set; get; }
        /// <summary>
        /// 是否特色
        /// </summary>
        public bool? IsSpecial { set; get; }
        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { set; get; }
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
        /// <summary>
        /// Repeat
        /// </summary>
        public event ErrorHanlder Repeat;
        /// <summary>
        /// AbandonSuccess
        /// </summary>
        public event AbandonHanlder AbandonSuccess;
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                var exist = repository.ReadTable<Layers.Data.Sqls.PvdCrm.nFixedBrands>().Where(item => item.EnterpriseID == this.EnterpriseID && item.Brand == this.Brand);
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    if (exist.Any())
                    {
                        this.Repeat(this, new ErrorEventArgs());
                        return;
                    }
                    this.ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.nFixedBrand);
                    repository.Insert(new Layers.Data.Sqls.PvdCrm.nFixedBrands
                    {
                        ID = this.ID,
                        EnterpriseID = this.EnterpriseID,
                        Brand = this.Brand,
                        IsProhibited = this.IsProhibited,
                        IsPromoted = this.IsPromoted,
                        IsDiscounted = this.IsDiscounted,
                        IsAdvantaged = this.IsAdvantaged,
                        IsSpecial = this.IsSpecial,
                        Summary = this.Summary,
                        CreatorID = this.CreatorID,
                    });
                }
                else
                {
                    repository.Update<Layers.Data.Sqls.PvdCrm.nFixedBrands>(new
                    {
                        EnterpriseID = this.EnterpriseID,
                        Brand = this.Brand,
                        IsProhibited = this.IsProhibited,
                        IsPromoted = this.IsPromoted,
                        IsDiscounted = this.IsDiscounted,
                        IsAdvantaged = this.IsAdvantaged,
                        IsSpecial = this.IsSpecial,
                        Summary = this.Summary
                    }, item => item.ID == this.ID);
                }

                this.EnterError?.Invoke(this, new ErrorEventArgs());
                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }
        public void Abandon()
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                repository.Delete<Layers.Data.Sqls.PvdCrm.nFixedBrands>(item => item.ID == this.ID);
                this.AbandonSuccess?.Invoke(this, new AbandonedEventArgs(this));
            }
        }
        #endregion
    }
}
