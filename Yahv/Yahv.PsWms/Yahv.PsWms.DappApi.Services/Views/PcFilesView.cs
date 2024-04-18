using Layers.Data;
using Layers.Data.Sqls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.DappApi.Services.Enums;
using Yahv.PsWms.DappApi.Services.Models;
using Yahv.Underly;

namespace Yahv.PsWms.DappApi.Services.Views
{
    public class PcFilesView : QueryView<PcFile, PsWmsRepository>
    {
        #region 构造函数
        public PcFilesView()
        {
        }

        public PcFilesView(PsWmsRepository reponsitory) : base(reponsitory)
        {
        }

        protected PcFilesView(PsWmsRepository reponsitory, IQueryable<PcFile> iQueryable) : base(reponsitory, iQueryable)
        {
        }
        #endregion

        protected override IQueryable<PcFile> GetIQueryable()
        {
            var view = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.PcFiles>()
                       select new PcFile
                       {
                           ID = entity.ID,
                           AdminID = entity.AdminID,
                           CreateDate = entity.CreateDate,
                           MainID = entity.MainID,
                           SiteuserID = entity.SiteuserID,
                           Type = (Enums.FileType)entity.Type,
                           Url = entity.Url,
                           CustomName = entity.CustomName
                       };

            return view;
        }

        /// <summary>
        /// 返回文件的具体内容
        /// </summary>
        /// <returns></returns>
        public object[] ToMyArray()
        {
            var view = this.IQueryable.Cast<PcFile>();
            var urlPath = ConfigurationManager.AppSettings["PrexUrl"].TrimEnd('/');
            var ienum_view = view.ToArray();
            var result = ienum_view.Select(item => new
            {
                AdminID = item.AdminID,
                SiteuserID = item.SiteuserID,
                MainID = item.MainID,
                ID = item.ID,
                Type = (int)item.Type,
                TypeDes = item.Type.GetDescription(),
                Url = urlPath + '/' + item.Url,
                CutomName = item.CustomName,
                CreateDate = item.CreateDate,
            });

            return result.ToArray();
        }

        /// <summary>
        /// 根据MainID搜索文件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PcFilesView SearchByMainID(string id)
        {
            var filesView = this.IQueryable.Cast<PcFile>();

            var linq = from file in filesView
                       where file.MainID == id
                       select file;

            var view = new PcFilesView(this.Reponsitory, linq)
            {
            };

            return view;
        }

        /// <summary>
        /// 根据MainID搜索文件
        /// </summary>
        /// <param name="id">NoticeID 或者 FormID</param>
        /// <returns></returns>
        public PcFilesView SearchByID(string id)
        {
            var filesView = this.IQueryable.Cast<PcFile>();
            var noticeView = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Notices>()
                             where notice.ID == id || notice.FormID == id
                             select new
                             {
                                 notice.ID,
                                 notice.FormID
                             };
            var nids = noticeView.ToArray();
            var ids = nids.Select(item => item.ID).Distinct().Concat(nids.Select(item => item.FormID).Distinct());

            var linq = from file in filesView
                       where ids.Contains(file.MainID)
                       select file;

            return new PcFilesView(this.Reponsitory, linq);
        }

        /// <summary>
        /// 根据通知项ID来搜索
        /// </summary>
        /// <param name="id">NoticeItemID 或者 FormItemID</param>
        /// <returns></returns>
        public PcFilesView SearchByItemID(string id)
        {
            var filesView = this.IQueryable.Cast<PcFile>();
            var noticeitemView = from noticeitem in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.NoticeItems>()
                                 where noticeitem.ID == id || noticeitem.FormItemID == id
                                 select new
                                 {
                                     noticeitem.ID,
                                     noticeitem.FormItemID,
                                 };
            var nids = noticeitemView.ToArray();
            var ids = nids.Select(item => item.ID).Distinct().Concat(nids.Select(item => item.FormItemID).Distinct());

            var linq = from file in filesView
                       where ids.Contains(file.MainID)
                       select file;

            return new PcFilesView(this.Reponsitory, linq);
        }

        /// <summary>
        /// 根据FileID搜索
        /// </summary>
        /// <param name="fileID"></param>
        /// <returns></returns>
        public PcFilesView SearchByFileID(string fileID)
        {
            var filesView = this.IQueryable.Cast<PcFile>();

            var linq = from file in filesView
                       where file.ID == fileID
                       select file;

            var view = new PcFilesView(this.Reponsitory, linq);

            return view;
        }

        /// <summary>
        /// 根据FileType搜索
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public PcFilesView SearchByType(Services.Enums.FileType type)
        {
            var filesView = this.IQueryable.Cast<PcFile>();

            var linq = from file in filesView
                       where file.Type == type
                       select file;

            var view = new PcFilesView(this.Reponsitory, linq);

            return view;
        }

        /// <summary>
        /// 根据FileType搜索
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public PcFilesView SearchByType(Services.Enums.FileType[] types)
        {
            var filesView = this.IQueryable.Cast<PcFile>();

            var linq = from file in filesView
                       where types.Contains(file.Type)
                       select file;

            var view = new PcFilesView(this.Reponsitory, linq);

            return view;
        }
        /// <summary>
        /// 根据文件ID删除对应的文件
        /// </summary>
        /// <param name="fileID"></param>
        public void DeleteFile(string fileID)
        {
            if (string.IsNullOrWhiteSpace(fileID))
            {
                throw new Exception("参数不正确，FileID不能为Null或空字符串");
            }

            var view = new PcFilesView(this.Reponsitory);

            var file = view.SingleOrDefault(item => item.ID == fileID);

            if (file == null)
            {
                throw new Exception("您所要删除的文件不存在!");
            }

            // 删除对应的文件
            var fileInfo = new FileInfo(string.Concat(ConfigurationManager.AppSettings["PrexPath"].TrimEnd('\\'), "\\", file.Url.Replace('/', '\\')));
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }

            this.Reponsitory.Delete<Layers.Data.Sqls.PsWms.PcFiles>(item => item.ID == fileID);
        }

        /// <summary>
        /// 删除多个文件
        /// </summary>
        /// <param name="fileIDs"></param>
        public void DeleteFiles(string[] fileIDs)
        {
            if (fileIDs == null || fileIDs.Count() == 0)
            {
                throw new Exception("参数不正确,FileIDs 不能为Null或空字符串");
            }

            var view = new PcFilesView(this.Reponsitory);

            foreach (var fileID in fileIDs)
            {
                var file = view.SingleOrDefault(item => item.ID == fileID);

                if (file == null)
                {
                    throw new Exception("您所要删除的文件不存在!");
                }

                // 删除对应的文件
                var fileInfo = new FileInfo(string.Concat(ConfigurationManager.AppSettings["PrexPath"].TrimEnd('\\'), "\\", file.Url.Replace('/', '\\')));
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }
            }

            this.Reponsitory.Delete<Layers.Data.Sqls.PsWms.PcFiles>(item => fileIDs.Contains(item.ID));
        }

        /// <summary>
        /// 生成一个FileID
        /// </summary>
        public string GenerateFileID()
        {
            return PKeySigner.Pick(Services.Enums.PKeyType.File);
        }

        /// <summary>
        /// 同时保存多个PcFiles对象
        /// </summary>
        /// <param name="files"></param>
        public void Enter(params PcFile[] files)
        {
            this.Reponsitory.Insert(files.Select(file => new Layers.Data.Sqls.PsWms.PcFiles
            {
                ID = file.ID,
                MainID = file.MainID,
                Type = (int)file.Type,
                CustomName = file.CustomName,
                Url = file.Url,
                CreateDate = file.CreateDate,
                AdminID = file.AdminID,
                SiteuserID = file.SiteuserID,
            }));
        }

        /// <summary>
        /// 保存一个PcFiles对象
        /// </summary>
        /// <param name="file"></param>
        public void Enter(PcFile file)
        {
            this.Enter(new[] { file });
        }


    }
}
