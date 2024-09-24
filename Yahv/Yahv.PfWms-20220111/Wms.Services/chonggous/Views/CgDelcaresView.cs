using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Wms.Services;
using Yahv.Services.Enums;
using Newtonsoft.Json.Linq;
using Yahv.Utils.Serializers;
using Yahv.Underly;
using Layers.Data;
using Layers.Data.Sqls.PvWms;
using Yahv.Utils.Converters.Contents;
using Wms.Services.Extends;
using Yahv.Underly.Enums;
using Wms.Services.Enums;

namespace Wms.Services.chonggous.Views
{
    /// <summary>
    /// 申报视图
    /// </summary>
    /// <remarks>
    /// 华芯通申报视图：CgDelcaresTopView
    /// </remarks>
    public class CgDelcaresView : QueryView<object, PvWmsRepository>
    {

        #region 构造函数 

        public CgDelcaresView()
        {

        }

        protected CgDelcaresView(PvWmsRepository reponsitory) : base(reponsitory)
        {
        }

        protected CgDelcaresView(PvWmsRepository reponsitory, IQueryable<object> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 乔霞UI字段要求
        /// 订单号
        /// 申报状态
        /// 装箱时间
        /// 装箱人
        /// 客户编号
        /// 库位
        /// 规格
        /// 总重
        /// 毛重
        /// 型号
        /// 品牌
        /// 批次
        /// 数量
        /// 原产地
        /// </remarks>
        protected override IQueryable<object> GetIQueryable()
        {
            var adminTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>();
            // sorting.Quantity
            // sorting.Weight
            // 总毛重：sum(sorting.Weight)
            // 毛重：sum(sorting.Weight)/所有的数量*分拣的数量 

            //已经第数不清楚的次，进行修改了。

            return from log in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_Declare>()
                   join admin in adminTopView on log.AdminID equals admin.ID
                   orderby log.CreateDate descending
                   select new MyReport
                   {
                       ID = log.ID,
                       TinyOrderID = log.TinyOrderID,
                       DeclareStatus = (TinyOrderDeclareStatus)log.Status,
                       CreateDate = log.CreateDate,
                       EnterCode = log.EnterCode,
                       AdminID = admin.ID,
                       Packer = admin.RealName,
                   };
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<MyReport> iquery = this.IQueryable.Cast<MyReport>().OrderByDescending(item => item.ID);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }


            var adminTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>();
            //var waybillsTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>();

            //需要申报日志数据的支持 Logs_Declare

            var linq_declares =
                from ditem in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_DeclareItem>() //最终的那个。
                join storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>() on ditem.StorageID equals storage.ID
                join product in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>() on storage.ProductID equals product.ID
                join admin in adminTopView on ditem.AdminID equals admin.ID //分拣人员
                select new
                {
                    #region 视图
                    ditem.TinyOrderID,
                    ditem.BoxCode,
                    ditem.Status,
                    Declare = new
                    {
                        ID = ditem.ID,
                        //ditem.TinyOrderID,
                        ditem.Quantity,
                        Packer = admin.RealName, //分拣人
                        ditem.Weight,
                        ditem.BoxCode,
                    },
                    storage.InputID,
                    ditem.OutputID,
                    Procut = new
                    {
                        PartNumber = product.PartNumber,
                        Manufacturer = product.Manufacturer,
                        //PackageCase = product.PackageCase,
                        //Packaging = product.Packaging,
                        storage.Origin,
                        storage.DateCode,
                    }

                    #endregion
                };

            // 获取符合条件的ID            
            var ienum_myReport = iquery.ToArray();
            var tinyIds = ienum_myReport.Select(item => item.TinyOrderID).Distinct();
            var ienum_declares = linq_declares.Where(item => tinyIds.Contains(item.TinyOrderID) && item.Status == 1).ToArray();

            var ienums_boxCodes = ienum_declares.Select(item => item.BoxCode);

            var inputsID = ienum_declares.Select(item => item.InputID);
            var OutputsID = ienum_declares.Select(item => item.OutputID);

            //获取入库条件
            var linq_inputCondition = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                      where inputsID.Contains(notice.InputID)
                                      select new
                                      {
                                          key = notice.InputID,
                                          notice.Conditions
                                      };
            //获取出库条件
            var linq_outputCondition = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                       where OutputsID.Contains(notice.OutputID)
                                       select new
                                       {
                                           key = notice.OutputID,
                                           notice.Conditions
                                       };

            var ienums_condition = linq_inputCondition.ToArray().Union(linq_outputCondition.ToArray());
            //获取入库条件
            var linq_box = from box in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Boxes>()
                           where ienums_boxCodes.Contains(box.Series)
                           select new
                           {
                               Code = box.ID,
                               box.Series,
                               box.PackageType
                           };
            var ienum_box = linq_box.ToArray();

            var linq = from report in ienum_myReport
                       join merge in ienum_declares on report.TinyOrderID equals merge.TinyOrderID into merges
                       let mFirst = merges.FirstOrDefault()
                       let PackageType = ienum_box.FirstOrDefault(ibox => ibox.Series == mFirst?.BoxCode)?.PackageType ?? PackageTypes.CartonBox.GBCode
                       select new
                       {
                           report.TinyOrderID,
                           report.DeclareStatus,
                           report.EnterCode,
                           BoxingDate = report.CreateDate,
                           report.Packer,
                           PackageType = PackageType,
                           Items = merges.Select(item => new
                           {
                               Condition = ienums_condition.
                                    FirstOrDefault(condition => condition.key == (item.OutputID ?? item.InputID))?.
                                    Conditions.JsonTo<NoticeCondition>(),
                               item.Procut,
                               item.Declare,
                           })
                       };

            var ienum_linq = linq.ToArray();
            // 为了计算并添加LQuantity
            var results = ienum_linq.Select(item => new
            {
                item.TinyOrderID,
                item.DeclareStatus,
                item.EnterCode,
                item.BoxingDate,
                item.Packer,
                item.PackageType,
                TotalWeight = item.Items.Sum(d => d.Declare.Weight),
                TotalBoxCode = CgSzSortingsView.GetTotalPart(item.Items.Select(d => d.Declare.BoxCode.ToUpper().Trim()).Distinct()),
                item.Items,
            }).Where(item => item.Items.ToArray().Count() > 0);

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }



            return new
            {
                Total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                Data = results.ToArray(),
            };
        }

        #region 搜索方法

        string wareHouseID;

        ///// <summary>
        ///// 根据库房ID搜索
        ///// </summary>
        ///// <param name="warehouseID"></param>
        ///// <returns></returns>
        //public CgReportsView1 SearchByWareHouseID(string wareHouseID)
        //{
        //    this.wareHouseID = wareHouseID;
        //    var waybillView = this.IQueryable.Cast<MyReport>();

        //    if (string.IsNullOrWhiteSpace(this.wareHouseID))
        //    {
        //        throw new ArgumentNullException(nameof(wareHouseID), "参数必须要有有效值");
        //    }

        //    var linq = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
        //               join waybill in waybillView on notice.WaybillID equals waybill.ID
        //               where notice.WareHouseID == this.wareHouseID
        //               select waybill;

        //    var view = new CgReportsView1(this.Reponsitory, linq)
        //    {
        //        wareHouseID = this.wareHouseID,
        //    };

        //    return view;
        //}

        /// <summary>
        /// 根据状态查询 
        /// </summary>
        /// <param name="status">状态</param>
        /// <returns></returns>
        public CgDelcaresView SearchByStatus(TinyOrderDeclareStatus status)
        {
            var linq = this.IQueryable.Cast<MyReport>().Where(item => item.DeclareStatus == status);

            var view = new CgDelcaresView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        /// <summary>
        /// 根据包装人（装箱人）
        /// </summary>
        /// <param name="realName">装箱人真实姓名</param>
        /// <returns>装箱人的视图</returns>
        public CgDelcaresView SearchByPacker(string realName)
        {
            var myReportView = this.IQueryable.Cast<MyReport>().Where(item => item.Packer.Contains(realName) || item.AdminID == realName);

            var linq = myReportView;

            var view = new CgDelcaresView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 根据TinyOrderID搜索
        /// </summary>
        /// <param name="tinyOrderID">装箱人真实姓名</param>
        /// <returns>装箱人的视图</returns>
        public CgDelcaresView SearchByID(string tinyOrderID)
        {
            var myReportView = this.IQueryable.Cast<MyReport>().Where(item => item.TinyOrderID == tinyOrderID);

            var linq = myReportView;

            var view = new CgDelcaresView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 根据库房ID搜索
        /// </summary>
        /// <param name="txt">箱号、型号、品牌</param>
        /// <returns></returns>
        public CgDelcaresView SearchByFirst(string txt)
        {
            var adminTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>();

            var linq_declareitem =
                   from ditem in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_DeclareItem>() //最终的那个。
                   join storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>() on ditem.StorageID equals storage.ID
                   join product in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>() on storage.ProductID equals product.ID
                   join report in this.IQueryable.Cast<MyReport>() on ditem.TinyOrderID equals report.TinyOrderID
                   where product.PartNumber.Contains(txt) || product.Manufacturer.Contains(txt) || ditem.BoxCode.Contains(txt) || ditem.TinyOrderID.Contains(txt)
                   select ditem.TinyOrderID;

            var linq = from report in this.IQueryable.Cast<MyReport>()
                       join tinyId in linq_declareitem.Distinct().OrderByDescending(item => item).Take(500) on report.TinyOrderID equals tinyId
                       select report;

            var view = new CgDelcaresView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 获取制定TinyOrder的申报项
        /// </summary>
        /// <param name="arguments">TinyOrderID 数组</param>
        /// <returns>TinyOrder的申报项</returns>
        public DeclarationApply SearhDeclareByTinyOrderID(params string[] arguments)
        {
            var linq = from item in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_DeclareItem>()
                       where arguments.Contains(item.TinyOrderID)
                       select new
                       {
                           TinyOrderID = item.TinyOrderID,
                           OrderItemID = item.OrderItemID,
                           item.AdminID,
                           DeclareID = item.ID,
                       };

            //var results = from item in linq.ToArray()
            //              group item by new
            //              {
            //                  item.TinyOrderID,
            //                  item.AdminID
            //              } into groups
            //              select new DeclareData
            //              {
            //                  AdminID = groups.Key.AdminID,
            //                  TinyOrderID = groups.Key.TinyOrderID,
            //                  DeclareIDs = groups.Select(item => item.DeclareID).ToList()
            //              };


            var results = from item in linq.ToArray()
                          select new DeclarationApplyItem
                          {
                              AdminID = item.AdminID,
                              TinyOrderID = item.TinyOrderID,
                              DeclareID = item.DeclareID
                          };

            return new DeclarationApply
            {
                Items = results.ToList()
            };
        }

        #endregion


        /// <summary>
        /// 更新Input
        /// </summary>
        /// <param name="mups">进项信息 数组</param>
        public void UpdateInput(params MyUpdatePut[] mups)
        {
            PvWmsRepository repository = this.Reponsitory;
            var tSql = repository.TSql;

            var uniques = mups.Select(item => item.Unique);

            var linq = from entity in repository.ReadTable<Layers.Data.Sqls.PvWms.Inputs>()
                       where uniques.Contains(entity.ID)
                       select new
                       {
                           entity.ID,
                           entity.TinyOrderID,
                           entity.ItemID
                       };
            var inputs = linq.ToArray();

            var logsDeclareItemView = from entity in repository.GetTable<Layers.Data.Sqls.PvWms.Logs_DeclareItem>()
                                      join storage in repository.GetTable<Layers.Data.Sqls.PvWms.Storages>() on entity.StorageID equals storage.ID
                                      select new
                                      {
                                          entity.ID,
                                          entity.OrderItemID,
                                          entity.TinyOrderID,
                                          entity.Quantity,
                                          entity.StorageID,
                                          storage.InputID,
                                      };

            foreach (var mup in mups)
            {
                var input = inputs.Single(item => item.ID == mup.Unique);
                //// 添加调试日志
                //var changeMsg = $"要更改的Input的ID为{input.ID}, TinyOrderID为 {input.TinyOrderID}, ItemID为{input.ItemID}\r\n";

                tSql.Update<Layers.Data.Sqls.PvWms.Inputs>(new
                {
                    ItemID = mup.ItemID,
                    TinyOrderID = mup.TinyOrderID,
                    Currency = (int)Enum.Parse(typeof(Currency), mup.Currency),
                    UnitPrice = mup.Price
                }, item => item.ID == mup.Unique);

                // 添加调试日志
                //var inputAfter = repository.ReadTable<Layers.Data.Sqls.PvWms.Inputs>().Single(item => item.ID == mup.Unique);
                //changeMsg += $"更改后的Input的ID为{inputAfter.ID}, TinyOrderID为 {inputAfter.TinyOrderID}, ItemID为{inputAfter.ItemID}";
                //LitTools.Current["更新订单"].Log("调用：UpdateInput方法影响结果: \r\n" + changeMsg);

                //var ldis = repository.GetTable<Layers.Data.Sqls.PvWms.Logs_DeclareItem>()
                //         .Where(item => item.TinyOrderID == input.TinyOrderID && item.OrderItemID == input.ItemID)
                //         .Select(item => item.ID);

                var ldis = logsDeclareItemView.Where(item => item.InputID == mup.Unique).Select(item => item.ID);                         

                //更新申报日志
                tSql.Update<Layers.Data.Sqls.PvWms.Logs_DeclareItem>(new
                {
                    TinyOrderID = mup.TinyOrderID,
                    OrderItemID = mup.ItemID,
                }, item => ldis.Contains(item.ID));

                ////更新申报日志
                //tSql.Update<Layers.Data.Sqls.PvWms.Logs_DeclareItem>(new
                //{
                //    TinyOrderID = mup.TinyOrderID,
                //}, item => item.TinyOrderID == input.TinyOrderID && item.OrderItemID == input.ItemID);
            }
        }

        /// <summary>
        /// 更新ouput
        /// </summary>
        /// <param name="mups">销项信息 数组</param>
        public void UpdateOutput(params MyUpdatePut[] mups)
        {
            var uniques = mups.Select(item => item.Unique);

            var linq = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>()
                       where uniques.Contains(entity.ID)
                       select new
                       {
                           entity.ID,
                           entity.TinyOrderID,
                           entity.ItemID
                       };
            var outputs = linq.ToArray();

            foreach (var mup in mups)
            {
                var output = outputs.Single(item => item.ID == mup.Unique);

                this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Outputs>(new
                {
                    ItemID = mup.ItemID,
                    TinyOrderID = mup.TinyOrderID,
                    Currency = (int)Enum.Parse(typeof(Currency), mup.Currency),
                    Price = mup.Price
                }, item => item.ID == mup.Unique);

                //更新申报日志
                this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Logs_DeclareItem>(new
                {
                    TinyOrderID = mup.TinyOrderID,
                }, item => item.TinyOrderID == output.TinyOrderID && item.OrderItemID == output.ItemID);

            }
        }

        /// <summary>
        /// 更新Input信息
        /// </summary>
        /// <param name="inputID"></param>
        /// <param name="itemID"></param>
        /// <param name="tinyOrderID"></param>
        [Obsolete("经荣检要求：已经废弃，但是想不通原先要求为什么是这样？")]
        public void UpdateInput(string inputID, string itemID, string tinyOrderID,
            Currency? currency, decimal? price)
        {
            var linq = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>()
                       where entity.ID == inputID
                       select new
                       {
                           entity.TinyOrderID,
                           entity.ItemID
                       };
            var input = linq.SingleOrDefault();

            if (input == null)
            {
                throw new Exception("严重的错误！");
            }

            //更新Input
            this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Inputs>(new
            {
                ItemID = itemID,
                TinyOrderID = tinyOrderID,
                Currency = currency,
                UnitPrice = price
            }, item => item.ID == inputID);

            //更新申报日志
            this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Logs_DeclareItem>(new
            {
                TinyOrderID = tinyOrderID,
            }, item => item.TinyOrderID == input.TinyOrderID && item.OrderItemID == input.ItemID);

        }

        /// <summary>
        /// 更新Output信息
        /// </summary>
        /// <param name="outputID"></param>
        /// <param name="itemID"></param>
        /// <param name="tinyOrderID"></param>
        [Obsolete("经荣检要求：已经废弃，但是想不通原先要求为什么是这样？")]
        public void UpdateOutput(string outputID, string itemID, string tinyOrderID,
            Currency? currency, decimal? price)
        {
            var linq = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>()
                       where entity.ID == outputID
                       select new
                       {
                           entity.TinyOrderID,
                           entity.ItemID
                       };
            var output = linq.SingleOrDefault();

            if (output == null)
            {
                throw new Exception("严重的错误！");
            }

            this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Outputs>(new
            {
                ItemID = itemID,
                TinyOrderID = tinyOrderID,
                Price = price,
                Currency = currency
            }, item => item.ID == outputID);

            //更新申报日志
            this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Logs_DeclareItem>(new
            {
                TinyOrderID = tinyOrderID,
            }, item => item.TinyOrderID == output.TinyOrderID && item.OrderItemID == output.ItemID);
        }

        /// <summary>
        /// 申报
        /// </summary>
        /// <param name="arry">小订单ID</param>
        /// <remarks>
        /// 经过与董建讨论，决定增加本方法
        /// 经过沈忱要求，决定增加累加订单状态的功能
        /// </remarks>
        public void Delcaring(string adminID, params string[] arry)
        {
            #region 正常检测通过的TinyOrderID 进行报关申请处理, 默认认为通过装箱的均已是检查验证通过的
            if (arry.Length > 0)
            {
                var linq_output = from ouput in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>()
                                  where arry.Contains(ouput.TinyOrderID)
                                  select ouput.OrderID;

                var linq_input = from input in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>()
                                 where arry.Contains(input.TinyOrderID)
                                 select input.OrderID;

                var ordersID = linq_output.ToArray().Concat(linq_input.ToArray());

                /* 根据荣捡和沈忱的新要求, 在申报后,只是进行了申报操作, 具体的状态还是在荣捡处修改, 所以删除
                using (var pvcenterReponsitory = new PvCenterReponsitory())
                {
                    foreach (var orderID in ordersID)
                    {
                        pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                        {
                            IsCurrent = false,
                        }, item => item.Type == (int)OrderStatusType.MainStatus && item.MainID == orderID);

                        pvcenterReponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder
                        {
                            ID = Guid.NewGuid().ToString(),
                            MainID = orderID,
                            Type = (int)OrderStatusType.MainStatus, //订单支付状态，  
                            Status = (int)CgOrderStatus.待发货,
                            CreateDate = DateTime.Now,
                            CreatorID = adminID,
                            IsCurrent = true,
                        });
                    }
                }
                */

                this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Logs_Declare>(new
                {
                    Status = TinyOrderDeclareStatus.Declaring
                }, item => arry.Contains(item.TinyOrderID));
            }
            #endregion

        }

        #region Helper Class
        /// <summary>
        /// 符合Sorting视图头部定义的内部类
        /// </summary>
        private class MyReport
        {
            public string ID { get; set; }
            public string TinyOrderID { get; set; }
            public TinyOrderDeclareStatus DeclareStatus { get; set; }

            public string EnterCode { get; set; }

            public DateTime CreateDate { get; set; }

            public string Packer { get; set; }

            public string AdminID { get; set; }

        }

        /// <summary>
        /// 修改进项与销项信息
        /// </summary>
        public class MyUpdatePut
        {
            /// <summary>
            /// 唯一性
            /// </summary>
            /// <remarks>
            /// InputID 或  OutputID
            /// </remarks>
            public string Unique { get; set; }
            /// <summary>
            /// 订单项ID
            /// </summary>
            public string ItemID { get; set; }
            /// <summary>
            /// 小订单ID
            /// </summary>
            public string TinyOrderID { get; set; }

            /// <summary>
            /// 币种
            /// </summary>
            public string Currency { get; set; }
            /// <summary>
            /// 核算单价
            /// </summary>
            public decimal? Price { get; set; }

        }

        /// <summary>
        /// 报关申请数据要求
        /// </summary>
        public class DeclarationApply
        {
            /// <summary>
            /// 报关申请内容
            /// </summary>
            public List<DeclarationApplyItem> Items { get; set; }
        }

        /// <summary>
        /// 一个分拣/拣货
        /// </summary>
        public class DeclarationApplyItem
        {
            /// <summary>
            /// 小订单ID
            /// </summary>
            public string TinyOrderID { get; set; }

            /// <summary>
            /// 操作人ID
            /// </summary>
            public string AdminID { get; set; }

            /// <summary>
            /// 分拣/拣货ID
            /// </summary>
            public string DeclareID { get; set; }
        }

        /// <summary>
        /// 报关申请数据要求
        /// </summary>
        private class DeclareData
        {
            /// <summary>
            /// 小订单ID
            /// </summary>
            public string TinyOrderID { get; set; }

            /// <summary>
            /// 操作人ID
            /// </summary>
            public string AdminID { get; set; }

            /// <summary>
            /// 分拣/拣货ID
            /// </summary>
            public List<string> DeclareIDs { get; set; }
        }

        #endregion
    }
}
