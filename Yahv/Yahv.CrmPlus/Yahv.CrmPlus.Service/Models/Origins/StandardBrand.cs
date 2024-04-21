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
using YaHv.CrmPlus.Services.Models.Origins;

namespace Yahv.CrmPlus.Service.Models.Origins
{
    /// <summary>
    ///标准品牌
    /// </summary>
    public class StandardBrand : IUnique, IDataEntity
    {
        #region 属性
        /// <summary>
        /// 唯一码
        /// </summary>
        /// <chenhan>保障全局唯一</chenhan>
        public string ID { set; get; }
        /// <summary>
        /// 品牌名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 简称
        /// </summary>
        public string Code { set; get; }

        /// <summary>
        /// 中文名称
        /// </summary>
        public string ChineseName { get; set; }
        /// <summary>
        /// 是否是代理
        /// </summary>

        public bool IsAgent { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }

        public DateTime ModifyDate { get; set; }


        public string CreatorID { get; set; }

        public Admin Admin { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public DataStatus Status { set; get; }

        public string Summary { get; set; }

        #endregion


        public StandardBrand()
        {
            this.Status = DataStatus.Normal;
            this.ModifyDate = this.CreateDate = DateTime.Now;
        }
        #region 持久化
        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            {
                //添加
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.StandardBrands>().Any(item => item.ID == this.ID))
                {
                    this.ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.Brand);
                    //this.ID = Guid.NewGuid().ToString();
                    reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.StandardBrands()
                    {
                        ID = this.ID,
                        Name = this.Name,
                        Code = this.Code,
                        ChineseName = this.ChineseName,
                        IsAgent = this.IsAgent,
                        Summary = this.Summary,
                        CreateDate = this.CreateDate,
                        ModifyDate = this.ModifyDate,
                        CreatorID = this.CreatorID,
                        Status = (int)this.Status,
                    });
                }
                //修改
                else
                {
                    this.ModifyDate = DateTime.Now;
                    reponsitory.Update<Layers.Data.Sqls.PvdCrm.StandardBrands>(new
                    {
                        ID = this.ID,
                        Name = this.Name,
                        Code = this.Code,
                        ChineseName = this.ChineseName,
                        IsAgent = this.IsAgent,
                        Summary = this.Summary,
                        CreateDate = this.CreateDate,
                        ModifyDate = this.ModifyDate,
                        CreatorID = this.CreatorID,
                        Status = (int)this.Status,
                    }, item => item.ID == this.ID);
                }
                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }

        public void Abandon()
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvdCrm.StandardBrands>(new
                {
                    Status = (int)DataStatus.Closed
                }, item => item.ID == this.ID);
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
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


        public event SuccessHanlder AbandonSuccess;

        #endregion
    }
}
