//using Layers.Data.Sqls;
//using Layers.Linq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Yahv.Usually;
//using Yahv.Utils.Converters.Contents;
//using Yahv.Underly;

//namespace YaHv.Csrm.Services.Models.Origins
//{
//    /// <summary>
//    /// 原厂品牌
//    /// </summary>
//    public class ManufacturerBrand : Yahv.Linq.IUnique
//    {
//        public ManufacturerBrand()
//        {
//            this.CreateDate = DateTime.Now;
//        }
//        #region 属性
//        public string ID { set; get; }
//        public string BrandID { set; get; }
//        /// <summary>
//        /// 品牌名称
//        /// </summary>
//        public string BrandName { set; get; }
//        /// <summary>
//        /// 原厂ID
//        /// </summary>
//        public string SupplierID { set; get; }

//        /// <summary>
//        /// 原厂名称
//        /// </summary>
//        public string SupplierName { set; get; }
//        /// <summary>
//        /// 创建时间
//        /// </summary>

//        public DateTime CreateDate { set; get; }
//        /// <summary>
//        /// 品牌状态
//        /// </summary>

//        public GeneralStatus Status { set; get; }
//        #endregion


//        #region 事件
//        /// <summary>
//        /// EnterSuccess
//        /// </summary>
//        public event SuccessHanlder EnterSuccess;
//        /// <summary>
//        /// AbandonSuccess
//        /// </summary>
//        public event SuccessHanlder AbandonSuccess;

//        public event ErrorHanlder Repeat;
//        #endregion


//        #region 持久化

//        public void Enter()
//        {
//            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
//            {
//                //品牌是否存在
//                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.Brands>().Any(item => item.Name == this.BrandName))
//                {
//                    this.BrandID = new Views.Origins.BrandsOrigin().SingleOrDefault(item => item.Name == this.BrandName).ID;
//                }
//                else
//                {
//                    this.BrandID = Layers.Data.PKeySigner.Pick(PKeyType.Brand);
//                    repository.Insert(new Layers.Data.Sqls.PvbCrm.Brands
//                    {
//                        ID = this.BrandID,
//                        Name = this.BrandName,
//                        ShortName = null,
//                        CreateDate = this.CreateDate,
//                        Status = (int)GeneralStatus.Normal
//                    });
//                }
//                //原厂与品牌的关系是否存在
//                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.nBrand>().Any(item => item.BrandID == this.BrandID && item.EnterpriseID == this.SupplierID && item.Type == 1))
//                {
//                    if (string.IsNullOrWhiteSpace(this.ID) && this != null && this.Repeat != null)
//                    {
//                        this.Repeat(this, new ErrorEventArgs());
//                    }
//                }
//                else
//                {
//                    this.ID = Layers.Data.PKeySigner.Pick(PKeyType.nBrand);
//                    repository.Insert(new Layers.Data.Sqls.PvbCrm.nBrands
//                    {
//                        ID = this.BrandID,
//                        EnterpriseID = this.SupplierID,
//                        BrandID = this.BrandID,
//                        CreateDate = this.CreateDate,
//                        Type = 1
//                    });

//                }
//            }
//            if (this != null && this.EnterSuccess != null)
//            {
//                this.EnterSuccess(this, new SuccessEventArgs(this));
//            }
//        }

//        public void Abandon()
//        {
//            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
//            {
//                repository.Delete<Layers.Data.Sqls.PvbCrm.nBrand>(item => item.ID == this.ID);
//            }
//            if (this != null && this.AbandonSuccess != null)
//            {
//                this.AbandonSuccess(this, new SuccessEventArgs(this));
//            }
//        }
//        #endregion
//    }
//}
