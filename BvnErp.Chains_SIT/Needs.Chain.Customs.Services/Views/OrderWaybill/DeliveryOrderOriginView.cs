using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Needs.Ccs.Services.Views
{
    public class DeliveryOrderOriginView : UniqueView<Models.DeliveryOrder, ScCustomsReponsitory>
    {
        public DeliveryOrderOriginView()
        {
        }

        public DeliveryOrderOriginView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.DeliveryOrder> GetIQueryable()
        {
            //var clientsView = new ClientsView(this.Reponsitory);
            var adminView = new AdminsTopView(this.Reponsitory);

            var iQuery = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CgSzOutputWaybillsTopView>()
                         join d in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MainOrders>() on c.OrderID equals d.ID                       
                         join f in adminView on d.AdminID equals f.ID
                         join file in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FilesDescriptionTopView>().Where(t=>t.Type == (int)Models.ApiModels.Files.FileType.DeliverGoods)
                         on c.WaybillID equals file.WaybillID into tempfile
                         from file in tempfile.DefaultIfEmpty()
                         select new DeliveryOrder
                         {
                             ID = c.WaybillID,
                             WayBillType = (WaybillType)c.WaybillType,
                             Quantity = c.Quantity,
                             OrderID = c.OrderID,
                             IsModify = (CgPickingExcuteStatus)c.IsModify,
                             AppointTime = c.AppointTime,
                             CreateDate = c.CreateDate,
                             Code = c.Code,
                             //ConfirmReceiptStatus = (CgPickingExcuteStatus)c.ConfirmReceiptStatus,
                             coeAddress = c.coeAddress,
                             coePlace = c.coePlace,
                             coeContact = c.coeContact,
                             coePhone = c.coePhone,
                             coeIDNumber = c.coeIDNumber,
                             Expr1 = c.Expr1,
                             ExpressTy = c.ExType,
                             ExpressPayType = (Enums.PayType)c.ExPayType,
                             wldCarNumber1 = c.wldCarNumber1,
                             wldDriver = c.wldDriver,
                             wldTakingAddress = c.wldTakingAddress,
                             wldTakingContact = c.wldTakingContact,
                             wldTakingDate = c.wldTakingDate,
                             wldTakingPhone = c.wldTakingPhone,
                             CarrierName = c.CarrierName,
                             CompanyName = c.CompanyName,
                             ClientCode = c.ClientCode,
                             Admin = f,
                             Url = file.Url
                         };
            return iQuery;
        }       
    }
}
