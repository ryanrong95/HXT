using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using System.Linq.Expressions;
using Yahv.Linq.Generic;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Layers.Data.Sqls;
using System.Web;
using System.IO;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.Utils.Serializers;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    /// <summary>
    /// 中心文件视图
    /// </summary>
    public class CenterFilesView : CenterFilesTopView
    {
        private readonly string Folder = @"Files";

        public CenterFilesView()
        {

        }

        public CenterFilesView(PvCenterReponsitory reponsitory) : base(reponsitory)
        {

        }

        public CenterFilesView(PvCenterReponsitory reponsitory, IQueryable<CenterFileDescription> iQueryable) : base(reponsitory, iQueryable)
        {

        }

        protected override IQueryable<CenterFileDescription> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.Status != FileDescriptionStatus.Delete);
        }


        /// <summary>
        /// 根据订单ID查询文件集合
        /// </summary>
        /// <param name="OrderID">代仓储订单ID</param>
        /// <returns></returns>
        public CenterFilesView SearchByOrderID(string OrderID)
        {
            var iQuery = this.IQueryable.Where(item => item.WsOrderID == OrderID);

            return new CenterFilesView(this.Reponsitory, iQuery);
        }

        /// <summary>
        /// 根据订单ID查询文件集合
        /// </summary>
        /// <param name="OrderID">代仓储订单ID</param>
        /// <returns></returns>
        public CenterFilesView SearchByLsOrderID(string LsOrderID)
        {
            var iQuery = this.IQueryable.Where(item => item.LsOrderID == LsOrderID);

            return new CenterFilesView(this.Reponsitory, iQuery);
        }

        /// <summary>
        /// 根据订单ID查询文件集合
        /// </summary>
        /// <param name="OrderID">代仓储订单ID</param>
        /// <returns></returns>
        public CenterFilesView SearchByApplicationID(string ApplicationID)
        {
            var iQuery = this.IQueryable.Where(item => item.ApplicationID == ApplicationID);

            return new CenterFilesView(this.Reponsitory, iQuery);
        }

        /// <summary>
        /// 根据运单ID查询文件集合
        /// </summary>
        /// <param name="WaybillID">运单ID</param>
        /// <returns></returns>
        public CenterFilesView SearchByWaybillID(string WaybillID)
        {
            var iQuery = this.IQueryable.Where(item => item.WaybillID == WaybillID);

            return new CenterFilesView(this.Reponsitory, iQuery);
        }


        #region 文件上传
        /// <summary>
        /// 上传文件处理
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public void Upload(params CenterFileDescription[] files)
        {
            foreach (var file in files)
            {
                var dic = new { file.CustomName, file.WsOrderID, file.ClientID, file.LsOrderID, file.AdminID, file.ApplicationID };

                var FullFileName = AppDomain.CurrentDomain.BaseDirectory + file.Url;

                //本地文件上传到服务器
                var result = Upload(FullFileName, (Underly.FileType)file.Type, dic);
                ////传输日志记录
                Yahv.PvWsOrder.Services.Logger.log("NPC", new Yahv.Services.Models.OperatingLog
                {
                    MainID = file.WsOrderID ?? file.ApplicationID ?? file.LsOrderID ?? file.ClientID,
                    Operation = "文件上传服务器",
                    Summary = result.Json(),
                });
                //上传完成后删除
                if (File.Exists(FullFileName))
                {
                    File.Delete(FullFileName);
                }
            }

        }

        /// <summary>
        /// 根据传入条件删除数据
        /// </summary>
        /// <param name="expression"></param>
        public void DeleteByLambda(Expression<Func<Layers.Data.Sqls.PvCenter.FilesDescription, bool>> expression)
        {
            Reponsitory.Update<Layers.Data.Sqls.PvCenter.FilesDescription>(new
            {
                Status = (int)FileDescriptionStatus.Delete,
            }, expression);
        }

        /// <summary>
        /// 芯达通文件处理
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public void XDTUpload(params CenterFileDescription[] files)
        {
            foreach (var file in files)
            {
                var dic = new { file.CustomName, file.WsOrderID, file.ClientID, file.LsOrderID, file.AdminID, file.ApplicationID };

                var FullFileName = AppDomain.CurrentDomain.BaseDirectory + file.Url;

                //本地文件上传到服务器
                var result = Upload(FullFileName, (Underly.FileType)file.Type, dic);

                ////传输日志记录
                Yahv.PvWsOrder.Services.Logger.log("NPC", new Yahv.Services.Models.OperatingLog
                {
                    MainID = file.ClientID ?? file.WsOrderID,
                    Operation = "文件上传服务器",
                    Summary = result.Json(),
                });

                ///报关委托书和对账单需要跟单员审核
                if (file.Type == (int)FileType.OrderBill || file.Type == (int)FileType.AgentTrustInstrument || file.Type == (int)FileType.SalesContract)
                {
                    Reponsitory.Update<Layers.Data.Sqls.PvCenter.FilesDescription>(new
                    {
                        Status = 400,
                    }, item => item.WsOrderID == file.WsOrderID && item.Type == file.Type);

                    Reponsitory.Update<Layers.Data.Sqls.PvCenter.FilesDescription>(new
                    {
                        Status = 100,
                    }, item => result.Select(a => a.FileID).Contains(item.ID));
                }
            }
        }

        ///// <summary>
        ///// 删除文件
        ///// </summary>
        ///// <param name="ids"></param>
        ///// <returns></returns>
        //public  bool Delete(params string[] ids)
        //{
        //    try
        //    {
        //       base.Delete(ids);
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        /// <summary>
        /// 上传文件保存到本地服务器
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public string fileSave(HttpPostedFileBase file)
        {
            //文件重命名
            var fileName = this.ReName(file.FileName);
            //拼凑出保存到本地的全路径
            string Url = this.Folder + @"\" + fileName;
            string fullUrl = AppDomain.CurrentDomain.BaseDirectory + this.Folder + @"\" + fileName;
            FileInfo fileInfo = new FileInfo(fullUrl);
            if (!fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }
            file.SaveAs(fullUrl);
            return Url;
        }

        /// <summary>
        /// 文件名称重命名
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string ReName(string fileName)
        {
            var ext = System.IO.Path.GetExtension(fileName);
            var random = new Random(Guid.NewGuid().GetHashCode());
            var newName = DateTime.Now.ToString("hhmmssfff") + random.Next(1000, 9999);
            return newName + ext;
        }
        #endregion
    }
}
