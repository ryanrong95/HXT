using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class CostApplyFilesView
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        public CostApplyFilesView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public CostApplyFilesView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        public List<CostApplyFilesViewModel> GetResults(string costApplyID)
        {
            var costApplyFiles = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.CostApplyFiles>();

            var results = from costApplyFile in costApplyFiles
                          where costApplyFile.CostApplyID == costApplyID
                             && costApplyFile.Status == (int)Enums.Status.Normal
                          orderby costApplyFile.CreateDate descending
                          select new CostApplyFilesViewModel
                          {
                              CostApplyFileID = costApplyFile.ID,
                              FileName = costApplyFile.Name,
                              FileFormat = costApplyFile.FileFormat,
                              Url = costApplyFile.URL,
                          };

            return results.ToList();
        }
    }
    
    public class CostApplyFilesViewModel
    {
        /// <summary>
        /// CostApplyFileID
        /// </summary>
        public string CostApplyFileID { get; set; } = string.Empty;

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// 文件格式
        /// </summary>
        public string FileFormat { get; set; } = string.Empty;

        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; set; } = string.Empty;
    }
}
