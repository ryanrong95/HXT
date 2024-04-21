using Aspose.Words;
using Aspose.Words.Tables;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.Csrm.Services.Models;

namespace YaHv.Csrm.Services
{
    public static class StorageAgreement
    {
        public static void Export(string clientid, string filePath, bool toHtml = false)
        {
            var client = new Views.Rolls.WsClientsRoll()[clientid];
            string PartA = client.Enterprise.Name;
            string PartB = string.Empty;
            if (client.ServiceType == ServiceType.Warehouse || client.ServiceType == ServiceType.Both)
            {

                switch (client.StorageType)
                {
                    case WsIdentity.Mainland:
                        PartB = "深圳市芯达通供应链管理有限公司";
                        break;
                    case WsIdentity.HongKong:
                        PartB = "香港畅运国际物流有限公司";
                        break;
                    case WsIdentity.Personal:
                        PartB = "深圳市芯达通供应链管理有限公司";
                        break;
                    default:
                        break;
                }
            }
            var tempPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "templates\\香港本地交货协议-芯达通.doc");
            Aspose.Words.Document doc = new Aspose.Words.Document(tempPath);//新建一个空白的文档
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);

            Dictionary<string, string> dic = ContractDic(PartA, PartB);

            foreach (var key in dic.Keys)   //循环键值对
            {
                builder.MoveToBookmark(key);  //将光标移入书签的位置
                builder.Write(dic[key]);   //填充值
            }
            //移动光标至页眉
            builder.MoveToHeaderFooter(HeaderFooterType.HeaderPrimary);
            builder.Write(PartB);
            DeleteFolder(AppDomain.CurrentDomain.BaseDirectory + @"\Files\Dowload\StorageAgreements");//清空文件
            if (toHtml)
            {
                doc.Save(filePath, Aspose.Words.SaveFormat.Html);
            }
            else
            {
                doc.Save(filePath);
            }
        }
        static void DeleteFolder(string dir)
        {
            foreach (string d in System.IO.Directory.GetFileSystemEntries(dir))
            {
                if (System.IO.File.Exists(d))
                {
                    System.IO.FileInfo fi = new System.IO.FileInfo(d);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                        fi.Attributes = System.IO.FileAttributes.Normal;
                    System.IO.File.Delete(d);//直接删除其中的文件  
                }
                else
                {
                    System.IO.DirectoryInfo d1 = new System.IO.DirectoryInfo(d);
                    if (d1.GetFiles().Length != 0)
                    {
                        DeleteFolder(d1.FullName);////递归删除子文件夹
                    }
                    System.IO.Directory.Delete(d);
                }
            }
        }
        static Dictionary<string, string> ContractDic(string PartA, string PartB)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();   //创建键值对   第一个string 为书签名称 第二个string为要填充的数据
            #region 组装内容
            dic.Add("PartA", PartA);
            dic.Add("PartB", PartB);
            dic.Add("PartA1", PartA);
            dic.Add("PartB1", PartB);
            dic.Add("PartB2", PartB);
            #endregion
            return dic;
        }

        /// <summary>
        /// 生成ID
        /// </summary>
        /// <returns>返回ID</returns>
        static public string GenID()
        {
            return Layers.Data.PKeySigner.Pick(PKeyType.FileDecription);
        }

        /// <summary>
        /// 录入文件信息
        /// </summary>
        /// <param name="message">信息对象</param>
        public static object Add(Models.FileMessage message)
        {
            try
            {
                string id = GenID();
                using (var repository = LinqFactory<PvCenterReponsitory>.Create())
                {
                    //营业执照，登记证，企业logo,服务协议
                    if (repository.ReadTable<Layers.Data.Sqls.PvCenter.FilesDescription>().Any(item => item.ClientID == message.ClientID && item.Type == (int)message.Type))
                    {
                        repository.Update<Layers.Data.Sqls.PvCenter.FilesDescription>(new { Status = (int)Yahv.Services.Models.FileDescriptionStatus.Delete }, item => item.ClientID == message.ClientID && item.Type == (int)message.Type);
                    }
                    repository.Insert(new Layers.Data.Sqls.PvCenter.FilesDescription
                    {
                        ID = id,
                        LsOrderID = message.LsOrderID,
                        ApplicationID = message.ApplicationID,
                        WsOrderID = message.WsOrderID,
                        WaybillID = message.WaybillID,
                        NoticeID = message.NoticeID,
                        StorageID = message.StorageID,
                        ShipID = message.ShipID,
                        CustomName = message.CustomName,
                        Type = message.Type,
                        Url = message.Url.Replace(Yahv.Services.Models.CenterFile.Web, ""),
                        CreateDate = DateTime.Now,
                        ClientID = message.ClientID,
                        AdminID = message.AdminID,
                        InputID = message.InputID,
                        Status = (int)Yahv.Services.Models.FileDescriptionStatus.Normal,
                        PayID = message.PayID,
                        StaffID = message.StaffID,
                        ErmApplicationID = message.ErmApplicationID,
                    });
                    return new Yahv.Services.Models.UploadResult
                    {
                        FileID = id,
                        FileName = message.CustomName,
                        Url = Yahv.Services.Models.CenterFile.Web + string.Concat(message.Url)
                    };
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }

    }
}
