//using Layers.Data.Sqls;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Cryptography;
//using System.Text;
//using System.Threading.Tasks;
//using Wms.Services.Enums;
//using Wms.Services.Extends;
//using Yahv.Linq;
//using Yahv.Linq.Persistence;
//using Yahv.Usually;
//using Yahv.Utils.Converters.Contents;

//namespace Wms.Services.Models
//{
//    /// <summary>
//    /// 产品类
//    /// </summary>
//    public class StandardProduct : IUnique, IPersisting
//    {
//        #region 事件
//        public event SuccessHanlder EnterSuccess;
//        public event ErrorHanlder EnterError;
//        #endregion

//        #region 属性
//        /// <summary>
//        /// 唯一标识 MD5（Catalog+PartNumber+Manufacturer+Packaging+PackageCase）
//        /// </summary>
//        public string ID { get; internal set; }
//        /// <summary>
//        /// 品名
//        /// </summary>
//        public string Catalog { get; set; }
//        /// <summary>
//        /// 型号
//        /// </summary>
//        public string PartNumber { get; set; }
//        /// <summary>
//        /// 制造商
//        /// </summary>
//        public string Manufacturer { get; set; }
//        /// <summary>
//        /// 包装
//        /// </summary>
//        public string Packing { get; set; }
//        /// <summary>
//        /// 封装
//        /// </summary>
//        public string PackageCase { get; set; }
//        /// <summary>
//        /// 创建之日(默认当前时间)
//        /// </summary>
//        public DateTime CreateDate { get; internal set; }
//        /// <summary>
//        /// 单毛重上界
//        /// </summary>
//        public decimal? UnitGrossWeightTL { get; set; }
//        /// <summary>
//        /// 单毛重下界
//        /// </summary>
//        public decimal? UnitGrossWeightBL { get; set; }
//        /// <summary>
//        /// 单体积
//        /// </summary>
//        public decimal? UnitGrossVolume { get; set; }
//        #endregion

//        #region 持久化
//        public void Enter()
//        {
//            try
//            {
//                using (var repository = new PvWmsRepository())
//                {
//                    this.ID = string.Concat(this.Manufacturer?.ToUpper().Trim(),this.PartNumber?.ToUpper().Trim(),this.Packing?.ToUpper().Trim(), this.PackageCase?.ToUpper().Trim(),this.Catalog?.ToUpper().Trim()).MD5();
//                    //if (!new Views.StandardProductsView().Any(item => item.ID == this.ID))
//                    //{
//                    try
//                    {
//                        this.CreateDate = DateTime.Now;
//                        repository.Insert(this.ToLinq());
//                    }
//                    catch
//                    { }
//                    //}
                   
//                }
//                EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
//            }
//            catch (Exception ex)
//            {
//                EnterError?.Invoke(this, new ErrorEventArgs(ex.Message, ErrorType.System));
//            }
//        }

//        public void Abandon()
//        {
//            throw new NotImplementedException();
//        }
//        #endregion
//    }
//}
