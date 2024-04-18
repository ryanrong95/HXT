using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class TaxManageForWxView : QueryView<TaxManageForWxViewModel, ScCustomsReponsitory>
    {
        private string _invoiceNoticeID { get; set; }

        public TaxManageForWxView()
        {

        }

        public TaxManageForWxView(string invoiceNoticeID)
        {
            this._invoiceNoticeID = invoiceNoticeID;
        }

        public TaxManageForWxView(ScCustomsReponsitory reponsitory, IQueryable<TaxManageForWxViewModel> iQueryable) : base(reponsitory, iQueryable)
        {

        }

        protected override IQueryable<TaxManageForWxViewModel> GetIQueryable()
        {
            var taxManages = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.TaxManage>();
            var taxManageMaps = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.TaxManageMap>();

            var iQuery = from taxManage in taxManages
                         join taxManageMap in taxManageMaps on taxManage.ID equals taxManageMap.TaxManageID
                         where taxManageMap.InvoiceNoticeID == this._invoiceNoticeID
                            && taxManage.Status == (int)Enums.Status.Normal
                         select new TaxManageForWxViewModel
                         {
                             TaxManageID = taxManage.ID,
                             InvoiceCode = taxManage.InvoiceCode,
                             InvoiceNo = taxManage.InvoiceNo,
                             InvoiceDate = taxManage.InvoiceDate,
                             Amount = taxManage.Amount,
                             InvoiceType = (Enums.InvoiceType)taxManage.InvoiceType,
                         };

            return iQuery;
        }

        public TaxManageForWxViewModel[] GetTaxManages()
        {
            IQueryable<TaxManageForWxViewModel> iquery = this.IQueryable.Cast<TaxManageForWxViewModel>();

            return iquery.ToArray();
        }

        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<TaxManageForWxViewModel> iquery = this.IQueryable.Cast<TaxManageForWxViewModel>();
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myTaxManages = iquery.ToArray();

            var ienums_linq = from taxManage in ienum_myTaxManages
                              orderby taxManage.InvoiceCode, taxManage.InvoiceNo, taxManage.InvoiceDate
                              select new TaxManageForWxViewModel
                              {
                                  TaxManageID = taxManage.TaxManageID,
                                  InvoiceCode = taxManage.InvoiceCode,
                                  InvoiceNo = taxManage.InvoiceNo,
                                  InvoiceDate = taxManage.InvoiceDate,
                                  Amount = taxManage.Amount,
                                  InvoiceType = taxManage.InvoiceType,
                              };

            var results = ienums_linq;

            Func<TaxManageForWxViewModel, object> convert = item => new
            {
                TaxManageID = item.TaxManageID,
                InvoiceCode = item.InvoiceCode,
                InvoiceNo = item.InvoiceNo,
                InvoiceDate = item.InvoiceDate?.ToString("yyyy.MM.dd"),
                Amount = item.Amount.ToString("0.00"),
                InvoiceTypeInt = (int)item.InvoiceType,
                InvoiceTypeName = item.InvoiceType.GetDescription(),
            };

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(convert).Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        /// <summary>
        /// 插入新的发票信息
        /// </summary>
        /// <param name="newModel"></param>
        public void InsertNewInvoice(TaxManageForWxInsertModel newModel)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var existTaxManage = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.TaxManage>()
                    .Where(t => t.InvoiceCode == newModel.InvoiceCode
                             && t.InvoiceNo == newModel.InvoiceNo
                             && t.InvoiceDate == newModel.InvoiceDate
                             && t.Status == (int)Enums.Status.Normal)
                    .FirstOrDefault();

                if (existTaxManage != null)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.TaxManageMap>(new Layer.Data.Sqls.ScCustoms.TaxManageMap
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        TaxManageID = existTaxManage.ID,
                        InvoiceNoticeID = newModel.InvoiceNoticeID,
                    });
                }
                else
                {
                    #region 查发票类型

                    var theInvoiceNotice = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNotices>()
                        .Where(t => t.ID == newModel.InvoiceNoticeID).FirstOrDefault();
                    int invoiceType = theInvoiceNotice.InvoiceType;
                    string clientID = theInvoiceNotice.ClientID;

                    #endregion

                    #region 查公司名称

                    var clients = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
                    var companies = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();

                    var theCompanyMdoel = (from client in clients
                                           join company in companies on client.CompanyID equals company.ID
                                           where client.ID == clientID
                                           select new
                                           {
                                               ClientID = client.ID,
                                               CompanyName = company.Name,
                                           }).FirstOrDefault();

                    string companyName = theCompanyMdoel.CompanyName;

                    #endregion

                    string newTaxManageID = Guid.NewGuid().ToString("N");

                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.TaxManage>(new Layer.Data.Sqls.ScCustoms.TaxManage
                    {
                        ID = newTaxManageID,
                        InvoiceCode = newModel.InvoiceCode,
                        InvoiceNo = newModel.InvoiceNo,
                        InvoiceDate = newModel.InvoiceDate,
                        SellsName = companyName,
                        Amount = newModel.InvoiceAmount,
                        IsVaild = 0,
                        InvoiceType = invoiceType,
                        BusinessType = (int)Enums.BusinessType.Declare,
                        Status = (int)Enums.Status.Normal,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                    });

                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.TaxManageMap>(new Layer.Data.Sqls.ScCustoms.TaxManageMap
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        TaxManageID = newTaxManageID,
                        InvoiceNoticeID = newModel.InvoiceNoticeID,
                    });
                }
            }

            UpdateInvoiceNoticeItems(newModel.InvoiceDate, newModel.InvoiceNoticeID);
        }

        /// <summary>
        /// 更新 InvoiceNoticeItems
        /// </summary>
        /// <param name="invoiceDate"></param>
        /// <param name="invoiceNoticeID"></param>
        private void UpdateInvoiceNoticeItems(DateTime? invoiceDate, string invoiceNoticeID)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                #region 使用当前发票信息, 更新 InvoiceNoticeItem

                var taxManages = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.TaxManage>();
                var taxManageMaps = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.TaxManageMap>();

                var taxManageModels = (from taxManage in taxManages
                                       join taxManageMap in taxManageMaps on taxManage.ID equals taxManageMap.TaxManageID
                                       where taxManage.Status == (int)Enums.Status.Normal
                                          && taxManageMap.InvoiceNoticeID == invoiceNoticeID
                                       select new
                                       {
                                           TaxManageID = taxManage.ID,
                                           InvoiceCode = taxManage.InvoiceCode,
                                           InvoiceNo = taxManage.InvoiceNo,
                                       }).ToArray();

                string invoiceNos = string.Empty;
                if (taxManageModels != null && taxManageModels.Length > 0)
                {
                    var arr = taxManageModels.Select(t => t.InvoiceCode + t.InvoiceNo).ToArray();
                    invoiceNos = string.Join(",", arr);
                }

                if (invoiceDate == null)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>(new
                    {
                        InvoiceNo = invoiceNos,
                        UpdateDate = DateTime.Now,
                    }, t => t.InvoiceNoticeID == invoiceNoticeID);
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>(new
                    {
                        InvoiceNo = invoiceNos,
                        InvoiceDate = invoiceDate,
                        UpdateDate = DateTime.Now,
                    }, t => t.InvoiceNoticeID == invoiceNoticeID);
                }

                #endregion
            }
        }

        /// <summary>
        /// 删除发票信息
        /// </summary>
        /// <param name="taxManageID"></param>
        /// <param name="invoiceNoticeID"></param>
        public void DeleteInvoice(string taxManageID, string invoiceNoticeID)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.TaxManageMap>(t => t.TaxManageID == taxManageID && t.InvoiceNoticeID == invoiceNoticeID);

                int theTaxManageIDMapCount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.TaxManageMap>()
                    .Where(t => t.TaxManageID == taxManageID).Count();

                if (theTaxManageIDMapCount == 0)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.TaxManage>(new
                    {
                        Status = (int)Enums.Status.Delete,
                    }, t => t.ID == taxManageID);
                }
            }

            UpdateInvoiceNoticeItems(null, invoiceNoticeID);
        }
    }

    public class TaxManageForWxViewModel
    {
        public string TaxManageID { get; set; }

        public string InvoiceCode { get; set; }

        public string InvoiceNo { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public decimal Amount { get; set; }

        public Enums.InvoiceType InvoiceType { get; set; }
    }

    public class TaxManageForWxInsertModel
    {
        public string InvoiceNoticeID { get; set; }

        //public string InvoiceTypeInt { get; set; }

        public string InvoiceCode { get; set; }

        public string InvoiceNo { get; set; }

        public decimal InvoiceAmount { get; set; }

        public DateTime InvoiceDate { get; set; }
    }
}
