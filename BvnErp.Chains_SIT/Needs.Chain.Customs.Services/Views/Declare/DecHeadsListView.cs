using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{

    public class DecHeadsListView : UniqueView<DecHeadList, ScCustomsReponsitory>
    {
        public DecHeadsListView()
        {
        }

        internal DecHeadsListView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<DecHeadList> GetIQueryable()
        {
            var adminView = new AdminsTopView(this.Reponsitory);
            var billsView = new ManifestConsignmentsView(this.Reponsitory);

            return from head in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                   join admin in adminView on head.InputerID equals admin.ID into g
                   from temp in g.DefaultIfEmpty()
                   orderby head.CreateTime descending
                   select new DecHeadList
                   {
                       ID = head.ID,
                       ContrNo = head.ContrNo,
                       OrderID = head.OrderID,
                       BillNo = head.BillNo,
                       EntryId = head.EntryId,
                       PreEntryId = head.PreEntryId,
                       AgentName = head.AgentName,
                       ConsignorName = head.ConsignorName,
                       ConsigneeName = head.ConsigneeName,
                       IsQuarantine = head.IsQuarantine.Value,
                       IsInspection = head.IsInspection,
                       CreateTime = head.CreateTime,
                       InputerID = temp.RealName,
                       Status = head.CusDecStatus,
                       VoyageID = head.VoyNo,
                       Transformed = billsView.Any(t => t.ID == head.BillNo),
                       PackNo = head.PackNo,
                       GrossWeight = head.GrossWt,
                       IsSuccess = head.IsSuccess,
                       CustomsPortCode = head.CustomMaster
                   };
        }
    }

    /// <summary>
    /// 报关单--草稿
    /// </summary>
    public class DecHeadsDraftListView : UniqueView<DecHeadList, ScCustomsReponsitory>
    {
        public DecHeadsDraftListView()
        {
        }

        internal DecHeadsDraftListView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<DecHeadList> GetIQueryable()
        {
            var adminView = new AdminsTopView(this.Reponsitory);
            var billsView = new ManifestConsignmentsView(this.Reponsitory);
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();

            var decHeadSpecialTypes = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeadSpecialTypes>();
            var decNoticeVoyages = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecNoticeVoyages>();
            var voyages = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Voyages>();

            return from head in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                   join admin in adminView on head.InputerID equals admin.ID into g
                   from temp in g.DefaultIfEmpty()
                   where head.CusDecStatus == MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.Draft)

                   join order in orders
                        on new { OrderID = head.OrderID, OrderStatus = (int)Enums.Status.Normal }
                        equals new { OrderID = order.ID, OrderStatus = order.Status }
                        into orders2
                   from order in orders2.DefaultIfEmpty()
                   join client in clients
                        on new { ClientID = order.ClientID, ClientStatus = (int)Enums.Status.Normal }
                        equals new { ClientID = client.ID, ClientStatus = client.Status }
                        into clients2
                   from client in clients2.DefaultIfEmpty()


                   join decHeadSpecialType in decHeadSpecialTypes
                        on new { DecHeadID = head.ID, DecHeadSpecialTypeStatus = (int)Enums.Status.Normal, DecHeadSpecialType = (int)Enums.DecHeadSpecialTypeEnum.CharterBus }
                        equals new { DecHeadID = decHeadSpecialType.DecHeadID, DecHeadSpecialTypeStatus = decHeadSpecialType.Status, DecHeadSpecialType = decHeadSpecialType.Type }
                        into decHeadSpecialTypesCharterBus
                   from decHeadSpecialTypeCharterBus in decHeadSpecialTypesCharterBus.DefaultIfEmpty()
                   join decHeadSpecialType in decHeadSpecialTypes
                        on new { DecHeadID = head.ID, DecHeadSpecialTypeStatus = (int)Enums.Status.Normal, DecHeadSpecialType = (int)Enums.DecHeadSpecialTypeEnum.HighValue }
                        equals new { DecHeadID = decHeadSpecialType.DecHeadID, DecHeadSpecialTypeStatus = decHeadSpecialType.Status, DecHeadSpecialType = decHeadSpecialType.Type }
                        into decHeadSpecialTypesHighValue
                   from decHeadSpecialTypeHighValue in decHeadSpecialTypesHighValue.DefaultIfEmpty()
                   join decHeadSpecialType in decHeadSpecialTypes
                        on new { DecHeadID = head.ID, DecHeadSpecialTypeStatus = (int)Enums.Status.Normal, DecHeadSpecialType = (int)Enums.DecHeadSpecialTypeEnum.Inspection }
                        equals new { DecHeadID = decHeadSpecialType.DecHeadID, DecHeadSpecialTypeStatus = decHeadSpecialType.Status, DecHeadSpecialType = decHeadSpecialType.Type }
                        into decHeadSpecialTypesInspection
                   from decHeadSpecialTypeInspection in decHeadSpecialTypesInspection.DefaultIfEmpty()
                   join decHeadSpecialType in decHeadSpecialTypes
                        on new { DecHeadID = head.ID, DecHeadSpecialTypeStatus = (int)Enums.Status.Normal, DecHeadSpecialType = (int)Enums.DecHeadSpecialTypeEnum.Quarantine }
                        equals new { DecHeadID = decHeadSpecialType.DecHeadID, DecHeadSpecialTypeStatus = decHeadSpecialType.Status, DecHeadSpecialType = decHeadSpecialType.Type }
                        into decHeadSpecialTypesQuarantine
                   from decHeadSpecialTypeQuarantine in decHeadSpecialTypesQuarantine.DefaultIfEmpty()
                   join decHeadSpecialType in decHeadSpecialTypes
                        on new { DecHeadID = head.ID, DecHeadSpecialTypeStatus = (int)Enums.Status.Normal, DecHeadSpecialType = (int)Enums.DecHeadSpecialTypeEnum.CCC }
                        equals new { DecHeadID = decHeadSpecialType.DecHeadID, DecHeadSpecialTypeStatus = decHeadSpecialType.Status, DecHeadSpecialType = decHeadSpecialType.Type }
                        into decHeadSpecialTypesCCC
                   from decHeadSpecialTypeCCC in decHeadSpecialTypesCCC.DefaultIfEmpty()


                   join decNoticeVoyage in decNoticeVoyages
                        on new { DecNoticeID = head.DeclarationNoticeID, DecNoticeVoyageStatus = (int)Enums.Status.Normal }
                        equals new { DecNoticeID = decNoticeVoyage.DecNoticeID, DecNoticeVoyageStatus = decNoticeVoyage.Status }
                        into decNoticeVoyages2
                   from decNoticeVoyage in decNoticeVoyages2.DefaultIfEmpty()

                   join voyage in voyages
                        on new { VoyageID = decNoticeVoyage.VoyageID, VoyageStatus = (int)Enums.Status.Normal }
                        equals new { VoyageID = voyage.ID, VoyageStatus = voyage.Status }
                        into voyages2
                   from voyage in voyages2.DefaultIfEmpty()


                   orderby head.CreateTime descending
                   select new DecHeadList
                   {
                       ID = head.ID,
                       ContrNo = head.ContrNo,
                       OrderID = head.OrderID,
                       BillNo = head.BillNo,
                       //EntryId = head.EntryId,
                       //PreEntryId = head.PreEntryId,
                       AgentName = head.AgentName,
                       ConsignorName = head.ConsignorName,
                       ConsigneeName = head.ConsigneeName,
                       //IsInspection = head.IsInspection,
                       //IsQuarantine = head.IsQuarantine,
                       CreateTime = head.CreateTime,
                       InputerID = temp.RealName,
                       Status = head.CusDecStatus,
                       Transformed = billsView.Any(t => t.ID == head.BillNo),
                       ClientCode = client.ClientCode,
                       ClientName = head.OwnerName,
                       IsCharterBus = (decHeadSpecialTypeCharterBus != null),
                       IsHighValue = (decHeadSpecialTypeHighValue != null),
                       IsInspection = (decHeadSpecialTypeInspection != null),
                       IsQuarantine = (decHeadSpecialTypeQuarantine != null),
                       IsCCC = (decHeadSpecialTypeCCC != null),
                       VoyageID = decNoticeVoyage.VoyageID,
                       VoyageType = (Enums.VoyageType)(voyage.Type != null ? voyage.Type : 0),
                       PackNo = head.PackNo,
                   };
        }

    }

    /// <summary>
    /// 报关单--已制单
    /// </summary>
    public class DecHeadsMakedListView : UniqueView<DecHeadList, ScCustomsReponsitory>
    {
        public DecHeadsMakedListView()
        {
        }

        internal DecHeadsMakedListView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<DecHeadList> GetIQueryable()
        {
            var adminView = new AdminsTopView(this.Reponsitory);
            var billsView = new ManifestConsignmentsView(this.Reponsitory);

            return from head in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                   join admin in adminView on head.InputerID equals admin.ID into g
                   from temp in g.DefaultIfEmpty()
                   where head.CusDecStatus == MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.Make)
                   orderby head.CreateTime descending
                   select new DecHeadList
                   {
                       ID = head.ID,
                       ContrNo = head.ContrNo,
                       OrderID = head.OrderID,
                       BillNo = head.BillNo,
                       EntryId = head.EntryId,
                       PreEntryId = head.PreEntryId,
                       AgentName = head.AgentName,
                       ConsignorName = head.ConsignorName,
                       ConsigneeName = head.ConsigneeName,
                       //IsInspection = head.IsInspection,
                       //IsQuarantine = head.IsQuarantine,
                       CreateTime = head.CreateTime,
                       InputerID = temp.RealName,
                       Status = head.CusDecStatus,
                       Transformed = billsView.Any(t => t.ID == head.BillNo)
                   };
        }
    }

    /// <summary>
    /// 报关单--已导入
    /// </summary>
    public class DecHeadsImportedListView : UniqueView<UploadDecHead, ScCustomsReponsitory>
    {
        public DecHeadsImportedListView()
        {
        }

        internal DecHeadsImportedListView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<UploadDecHead> GetIQueryable()
        {
            var adminView = new AdminsTopView(this.Reponsitory);
            var billsView = new ManifestConsignmentsView(this.Reponsitory);

            var fileView = new DecHeadFilesView(this.Reponsitory);
            var decListView = new Views.DecOriginListsView(this.Reponsitory);

            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var consignees = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderConsignees>();

            var decHeadSpecialTypes = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeadSpecialTypes>();
            var decNoticeVoyages = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecNoticeVoyages>();
            var voyages = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Voyages>();

            return from head in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                   join admin in adminView on head.InputerID equals admin.ID into g
                   from temp in g.DefaultIfEmpty()

                   join file in fileView on head.ID equals file.DecHeadID into files
                   join declist in decListView on head.ID equals declist.DeclarationID into declists

                   join order in orders
                        on new { OrderID = head.OrderID, OrderStatus = (int)Enums.Status.Normal }
                        equals new { OrderID = order.ID, OrderStatus = order.Status }
                        into orders2
                   from order in orders2.DefaultIfEmpty()
                   join client in clients
                        on new { ClientID = order.ClientID, ClientStatus = (int)Enums.Status.Normal }
                        equals new { ClientID = client.ID, ClientStatus = client.Status }
                        into clients2
                   from client in clients2.DefaultIfEmpty()
                       //
                   join consignee in consignees
                        on new { OrderID = order.ID, Status = (int)Enums.Status.Normal }
                        equals new { OrderID = consignee.OrderID, Status = consignee.Status }
                        into consignees2
                   from consignee in consignees2.DefaultIfEmpty()


                   join decHeadSpecialType in decHeadSpecialTypes
                        on new { DecHeadID = head.ID, DecHeadSpecialTypeStatus = (int)Enums.Status.Normal, DecHeadSpecialType = (int)Enums.DecHeadSpecialTypeEnum.CharterBus }
                        equals new { DecHeadID = decHeadSpecialType.DecHeadID, DecHeadSpecialTypeStatus = decHeadSpecialType.Status, DecHeadSpecialType = decHeadSpecialType.Type }
                        into decHeadSpecialTypesCharterBus
                   from decHeadSpecialTypeCharterBus in decHeadSpecialTypesCharterBus.DefaultIfEmpty()
                   join decHeadSpecialType in decHeadSpecialTypes
                        on new { DecHeadID = head.ID, DecHeadSpecialTypeStatus = (int)Enums.Status.Normal, DecHeadSpecialType = (int)Enums.DecHeadSpecialTypeEnum.HighValue }
                        equals new { DecHeadID = decHeadSpecialType.DecHeadID, DecHeadSpecialTypeStatus = decHeadSpecialType.Status, DecHeadSpecialType = decHeadSpecialType.Type }
                        into decHeadSpecialTypesHighValue
                   from decHeadSpecialTypeHighValue in decHeadSpecialTypesHighValue.DefaultIfEmpty()
                   join decHeadSpecialType in decHeadSpecialTypes
                        on new { DecHeadID = head.ID, DecHeadSpecialTypeStatus = (int)Enums.Status.Normal, DecHeadSpecialType = (int)Enums.DecHeadSpecialTypeEnum.Inspection }
                        equals new { DecHeadID = decHeadSpecialType.DecHeadID, DecHeadSpecialTypeStatus = decHeadSpecialType.Status, DecHeadSpecialType = decHeadSpecialType.Type }
                        into decHeadSpecialTypesInspection
                   from decHeadSpecialTypeInspection in decHeadSpecialTypesInspection.DefaultIfEmpty()
                   join decHeadSpecialType in decHeadSpecialTypes
                        on new { DecHeadID = head.ID, DecHeadSpecialTypeStatus = (int)Enums.Status.Normal, DecHeadSpecialType = (int)Enums.DecHeadSpecialTypeEnum.Quarantine }
                        equals new { DecHeadID = decHeadSpecialType.DecHeadID, DecHeadSpecialTypeStatus = decHeadSpecialType.Status, DecHeadSpecialType = decHeadSpecialType.Type }
                        into decHeadSpecialTypesQuarantine
                   from decHeadSpecialTypeQuarantine in decHeadSpecialTypesQuarantine.DefaultIfEmpty()
                   join decHeadSpecialType in decHeadSpecialTypes
                        on new { DecHeadID = head.ID, DecHeadSpecialTypeStatus = (int)Enums.Status.Normal, DecHeadSpecialType = (int)Enums.DecHeadSpecialTypeEnum.CCC }
                        equals new { DecHeadID = decHeadSpecialType.DecHeadID, DecHeadSpecialTypeStatus = decHeadSpecialType.Status, DecHeadSpecialType = decHeadSpecialType.Type }
                        into decHeadSpecialTypesCCC
                   from decHeadSpecialTypeCCC in decHeadSpecialTypesCCC.DefaultIfEmpty()


                   join decNoticeVoyage in decNoticeVoyages
                        on new { DecNoticeID = head.DeclarationNoticeID, DecNoticeVoyageStatus = (int)Enums.Status.Normal }
                        equals new { DecNoticeID = decNoticeVoyage.DecNoticeID, DecNoticeVoyageStatus = decNoticeVoyage.Status }
                        into decNoticeVoyages2
                   from decNoticeVoyage in decNoticeVoyages2.DefaultIfEmpty()

                   join voyage in voyages
                        on new { VoyageID = decNoticeVoyage.VoyageID, VoyageStatus = (int)Enums.Status.Normal }
                        equals new { VoyageID = voyage.ID, VoyageStatus = voyage.Status }
                        into voyages2
                   from voyage in voyages2.DefaultIfEmpty()


                   where head.CusDecStatus != MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.Draft)
                   orderby head.ContrNo descending
                   select new UploadDecHead
                   {
                       ID = head.ID,
                       ContrNo = head.ContrNo,
                       OrderID = head.OrderID,
                       BillNo = head.BillNo,
                       EntryId = head.EntryId,
                       SeqNo = head.SeqNo,
                       PreEntryId = head.PreEntryId,
                       AgentName = head.AgentName,
                       ConsignorName = head.ConsignorName,
                       ConsigneeName = head.ConsigneeName,
                       ConsigneeAddress = consignee.Address,
                       //IsInspection = head.IsInspection,
                       //IsQuarantine = head.IsQuarantine,
                       CreateTime = head.CreateTime,
                       InputerID = temp.RealName,
                       CusDecStatus = head.CusDecStatus,
                       files = files,
                       Transformed = billsView.Any(t => t.ID == head.BillNo),
                       DDate = head.DDate,
                       ClientCode = client.ClientCode,
                       ClientType = (Enums.ClientType)client.ClientType,
                       ClientName = head.OwnerName,
                       IsCharterBus = (decHeadSpecialTypeCharterBus != null),
                       IsHighValue = (decHeadSpecialTypeHighValue != null),
                       IsInspection = (decHeadSpecialTypeInspection != null),
                       IsQuarantine = (decHeadSpecialTypeQuarantine != null),
                       IsCCC = (decHeadSpecialTypeCCC != null),
                       VoyageID = decNoticeVoyage.VoyageID,
                       VoyageType = (Enums.VoyageType)(voyage.Type != null ? voyage.Type : 0),

                       PackNo = head.PackNo,
                       GrossWt = head.GrossWt,
                       TotalQty = declists.Sum(t => t.GQty),
                       ModelAmount = declists.Count(),
                       TotalAmount = declists.Sum(t => t.DeclTotal),
                   };
        }
    }


    /// <summary>
    /// 报关单--未上传
    /// </summary>
    public class UnUploadDecHeadsListView : UniqueView<UploadDecHead, ScCustomsReponsitory>
    {
        public UnUploadDecHeadsListView()
        {
        }

        internal UnUploadDecHeadsListView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<UploadDecHead> GetIQueryable()
        {
            var fileView = new DecHeadFilesView(this.Reponsitory);
            var decListView = new Views.DecOriginListsView(this.Reponsitory);
            return from head in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                   join file in fileView on head.ID equals file.DecHeadID into files
                   join declist in decListView on head.ID equals declist.DeclarationID into declists
                   orderby head.CreateTime descending
                   select new UploadDecHead
                   {
                       ID = head.ID,
                       ContrNo = head.ContrNo,
                       OrderID = head.OrderID,
                       BillNo = head.BillNo,
                       EntryId = head.EntryId,
                       PreEntryId = head.PreEntryId,
                       AgentName = head.AgentName,
                       ConsignorName = head.ConsignorName,
                       ConsigneeName = head.ConsigneeName,
                       IsInspection = head.IsInspection,
                       DDate = head.DDate,
                       CusDecStatus = head.CusDecStatus,
                       IsSuccess = head.IsSuccess,
                       files = files,
                       Currency = declists.First() == null ? "" : declists.First().TradeCurr,
                       DecAmount = declists.Sum(t => t.DeclTotal),

                   };
        }
    }

    /// <summary>
    /// 报关单--缴税
    /// </summary>
    public class DecTaxView : UniqueView<DecTax, ScCustomsReponsitory>
    {
        public DecTaxView()
        {
        }

        internal DecTaxView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<DecTax> GetIQueryable()
        {
            var decHeadView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var decListView = new Views.DecOriginListsView(this.Reponsitory);
            var fileView = new DecHeadFilesView(this.Reponsitory);
            var flowView = new DecTaxFlowsView(this.Reponsitory);

            return from decTax in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxs>()
                   join dechead in decHeadView on decTax.ID equals dechead.ID
                   join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on dechead.OrderID equals order.ID
                   join file in fileView on decTax.ID equals file.DecHeadID into files
                   join flow in flowView on decTax.ID equals flow.DecheadID into flows
                   join declist in decListView on decTax.ID equals declist.DeclarationID into declists
                   join client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>() on order.ClientID equals client.ID
                   join company in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>() on client.CompanyID equals company.ID
                   select new DecTax
                   {
                       ID = dechead.ID,
                       OrderID = dechead.OrderID,
                       ContrNo = dechead.ContrNo,
                       EntryId = dechead.EntryId,
                       DDate = dechead.DDate,
                       ConsumeName = dechead.OwnerName,
                       OwnerName = company.Name,
                       CusDecStatus = dechead.CusDecStatus,
                       CustomMaster = dechead.CustomMaster,
                       IsSuccess = dechead.IsSuccess,
                       Currency = declists.First() == null ? "" : declists.First().TradeCurr,
                       DecAmount = declists.Sum(t => t.DeclTotal),
                       OrderAgentAmount = order.DeclarePrice,

                       InvoiceType = (Enums.InvoiceType)decTax.InvoiceType,
                       DecTaxStatus = (Enums.DecTaxStatus)decTax.Status,
                       UploadStatus = (Enums.UploadStatus)decTax.IsUpload,
                       HandledType = decTax.HandledType,
                       CreateDate = decTax.CreateDate,
                       UpdateDate = decTax.UpdateDate,
                       Summary = decTax.Summary,

                       files = files,
                       flows = flows,
                       TariffValue = (from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                                      join tax in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>() on item.ID equals tax.OrderItemID
                                      where item.OrderID == dechead.OrderID && tax.Type == (int)Enums.CustomsRateType.ImportTax
                                      select tax.Value).Sum(),
                       AddedValue = (from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                                     join tax in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>() on item.ID equals tax.OrderItemID
                                     where item.OrderID == dechead.OrderID && tax.Type == (int)Enums.CustomsRateType.AddedValueTax
                                     select tax.Value).Sum(),
                       ConsignorCode = dechead.ConsignorCode
                   };
        }
    }

    public class DecHeadsExcelImportedListView : UniqueView<UploadDecHead, ScCustomsReponsitory>
    {
        public DecHeadsExcelImportedListView()
        {
        }

        internal DecHeadsExcelImportedListView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<UploadDecHead> GetIQueryable()
        {
            var adminView = new AdminsTopView(this.Reponsitory);
            var billsView = new ManifestConsignmentsView(this.Reponsitory);

            var fileView = new DecHeadFilesView(this.Reponsitory);
            var decListView = new Views.DecOriginListsView(this.Reponsitory);

            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();

            var decHeadSpecialTypes = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeadSpecialTypes>();
            var decNoticeVoyages = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecNoticeVoyages>();
            var voyages = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Voyages>();

            return from head in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                   join admin in adminView on head.InputerID equals admin.ID into g
                   from temp in g.DefaultIfEmpty()

                   join file in fileView on head.ID equals file.DecHeadID into files
                   join declist in decListView on head.ID equals declist.DeclarationID into declists

                   join order in orders
                        on new { OrderID = head.OrderID, OrderStatus = (int)Enums.Status.Normal }
                        equals new { OrderID = order.ID, OrderStatus = order.Status }
                        into orders2
                   from order in orders2.DefaultIfEmpty()
                   join client in clients
                        on new { ClientID = order.ClientID, ClientStatus = (int)Enums.Status.Normal }
                        equals new { ClientID = client.ID, ClientStatus = client.Status }
                        into clients2
                   from client in clients2.DefaultIfEmpty()


                   join decHeadSpecialType in decHeadSpecialTypes
                        on new { DecHeadID = head.ID, DecHeadSpecialTypeStatus = (int)Enums.Status.Normal, DecHeadSpecialType = (int)Enums.DecHeadSpecialTypeEnum.CharterBus }
                        equals new { DecHeadID = decHeadSpecialType.DecHeadID, DecHeadSpecialTypeStatus = decHeadSpecialType.Status, DecHeadSpecialType = decHeadSpecialType.Type }
                        into decHeadSpecialTypesCharterBus
                   from decHeadSpecialTypeCharterBus in decHeadSpecialTypesCharterBus.DefaultIfEmpty()
                   join decHeadSpecialType in decHeadSpecialTypes
                        on new { DecHeadID = head.ID, DecHeadSpecialTypeStatus = (int)Enums.Status.Normal, DecHeadSpecialType = (int)Enums.DecHeadSpecialTypeEnum.HighValue }
                        equals new { DecHeadID = decHeadSpecialType.DecHeadID, DecHeadSpecialTypeStatus = decHeadSpecialType.Status, DecHeadSpecialType = decHeadSpecialType.Type }
                        into decHeadSpecialTypesHighValue
                   from decHeadSpecialTypeHighValue in decHeadSpecialTypesHighValue.DefaultIfEmpty()
                   join decHeadSpecialType in decHeadSpecialTypes
                        on new { DecHeadID = head.ID, DecHeadSpecialTypeStatus = (int)Enums.Status.Normal, DecHeadSpecialType = (int)Enums.DecHeadSpecialTypeEnum.Inspection }
                        equals new { DecHeadID = decHeadSpecialType.DecHeadID, DecHeadSpecialTypeStatus = decHeadSpecialType.Status, DecHeadSpecialType = decHeadSpecialType.Type }
                        into decHeadSpecialTypesInspection
                   from decHeadSpecialTypeInspection in decHeadSpecialTypesInspection.DefaultIfEmpty()
                   join decHeadSpecialType in decHeadSpecialTypes
                        on new { DecHeadID = head.ID, DecHeadSpecialTypeStatus = (int)Enums.Status.Normal, DecHeadSpecialType = (int)Enums.DecHeadSpecialTypeEnum.Quarantine }
                        equals new { DecHeadID = decHeadSpecialType.DecHeadID, DecHeadSpecialTypeStatus = decHeadSpecialType.Status, DecHeadSpecialType = decHeadSpecialType.Type }
                        into decHeadSpecialTypesQuarantine
                   from decHeadSpecialTypeQuarantine in decHeadSpecialTypesQuarantine.DefaultIfEmpty()
                   join decHeadSpecialType in decHeadSpecialTypes
                        on new { DecHeadID = head.ID, DecHeadSpecialTypeStatus = (int)Enums.Status.Normal, DecHeadSpecialType = (int)Enums.DecHeadSpecialTypeEnum.CCC }
                        equals new { DecHeadID = decHeadSpecialType.DecHeadID, DecHeadSpecialTypeStatus = decHeadSpecialType.Status, DecHeadSpecialType = decHeadSpecialType.Type }
                        into decHeadSpecialTypesCCC
                   from decHeadSpecialTypeCCC in decHeadSpecialTypesCCC.DefaultIfEmpty()


                   join decNoticeVoyage in decNoticeVoyages
                        on new { DecNoticeID = head.DeclarationNoticeID, DecNoticeVoyageStatus = (int)Enums.Status.Normal }
                        equals new { DecNoticeID = decNoticeVoyage.DecNoticeID, DecNoticeVoyageStatus = decNoticeVoyage.Status }
                        into decNoticeVoyages2
                   from decNoticeVoyage in decNoticeVoyages2.DefaultIfEmpty()

                   join voyage in voyages
                        on new { VoyageID = decNoticeVoyage.VoyageID, VoyageStatus = (int)Enums.Status.Normal }
                        equals new { VoyageID = voyage.ID, VoyageStatus = voyage.Status }
                        into voyages2
                   from voyage in voyages2.DefaultIfEmpty()


                   where head.CusDecStatus == MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.E0)
                   orderby head.ContrNo descending
                   select new UploadDecHead
                   {
                       ID = head.ID,
                       ContrNo = head.ContrNo,
                       OrderID = head.OrderID,
                       BillNo = head.BillNo,
                       EntryId = head.EntryId,
                       SeqNo = head.SeqNo,
                       PreEntryId = head.PreEntryId,
                       AgentName = head.AgentName,
                       ConsignorName = head.ConsignorName,
                       ConsigneeName = head.ConsigneeName,
                       //IsInspection = head.IsInspection,
                       //IsQuarantine = head.IsQuarantine,
                       CreateTime = head.CreateTime,
                       InputerID = temp.RealName,
                       CusDecStatus = head.CusDecStatus,
                       files = files,
                       Transformed = billsView.Any(t => t.ID == head.BillNo),
                       DDate = head.DDate,
                       ClientCode = client.ClientCode,
                       ClientName = head.OwnerName,
                       IsCharterBus = (decHeadSpecialTypeCharterBus != null),
                       IsHighValue = (decHeadSpecialTypeHighValue != null),
                       IsInspection = (decHeadSpecialTypeInspection != null),
                       IsQuarantine = (decHeadSpecialTypeQuarantine != null),
                       IsCCC = (decHeadSpecialTypeCCC != null),
                       VoyageID = decNoticeVoyage.VoyageID,
                       VoyageType = (Enums.VoyageType)(voyage.Type != null ? voyage.Type : 0),

                       PackNo = head.PackNo,
                       GrossWt = head.GrossWt,
                       TotalQty = declists.Sum(t => t.GQty),
                       ModelAmount = declists.Count(),
                       TotalAmount = declists.Sum(t => t.DeclTotal),
                   };
        }
    }


    public class DecHeadsListViewRJ : QueryView<DecHeadList, ScCustomsReponsitory>
    {
        public DecHeadsListViewRJ()
        {
        }

        internal DecHeadsListViewRJ(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected DecHeadsListViewRJ(ScCustomsReponsitory reponsitory, IQueryable<DecHeadList> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<DecHeadList> GetIQueryable()
        {
            var decHeadsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var ordersView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var consigneeAddressView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderConsignees>();
            var declarationNotices = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarationNotices>();

            var iQuery = from dechead in decHeadsView
                         join order in ordersView on dechead.OrderID equals order.ID
                         join consignee in consigneeAddressView on dechead.OrderID equals consignee.OrderID
                         join declarationNotice in declarationNotices on dechead.DeclarationNoticeID equals declarationNotice.ID
                         where order.Status == (int)Enums.Status.Normal
                         orderby dechead.ContrNo descending
                         select new DecHeadList
                         {
                             ID = dechead.ID,
                             ContrNo = dechead.ContrNo,
                             BillNo = dechead.BillNo,
                             OrderID = dechead.OrderID,
                             ClientID = order.ClientID,
                             ClientName = dechead.OwnerName,
                             ConsigneeAddress = consignee.Address,
                             VoyageID = dechead.VoyNo,
                             PackNo = dechead.PackNo,
                             CreateTime = dechead.CreateTime,
                             InputerID = dechead.InputerID,
                             Status = dechead.CusDecStatus,
                             SeqNo = dechead.SeqNo,
                             GrossWt = dechead.GrossWt,
                             DDate = dechead.DDate.Value,
                             EntryId = dechead.EntryId,
                             CreateDeclareAdminID = declarationNotice.CreateDeclareAdminID,
                             CustomSubmiterAdminID = dechead.SubmitCustomAdminID,
                             DoubleCheckAdminID = dechead.DoubleCheckerAdminID,
                             GuaranteeNo = dechead.GuaranteeNo
                         };
            return iQuery;
        }

        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Models.DecHeadList> iquery = this.IQueryable.Cast<Models.DecHeadList>().OrderByDescending(item => item.ID);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myDeclares = iquery.ToArray();

            //获取报关单的ID
            var declaresID = ienum_myDeclares.Select(item => item.ID);

            //获取订单的ID
            var ordersID = ienum_myDeclares.Select(item => item.OrderID);

            //客户ID
            var clientsID = ienum_myDeclares.Select(item => item.ClientID);

            //运输批次号
            var voyagesID = ienum_myDeclares.Select(item => item.VoyageID);

            //录入人
            var inputersID = ienum_myDeclares.Select(item => item.InputerID);

            var decHeadSpecoalTypeView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeadSpecialTypes>();
            var voyagesView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Voyages>();
            var clientsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var manifestConsignView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ManifestConsignments>();
            var declistView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>();
            var decheadFileView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeadFiles>();



            #region 客户编号

            var linq_ClientCode = from client in clientsView
                                  where clientsID.Contains(client.ID)
                                  select new
                                  {
                                      ClientID = client.ID,
                                      ClientCode = client.ClientCode,
                                      ClientType = client.ClientType
                                  };

            var ienums_ClientCode = linq_ClientCode.ToArray();

            //获取特殊类型？可能是这样叫做
            var linqs_special = from special in decHeadSpecoalTypeView
                                where special.Status == (int)Enums.Status.Normal && declaresID.Contains(special.DecHeadID)
                                select new
                                {
                                    special.DecHeadID,
                                    oSpecialType = (Enums.DecHeadSpecialTypeEnum)special.Type
                                };

            var ienums_special = linqs_special.ToArray();

            #endregion

            #region 获取运输类型

            var linq_Voyage = from voyage in voyagesView
                              where voyage.Status == (int)Enums.Status.Normal && voyagesID.Contains(voyage.ID)
                              select new
                              {
                                  VoyageNo = voyage.ID,
                                  Type = (Enums.VoyageType)voyage.Type
                              };

            var ienums_voyage = linq_Voyage.ToArray();

            #endregion

            #region 录入人

            var clientAdminsView = new ClientAdminsView(this.Reponsitory);

            var linq_declarant = from admin in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>()
                                 where inputersID.Contains(admin.ID)
                                 select new
                                 {
                                     AdminID = admin.ID,
                                     ByName = admin.Byname,
                                 };
            var ienums_declarant = linq_declarant.ToArray();

            #endregion

            #region 舱单

            var linq_manifest = from manifest in manifestConsignView
                                where voyagesID.Contains(manifest.ManifestID)
                                select new
                                {
                                    VoyageNo = manifest.ManifestID,
                                    BillNo = manifest.ID
                                };
            var ienums_manifest = linq_manifest.ToArray();

            #endregion

            #region 表体

            var linq_declist = from declist in declistView
                               where declaresID.Contains(declist.DeclarationID)
                               select new
                               {
                                   ID = declist.ID,
                                   DecHeadID = declist.DeclarationID,
                                   GQty = declist.GQty,
                                   DeclTotal = declist.DeclTotal,
                                   CaseNo = declist.CaseNo
                               };
            var ienums_declist = linq_declist.ToArray();
            var groups_declist = from item in ienums_declist
                                 group item by item.DecHeadID into groups
                                 select new
                                 {
                                     DecHeadID = groups.Key,
                                     TotalQty = groups.Sum(t => t.GQty),
                                     ModelAmount = groups.Select(t => t.ID).Distinct().Count(),
                                     TotalAmount = groups.Sum(t => t.DeclTotal),
                                 };

            #endregion

            #region 报关单附件

            var linq_decheadfile = from file in decheadFileView
                                   where file.FileType == (int)Enums.FileType.DecHeadFile && file.Status == (int)Enums.Status.Normal
                                   && declaresID.Contains(file.DecHeadID)
                                   select new
                                   {
                                       DecHeadID = file.DecHeadID,
                                       Url = file.Url
                                   };
            var ienums_decheadfile = linq_decheadfile.ToArray();

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
                                              CreateDeclareAdminName = g.FirstOrDefault().ByName,
                                          };

            var ienums_createDeclareAdmin = linq_createDeclareAdmin.ToArray();

            #endregion

            #region 发单人

            var customSubmiterAdminIDs = ienum_myDeclares.Select(item => item.CustomSubmiterAdminID);

            var linq_customSubmiterAdmin = from admin in adminsTopView2
                                           where customSubmiterAdminIDs.Contains(admin.OriginID)
                                           group admin by new { admin.OriginID } into g
                                           select new
                                           {
                                               CustomSubmiterAdminID = g.Key.OriginID,
                                               CustomSubmiterAdminName = g.FirstOrDefault().ByName,
                                           };

            var ienums_customSubmiterAdmin = linq_customSubmiterAdmin.ToArray();

            #endregion

            #region 复核人
            var doubleCheckAdminIDs = ienum_myDeclares.Select(item => item.DoubleCheckAdminID);

            var linq_doubleCheckAdmin = from admin in adminsTopView2
                                        where doubleCheckAdminIDs.Contains(admin.OriginID)
                                        group admin by new { admin.OriginID } into g
                                        select new
                                        {
                                            DoubleCheckAdminID = g.Key.OriginID,
                                            DoubleCheckAdminName = g.FirstOrDefault().ByName,
                                        };

            var ienums_doubleCheckAdmin = linq_doubleCheckAdmin.ToArray();
            #endregion

            #region 草稿下，特殊显示退回的单子

            var IsDraft = false;
            var ienums_trace = new List<string>();
            if (total > 0 && ienum_myDeclares.First().Status == MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.Draft))
            {
                IsDraft = true;
                var decTraceView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTraces>();
                var linq_trace = from trace in decTraceView
                                 where declaresID.Contains(trace.DeclarationID)
                                 && trace.Channel == MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.PendingDoubleCheck)
                                 && trace.Message.Contains("退回该报关单")
                                 select new
                                 {
                                     trace.DeclarationID
                                 };
                foreach (var trace in linq_trace)
                {
                    ienums_trace.Add(trace.DeclarationID);
                }
            }

            #endregion

            var ienums_linq = from head in ienum_myDeclares
                                  //select new { };
                              join special in ienums_special on head.ID equals special.DecHeadID into specials
                              join _createDeclareAdmin in ienums_createDeclareAdmin on head.CreateDeclareAdminID equals _createDeclareAdmin.CreateDeclareAdminID
                              into ienums_createDeclareAdmin2
                              from createDeclareAdmin in ienums_createDeclareAdmin2.DefaultIfEmpty()

                              join _customSubmiterAdmin in ienums_customSubmiterAdmin on head.CustomSubmiterAdminID equals _customSubmiterAdmin.CustomSubmiterAdminID
                              into ienums_customSubmiterAdmin2
                              from customSubmiterAdmin in ienums_customSubmiterAdmin2.DefaultIfEmpty()

                              join _doubleCheckAdmin in ienums_doubleCheckAdmin on head.DoubleCheckAdminID equals _doubleCheckAdmin.DoubleCheckAdminID
                              into ienums_doubleCheckAdmin2
                              from doubleCheckerAdmin in ienums_doubleCheckAdmin2.DefaultIfEmpty()

                              let declarant = ienums_declarant.SingleOrDefault(item => item.AdminID == head.InputerID)
                              let isbill = ienums_manifest.Any(item => item.VoyageNo == head.VoyageID && item.BillNo == head.BillNo)
                              let client = ienums_ClientCode.SingleOrDefault(item => item.ClientID == head.ClientID)
                              let voyageType = ienums_voyage.SingleOrDefault(item => item.VoyageNo == head.VoyageID)
                              let groupdeclist = groups_declist.SingleOrDefault(item => item.DecHeadID == head.ID)
                              let file = ienums_decheadfile.SingleOrDefault(item => item.DecHeadID == head.ID)
                              select new Models.DecHeadList
                              {

                                  ID = head.ID,
                                  ContrNo = head.ContrNo,
                                  OrderID = head.OrderID,
                                  BillNo = head.BillNo,
                                  //EntryId = head.EntryId,
                                  //PreEntryId = head.PreEntryId,
                                  AgentName = head.AgentName,
                                  ConsignorName = head.ConsignorName,
                                  ConsigneeName = head.ConsigneeName,
                                  ConsigneeAddress = head.ConsigneeAddress,
                                  //IsInspection = head.IsInspection,
                                  //IsQuarantine = head.IsQuarantine,
                                  CreateTime = head.CreateTime,
                                  InputerID = declarant.ByName,
                                  Status = head.Status,
                                  Transformed = isbill,
                                  ClientCode = client.ClientCode,
                                  ClientName = head.ClientName,
                                  ClientType = (Enums.ClientType)client.ClientType,
                                  SeqNo = head.SeqNo,
                                  EntryId = head.EntryId,
                                  DDate = head.DDate,
                                  GuaranteeNo = head.GuaranteeNo,

                                  //特殊类型相关
                                  IsCharterBus = specials.Any(item => item.oSpecialType == Enums.DecHeadSpecialTypeEnum.CharterBus),
                                  IsHighValue = specials.Any(item => item.oSpecialType == Enums.DecHeadSpecialTypeEnum.HighValue),
                                  IsInspection = specials.Any(item => item.oSpecialType == Enums.DecHeadSpecialTypeEnum.Inspection),
                                  IsQuarantine = specials.Any(item => item.oSpecialType == Enums.DecHeadSpecialTypeEnum.Quarantine),
                                  IsCCC = specials.Any(item => item.oSpecialType == Enums.DecHeadSpecialTypeEnum.CCC),
                                  IsOrigin = specials.Any(item => item.oSpecialType == Enums.DecHeadSpecialTypeEnum.OriginATRate),
                                  IsSenOrigin = specials.Any(item => item.oSpecialType == Enums.DecHeadSpecialTypeEnum.SenOrigin),

                                  IsCheckReturned = IsDraft == true ? (ienums_trace.Contains(head.ID) == true ? true : false) : false,

                                  VoyageID = head.VoyageID,
                                  VoyageType = (Enums.VoyageType)voyageType.Type,
                                  PackNo = head.PackNo,

                                  GrossWt = head.GrossWt,
                                  TotalQty = groupdeclist.TotalQty,
                                  ModelAmount = groupdeclist.ModelAmount,
                                  TotalAmount = groupdeclist.TotalAmount,

                                  IsHeadFile = file != null,
                                  URL = file?.Url.ToUrl(),
                                  //纯为了显示的字段，向下补充。

                                  CreateDeclareAdminID = head.CreateDeclareAdminID,
                                  CreateDeclareAdminName = createDeclareAdmin != null ? createDeclareAdmin.CreateDeclareAdminName : "",
                                  CustomSubmiterAdminID = head.CustomSubmiterAdminID,
                                  CustomSubmiterAdminName = customSubmiterAdmin != null ? customSubmiterAdmin.CustomSubmiterAdminName : "",
                                  DoubleCheckAdminID = head.DoubleCheckAdminID,
                                  DoubleCheckAdminName = doubleCheckerAdmin != null ? doubleCheckerAdmin.DoubleCheckAdminName : "",
                                  PackBox = string.Join(";", linq_declist.Where(item => item.DecHeadID == head.ID).Select(item => item.CaseNo)),
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


            Func<Needs.Ccs.Services.Models.DecHeadList, object> convert = head => new
            {
                ID = head.ID,
                ContrNO = head.ContrNo,
                OrderID = head.OrderID,
                BillNo = head.BillNo,
                EntryId = head.EntryId,
                SeqNo = head.SeqNo,
                //AgentName = head.AgentName,
                //IsInspection = head.InspQuarName,
                CreateDate = head.CreateTime.ToString("yyyy-MM-dd HH:mm"),
                InputerID = head.InputerID,
                Status = head.Status,
                StatusName = head.StatusName,
                DDate = head.DDate.ToString("yyyy-MM-dd HH:mm"),
                Transformed = head.Transformed,
                TransformedName = head.Transformed == true ? "是" : "否",
                IsDecHeadFile = head.IsHeadFile,
                URL = Needs.Utils.FileDirectory.Current.FileServerUrl + @"/" + head.URL,
                ClientCode = head.ClientCode,
                ClientName = head.ClientName,
                ClientType = head.ClientType.GetDescription(),
                ConsigneeAddress = head.ConsigneeAddress,
                IsCharterBus = head.IsCharterBus,
                IsHighValue = head.IsHighValue,
                IsInspection = head.IsInspection,
                IsQuarantine = head.IsQuarantine,
                IsCCC = head.IsCCC,
                IsOrigin = head.IsOrigin,
                IsSenOrigin = head.IsSenOrigin,
                IsCheckReturned = head.IsCheckReturned,
                VoyageID = head.VoyageID,
                VoyageType = head.VoyageType.GetDescription(),
                GuaranteeNo = head.GuaranteeNo,

                PackNo = head.PackNo,
                GrossWt = head.GrossWt,
                TotalQty = head.TotalQty,
                ModelAmount = head.ModelAmount,
                TotalAmount = head.TotalAmount,

                CreateDeclareAdminID = head.CreateDeclareAdminID,
                CreateDeclareAdminName = head.CreateDeclareAdminName,
                CustomSubmiterAdminID = head.CustomSubmiterAdminID,
                CustomSubmiterAdminName = head.CustomSubmiterAdminName,
                DoubleCheckerAdminID = head.DoubleCheckAdminID,
                DoubleCheckerAdminName = head.DoubleCheckAdminName,
                PackBox = head.PackBox,
            };


            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        /// <summary>
        /// 过滤状态--草稿
        /// </summary>
        /// <returns>视图</returns>
        public DecHeadsListViewRJ SearchByDraft()
        {
            var linq = from query in this.IQueryable
                       where query.Status == MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.Draft)
                       select query;

            var view = new DecHeadsListViewRJ(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 过滤状态--表格申报
        /// </summary>
        /// <returns>视图</returns>
        public DecHeadsListViewRJ SearchByE0()
        {
            var linq = from query in this.IQueryable
                       where query.Status == MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.E0)
                       select query;

            var view = new DecHeadsListViewRJ(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 过滤状态--待复核
        /// </summary>
        /// <returns>视图</returns>
        public DecHeadsListViewRJ SearchBy05()
        {
            var linq = from query in this.IQueryable
                       where query.Status == MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.PendingDoubleCheck)
                       select query;

            var view = new DecHeadsListViewRJ(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 过滤状态--已复核
        /// </summary>
        /// <returns>视图</returns>
        public DecHeadsListViewRJ SearchBy06()
        {
            var linq = from query in this.IQueryable
                       where query.Status == MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.DoubleChecked)
                       select query;

            var view = new DecHeadsListViewRJ(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 过滤状态--已申报
        /// </summary>
        /// <returns>视图</returns>
        public DecHeadsListViewRJ SearchByDeclared()
        {
            var linq = from query in this.IQueryable
                       where query.Status != MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.E0) && query.Status != MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.Draft)
                             && query.Status != MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.DoubleChecked) && query.Status != MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.PendingDoubleCheck)
                       select query;

            var view = new DecHeadsListViewRJ(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 查询合同号
        /// </summary>
        /// <param name="orderID">订单ID</param>
        /// <returns>视图</returns>
        public DecHeadsListViewRJ SearchByContractID(string contractID)
        {
            var linq = from query in this.IQueryable
                       where query.ContrNo.Contains(contractID)
                       select query;

            var view = new DecHeadsListViewRJ(this.Reponsitory, linq);
            return view;
        }


        /// <summary>
        /// 报关单号查询
        /// </summary>
        /// <param name="entryId"></param>
        /// <returns></returns>
        public DecHeadsListViewRJ SearchByEntryID(string entryId)
        {
            var linq = from query in this.IQueryable
                       where query.EntryId.Contains(entryId)
                       select query;

            var view = new DecHeadsListViewRJ(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 查询订单号
        /// </summary>
        /// <param name="orderID">订单ID</param>
        /// <returns>视图</returns>
        public DecHeadsListViewRJ SearchByOrderID(string orderID)
        {
            var linq = from query in this.IQueryable
                       where query.OrderID.Contains(orderID)
                       select query;

            var view = new DecHeadsListViewRJ(this.Reponsitory, linq);
            return view;
        }

        public DecHeadsListViewRJ SearchBySeqNo(string seqNo)
        {
            var linq = from query in this.IQueryable
                       where query.SeqNo.Contains(seqNo)
                       select query;

            var view = new DecHeadsListViewRJ(this.Reponsitory, linq);
            return view;
        }

        public DecHeadsListViewRJ SearchByCusReceiptCode(string cusReceiptCode)
        {
            var linq = from query in this.IQueryable
                       where query.Status.Contains(cusReceiptCode)
                       select query;

            var view = new DecHeadsListViewRJ(this.Reponsitory, linq);
            return view;
        }

        public DecHeadsListViewRJ SearchByVoyageID(string voyageID)
        {
            var linq = from query in this.IQueryable
                       where query.VoyageID.Contains(voyageID)
                       select query;

            var view = new DecHeadsListViewRJ(this.Reponsitory, linq);
            return view;
        }

        public DecHeadsListViewRJ SearchByVoyageType(int intVoyageType)
        {
            var linq = from query in this.IQueryable
                       where query.VoyageType == (Enums.VoyageType)intVoyageType
                       select query;

            var view = new DecHeadsListViewRJ(this.Reponsitory, linq);
            return view;
        }

        public DecHeadsListViewRJ SearchByFrom(DateTime fromtime)
        {
            var linq = from query in this.IQueryable
                       where query.CreateTime >= fromtime
                       select query;

            var view = new DecHeadsListViewRJ(this.Reponsitory, linq);
            return view;
        }

        public DecHeadsListViewRJ SearchByTo(DateTime totime)
        {
            var linq = from query in this.IQueryable
                       where query.CreateTime <= totime
                       select query;

            var view = new DecHeadsListViewRJ(this.Reponsitory, linq);
            return view;
        }

        public DecHeadsListViewRJ SearchByClientName(string ClientName)
        {
            var linq = from query in this.IQueryable
                       where query.ClientName.Contains(ClientName)
                       select query;

            var view = new DecHeadsListViewRJ(this.Reponsitory, linq);
            return view;
        }


        public DecHeadsListViewRJ SearchBySpecialType(params Enums.DecHeadSpecialTypeEnum[] stypes)
        {

            var decheadSpecialView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeadSpecialTypes>();

            var myQuery = this.IQueryable;
            var iStypes = stypes.Select(item => (int)item);

            var linqs_decHeadID = from oVoyage in decheadSpecialView
                                  where oVoyage.Status == (int)Enums.Status.Normal && iStypes.Contains(oVoyage.Type)
                                  select oVoyage.DecHeadID;

            var distincts = linqs_decHeadID.Distinct();

            var linq = from query in myQuery
                       join item in distincts on query.ID/*发现！*/ equals item
                       select query;

            var view = new DecHeadsListViewRJ(this.Reponsitory, linq);
            return view;

        }

        public DecHeadsListViewRJ SearchByDeclareCreatorID(string adminID)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDeclareAdminID == adminID
                       select query;

            var view = new DecHeadsListViewRJ(this.Reponsitory, linq);
            return view;
        }

    }

}
