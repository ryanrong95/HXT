//using Layers.Data;
//using Layers.Data.Sqls;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Wms.Services.Enums;
//using Wms.Services.Extends;
//using Wms.Services.Views;
//using Yahv.Linq;
//using Yahv.Linq.Persistence;
//using Yahv.Services.Models;
//using Yahv.Usually;

//namespace Wms.Services.Models
//{
//    /// <summary>
//    /// 分拣类
//    /// </summary>
//    public class Sorting : IUnique, IPersisting
//    {
//        #region 属性
//        /// <summary>
//        /// 唯一码，四位年+2位月+2日+6位流水
//        /// </summary>
//        public string ID { get; internal set; }

//        /// <summary>
//        ///通知编号 
//        /// </summary>
//        public string NoticeID { get; set; }

//        /// <summary>
//        ///运单编号 
//        /// </summary>
//        public string WaybillID { get; set; }

//        /// <summary>
//        /// 装箱信息（箱号）
//        /// </summary>
//        public string BoxCode { get; set; }

//        /// <summary>
//        /// 分拣数量
//        /// </summary>
//        public decimal Quantity { get; set; }

//        /// <summary>
//        /// 分拣人
//        /// </summary>
//        public string AdminID { get; set; }

        

//        /// <summary>
//        /// 创建时间(发生时间)
//        /// </summary>
//        public DateTime CreateDate { get; internal set; }

//        /// <summary>
//        /// 重量
//        /// </summary>
//        public decimal? Weight { get; set; }

//        /// <summary>
//        /// 体积
//        /// </summary>
//        public decimal? Volume { get; set; }

        

//        #endregion

//        #region 扩展属性

//        public string AdminName {
//            get {
//                if (string.IsNullOrEmpty(this.AdminID))
//                {
//                    return "";
//                }
//                else
//                {
//                    return new AdminsView().Where(item => item.ID == this.AdminID).FirstOrDefault()?.RealName??"";        
//                }

             
//            }
//        }

//        private CenterFileDescription[] fileInfos;

//        public CenterFileDescription[] FileInfos
//        {
//            set { fileInfos = value; }
//        }
       
//        #endregion

//        #region 事件
//        public event SuccessHanlder SortingSuccess;
//        public event ErrorHanlder EnterError;
//        #endregion

//        #region 持久化
//        public void Enter()
//        {
//            try
//            {
//                using (var repository = new PvWmsRepository())
//                {
//                    this.ID = PKeySigner.Pick(PkeyType.Sortings);
//                    this.CreateDate = DateTime.Now;
//                    repository.Insert(this.ToLinq());
//                }

//                this.SortingSuccess?.Invoke(this, new SuccessEventArgs(this));
//            }
//            catch(Exception ex)
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
