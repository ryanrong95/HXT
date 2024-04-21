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
    public class vBrand : Yahv.Linq.IUnique
    {
        public vBrand()
        {
        }
        #region 属性
        public string ID { set; get; }
        public string BrandID { set; get; }
        public string AdminID { set; get; }
        public string AdminRealName { internal set; get; }
        public string AdminRoleName { internal set; get; }
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
        public event ErrorHanlder NameReapt;
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (!repository.ReadTable<Layers.Data.Sqls.PvbCrm.vBrands>().Any(item => item.BrandID == this.BrandID && item.AdminID == this.AdminID))
                {
                    this.ID = Layers.Data.PKeySigner.Pick(PKeyType.vBrand);
                    repository.Insert(new Layers.Data.Sqls.PvbCrm.vBrands
                    {
                        ID = this.ID,
                        BrandID = this.BrandID,
                        AdminID = this.AdminID
                    });
                }
                else
                {
                    this.NameReapt(this, new ErrorEventArgs());
                    return;
                }
                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        public void Abandon()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.vBrands>().Any(item => item.ID == this.ID))
                {
                    repository.Delete<Layers.Data.Sqls.PvbCrm.vBrands>(item => item.ID == this.ID);
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
