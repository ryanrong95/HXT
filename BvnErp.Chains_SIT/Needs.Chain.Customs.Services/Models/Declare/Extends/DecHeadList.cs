using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DecHeadList : IUnique
    {
        public string ID { get; set; }
        public string OrderID { get; set; }
        public string ContrNo { get; set; }
        public string BillNo { get; set; }
        public string EntryId { get; set; }
        public string PreEntryId { get; set; }
        public string AgentName { get; set; }
        public string ConsignorName { get; set; }
        public string ConsigneeName { get; set; }

        public string ConsigneeAddress { get; set; }

        //public bool IsInspection { get; set; }
        //public bool? IsQuarantine { get; set; }
        public DateTime CreateTime { get; set; }
        public string InputerID { get; set; }
        public string Status { get; set; }

        public string SeqNo { get; set; }

        public decimal GrossWt { get; set; }

        public decimal TotalQty { get; set; }

        public int ModelAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public string ClientID { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }

        public Enums.ClientType ClientType { get; set; }

        public DateTime DDate { get; set; }

        /// <summary>
        /// 是否包车
        /// </summary>
        //[Description("是否包车")]
        public bool IsCharterBus { get; set; }

        /// <summary>
        /// 是否高价值
        /// </summary>
        //[Description("是否高价值")]
        public bool IsHighValue { get; set; }

        public string URL { get; set; }

        public bool IsHeadFile { get; set; }

        /// <summary>
        /// 是否商检
        /// </summary>
        //[Description("是否商检")]
        public bool IsInspection { get; set; }

        /// <summary>
        /// 是否检疫
        /// </summary>
        //[Description("是否检疫")]
        public bool IsQuarantine { get; set; }

        /// <summary>
        /// 是否3C
        /// </summary>
        //[Description("是否3C")]
        public bool IsCCC { get; set; }

        /// <summary>
        /// 是否加征关税
        /// </summary>
        public bool IsOrigin { get; set; }

        public bool IsSuccess { get; set; }

        public bool IsSenOrigin { get; set; }

        /// <summary>
        /// 件数
        /// </summary>
        public int? PackNo { get; set; }

        /// <summary>
        /// 毛重
        /// </summary>
        public decimal? GrossWeight { get; set; }

        /// <summary>
        /// 运输批次号
        /// </summary>
        public string VoyageID { get; set; }

        /// <summary>
        /// 运输类型
        /// </summary>
        public Enums.VoyageType VoyageType { get; set; }

        /// <summary>
        /// 制单人ID
        /// </summary>
        public string CreateDeclareAdminID { get; set; }

        /// <summary>
        /// 制单人姓名
        /// </summary>
        public string CreateDeclareAdminName { get; set; }

        /// <summary>
        /// 发单人ID
        /// </summary>
        public string CustomSubmiterAdminID { get; set; }

        /// <summary>
        /// 发单人姓名
        /// </summary>
        public string CustomSubmiterAdminName { get; set; }
        public string DoubleCheckAdminID { get; set; }
        public string DoubleCheckAdminName { get; set; }

        /// <summary>
        /// 单据状态名字
        /// </summary>
        public string StatusName
        {
            get
            {
                return MultiEnumUtils.ToText<Enums.CusDecStatus>(this.Status);

                //using (var view = new Views.BaseCusReceiptCodesView())
                //{
                //    var query = view.Where(item => item.Code == this.Status);
                //    return query.FirstOrDefault()?.Name;
                //}
            }
        }

        /// <summary>
        /// 报关单是否 已经转换舱单
        /// </summary>
        public bool Transformed { get; set; }


        public string CustomsPortCode { get; set; }
        /// <summary>
        /// 口岸
        /// </summary>
        public string CustomsPort
        {
            get
            {
                if (!string.IsNullOrEmpty(this.CustomsPortCode))
                {
                    using (var view = new Views.BaseCustomMasterView())
                    {
                        var query = view.Where(item => item.Code == this.CustomsPortCode);
                        return query.FirstOrDefault()?.Name;
                    }
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// 是否复核退回
        /// </summary>
        public bool IsCheckReturned { get; set; } = false;

        /// <summary>
        /// 箱号
        /// </summary>
        public string PackBox { get; set; }

        //public string InspQuarName
        //{
        //    get
        //    {
        //        if (this.IsInspection && this.IsQuarantine.Value)
        //        {
        //            return "是/检疫";
        //        }
        //        else if (this.IsInspection==true&&this.IsQuarantine==false)
        //        {
        //            return "是/";
        //        }
        //        else if (this.IsInspection == false && this.IsQuarantine == true)
        //        {
        //            return "/检疫";
        //        }
        //        else
        //        {
        //            return "";
        //        }                
        //    }
        //}

        public string GuaranteeNo { get; set; }
    }
}
