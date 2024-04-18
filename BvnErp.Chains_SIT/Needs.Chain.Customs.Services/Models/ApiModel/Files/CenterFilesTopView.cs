using Layers.Data.Sqls;
using Needs.Utils.Converters;
using Needs.Utils.Serializers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 中心文件视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class CenterFilesTopView : QueryView<Models.CenterFileDescription, PvCenterReponsitory>, IDisposable
    {
        static string FileServerUrl = System.Configuration.ConfigurationManager.AppSettings["PvDataFileUrl"];
        static string Address = FileServerUrl + "/api/Upload";

        public CenterFilesTopView()
        {

        }

        public CenterFilesTopView(PvCenterReponsitory reponsitory) : base(reponsitory)
        {
        }

        public CenterFilesTopView(PvCenterReponsitory reponsitory, IQueryable<Models.CenterFileDescription> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Models.CenterFileDescription> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.FilesDescriptionTopView>()
                   select new Models.CenterFileDescription
                   {
                       ID = entity.ID,
                       WsOrderID = entity.WsOrderID,
                       LsOrderID = entity.LsOrderID,
                       ApplicationID = entity.ApplicationID,
                       WaybillID = entity.WaybillID,
                       NoticeID = entity.NoticeID,
                       StorageID = entity.StorageID,
                       CustomName = entity.CustomName,
                       Type = entity.Type,
                       Url = entity.Url,
                       CreateDate = entity.CreateDate,
                       ClientID = entity.ClientID,
                       AdminID = entity.AdminID,
                       InputID = entity.InputID,
                       Status = (Models.FileDescriptionStatus)entity.Status,
                       PayID = entity.PayID,
                       ShipID = entity.ShipID,
                   };
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="obj"></param>
        public void Modify(object obj, params string[] filesID)
        {
            if (filesID == null || filesID.Length == 0)
            {
                throw new NotImplementedException("不实现 全体Modify 的功能");
            }
            this.Reponsitory.Update<Layers.Data.Sqls.PvCenter.FilesDescription>(obj, item => filesID.Contains(item.ID));

        }


        /// <summary>
        /// 逻辑删除文件关系
        /// </summary>
        /// <param name="fileID">文件ID</param>
        public void AbandonFile(string fileID)
        {
            if (string.IsNullOrWhiteSpace(fileID))
            {
                throw new NotImplementedException("不能实现失效的功能！！");
            }
            var file = this.GetIQueryable().Select(item => new
            {
                item.ID,
                item.Url
            }).SingleOrDefault(item => item.ID == fileID);

            //补充删除
            if (file == null)
            {
                throw new NotSupportedException("要失效的文件并不存在！！");
            }

            this.Reponsitory.Update<Layers.Data.Sqls.PvCenter.FilesDescription>(new { Status = Enums.Status.Delete }, item => item.ID == fileID);
        }

        /// <summary>
        /// 逻辑删除文件关系
        /// </summary>
        public void NewDeleteFile(string clientID, int type, string ApplicationID)
        {
            this.Reponsitory.Update<Layers.Data.Sqls.PvCenter.FilesDescription>(new { Status = Enums.Status.Delete }, item => item.ClientID == clientID && item.Type == type && item.ApplicationID == ApplicationID);
        }
        /// <summary>
        /// 删除文件关系与文件本身
        /// </summary>
        /// <param name="fileID">文件ID</param>
        public void DeleteFile(string fileID)
        {
            if (string.IsNullOrWhiteSpace(fileID))
            {
                throw new NotImplementedException("不能实现删除的功能！！");
            }
            var file = this.GetIQueryable().Select(item => new
            {
                item.ID,
                item.Url
            }).SingleOrDefault(item => item.ID == fileID);

            //补充删除
            if (file == null)
            {
                throw new NotSupportedException("要删除的文件并不存在！！");
            }

            var finfo = new System.IO.FileInfo(string.Concat(System.Configuration.ConfigurationManager.AppSettings["SavePath"].TrimEnd('\\'), @"\", file.Url.Replace('/', '\\')));
            if (finfo.Exists)
            {
                finfo.Delete();
            }

            this.Reponsitory.Delete<Layers.Data.Sqls.PvCenter.FilesDescription>(item => item.ID == fileID);
        }


        /// <summary>
        ///插入
        /// </summary>
        /// <param name="obj"></param>
        public void Insert(CenterFileDescription entity)
        {
            DateTime liunxStart = new DateTime(1970, 1, 1);
            var linuxtime = (DateTime.Now - liunxStart).Ticks;
            string topID = "F" + linuxtime;
            var file = new Layers.Data.Sqls.PvCenter.FilesDescription();
            file.ID = topID;
            file.AdminID = entity.AdminID;
            file.Url = entity.Url;
            file.ApplicationID = entity.ApplicationID;
            file.CustomName = entity.CustomName;
            file.Type = entity.Type;
            file.CreateDate = entity.CreateDate.Value;
            file.Status = (int)entity.Status;
            this.Reponsitory.Insert<Layers.Data.Sqls.PvCenter.FilesDescription>(file);
            //  this.Reponsitory.Insert<Layers.Data.Sqls.PvCenter.FilesDescription>(entity);

        }

        /// <summary>
        ///插入
        /// </summary>
        /// <param name="obj"></param>
        public void Insert(CenterFileDescription entity, string topID)
        {
            //DateTime liunxStart = new DateTime(1970, 1, 1);
            //var linuxtime = (DateTime.Now - liunxStart).Ticks;
            //string topID = "F" + linuxtime;
            var file = new Layers.Data.Sqls.PvCenter.FilesDescription();
            file.ID = topID;
            file.AdminID = entity.AdminID;
            file.Url = entity.Url;
            file.ApplicationID = entity.ApplicationID;
            file.CustomName = entity.CustomName;
            file.Type = entity.Type;
            file.CreateDate = entity.CreateDate.Value;
            file.Status = (int)entity.Status;
            this.Reponsitory.Insert<Layers.Data.Sqls.PvCenter.FilesDescription>(file);
            //  this.Reponsitory.Insert<Layers.Data.Sqls.PvCenter.FilesDescription>(entity);

        }


        /// <summary>
        /// 上传
        /// </summary>
        /// <remarks>
        /// 外部调用
        /// </remarks>
        /// <param name="fileName">文件地址</param>
        /// <param name="dic">附带的参数</param>
        /// <example>
        /// 参见本类下的 test()
        /// </example>
        //static public Models.UploadResult[] Upload(string fileName, object dic = null)
        //{
        //    //目标就写死把？
        //    string address = Address;

        //    if (dic != null)
        //    {
        //        address = address + "?" + dic.GetQueryParams();
        //    }
        //    using (WebClient client = new WebClient())
        //    {
        //        var bytes = client.UploadFile(new Uri(address), "POST", fileName);
        //        string message = Encoding.UTF8.GetString(bytes);
        //        return message.JsonTo<Models.UploadResult[]>();
        //    }
        //}

        /// <summary>
        /// 上传
        /// </summary>
        /// <remarks>
        /// 外部调用
        /// </remarks>
        /// <param name="fileName">文件地址</param>
        /// <param name="dic">附带的参数</param>
        /// <returns>中心文件响应结果</returns>
        /// <example>
        /// 参见本类下的 test()
        /// </example>
        [Obsolete("经过：小辉、刘芳、董建、陈翰共同商议，做暂时废弃处理")]
        static public Models.UploadResult[] Upload<EnumType>(string fileName, EnumType type, object dic = null)
        {
            //目标就写死把？
            throw new NotSupportedException("不支持调用！");

            string address = Address;

            if (dic != null)
            {
                var query = dic.GetQueryDictionary();
                query["Type"] = type;

                address = address + "?" + query.GetQueryParams();
            }
            using (WebClient client = new WebClient())
            {
                var bytes = client.UploadFile(new Uri(address), "POST", fileName);
                string message = Encoding.UTF8.GetString(bytes);
                return message.JsonTo<Models.UploadResult[]>();
            }
        }

        /// <summary>
        /// 上传
        /// </summary>
        /// <remarks>
        /// 外部调用
        /// </remarks>
        /// <param name="fileName">文件地址</param>
        /// <param name="dic">附带的参数</param>
        /// <param name="type">通用文件类型</param>
        /// <returns>中心文件响应结果</returns>
        /// <example>
        /// 参见本类下的 test()
        /// </example>
        static public Models.UploadResult[] Upload(string fileName, ApiModels.Files.FileType type, object dic = null)
        {
            //目标就写死把？

            string address = Address;

            if (dic != null)
            {
                var query = dic.GetQueryDictionary().ToDictionary(item => item.Key, item =>
                {
                    if (item.Value is string)
                    {
                        return System.Web.HttpUtility.UrlEncode(item.Value as string);
                    }
                    return item.Value;
                });
                query["Type"] = type;

                address = address + "?" + query.GetQueryParams();
            }
            using (WebClient client = new WebClient())
            {
                var bytes = client.UploadFile(new Uri(address), "POST", fileName);
                string message = Encoding.UTF8.GetString(bytes);
                return message.JsonTo<Models.UploadResult[]>();
            }
        }

        static byte[] ReadBytes(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open,
              FileAccess.Read, FileShare.Read))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));

                return bytes;
            }
        }

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="fileNames">文件地址</param>
        /// <param name="dic">附带的参数</param>
        /// <param name="type">通用文件类型</param>
        /// <returns>中心文件响应结果</returns>
        static public Models.UploadResult[] Upload(ApiModels.Files.FileType type, object dic, params string[] fileNames)
        {
            string address = Address;

            CreateBytes cb = new CreateBytes();
            // 所有表单数据
            ArrayList bytesArray = new ArrayList();

            //// form 数据
            //bytesArray.Add(cb.CreateFieldData("jsonDataStr", ""));

            string ContentType = "application/octet-stream";

            int counter = 1;
            foreach (var fileName in fileNames)
            {
                // 文件表单
                bytesArray.Add(cb.CreateFieldData("file" + (counter++),
                    Path.GetFileName(fileName),
                    ContentType,
                    ReadBytes(fileName)));
            }

            byte[] bytes = cb.JoinBytes(bytesArray);

            // 返回的内容
            byte[] responseBytes;

            if (dic != null)
            {
                var query = dic.GetQueryDictionary();
                query["Type"] = type;

                address = address + "?" + query.GetQueryParams();
            }

            bool uploaded = cb.UploadData(address, bytes, out responseBytes);

            if (uploaded)
            {
                string message = Encoding.UTF8.GetString(responseBytes);
                return message.JsonTo<Models.UploadResult[]>();
            }

            return null;
        }


        static public void test()
        {
#pragma warning disable
            CenterFilesTopView.Upload(@"D:\uuws.b1b.com\2019\11\1F15726086428668107.jpg", new
            {
                WaybillID = "",
                NoticeID = "",
                StorageID = "",
                InputID = "",
                AdminID = "",
                ClientID = "",
                CustomName = "",
                PayID = "",
            });




            CenterFilesTopView.Upload(@"D:\uuws.b1b.com\2019\11\1F15726086428668107.jpg", ApiModels.Files.FileType.FollowGoods, new
            {
                WaybillID = "",
                NoticeID = "",
                StorageID = "",
                InputID = "",
                AdminID = "",
                ClientID = "",
                CustomName = "",
                PayID = "",
            });

            string[] files = { @"D:\Images\小甲\IMG_1276.JPG", @"D:\Images\小甲\IMG_1291.JPG" };

            CenterFilesTopView.Upload(ApiModels.Files.FileType.Test, new
            {
                WaybillID = "",
                NoticeID = "",
                StorageID = "",
                InputID = "",
                AdminID = "",
                ClientID = "",
                CustomName = "",
                PayID = "",
            }, files);

            CenterFilesTopView.Upload(ApiModels.Files.FileType.Test, new
            {
                WaybillID = "",
                NoticeID = "",
                StorageID = "",
                InputID = "",
                AdminID = "",
                ClientID = "",
                CustomName = "",
                PayID = "",
            }, @"D:\Images\小甲\IMG_1276.JPG", @"D:\Images\小甲\IMG_1291.JPG");

#pragma warning restore
        }
    }
}

