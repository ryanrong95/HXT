using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Enums;
using Wms.Services.Models;
using Yahv.Linq;
using Yahv.Services.Enums;
using Yahv.Underly.Erps;
using Yahv.Utils.Serializers;

namespace Wms.Services.Views
{

    public class BoxProductsRoll : QueryView<Models.DataBoxesProducts, PvWmsRepository>
    {


        public BoxProductsRoll()
        {

        }

        public BoxProductsRoll(PvWmsRepository repository) : base(repository)
        {

        }

        public BoxProductsRoll(PvWmsRepository repository, IQueryable<Models.DataBoxesProducts> iQueryable) : base(repository, iQueryable)
        {

        }



        IErpAdmin Admin;
        public BoxProductsRoll(IErpAdmin admin)
        {
            this.Admin = admin;
        }




        public BoxProductsRoll SearchByWarehouseID(string whid)
        {
            var iquery = from box in this.IQueryable
                         where box.WarehouseID == whid
                         select box;
            return new BoxProductsRoll(this.Reponsitory, iquery);
        }

        /// <summary>
        /// 暂时只支持箱号
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public BoxProductsRoll SearchBykey(string key)
        {
            var iquery = from box in this.IQueryable
                         where box.Code.Contains(key)
                         select box;
            return new BoxProductsRoll(this.Reponsitory, iquery);
        }


        public BoxProductsRoll SearchByStatus(params int[] Status)
        {
            var iquery = from box in this.IQueryable
                         where Status.Contains((int)box.Status)
                         select box;
            return new BoxProductsRoll(this.Reponsitory, iquery);
        }

        public BoxProductsRoll SearchByAdmins(params string[] adminids)
        {
            var iquery = from box in this.IQueryable
                         where adminids.Contains(box.AdminID)
                         select box;
            return new BoxProductsRoll(this.Reponsitory, iquery);
        }



        public BoxProductsRoll SearchByBoxIDs(params string[] boxids)
        {
            var iquery = from box in this.IQueryable
                         where boxids.Contains(box.ID)
                         select box;
            return new BoxProductsRoll(this.Reponsitory, iquery);
        }






        protected override IQueryable<DataBoxesProducts> GetIQueryable()
        {

            throw new Exception();

            //var linqboxes = from sorting in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>()
            //                   join notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on sorting.NoticeID equals notice.ID
            //                   join input in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>() on sorting.InputID equals input.ID
            //                   where notice.Source == (int)NoticeSource.AgentBreakCustoms
            //                   select new { sorting.BoxCode,input.TinyOrderID };



            //var boxcodes = linqboxes.Select(item => item.BoxCode).Distinct();
            //var tinyorderids = linqboxes.Select(item => item.TinyOrderID).Distinct();

            //var linqids = from box in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Boxes>()
            //              where boxcodes.Contains(box.ID)
            //              select new { box.ID };

            //var linq = linqids.Select(item => item.ID).Distinct();

            //return  from boxes in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Boxes>()
            //join item in linq on boxes.ID equals item
            //orderby boxes.CreateDate descending
            //select new Models.DataBoxesProducts
            //{
            //    ID = boxes.ID,
            //    AdminID = boxes.AdminID,
            //    CreateDate = boxes.CreateDate,
            //    Code = boxes.Code,
            //    Status = (BoxesStatus)boxes.Status,
            //    Summary = boxes.Summary,
            //    WarehouseID = boxes.WarehouseID,
            //};


            //var linqbox= from boxes in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Boxes>()
            //       join item in linq on boxes.ID equals item
            //       orderby boxes.CreateDate descending
            //       select new Models.DataBoxesProducts
            //       {
            //           ID = boxes.ID,
            //           AdminID = boxes.AdminID,
            //           CreateDate = boxes.CreateDate,
            //           Code = boxes.Code,
            //           Status = (BoxesStatus)boxes.Status,
            //           Summary = boxes.Summary,
            //           WarehouseID = boxes.WarehouseID,
            //       };
            //var boxesArray = linqbox.ToArray();
        }
    }

    public static class BoxesProductExtend
    {
        static public object ToPage(this IQueryable<DataBoxesProducts> iquery, string whid, int pageIndex = 1, int pageSize = 20)
        {
            int total = iquery.Count();
            var query = iquery.Skip(pageSize * (pageIndex - 1)).Take(pageSize);

            return new
            {
                Total = total,
                Size = pageSize,
                Index = pageIndex,
                Data = query.ToFillArray(whid)
            };

        }

        static public BoxesProducts[] ToData(this IQueryable<DataBoxesProducts> iquery, string whid, int pageIndex = 1, int pageSize = 20)
        {
            var query = iquery.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
            return query.ToFillArray(whid);

        }



        static public BoxesProducts[] ToFillArray(this IQueryable<DataBoxesProducts> iquery, string whid, PvWmsRepository rep = null)
        {
            if (rep == null)
            {
                rep = new PvWmsRepository();
            }
            var datas = iquery.ToList();

            var boxCodes = datas.Select(item => item.Code).Distinct().ToArray();
            var boxids = datas.Select(item => item.ID).Distinct().ToArray();


            throw new Exception();

            //var linq_waybillbox = from entity in rep.ReadTable<Layers.Data.Sqls.PvWms.WaybillBoxes>()
            //                      where boxids.Contains(entity.BoxID)
            //                      select new { entity.BoxID, entity.ShelveID, entity.Specs, entity.Weight };
            //var waybillboxes = linq_waybillbox.ToArray();

            var linqsortings = from entity in rep.ReadTable<Layers.Data.Sqls.PvWms.Sortings>()
                               where boxCodes.Contains(entity.BoxCode)
                               select new Yahv.Services.Models.Sorting
                               {
                                   ID = entity.ID,
                                   InputID = entity.InputID,
                                   //AdminID = entity.AdminID,
                                   //BoxCode = entity.BoxCode,
                                   //CreateDate = entity.CreateDate,
                                   NoticeID = entity.NoticeID,
                                   //Quantity = entity.Quantity,
                                   //Weight = entity.Weight,
                                   //WaybillID = entity.WaybillID

                               };

            var sortings = linqsortings.ToArray();

            var snoticeids = sortings.Select(item => item.NoticeID).Distinct().ToArray();

            var linq_notices = from notice in rep.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                               join sorting in rep.ReadTable<Layers.Data.Sqls.PvWms.Sortings>() on notice.ID equals sorting.NoticeID
                               where snoticeids.Contains(notice.ID) && notice.Source == (int)NoticeSource.AgentBreakCustoms && notice.WareHouseID == whid
                               select new PickingNotice
                               {
                                   ID = notice.ID,
                                   Weight = sorting.Weight ?? 0,
                                   NetWeight=sorting.NetWeight??0,
                                   Quantity = sorting.Quantity,
                                   BoxCode = notice.BoxCode,
                                   InputID = notice.InputID,
                                   OutputID = notice.OutputID,
                                   ProductID = notice.ProductID,
                                   Conditions = notice.Conditions.JsonTo<NoticeCondition>(),
                                   Sorting = new Yahv.Services.Models.Sorting
                                   {
                                       ID = sorting.ID,
                                       InputID = sorting.InputID,
                                       AdminID = sorting.AdminID,
                                       BoxCode = sorting.BoxCode,
                                       CreateDate = sorting.CreateDate,
                                       NoticeID = sorting.NoticeID,
                                       Quantity = sorting.Quantity,
                                       Weight = sorting.Weight,
                                       NetWeight=sorting.NetWeight,
                                       WaybillID = sorting.WaybillID

                                   }


                               };


            var notices = linq_notices.ToArray();
            var outputids = notices.Where(item => item.OutputID != null).Select(item => item.OutputID).Distinct().ToArray();
            var noticeids = notices.Select(item => item).Distinct().ToArray();
            var inputids = notices.Select(item => item.InputID).Distinct().ToArray();
            var linqinputs = from input in rep.ReadTable<Layers.Data.Sqls.PvWms.Inputs>()
                             where inputids.Contains(input.ID)
                             select new Yahv.Services.Models.Input()
                             {
                                 ID = input.ID,
                                 ProductID = input.ProductID,
                                 ClientID = input.ClientID,
                                 OrderID = input.OrderID,
                                 TinyOrderID = input.TinyOrderID,
                                 ItemID = input.ItemID

                             };

            var inputs = linqinputs.ToArray();





            var clientids = inputs.Select(item => item.ClientID).Distinct().ToArray();
            var linqClients = from client in rep.ReadTable<Layers.Data.Sqls.PvWms.ClientsTopView>()
                              where clientids.Contains(client.ID)
                              select new
                              {
                                  client.ID,
                                  client.Name,
                                  client.EnterCode,
                              };

            var clients = linqClients.ToArray();

            foreach (var item in inputs)
            {
                item.ClientName = clients.Where(tem => item.ClientID == tem.ID).FirstOrDefault()?.Name;

            }


            List<Yahv.Services.Models.Output> outputs = null;
            if (outputids.Length > 0)
            {
                var linqoutputs = from output in rep.ReadTable<Layers.Data.Sqls.PvWms.Outputs>()
                                  where outputids.Contains(output.ID)
                                  select new Yahv.Services.Models.Output()
                                  {
                                      ID = output.ID,
                                      InputID = output.InputID,
                                      OrderID = output.OrderID,
                                      TinyOrderID = output.TinyOrderID
                                  };

                outputs = linqoutputs.ToList();
            }


            var storages = from storage in rep.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                           where inputids.Contains(storage.InputID)
                           select new
                           {
                               storage.InputID,
                               storage.SortingID,
                               storage.ShelveID
                           };
            var productids = inputs.Select(item => item.ProductID).Distinct().ToArray();

            var products = from product in rep.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>()
                           where productids.Contains(product.ID)
                           select new Yahv.Services.Models.CenterProduct
                           {
                               ID = product.ID,
                               PartNumber = product.PartNumber,
                               Manufacturer = product.Manufacturer,
                               PackageCase = product.PackageCase,
                               Packaging = product.Packaging
                           };




            foreach (var item in notices)
            {
                item.Input = inputs.Where(tem => tem.ID == item.InputID).FirstOrDefault();
                if (outputs != null)
                {
                    item.Output = outputs.Where(tem => tem.ID == item.OutputID).FirstOrDefault();
                }
                item.Product = products.Where(tem => tem.ID == item.ProductID).FirstOrDefault();
                //var sorting = sortings.Where(tem => tem.InputID == item.InputID).FirstOrDefault();
                //if (sorting != null)
                //{
                //    item.Sorting = new Yahv.Services.Models.Sorting
                //    {
                //        ID = sorting.ID,
                //        InputID = sorting.InputID,
                //        AdminID = sorting.AdminID,
                //        BoxCode = sorting.BoxCode,
                //        CreateDate = sorting.CreateDate,
                //        NoticeID = sorting.NoticeID,
                //        Quantity = sorting.Quantity,
                //        Weight = sorting.Weight,
                //        WaybillID = sorting.WaybillID
                //    };
                //}


            }

            foreach (var item in datas)
            {
                var ns = notices.Where(tem => tem.Sorting.BoxCode == item.Code && tem.Conditions != null).ToArray();

                if (ns.Any(tem => tem.Conditions.IsCCC))
                {
                    item.IsCCC = true;
                }

                if (ns.Any(tem => tem.Conditions.IsCIQ))
                {
                    item.IsCIQ = true;
                }

                if (ns.Any(tem => tem.Conditions.IsEmbargo))
                {
                    item.IsEmbargo = true;
                }

                if (ns.Any(tem => tem.Conditions.IsHighPrice))
                {
                    item.IsHighPrice = true;
                }

                //var waybox = waybillboxes.Where(tem => tem.BoxID == item.ID).FirstOrDefault();

                item.EnterCode = clients.Where(tem => tem.ID == ns.FirstOrDefault()?.Input?.ClientID).FirstOrDefault()?.EnterCode;
                item.Notices = ns;
                item.TotalParts = ns.Count();
                //item.TotalWeight = waybox?.Weight ?? notices.Where(tem => tem.BoxCode == item.Code)?.Sum(tem => tem.Weight ?? 0) ?? 0;
                //item.ShelveID = waybox?.ShelveID ?? item.Notices.FirstOrDefault()?.ShelveID;
                //item.BoxingSpecs = waybox?.Specs ?? 0;


            }


            return datas.Select(item => (BoxesProducts)item).ToArray();
        }

    }
}
