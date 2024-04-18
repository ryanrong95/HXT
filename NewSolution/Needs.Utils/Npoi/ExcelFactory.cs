using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;

namespace Needs.Utils.Npoi
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
        /// 读取文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static IWorkbook ReadFile(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return WorkbookFactory.Create(file);
            }
        }
    }
}
