using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Needs.Utils.Descriptions;

namespace Needs.Ccs.Services.Views.DeclarationNotice
{
    public partial class DecNoticeListViewOpmz : QueryView<Models.DecNoticeListModel, ScCustomsReponsitory>
    {
        public DecNoticeListViewOpmz()
        {
        }

        protected DecNoticeListViewOpmz(ScCustomsReponsitory reponsitory, IQueryable<DecNoticeListModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Models.DecNoticeListModel> GetIQueryable()
        {
            var declarationNotices = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarationNotices>();
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();
            var decNoticeVoyages = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecNoticeVoyages>();
            var voyages = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Voyages>();
            var orderVoyages = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderVoyages>();
            var consigneeAddress = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderConsignees>();

            var iQuery = from declarationNotice in declarationNotices
                         join order in orders on declarationNotice.OrderID equals order.ID
                         join consignee in consigneeAddress on declarationNotice.OrderID equals consignee.OrderID
                         join client in clients on order.ClientID equals client.ID
                         join company in companies on client.CompanyID equals company.ID
                         where declarationNotice.Status == (int)Enums.DeclareNoticeStatus.UnDec
                             && order.OrderStatus == (int)Enums.OrderStatus.QuoteConfirmed
                             && order.Status == (int)Enums.Status.Normal
                             && client.Status == (int)Enums.Status.Normal
                             && company.Status == (int)Enums.Status.Normal
                         select new DecNoticeListModel
                         {
                             ID = declarationNotice.ID,
                             DecNoticeID = declarationNotice.ID,
                             OrderID = order.ID,
                             CreateDate = declarationNotice.CreateDate,
                             Currency = order.Currency,
                             ClientID = client.ID,
                             ClientName = company.Name,
                             ClientType = (Enums.ClientType)client.ClientType,
                             CompanyID = company.ID,
                             CreateDeclareAdminID = declarationNotice.CreateDeclareAdminID,
                             ConsigneeAddress = consignee.Address
                         };
            return iQuery;
        }

        /// <summary>
        /// 为避免重复使用正则表达式用切割
        /// </summary>
        Regex regex_number = new Regex(@"^(\D*)(\d+)(.*)$", RegexOptions.Singleline);
        /// <summary>
        /// 填充箱号
        /// </summary>
        /// <param name="boxcodes"></param>
        /// <returns></returns>
        /// <remarks>
        /// 配合hash 自动排重
        /// </remarks>
        int FillBoxcode(IEnumerable<string> boxcodes, int companyType)
        {
            if (companyType == (int)Needs.Ccs.Services.Enums.CompanyTypeEnums.Inside)
            {
                int count = 0;
                try
                {
                    List<string> pres = new List<string>();
                    foreach (var code in boxcodes)
                    {
                        string[] codes = code.Split('-');
                        if (!pres.Contains(codes[0].ToLower()))
                        {
                            pres.Add(codes[0].ToLower());
                        }
                    }

                    foreach (var pre in pres)
                    {
                        List<string> cases = new List<string>();
                        var preRelated = boxcodes.Where(t => t.ToLower().StartsWith(pre)).ToList();
                        foreach (var caseno in preRelated)
                        {
                            string[] casenos = caseno.Split('-');
                            if (!cases.Contains(casenos[2]))
                            {
                                cases.Add(casenos[2]);
                            }
                        }
                        count += cases.Count();
                    }
                    return count;
                }
                catch (Exception ex)
                {
                    ex.CcsLog("大赢家内单计算出错");
                    return count;
                }
            }
            else
            {
                HashSet<string> sets = new HashSet<string>();

                foreach (var code in boxcodes)
                {
                    string boxcode = code;

                    MatchCollection kuohaoResults = Regex.Matches(code, @"\(.*?\)", RegexOptions.Singleline);
                    if (kuohaoResults.Count > 0)
                    {
                        //生成不要的下标号 exceptIndexes
                        List<int> exceptIndexes = new List<int>();
                        for (int i = 0; i < kuohaoResults.Count; i++)
                        {
                            if (kuohaoResults[i].Success)
                            {
                                exceptIndexes.AddRange(Enumerable.Range(kuohaoResults[i].Index, kuohaoResults[i].Length).ToArray());
                            }
                        }
                        //得到 newChars
                        char[] originChars = boxcode.ToCharArray();
                        List<char> newChars = new List<char>();
                        for (int i = 0; i < originChars.Length; i++)
                        {
                            if (!exceptIndexes.Contains(i))
                            {
                                newChars.Add(originChars[i]);
                            }
                        }

                        boxcode = new string(newChars.ToArray());
                    }

                    var splits = boxcode.Split('-');
                    if (splits.Length == 1)
                    {
                        sets.Add(regex_number.Match(splits.First()).Groups[0].Value.Trim());
                        continue;
                    }

                    //以组的方式主要是利用切断
                    var prex = regex_number.Match(splits.First()).Groups[1].Value;
                    var first = int.Parse(regex_number.Match(splits.First()).Groups[2].Value);
                    var last = int.Parse(regex_number.Match(splits.Last()).Groups[2].Value);

                    //这样写主要就是为可以报错！
                    for (int index = first; index <= last; index++)
                    {
                        sets.Add(prex + index.ToString());
                    }
                }

                return sets.Count();
            }

        }

        int FillBoxcodeNew(IEnumerable<string> boxcodes, int companyType)
        {
            if (companyType == (int)Needs.Ccs.Services.Enums.CompanyTypeEnums.Inside)
            {
                return new CalculateContext(Enums.CompanyTypeEnums.Inside, boxcodes.ToList()).CalculatePacks();
            }
            else
            {
                return new CalculateContext(Enums.CompanyTypeEnums.Icgoo, boxcodes.ToList()).CalculatePacks();
            }
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Models.DecNoticeListModel> iquery = this.IQueryable.Cast<Models.DecNoticeListModel>().OrderByDescending(item => item.ID);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myDeclares = iquery.ToArray();

            //获取申报的ID
            var declaresID = ienum_myDeclares.Select(item => item.DecNoticeID).ToArray();

            //获取订单的ID
            var ordersID = ienum_myDeclares.Select(item => item.OrderID).ToArray();

            //客户ID
            var clientID = ienum_myDeclares.Select(item => item.ClientID).ToArray();

            var decNoticeVoyagesView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecNoticeVoyages>();
            var voyagesView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Voyages>();
            var orderVoyagesView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderVoyages>();
            var declarationNoticeItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarationNoticeItems>();
            
            var sortingViews = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>();

            var orderItemViews = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
            var icgooOrderMapView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.IcgooOrderMap>();
            //原产地税则
            var originsATRateView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OriginsATRateTopView>();
            var orderItemCategoryViews = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>();

            #region 获取运输批次

            var linq_dnVoyage = from dnVoyage in decNoticeVoyagesView
                                join voyage in voyagesView on dnVoyage.VoyageID equals voyage.ID
                                where dnVoyage.Status == (int)Enums.Status.Normal && declaresID.Contains(dnVoyage.DecNoticeID)
                                select new
                                {
                                    dnVoyage.DecNoticeID,
                                    VoyageID = voyage.ID,
                                    VoyageType = (Enums.VoyageType)voyage.Type
                                };

            var ienums_dnVoyage = linq_dnVoyage.ToArray();

            #endregion

            #region 获取箱号

            //隐式 Task ThreadPool 等都无法做到精确控制
            // region 中 最好写成显示多线程的

            var linq_declarationNoticeItems = from declarationNoticeItem in declarationNoticeItems
                                              where declaresID.Contains(declarationNoticeItem.DeclarationNoticeID)
                                              select new
                                              {
                                                  declarationNoticeItem.DeclarationNoticeID,
                                                  declarationNoticeItem.SortingID
                                              };

            var ienums_dnItem = linq_declarationNoticeItems.ToArray();
            //var ienums_sortings = ienums_dnItem.Select(item => item.SortingID).ToArray();

            //此处查询关联有问题 ryan 20211215
            //var linq_box_origin = from sorting in deliveriesTopViews
            //                      join declarationNoticeItem in linq_declarationNoticeItems on sorting.UnqiueID equals declarationNoticeItem.SortingID
            //                      //where ienums_sortings.Contains(sorting.UnqiueID)
            //                      select new
            //                      {
            //                          SortingID = sorting.UnqiueID,
            //                          sorting.OrderItemID,
            //                          sorting.BoxCode,
            //                          sorting.Weight,
            //                          sorting.Origin
            //                      };

            var linq_box_origin = from sorting in sortingViews
                                  where ordersID.Contains(sorting.OrderID) && sorting.Status == (int)Enums.Status.Normal
                                  select new
                                  {
                                      sorting.OrderID,
                                      sorting.OrderItemID,
                                      sorting.BoxIndex,
                                      sorting.GrossWeight                               
                                  };

          


            var groups_box = from box in linq_box_origin
                             group box by box.OrderID into groups
                             select new
                             {
                                 OrderID = groups.Key,
                                 Weight = groups.Select(item => item.GrossWeight ).Sum()
                             };


            #endregion

            #region 订单项目价格

            var orderItemsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();

            var linq_orderItems = from item in orderItemsView
                                  where item.Status == (int)Enums.Status.Normal && ordersID.Contains(item.OrderID)
                                  select new
                                  {
                                      OrderItemID = item.ID,
                                      item.Model,
                                      item.OrderID,
                                      item.Quantity,
                                      item.TotalPrice,
                                      item.GrossWeight,
                                      item.Origin
                                  };

            var ienums_orderItems = linq_orderItems.ToArray();
            var groups_orderItems = from item in ienums_orderItems
                                    group item by item.OrderID into groups
                                    select new
                                    {
                                        OrderID = groups.Key,
                                        TotalDeclarePrice = groups.Sum(t => t.TotalPrice),
                                        TotalQty = groups.Sum(t => t.Quantity),
                                    };

            #endregion

            #region 特殊类型

            //获取特殊类型？可能是这样叫做
            var linqs_special = from oVoyage in orderVoyagesView
                                where oVoyage.Status == (int)Enums.Status.Normal
                                select new
                                {
                                    oVoyage.OrderID,
                                    oVoyageType = (Enums.OrderSpecialType)oVoyage.Type
                                };

            var ienums_special = linqs_special.ToList();

            #endregion

            #region 敏感产地

            var SenOriginStr = System.Configuration.ConfigurationManager.AppSettings["SenOrigin"];
            var SenList = SenOriginStr.Split('|');

            #endregion

            #region 原产地加征 

            //当前所有itemID
            var itemIDs = ienums_orderItems.Select(t => t.OrderItemID).Distinct();
            //当前item对应的归类信息
            //var ienums_cate = orderItemCategoryViews.Where(t => itemIDs.Contains(t.OrderItemID)).ToArray();
            var ienums_cate = (from cate in orderItemCategoryViews
                               join item in orderItemViews on cate.OrderItemID equals item.ID
                               where ordersID.Contains(item.OrderID)
                               select new
                               {
                                   cate.OrderItemID,
                                   cate.HSCode,
                               }).ToArray();

            // 产地取OrderItem中的产地，OrderItem的产地是正确的，不正确的话库房会更改，更改结果同步到OrderItem
            //当前item的产地信息
            //var ienums_origin = ienums_box_origin.ToArray();

            var linqs_orderitemCategory = from item in ienums_orderItems
                                          join cate in ienums_cate on item.OrderItemID equals cate.OrderItemID
                                          //join origin in ienums_origin on item.OrderItemID equals origin.OrderItemID
                                          //where cate.Status == (int)Enums.Status.Normal && itemIDs.Contains(cate.OrderItemID)
                                          select new
                                          {
                                              item.OrderID,
                                              cate.OrderItemID,
                                              cate.HSCode,
                                              item.Origin
                                          };
            //当前item的 HSCode、Origin、OrderID
            var ienums_orderitemCategory = linqs_orderitemCategory.ToArray();

            //当前日期
            var dateNow = DateTime.Now.Date;
            //原产地加征税则
            var ienums_originrate = originsATRateView.ToArray();
            //循环订单
            foreach (var orderid in ordersID)
            {
                var isOri = false;
                var isSen = false;
                //遍历订单项
                foreach (var item in ienums_orderitemCategory.Where(t => t.OrderID == orderid))
                {
                    //判断当前型号是否原产地加征
                    if (ienums_originrate.Any(t => t.TariffID == item.HSCode && t.Origin == item.Origin && t.StartDate <= dateNow && (t.EndDate > dateNow || t.EndDate == null)))
                    {
                        //有加征，将订单加入特殊类型Array
                        ienums_special.Add(new { OrderID = orderid, oVoyageType = Enums.OrderSpecialType.OriginATRate });
                        isOri = true;
                    }

                    //判断是否敏感产地
                    if (SenList.Contains(item.Origin))
                    {
                        //有敏感产地，将订单加入特殊类型Array
                        ienums_special.Add(new { OrderID = orderid, oVoyageType = Enums.OrderSpecialType.SenOrigin });
                        isSen = true;
                    }

                    if (isOri && isSen)
                    {
                        break;//退出当前循环
                    }

                }
            };

            #endregion

            #region icgoo

            var icgooOrderView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.IcgooOrderMap>();

            var linq_icgoo = from map in icgooOrderView
                             where ordersID.Contains(map.OrderID)
                             select new
                             {
                                 map.OrderID,
                                 map.IcgooOrder,
                                 map.CompanyType
                             };
            var ienums_icgoo = linq_icgoo.ToArray();

            #endregion

            #region 跟单员

            var clientAdminsView = new ClientAdminsView(this.Reponsitory);

            var linq_declarant = from map in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAdmins>()
                                 join admin in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>() on map.AdminID equals admin.ID
                                 where clientID.Contains(map.ClientID)
                                    && map.Type == (int)Enums.ClientAdminType.Merchandiser
                                    && map.Status == (int)Enums.Status.Normal
                                 select new
                                 {
                                     map.ClientID,
                                     RealName = admin.RealName,
                                 };
            var ienums_declarant = linq_declarant.ToArray();

            #endregion

            #region 制单人

            var adminsTopView2 = new AdminsTopView2(this.Reponsitory);

            var createDeclareAdminIDs = ienum_myDeclares.Select(item => item.CreateDeclareAdminID);

            var linq_createDeclareAdmin = from admin in adminsTopView2
                                          where createDeclareAdminIDs.Contains(admin.OriginID)
                                          group admin by new { admin.OriginID } into g
                                          select new
                                          {
                                              CreateDeclareAdminID = g.Key.OriginID,
                                              CreateDeclareAdminName = g.FirstOrDefault().RealName,
                                          };

            var ienums_createDeclareAdmin = linq_createDeclareAdmin.ToArray();

            #endregion

            var ienums_linq = from declare in ienum_myDeclares
                              join _voyage in ienums_dnVoyage on declare.DecNoticeID equals _voyage.DecNoticeID into voyages
                              from voyage in voyages.DefaultIfEmpty()
                              join _createDeclareAdmin in ienums_createDeclareAdmin on declare.CreateDeclareAdminID equals _createDeclareAdmin.CreateDeclareAdminID
                              into ienums_createDeclareAdmin2
                              from createDeclareAdmin in ienums_createDeclareAdmin2.DefaultIfEmpty()
                              join special in ienums_special on declare.OrderID equals special.OrderID into specials
                              let ogroups_orderItems = groups_orderItems.Single(item => item.OrderID == declare.OrderID)
                              let oienums_orderItems = ienums_orderItems.Where(item => item.OrderID == declare.OrderID)
                              let icgoo = ienums_icgoo.SingleOrDefault(item => item.OrderID == declare.OrderID)
                              let declarant = ienums_declarant.SingleOrDefault(item => item.ClientID == declare.ClientID)
                              select new Models.DecNoticeListModel
                              {
                                  DecNoticeID = declare.DecNoticeID,
                                  OrderID = declare.OrderID,
                                  CreateDate = declare.CreateDate,
                                  Currency = declare.Currency,

                                  //TotalDeclarePriceDisplay是页面上的选择器中的字段

                                  ClientID = declare.ClientID,
                                  ClientName = declare.ClientName,
                                  ClientType = declare.ClientType,
                                  CompanyID = declare.CompanyID,

                                  //运输相关
                                  VoyageID = voyage?.VoyageID,
                                  VoyageType = (int)(voyage?.VoyageType ?? Enums.VoyageType.Error),
                                  VoyageTypeName = (voyage?.VoyageType ?? Enums.VoyageType.Error).GetDescription(),

                                  //一次计数
                                  PackNo = this.FillBoxcodeNew(linq_box_origin.Where(item => item.OrderID == declare.OrderID).Select(item => item.BoxIndex), icgoo == null ? 0 : icgoo.CompanyType),
                                  PackBox = string.Join(";", linq_box_origin.Where(item => item.OrderID == declare.OrderID).Select(item => item.BoxIndex)),
                                  /*实验使用*/ //VoyageTypeName = string.Join(",", ienums_box.Where(item => item.DeclarationNoticeID == declare.DecNoticeID).Select(item => item.BoxCode)),

                                  //特殊类型相关
                                  IsCharterBus = specials.Any(item => item.oVoyageType == Enums.OrderSpecialType.CharterBus),
                                  IsHighValue = specials.Any(item => item.oVoyageType == Enums.OrderSpecialType.HighValue),
                                  IsInspection = specials.Any(item => item.oVoyageType == Enums.OrderSpecialType.Inspection),
                                  IsQuarantine = specials.Any(item => item.oVoyageType == Enums.OrderSpecialType.Quarantine),
                                  IsCCC = specials.Any(item => item.oVoyageType == Enums.OrderSpecialType.CCC),
                                  IsOrigin = specials.Any(item => item.oVoyageType == Enums.OrderSpecialType.OriginATRate),
                                  IsSenOrigin = specials.Any(item => item.oVoyageType == Enums.OrderSpecialType.SenOrigin),

                                  TotalDeclarePrice = (ogroups_orderItems.TotalDeclarePrice).ToRound(4), //* ConstConfig.TransPremiumInsurance
                                  TotalQty = ogroups_orderItems.TotalQty,
                                  TotalModelQty = oienums_orderItems.Select(item => item.OrderItemID).Distinct().Count(),
                                  TotalGrossWeight = groups_box.SingleOrDefault(item => item.OrderID == declare.OrderID)?.Weight ?? 0m,
                                  IcgooOrder = icgoo?.IcgooOrder,
                                  DeclarantName = declarant?.RealName,

                                  CreateDeclareAdminID = declare.CreateDeclareAdminID,
                                  CreateDeclareAdminName = createDeclareAdmin != null ? createDeclareAdmin.CreateDeclareAdminName : "",

                                  //纯为了显示的字段，向下补充。
                                  ConsigneeAddress = declare.ConsigneeAddress,

                              };

            var results = ienums_linq;

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            Func<Needs.Ccs.Services.Models.DecNoticeListModel, object> convert = declareNotice => new
            {
                NoticeID = declareNotice.DecNoticeID,
                OrderID = declareNotice.OrderID,
                ClientName = declareNotice.ClientName,
                ClientType = declareNotice.ClientType.GetDescription(),
                ClientID = declareNotice.ClientID,
                CreateDate = declareNotice.CreateDate.ToShortDateString(),
                DeclarantName = declareNotice.DeclarantName,
                OrderSpecialTypeName = CalcOrderSpecialTypeName(declareNotice),
                VoyageID = declareNotice.VoyageID,
                VoyageTypeName = declareNotice.VoyageTypeName,
                PackNo = declareNotice.PackNo,
                TotalDeclarePriceDisplay = (declareNotice.TotalDeclarePrice * ConstConfig.TransPremiumInsurance).ToRound(4) + " (" + declareNotice.Currency + ")",
                TotalDeclarePrice = (declareNotice.TotalDeclarePrice * ConstConfig.TransPremiumInsurance).ToRound(4),
                TotalQty = declareNotice.TotalQty,
                TotalModelQty = declareNotice.TotalModelQty,
                TotalGrossWeight = declareNotice.TotalGrossWeight,
                CompanyID = declareNotice.CompanyID,
                IcgooOrder = declareNotice.IcgooOrder,
                CreateDeclareAdminID = declareNotice.CreateDeclareAdminID,
                CreateDeclareAdminName = declareNotice.CreateDeclareAdminName,
                ConsigneeAddress = declareNotice.ConsigneeAddress,
                PackBox = declareNotice.PackBox
            };


            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        private string CalcOrderSpecialTypeName(Needs.Ccs.Services.Models.DecNoticeListModel decNoticeListModel)
        {
            List<Enum> orderSpecialTypeEnumsList = new List<Enum>();
            if (decNoticeListModel.IsCharterBus)
            {
                orderSpecialTypeEnumsList.Add(Enums.OrderSpecialType.CharterBus);
            }

            if (decNoticeListModel.IsHighValue)
            {
                orderSpecialTypeEnumsList.Add(Enums.OrderSpecialType.HighValue);
            }

            if (decNoticeListModel.IsInspection)
            {
                orderSpecialTypeEnumsList.Add(Enums.OrderSpecialType.Inspection);
            }

            if (decNoticeListModel.IsQuarantine)
            {
                orderSpecialTypeEnumsList.Add(Enums.OrderSpecialType.Quarantine);
            }

            if (decNoticeListModel.IsCCC)
            {
                orderSpecialTypeEnumsList.Add(Enums.OrderSpecialType.CCC);
            }

            if (decNoticeListModel.IsOrigin)
            {
                orderSpecialTypeEnumsList.Add(Enums.OrderSpecialType.OriginATRate);
            }

            if (decNoticeListModel.IsSenOrigin)
            {
                orderSpecialTypeEnumsList.Add(Enums.OrderSpecialType.SenOrigin);
            }

            Enum[] orderSpecialTypeEnumsArray = orderSpecialTypeEnumsList.ToArray();

            return orderSpecialTypeEnumsArray.GetEnumsDescriptions("|");
        }

        #region Search Helper 

        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="orderID">订单ID</param>
        /// <returns>视图</returns>
        public DecNoticeListViewOpmz SearchByOrderID(string orderID)
        {
            var linq = from query in this.IQueryable
                       where query.OrderID.Contains(orderID)
                       select query;

            var view = new DecNoticeListViewOpmz(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 查询客户公司名
        /// </summary>
        /// <param name="name">客户公司名</param>
        /// <returns>视图</returns>
        public DecNoticeListViewOpmz SearchByClientName(string name)
        {
            var linq = from query in this.IQueryable
                       where query.ClientName.Contains(name)
                       select query;

            var view = new DecNoticeListViewOpmz(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 查询运输批次
        /// </summary>
        /// <param name="name">运输批次</param>
        /// <returns>视图</returns>
        public DecNoticeListViewOpmz SearchByVoyageID(string id)
        {
            var decNoticeVoyagesView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecNoticeVoyages>();
            var voyagesView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Voyages>();
            var myQuery = this.IQueryable;

            var linqs_dnVoyagesID = from dnVoyage in decNoticeVoyagesView
                                    join voyage in voyagesView on dnVoyage.VoyageID equals voyage.ID
                                    where dnVoyage.Status == (int)Enums.Status.Normal && voyage.ID.Contains(id)
                                    select dnVoyage.DecNoticeID;

            var distincts = linqs_dnVoyagesID.Distinct();

            var linq = from query in myQuery
                       join item in distincts on query.DecNoticeID equals item
                       select query;

            var view = new DecNoticeListViewOpmz(this.Reponsitory, linq);
            return view;
        }


        /// <summary>
        /// 运输类型查询
        /// </summary>
        /// <param name="realName">运输类型</param>
        /// <returns>视图</returns>
        public DecNoticeListViewOpmz SearchByVoyageType(Enums.VoyageType type)
        {
            var decNoticeVoyagesView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecNoticeVoyages>();
            var voyagesView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Voyages>();

            var myQuery = this.IQueryable;

            var linqs_dnVoyagesID = from dnVoyage in decNoticeVoyagesView
                                    join voyage in voyagesView on dnVoyage.VoyageID equals voyage.ID
                                    where dnVoyage.Status == (int)Enums.Status.Normal && voyage.Type == (int)type
                                    select dnVoyage.DecNoticeID;

            var distincts = linqs_dnVoyagesID.Distinct();

            var linq = from query in myQuery
                       join item in distincts on query.DecNoticeID equals item
                       select query;

            var view = new DecNoticeListViewOpmz(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 是否特殊类型
        /// </summary>
        /// <param name="realName">bool</param>
        /// <returns>视图</returns>
        public DecNoticeListViewOpmz SearchBySpecialType(params Enums.OrderSpecialType[] stypes)
        {
            var orderVoyagesView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderVoyages>();

            var myQuery = this.IQueryable;
            var iStypes = stypes.Select(item => (int)item);

            var linqs_ordersID = from oVoyage in orderVoyagesView
                                 where oVoyage.Status == (int)Enums.Status.Normal && iStypes.Contains(oVoyage.Type)
                                 select oVoyage.OrderID;

            var distincts = linqs_ordersID.Distinct();

            var linq = from query in myQuery
                       join item in distincts on query.OrderID/*发现！*/ equals item
                       select query;

            var view = new DecNoticeListViewOpmz(this.Reponsitory, linq);
            return view;
        }

        public DecNoticeListViewOpmz SearchByDeclareCreatorID(string adminID)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDeclareAdminID == adminID
                       select query;

            var view = new DecNoticeListViewOpmz(this.Reponsitory, linq);
            return view;
        }

        #endregion
    }
}
