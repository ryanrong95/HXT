using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 上传未上传的报关单
    /// </summary>
    public class UploadDecHead : DecHead
    {
        public string InputerID
        {
            get; set;
        }
        /// <summary>
        /// 币种
        /// </summary>
        public string Currency
        {
            get; set;
        }

        /// <summary>
        /// 报关总价
        /// </summary>
        public decimal? DecAmount
        {
            get; set;
        }

        public string ClientCode { get; set; }

        public Enums.ClientType ClientType { get; set; }

        public string ClientName { get; set; }

        public string ConsigneeAddress { get; set; }

        /// <summary>
        /// 报关单文件
        /// </summary>
        public IEnumerable<DecHeadFile> files { get; set; }

        /// <summary>
        /// 报关单是否上传
        /// </summary>
        public string IsDecHeadFile
        {
            get
            {
                var file = this.files.Where(item => item.FileType == Enums.FileType.DecHeadFile);
                return file.Count() == 0 ? "否" : "是";
            }
        }


        /// <summary>
        /// 报关单文件
        /// </summary>
        public DecHeadFile decheadFile
        {
            get { return this.files.Where(item => item.FileType == Enums.FileType.DecHeadFile).FirstOrDefault(); }
        }


        /// 关税发票是否上传
        /// </summary>
        public string IsDecHeadTariffFile
        {
            get
            {
                var file = this.files.Where(item => item.FileType == Enums.FileType.DecHeadTariffFile);
                return file.Count() == 0 ? "否" : "是";
            }
        }

        /// <summary>
        /// 增值税发票是否上传
        /// </summary>
        public string IsDecHeadVatFile
        {
            get
            {
                var file = this.files.Where(item => item.FileType == Enums.FileType.DecHeadVatFile);
                return file.Count() == 0 ? "否" : "是";
            }
        }


        /// <summary>
        /// 关税发票文件
        /// </summary>
        public DecHeadFile decheadTariffFile
        {
            get { return this.files.Where(item => item.FileType == Enums.FileType.DecHeadTariffFile).FirstOrDefault(); }
        }

        /// <summary>
        /// 增值税发票文件
        /// </summary>
        public DecHeadFile decheadVatFile
        {
            get { return this.files.Where(item => item.FileType == Enums.FileType.DecHeadVatFile).FirstOrDefault(); }
        }

        /// <summary>
        /// 单据状态名字
        /// </summary>
        public string StatusName
        {
            get
            {
                return MultiEnumUtils.ToText<Enums.CusDecStatus>(this.CusDecStatus);
            }
        }

        /// <summary>
        /// 报关单是否 已经转换舱单
        /// </summary>
        public bool Transformed { get; set; }

        //public string InspQuarName
        //{
        //    get
        //    {
        //        if (this.IsInspection && this.IsQuarantine.Value)
        //        {
        //            return "是/检疫";
        //        }
        //        else if (this.IsInspection == true && this.IsQuarantine == false)
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
        /// 运输批次号
        /// </summary>
        public string VoyageID { get; set; }

        /// <summary>
        /// 运输类型
        /// </summary>
        public Enums.VoyageType VoyageType { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        public decimal? TotalQty { get; set; }

        /// <summary>
        /// 型号数
        /// </summary>
        public int? ModelAmount { get; set; }

        /// <summary>
        /// 报关总金额
        /// </summary>
        public decimal? TotalAmount { get; set; }
    }
}
