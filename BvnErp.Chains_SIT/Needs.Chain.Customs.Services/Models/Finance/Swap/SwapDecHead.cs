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
    /// 换汇报关单
    /// </summary>
    public class SwapDecHead : DecHead
    {
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
        public decimal? SwapAmount
        {
            get; set;
        }

        /// <summary>
        /// 报关单文件
        /// </summary>
        public IEnumerable<DecHeadFile> files { get; set; }

        /// <summary>
        /// 报关单是否上传
        /// </summary>
        public bool IsDecHeadFile
        {
            get
            {
                var file = this.files.Where(item => item.FileType == Enums.FileType.DecHeadFile);
                return file.Count() == 0 ? false : true;
            }
        }

        /// <summary>
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
        /// 报关单文件
        /// </summary>
        public DecHeadFile decheadFile
        {
            get { return this.files.Where(item => item.FileType == Enums.FileType.DecHeadFile).FirstOrDefault(); }
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
    }
}
