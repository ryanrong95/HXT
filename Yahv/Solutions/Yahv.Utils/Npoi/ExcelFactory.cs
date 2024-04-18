using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Utils.Npoi
{
    public static class ExcelFactory
    {
        /// <summary>
        /// Excel版本
        /// </summary>
        public enum ExcelVersion
        {
            /// <summary>
            /// Excel03
            /// </summary>
            Excel03 = 0,

            /// <summary>
            /// Excel07及以上版本
            /// </summary>
            Excel07 = 1
        }

        /// <summary>
        /// 创建Excel workbook
        /// </summary>
        /// <param name="version">Excel版本，默认xlsx</param>
        /// <returns></returns>
        public static IWorkbook Create(ExcelVersion version = ExcelVersion.Excel07)
        {
            IWorkbook workbook = null;

            switch (version)
            {
                case ExcelVersion.Excel03:
                    workbook = new HSSFWorkbook();
                    break;
                case ExcelVersion.Excel07:
                    workbook = new XSSFWorkbook();
                    break;
                default:
                    workbook = new XSSFWorkbook();
                    break;
            }

            return workbook;
        }

        /// <summary>
        /// 根据模板创建Excel workbook
        /// </summary>
        /// <param name="template">指定模板</param>
        /// <returns></returns>
        public static IWorkbook CreateByTemplate(string template)
        {
            if (!File.Exists(template))
            {
                return null;
            }

            using (FileStream file = new FileStream(template, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = WorkbookFactory.Create(file);
                return workbook;
            }
        }
    }
}
