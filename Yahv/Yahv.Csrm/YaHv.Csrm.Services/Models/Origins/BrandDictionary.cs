using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Linq;
using Yahv.Usually;
using Layers.Data.Sqls;
using YaHv.Csrm.Services.Extends;

namespace YaHv.Csrm.Services.Models.Origins
{
    /// <summary>
    /// 品牌同义词
    /// </summary>
    public class BrandDictionary : Yahv.Linq.IUnique
    {
        public BrandDictionary()
        {
            this.CreateDate = DateTime.Now;
        }
        #region 属性
        /// <summary>
        /// 唯一码
        /// </summary>
        /// <chenhan>保障全局唯一</chenhan>
        public string ID { set; get; }
        public string Name { set; get; }
        /// <summary>
        /// 其他名称
        /// </summary>
        public string OtherName { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }
        /// <summary>
        /// 来源
        /// </summary>
        public string Source { set; get; }
        /// <summary>
        /// 是否简称
        /// </summary>
        public bool IsShort { set; get; }
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
        public event ErrorHanlder NameRepeat;
        #endregion


        #region 持久化
        public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                //品牌名称，简称，同义词存在视为品牌已存在；不能新增
                // bool Havebrand = repository.ReadTable<Layers.Data.Sqls.PvbCrm.Brands>().Any(item => item.Name == this.Name || item.ShortName == this.OtherName);
                // bool Havedictionary = repository.ReadTable<Layers.Data.Sqls.PvbCrm.BrandDictionary>().Any(item => item.Name == this.Name || item.OtherName == this.OtherName);
                bool isExist = false;
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    isExist = repository.ReadTable<Layers.Data.Sqls.PvbCrm.Brands>().Any(item => item.Name == this.Name && item.ShortName == this.OtherName)
                        || repository.ReadTable<Layers.Data.Sqls.PvbCrm.BrandDictionary>().Any(item => item.Name == this.Name && item.OtherName == this.OtherName);

                }
              
                if (isExist)
                {
                    if (this != null && this.EnterSuccess != null)
                    {
                        this.NameRepeat(this, new ErrorEventArgs());
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(this.ID))
                    {
                        this.ID = Layers.Data.PKeySigner.Pick(PKeyType.BrandDic);
                        repository.Insert(this.ToLinq());
                    }
                    else
                    {
                        repository.Update<Layers.Data.Sqls.PvbCrm.BrandDictionary>(new
                        {
                            OtherName = this.OtherName,
                            Source = this.Source,
                            IsShort = this.IsShort
                        }, item => item.ID == this.ID);
                    }

                    if (this != null && this.EnterSuccess != null)
                    {
                        this.EnterSuccess(this, new Yahv.Usually.SuccessEventArgs(this));
                    }
                }
            }

        }

        public void Abandon()
        {
            using (var repository = LinqFactory<Layers.Data.Sqls.PvbCrmReponsitory>.Create())
            {
                repository.Delete<Layers.Data.Sqls.PvbCrm.BrandDictionary>(item => item.ID == this.ID);
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new Yahv.Usually.SuccessEventArgs(this));
                }
            }

        }
        #endregion
    }
}
