using System.Collections.Generic;
using System.Linq;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Payments.Views
{
    public class PaymentFileAlls : UniqueView<Yahv.Services.Models.CenterFileDescription, PvbCrmReponsitory>
    {
        string payID = string.Empty;

        public PaymentFileAlls(string payID)
        {
            this.payID = payID;
        }

        protected override IQueryable<Yahv.Services.Models.CenterFileDescription> GetIQueryable()
        {
            var files = new Yahv.Services.Views.CenterFilesTopView()
                .Where(t => t.PayID == payID && t.Status == Yahv.Services.Models.FileDescriptionStatus.Normal);
            return files;
        }

        /// <summary>
        /// 保存上传文件
        /// </summary>
        /// <param name="payId">财务ID</param>
        /// <param name="fileitems">文件列表</param>
        public void SaveFiles(IEnumerable<CenterFileDescription> fileitems)
        {
            var oldItems = this.Where(item => item.PayID == payID).ToList();
            SaveFiles(fileitems, oldItems);
        }

        #region 私有函数
        /// <summary>
        /// 保存申请文件
        /// </summary>
        /// <param name="newFiles"></param>
        /// <param name="oldFiles"></param>
        private void SaveFiles(IEnumerable<CenterFileDescription> newFiles, IEnumerable<CenterFileDescription> oldFiles)
        {
            string[] newids = newFiles.Select(item => item.ID).ToArray();
            string[] oldids = oldFiles.Select(item => item.ID).ToArray();
            using (PvCenterReponsitory reponsitory = LinqFactory<PvCenterReponsitory>.Create())
            {
                //删除原文件
                foreach (var id in oldids)
                {
                    if (!newids.Contains(id))
                    {
                        //删除原来的项
                        reponsitory.Update<Layers.Data.Sqls.PvCenter.FilesDescription>(new
                        {
                            PayID = "",
                            Status = FileDescriptionStatus.Delete,
                        }, item => item.ID == id && item.PayID == id);
                    }
                }
                //订单绑定新文件
                reponsitory.Update<Layers.Data.Sqls.PvCenter.FilesDescription>(new
                {
                    PayID = payID,
                }, item => newids.Contains(item.ID));
            }
        }
        #endregion
    }
}