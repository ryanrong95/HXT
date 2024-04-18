using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Utils.Converters;

namespace Uploader.Services.Views
{
    /// <summary>
    /// 应收实收 统计视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class FilesDescriptionView : QueryView<Models.FileDescription, PvCenterReponsitory>, IDisposable
    {
        public FilesDescriptionView()
        {

        }

        protected override IQueryable<Models.FileDescription> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.FilesDescriptionTopView>()
                   select new Models.FileDescription
                   {
                       ID = entity.ID,
                       LsOrderID = entity.LsOrderID,
                       WsOrderID = entity.WsOrderID,
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
                       Status = (Yahv.Services.Models.FileDescriptionStatus)entity.Status,
                       PayID = entity.PayID,
                       StaffID = entity.StaffID,
                       ErmApplicationID = entity.ErmApplicationID,
                   };
        }

        //static public string GenID()
        //{
        //    return "F" + DateTime.Now.LinuxTicks();
        //}

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
        public void Add(Models.FileMessage message)
        {
            try
            {
                this.Reponsitory.Insert(new Layers.Data.Sqls.PvCenter.FilesDescription
                {
                    ID = System.IO.Path.GetFileNameWithoutExtension(message.Url),
                    LsOrderID = message.LsOrderID,
                    ApplicationID = message.ApplicationID,
                    WsOrderID = message.WsOrderID,
                    WaybillID = message.WaybillID,
                    NoticeID = message.NoticeID,
                    StorageID = message.StorageID,
                    ShipID = message.ShipID,
                    CustomName = message.CustomName,
                    Type = message.Type,
                    Url = message.Url,
                    CreateDate = DateTime.Now,
                    ClientID = message.ClientID,
                    AdminID = message.AdminID,
                    InputID = message.InputID,
                    Status = (int)Yahv.Services.Models.FileDescriptionStatus.Normal,
                    PayID = message.PayID,
                    StaffID = message.StaffID,
                    ErmApplicationID = message.ErmApplicationID,
                });
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="obj">自定义文件修改属性</param>
        /// <param name="filesID">要修改的文件ID</param>
        public void Modify(object obj, params string[] filesID)
        {
            if (filesID == null || filesID.Length == 0)
            {
                throw new NotImplementedException("不实现 全体Modify 的功能");
            }
            this.Reponsitory.Update<Layers.Data.Sqls.PvCenter.FilesDescription>(obj, item => filesID.Contains(item.ID));

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

            var finfo = new System.IO.FileInfo(string.Concat(ConfigurationManager.AppSettings["SavePath"].TrimEnd('\\'), @"\", file.Url.Replace('/', '\\')));
            if (finfo.Exists)
            {
                finfo.Delete();
            }

            this.Reponsitory.Delete<Layers.Data.Sqls.PvCenter.FilesDescription>(item => item.ID == fileID);


        }

        ///// <summary>
        ///// 删除文件关系与文件本身（复数）
        ///// </summary>
        ///// <param name="fileID">文件ID</param>
        //public void DeleteFiles(string[] fileIDs)
        //{
        //    foreach (var fileID in fileIDs)
        //    {
        //        if (string.IsNullOrWhiteSpace(fileID))
        //        {
        //            throw new NotImplementedException("不能实现删除的功能！！");
        //        }
        //        var file = this.GetIQueryable().Select(item => new
        //        {
        //            item.ID,
        //            item.Url
        //        }).SingleOrDefault(item => item.ID == fileID);

        //        //补充删除
        //        if (file == null)
        //        {
        //            throw new NotSupportedException("要删除的文件并不存在！！");
        //        }

        //        var finfo = new System.IO.FileInfo(string.Concat(ConfigurationManager.AppSettings["SavePath"].TrimEnd('\\'), @"\", file.Url.Replace('/', '\\')));
        //        if (finfo.Exists)
        //        {
        //            finfo.Delete();
        //        }
        //        this.Reponsitory.Delete<Layers.Data.Sqls.PvCenter.FilesDescription>(item => item.ID == fileID);
        //    }

        //}
    }
}
