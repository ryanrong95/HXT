using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Enums;
using Wms.Services.Models;
using Yahv.Services.Enums;

namespace WebApp.Services
{
    public class Notice
    {
        /// <summary>
        /// 通知类型：入库通知、出库通知、分拣通知、检测通知、捡货通知、客户自提通知
        /// </summary>

        public NoticeSource Type { get; set; }

        /// <summary>
        /// 仓库编号(通知给哪个库房)
        /// </summary>
        public string WareHouseID { get; set; }

        /// <summary>
        /// 运单编号
        /// </summary>
        public string WaybillID { get; set; }

        /// <summary>
        /// 产品批次号
        /// </summary>
        public string DateCode { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }


        /// <summary>
        /// 条件(Json开发)
        /// </summary>
        public NoticeCondition Conditions { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }


        /// <summary>
        /// 来源：1.采购：采购入库、馈赠入库、退货入库、补货入库；2.销售：销售出库、馈赠出库、补货出库；3.代仓储：代收货入库、代发货出库（包含代报关：）、报关入库、报关出库；4.备货：备货入库；5.转运：转运入库、转运出库
        /// </summary>

        public NoticeType Source { get; set; }
         
        /// <summary>
        /// 目标：Address：按地址分拣、转运；
        ///       Customs：报关；
        ///       Owner：按所有人分拣，按客户分拣，按内部公司；
        ///       OrderID：按订单分拣；
        ///       OrderID->Notice：按订单->通知分拣；
        ///       Purchaser：按采购员分拣；
        ///       Sales：按销售分拣
        ///       Cs：按客户分拣
        /// </summary>
        public NoticesTarget Target { get; set; }
        
        /// <summary>
        /// 重量
        /// </summary>
        public decimal? Weight { get; set; }

        /// <summary>
        /// 体积
        /// </summary>
        public decimal? Volume { get; set; }

        /// <summary>
        /// 产品信息
        /// </summary>
        public Product Product { get; set; }

        /// <summary>
        /// 文件信息
        /// </summary>
        public File [] Files { get; set; }

        /// <summary>
        /// 进项信息
        /// </summary>
        public Input Input { get; set; }

        /// <summary>
        /// 销项信息
        /// </summary>
        public Output Output { get; set; }


        /// <summary>
        /// 供应商
        /// </summary>
        public string Supplier { get; set; }

    }
}
