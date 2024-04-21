//using Layers.Data;
//using Layers.Data.Sqls;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Serialization;
//using System.Text;
//using System.Threading.Tasks;
//using System.Transactions;
//using Wms.Services.Enums;
//using Wms.Services.Extends;
//using Wms.Services.Views;
//using Yahv.Linq;
//using Yahv.Linq.Persistence;
//using Yahv.Underly;
//using Yahv.Usually;

//namespace Wms.Services.Models
//{
//    public class Notices : IUnique, IPersisting
//    {

//        #region 事件
//        public event SuccessHanlder NoticeSuccess;
//        public event ErrorHanlder NoticeFailed;

//        #endregion

//        #region 属性

//        /// <summary>
//        /// 唯一码
//        /// </summary>

//        public string ID { get; set; }

//        /// <summary>
//        /// 通知类型：入库通知、出库通知、分拣通知、检测通知、捡货通知、客户自提通知
//        /// </summary>

//        public NoticesType Type { get; set; }

//        /// <summary>
//        /// 仓库编号
//        /// </summary>
//        public string WareHouseID { get; set; }

//        ///// <summary>
//        ///// 仓库信息
//        ///// </summary>
//        //public Warehouses Warehouse
//        //{
//        //    get
//        //    {
//        //        return new Views.WarehousesView()[this.WareHouseID];
//        //    }
//        //}

//        /// <summary>
//        /// 运单编号
//        /// </summary>
//        public string WaybillID { get; set; }

//        private Waybills waybills;
//        /// <summary>
//        /// 运单信息
//        /// </summary>
//        public Waybills Waybills
//        {
//            get
//            {
//                if (waybills == null)
//                {
//                    waybills = new Views.WaybillsView()[this.WaybillID];
//                }
//                return waybills;
//            }
//        }

//        /// <summary>
//        /// 进项编号
//        /// </summary>
//        public string InputID { get; set; }

//        private Inputs inputs;
//        /// <summary>
//        /// 进项信息
//        /// </summary>
//        public Inputs Inputs
//        {
//            get
//            {
//                if (inputs == null)
//                {
//                    inputs = new Views.InputsView()[this.InputID];
//                }
//                return inputs;
//            }
//            set
//            {
//                inputs = value;
//            }
//        }

//        public string DateCode { get; set; }


//        /// <summary>
//        /// 销项编号
//        /// </summary>
//        public string OutputID { get; set; }


//        private Outputs outputs;
//        /// <summary>
//        /// 销项信息
//        /// </summary>
//        public Outputs Outputs
//        {
//            get
//            {
//                if (outputs == null)
//                {
//                    outputs = new Views.OutputsView()[this.OutputID];
//                }
//                return outputs;
//            }
//            set
//            {
//                outputs = value;
//            }
//        }

//        /// <summary>
//        /// 产品编号
//        /// </summary>
//        public string ProductID { get; set; }

//        /// <summary>
//        /// 供应商
//        /// </summary>
//        public string Supplier { get; set; }




//        /// <summary>
//        /// 数量
//        /// </summary>
//        public int Quantity { get; set; }

//        /// <summary>
//        /// 实到数量
//        /// </summary>
//        public int TruetoQuantity { get; set; }

//        /// <summary>
//        /// 分拣的数量
//        /// </summary>
//        public int SortedQuantity
//        {

//            get
//            {
//                var list = new SortingsView().Where(item => item.NoticeID == this.ID);
//                if (list.Count() <= 0)
//                {
//                    return 0;
//                }
//                return list.Sum(item => item.Quantity);
//            }
//        }

//        /// <summary>
//        /// 处理前端拆项时用。
//        /// </summary>
//        public string PID { get; set; }

//        /// <summary>
//        /// 条件(Json开发)
//        /// </summary>
//        public NoticeCondition Conditions { get; set; }

//        /// <summary>
//        /// 创建时间
//        /// </summary>
//        public DateTime CreateDate { get; set; }

//        /// <summary>
//        /// 货架编号
//        /// </summary>
//        public string ShelveID { get; set; }

//        public BaseShelves Shelves
//        {
//            get
//            {
//                return new Views.ShelvesView()[this.ShelveID];
//            }
//        }

//        /// <summary>
//        /// 状态：等待Waiting、关闭Closed、（货物）丢失Lost、完成Completed
//        /// </summary>

//        public NoticesStatus Status { get; set; }

//        /// <summary>
//        /// 来源：1.采购：采购入库、馈赠入库、退货入库、补货入库；2.销售：销售出库、馈赠出库、补货出库；3.代仓储：代收货入库、代发货出库（包含代报关：）、报关入库、报关出库；4.备货：备货入库；5.转运：转运入库、转运出库
//        /// </summary>

//        public NoticesSource Source { get; set; }

//        /// <summary>
//        /// 目标：Address：按地址分拣、转运；
//        ///       Customs：报关；
//        ///       Owner：按所有人分拣，按客户分拣，按内部公司；
//        ///       OrderID：按订单分拣；
//        ///       OrderID->Notice：按订单->通知分拣；
//        ///       Purchaser：按采购员分拣；
//        ///       Sales：按销售分拣
//        ///       Cs：按客户分拣
//        /// </summary>
//        public NoticesTarget Target { get; set; }

//        /// <summary>
//        /// 箱号
//        /// </summary>
//        public string BoxCode { get; set; }

//        public Boxes Boxes
//        {
//            get
//            {

//                return new Views.BoxesView()[this.BoxCode];
//            }
//        }

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
//        /// <summary>
//        /// Type的枚举描述
//        /// </summary>
//        public string TypeDes
//        {
//            get
//            {
//                return this.Type.GetDescription();
//            }
//        }

//        /// <summary>
//        /// Status的枚举描述
//        /// </summary>
//        public string StatusDes
//        {
//            get
//            {
//                return this.Status.GetDescription();
//            }
//        }

//        /// <summary>
//        /// Source的枚举描述
//        /// </summary>
//        public string SourceDes
//        {
//            get
//            {
//                return this.Source.GetDescription();
//            }
//        }



//        /// <summary>
//        /// Target的枚举描述
//        /// </summary>
//        public string TargetDes
//        {
//            get
//            {
//                return this.Target.GetDescription();
//            }
//        }
                   
//        public FileInfo[] FileInfos { get; set; }

//        private StandardProduct standardProduct;
//        public StandardProduct StandardProducts
//        {
//            get
//            {
//                if (standardProduct == null)
//                {
//                    standardProduct = new Views.StandardProductsView()[this.ProductID];
//                }
//                return standardProduct;
//            }
//            set
//            {
//                standardProduct = value;
//            }
//        }

//        #endregion

//        #region 方法
//        public void Enter()
//        {
//            try
//            {


//                using (var responsiry = new PvWmsRepository())
//                {

//                    if (string.IsNullOrWhiteSpace(this.ID))
//                    {
//                        this.ID = PKeySigner.Pick(PkeyType.Notices);
//                        this.CreateDate = DateTime.Now;
//                        if (this.Status == 0)
//                        {
//                            this.Status = NoticesStatus.Waiting;
//                        }
//                        responsiry.Insert(this.ToLinq());
//                    }
//                    else
//                    {
//                        responsiry.Update(this.ToLinq(), item => item.ID == this.ID);
//                    }

//                }


//                this.NoticeSuccess?.Invoke(this, new SuccessEventArgs(this));
//            }
//            catch (Exception ex)
//            {
//                this.NoticeFailed?.Invoke(this, new ErrorEventArgs("Failed!!"));
//            }
//        }

//        public void Abandon()
//        {
//            throw new NotImplementedException();
//        }
//        #endregion
//    }
//}
