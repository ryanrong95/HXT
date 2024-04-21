using Layers.Data;
using Layers.Data.Sqls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Layers.Linq;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Models.Rolls;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Utils.Extends;
using Yahv.Utils.Serializers;

namespace Yahv.Finance.Services.Views.Rolls
{
    public class FilesDescriptionRoll : QueryView<FilesDescription, PvFinanceReponsitory>
    {
        static string FileServerUrl = ConfigurationManager.AppSettings["FinanceApiUrl"];

        public FilesDescriptionRoll()
        {
        }

        public FilesDescriptionRoll(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected FilesDescriptionRoll(PvFinanceReponsitory reponsitory, IQueryable<FilesDescription> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<FilesDescription> GetIQueryable()
        {
            var filesDescriptionOrigin = new FilesDescriptionOrigin(this.Reponsitory);
            var filesMapOrigin = new FilesMapOrigin(this.Reponsitory);

            var iQuery = from filesDescription in filesDescriptionOrigin
                         join filesMap in filesMapOrigin on filesDescription.ID equals filesMap.FileID into filesMapOrigin2
                         where filesDescription.Status == GeneralStatus.Normal
                         orderby filesDescription.CreateDate
                         select new FilesDescription
                         {
                             ID = filesDescription.ID,
                             Type = filesDescription.Type,
                             Url = filesDescription.Url,
                             CustomName = filesDescription.CustomName,
                             CreateDate = filesDescription.CreateDate,
                             CreatorID = filesDescription.CreatorID,
                             Status = filesDescription.Status,

                             FilesMapsIEnum = filesMapOrigin2,
                         };

            return iQuery;
        }

        /// <summary>
        /// 根据 FilesMap 中的 Name 和 Value 查询
        /// </summary>
        /// <param name="filesMapName"></param>
        /// <param name="filesMapValue"></param>
        /// <returns></returns>
        public FilesDescriptionRoll SearchByFilesMapValue(string filesMapName, string filesMapValue)
        {
            var linq = from query in this.IQueryable
                       where query.FilesMapsIEnum.Any(t => t.Name == filesMapName && t.Value == filesMapValue)
                       select query;

            var view = new FilesDescriptionRoll(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据类型查询
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public FilesDescriptionRoll SearchByType(FileDescType type)
        {
            var linq = from query in this.IQueryable
                       where query.Type == type
                       select query;

            var view = new FilesDescriptionRoll(this.Reponsitory, linq);
            return view;
        }


        /// <summary>
        /// 生成ID
        /// </summary>
        /// <returns>返回ID</returns>
        static public string GenID()
        {
            return PKeySigner.Pick(Yahv.Finance.Services.PKeyType.FilesDesc);
        }

        /// <summary>
        /// 录入文件信息
        /// Url 有规则生成
        /// Type 文件类型
        /// CustomName 原文件名称
        /// CreatorID 创建者ID
        /// </summary>
        /// <param name="file"></param>
        public void Add(FilesDescription file)
        {
            try
            {
                //string fileID = System.IO.Path.GetFileNameWithoutExtension(file.Url);
                string fileID = PKeySigner.Pick(PKeyType.FilesDesc);

                this.Reponsitory.Insert(new Layers.Data.Sqls.PvFinance.FilesDescription
                {
                    ID = fileID,
                    Type = (int)file.Type,
                    Url = file.Url,
                    CustomName = file.CustomName,
                    CreateDate = DateTime.Now,
                    CreatorID = file.CreatorID,
                    Status = (int)GeneralStatus.Normal,
                });

                if (file.FilesMapsArray != null && file.FilesMapsArray.Length > 0)
                {
                    foreach (var filesMap in file.FilesMapsArray)
                    {
                        this.Reponsitory.Insert(new Layers.Data.Sqls.PvFinance.FilesMap
                        {
                            ID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.FilesMap),
                            FileID = fileID,
                            Name = filesMap.Name,
                            Value = filesMap.Value,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="file"></param>
        public void Add(FilesDescription[] files)
        {
            using (var reponstory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                try
                {
                    var filesDescs = new List<Layers.Data.Sqls.PvFinance.FilesDescription>();
                    var filesMaps = new List<Layers.Data.Sqls.PvFinance.FilesMap>();

                    foreach (var file in files)
                    {
                        //string fileID = System.IO.Path.GetFileNameWithoutExtension(file.Url);
                        string fileID = PKeySigner.Pick(PKeyType.FilesDesc);

                        filesDescs.Add(new Layers.Data.Sqls.PvFinance.FilesDescription
                        {
                            ID = fileID,
                            Type = (int)file.Type,
                            Url = file.Url,
                            CustomName = file.CustomName,
                            CreateDate = DateTime.Now,
                            CreatorID = file.CreatorID,
                            Status = (int)GeneralStatus.Normal,
                        });

                        if (file.FilesMapsArray != null && file.FilesMapsArray.Length > 0)
                        {
                            foreach (var filesMap in file.FilesMapsArray)
                            {
                                filesMaps.Add(new Layers.Data.Sqls.PvFinance.FilesMap
                                {
                                    ID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.FilesMap),
                                    FileID = fileID,
                                    Name = filesMap.Name,
                                    Value = filesMap.Value,
                                });
                            }
                        }
                    }

                    reponstory.Insert(filesDescs.ToArray());
                    reponstory.Insert(filesMaps.ToArray());
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileID"></param>
        public void Delete(params string[] fileID)
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                //reponsitory.Delete<Layers.Data.Sqls.PvFinance.FilesMap>(item => fileID.Contains(item.FileID));
                //reponsitory.Delete<Layers.Data.Sqls.PvFinance.FilesDescription>(item => fileID.Contains(item.ID));
                reponsitory.Update<Layers.Data.Sqls.PvFinance.FilesDescription>(new
                {
                    Status = GeneralStatus.Deleted
                }, item => fileID.Contains(item.ID));
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filesMapName"></param>
        /// <param name="filesMapValue"></param>
        public void Delete(string filesMapName, string filesMapValue)
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                var filesDescription = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.FilesDescription>();
                var filesMaps = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.FilesMap>();

                var targetFileIDs = (from filesDes in filesDescription
                                     join filesMap in filesMaps on filesDes.ID equals filesMap.FileID
                                     where filesDes.Status == (int)GeneralStatus.Normal
                                        && filesMap.Name == filesMapName
                                        && filesMap.Value == filesMapValue
                                     select filesDes.ID).ToArray();

                //reponsitory.Delete<Layers.Data.Sqls.PvFinance.FilesMap>(item => item.Name == filesMapName && item.Value == filesMapValue);
                //reponsitory.Delete<Layers.Data.Sqls.PvFinance.FilesDescription>(item => targetFileIDs.Contains(item.ID));
                reponsitory.Update<Layers.Data.Sqls.PvFinance.FilesDescription>(new
                {
                    Status = GeneralStatus.Deleted
                }, item => targetFileIDs.Contains(item.ID));
            }
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

            var finfo = new System.IO.FileInfo(string.Concat(ConfigurationManager.AppSettings["FileSavePath"].TrimEnd('\\'), @"\", file.Url.Replace('/', '\\')));
            if (finfo.Exists)
            {
                finfo.Delete();
            }

            //this.Reponsitory.Delete<Layers.Data.Sqls.PvFinance.FilesDescription>(item => item.ID == fileID);
            this.Reponsitory.Update<Layers.Data.Sqls.PvFinance.FilesDescription>(new
            {
                Status = GeneralStatus.Deleted
            }, item => item.ID == fileID);
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
        /// 上传文件和数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dic"></param>
        /// <param name="fileNames"></param>
        /// <returns></returns>
        static public UploadResult[] UploadFileAndData(FileDescType type, object dic, params string[] fileNames)
        {
            string address = FileServerUrl + "/Upload/UploadFileAndData";

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
                return message.JsonTo<UploadResult[]>();
            }

            return null;
        }

        /// <summary>
        /// 单独上传文件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fileNames"></param>
        /// <returns></returns>
        static public UploadResult[] UploadFile(FileDescType type, params string[] fileNames)
        {
            string address = FileServerUrl + "/Upload/UploadFile";

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



            bool uploaded = cb.UploadData(address, bytes, out responseBytes);

            if (uploaded)
            {
                string message = Encoding.UTF8.GetString(responseBytes);
                return message.JsonTo<UploadResult[]>();
            }

            return null;
        }

    }
}
