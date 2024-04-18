using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PsWms.SzMvc.Services.Views
{
    public class VoucherInfoView
    {
        private string[] _voucherIDs { get; set; }

        public VoucherInfoView(string[] voucherIDs)
        {
            this._voucherIDs = voucherIDs;
        }

        public VoucherInfoViewModel[] GetVoucherInfos()
        {
            using (var PsOrderRepository = new PsOrderRepository())
            {
                var vouchers = PsOrderRepository.ReadTable<Layers.Data.Sqls.PsOrder.Vouchers>();
                var theVouchers = (from voucher in vouchers
                                      where this._voucherIDs.Contains(voucher.ID)
                                      select new VoucherInfoViewModel
                                      {
                                          VoucherID = voucher.ID,
                                          CutDateIndex = voucher.CutDateIndex.ToString(),
                                      }).ToArray();

                return theVouchers;
            }
        }
    }

    public class VoucherInfoViewModel
    {
        public string VoucherID { get; set; }

        public string CutDateIndex { get; set; }
    }
}
