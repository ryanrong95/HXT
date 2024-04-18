using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Utils.Extends;
using Yahv.Utils.Serializers;

namespace Yahv.PvWsOrder.Services.Extends
{
    /// <summary>
    /// 导出电子文件扩展类
    /// </summary>
    public static class ExportFileExtends
    {
        /// <summary>
        /// 租赁电子合同内容
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> LeasingContractDic(object obj)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();   //创建键值对   第一个string 为书签名称 第二个string为要填充的数据
            var query = obj.GetQueryDictionary();
            #region 组装内容
            dic.Add("CreateTime", DateTime.Now.ToString("yyyy年MM月dd日"));
            dic.Add("ClientName", query["ClientName"].ToString());
            dic.Add("ClientName1", query["ClientName1"].ToString());
            #endregion
            return dic;
        }

        /// <summary>
        /// 会员端导出
        /// </summary>
        /// <param name="order"></param>
        /// <param name="filePath">文件保存路径</param>
        static public void Export(this ClientModels.LsOrderExtends order, string filePath)
        {
            //拼接export对象
            var export = new ExportFile();
            export.FilePath = filePath;
            export.TempFilePath = "Template\\服务协议模板(书签).docx";
            export.dic = LeasingContractDic(new
            {
                ClientName = "客户1",
                ClientName1 = "客户2"
            });
            export.SaveAs();
        }

        /// <summary>
        /// 管理端导出
        /// </summary>
        /// <param name="order"></param>
        static public void Export(this Models.LsOrder order, string fileName)
        {
            Common.FileDirectory fileDir = new Common.FileDirectory(fileName, Underly.FileType.Contract);
            fileDir.CreateDirectory();
            string filePath = fileDir.DownLoadRoot + fileName;
            //拼接export对象
            var export = new ExportFile();
            export.FilePath = filePath;
            export.TempFilePath = @"Content\templates\服务协议模板(书签).docx";
            export.dic = LeasingContractDic(new
            {
                ClientName = order.Payee.Name,
                ClientName1 = order.wsClient.Name,
            });
            export.SaveAs();
        }
    }

    /// <summary>
    /// 合同类
    /// </summary>
    public class ExportFile
    {
        internal ExportFile() { }

        /// <summary>
        /// 文件保存路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 合同内容
        /// </summary>
        public Dictionary<string, string> dic { get; set; }

        /// <summary>
        /// 文件模板路径
        /// </summary>
        public string TempFilePath { get; set; }

        /// <summary>
        /// 保存文件
        /// </summary>
        public void SaveAs()
        {
            var tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TempFilePath);
            Aspose.Words.Document doc = new Aspose.Words.Document(tempPath);//新建一个空白的文档
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
            foreach (var key in dic.Keys)   //循环键值对
            {
                builder.MoveToBookmark(key);  //将光标移入书签的位置
                builder.Write(dic[key]);   //填充值
            }
            doc.Save(FilePath); //保存word
        }
    }
}
