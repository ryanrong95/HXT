using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;
using Yahv.Underly;

namespace YaHv.Csrm.Services.Models.Origins
{
    /// <summary>
    /// 标准品牌
    /// </summary>
    [Obsolete]
    public class Brand : Yahv.Linq.IUnique
    {
        public Brand()
        {
            this.CreateDate = DateTime.Now;
            this.Status = GeneralStatus.Normal;
        }
        #region 属性
        /// <summary>
        /// 唯一码
        /// </summary>
        /// <chenhan>保障全局唯一</chenhan>
        public string ID { set; get; }
        public string Name { set; get; }
        /// <summary>
        /// 简称
        /// </summary>
        public string ShortName { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { set; get; }

        #endregion

        //#region 同义词
        //IEnumerable<BrandDictionary> brandDictionary;
        //public IEnumerable<BrandDictionary> BrandDictionary
        //{
        //    get
        //    {
        //        if (this.brandDictionary == null)
        //        {
        //            this.brandDictionary = new Views.Rolls.BrandDictionaryRoll(this.Name);
        //        }
        //        return this.brandDictionary;
        //    }
        //}
        //#endregion

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
                //bool Havebrand = repository.ReadTable<Layers.Data.Sqls.PvbCrm.Brands>().Any(item => item.Name == this.Name || item.ShortName == this.Name);
                //bool Havedictionary = repository.ReadTable<Layers.Data.Sqls.PvbCrm.BrandDictionary>().Any(item => item.Name == this.Name || item.OtherName == this.Name);
                bool isExist = false;
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    isExist = repository.ReadTable<Layers.Data.Sqls.PvbCrm.Brands>().Any(item => item.Name == this.Name || item.ShortName == this.ShortName)
                        || repository.ReadTable<Layers.Data.Sqls.PvbCrm.BrandDictionary>().Any(item => item.Name == this.Name && item.OtherName == this.ShortName);

                }

                if (isExist)
                {
                    if (this != null && this.NameRepeat != null)
                    {
                        this.NameRepeat(this, new ErrorEventArgs());
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(this.ID))
                    {
                        this.ID = Layers.Data.PKeySigner.Pick(PKeyType.Brand);
                        repository.Insert(new Layers.Data.Sqls.PvbCrm.Brands
                        {
                            ID = this.ID,
                            Name = this.Name,
                            ShortName = this.ShortName,
                            CreateDate = this.CreateDate,
                            Status = (int)this.Status
                        });
                    }
                    else
                    {
                        repository.Update<Layers.Data.Sqls.PvbCrm.Brands>(new
                        {
                            Name = this.Name,
                            ShortName = this.ShortName,
                        }, item => item.ID == this.ID);

                    }
                    if (this != null && this.EnterSuccess != null)
                    {
                        this.EnterSuccess(this, new SuccessEventArgs(this));
                    }
                }

            }
        }

        public void Abandon()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                //repository.Delete<Layers.Data.Sqls.PvbCrm.Brands>(item => item.ID == this.ID);
                repository.Update<Layers.Data.Sqls.PvbCrm.Brands>(new
                {
                    Status = (int)GeneralStatus.Deleted
                }, item => item.ID == this.ID);
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }

        }
        #endregion
    }
}
