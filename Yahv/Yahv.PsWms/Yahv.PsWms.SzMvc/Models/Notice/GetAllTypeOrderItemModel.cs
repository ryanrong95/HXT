using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PsWms.SzMvc.Models
{
    public class GetAllTypeOrderItemSearchModel
    {
        /// <summary>
        /// BatchID
        /// </summary>
        public string BatchID { get; set; }
    }

    public class GetAllTypeOrderItemReturnModel
    {

        public OrderItem[] topview { get; set; }

        public OrderItem[] origin { get; set; }

        public OrderItem[] new1 { get; set; }


        public class OrderItem
        {
            public string OrderItemID { get; set; }

            public string OrderID { get; set; }

            public string ProductID { get; set; }

            public string CustomCode { get; set; }

            public string StocktakingTypeDes { get; set; }

            public int Mpq { get; set; }

            public int PackageNumber { get; set; }

            public int Total { get; set; }

            public string CreateDateDes { get; set; }

            public string ModifyDateDes { get; set; }

            public string StatusDes { get; set; }

            public string BakPartnumber { get; set; }

            public string BakBrand { get; set; }

            public string BakPackage { get; set; }

            public string BakDateCode { get; set; }

            public string NoticeID { get; set; }

            public string NoticeItemID { get; set; }
        }
    }
}