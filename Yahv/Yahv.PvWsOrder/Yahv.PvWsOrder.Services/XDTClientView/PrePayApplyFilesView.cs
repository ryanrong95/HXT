using Layers.Data.Sqls;
using Layers.Data.Sqls.PvWsOrder;
using System;
using System.Linq;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.XDTModels;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.XDTClientView
{

    /// <summary>
    /// 代付货款委托书
    /// </summary>
    public class PrePayApplyFilesView : IUnique
    {
        public string ID { get; set; }

        public string ApplicationID { get; set; }

        public string FileName { get; set; }

        public FileType FileType { get; set; }

        public string FileFormat { get; set; }

        public string Url { get; set; }

        public string ClientID { get; set; }

        public string UserID { get; set; }

        public int Status { get; set; }

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }

        public void Enter()
        {
            using (Layers.Data.Sqls.PvCenterReponsitory reponsitory = new Layers.Data.Sqls.PvCenterReponsitory())
            {
                reponsitory.Update<Layers.Data.Sqls.PvCenter.FilesDescription>(new
                {
                    Status = 400,
                }, item => item.ApplicationID == this.ApplicationID && item.Type == (int)this.FileType);
                CenterFileDescription[] files = new CenterFileDescription[] {
                      new CenterFileDescription{
                           CustomName = this.FileName,
                           Url = this.Url,
                           AdminID = this.UserID,
                           ApplicationID =  this.ApplicationID,
                           Type =(int)this.FileType,
                      }
                };
                //上传文件
                new Yahv.PvWsOrder.Services.ClientViews.CenterFilesView().Upload(files);
            }
        }

        /// <summary>
        /// 根据代付申请id获取代付委托书的信息
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public PrePayApplyExtends GetProxyInfo(string applicationId)
        {
            using (Layers.Data.Sqls.PvWsOrderReponsitory reponsitory = new PvWsOrderReponsitory())
            {
                var linq = from apply in new Views.Origins.ApplicationsOrigin(reponsitory)
                           join payee in new Views.Origins.ApplicationPayeesOrigin(reponsitory)
                               on apply.ID equals payee.ApplicationID
                           join npayee in reponsitory.ReadTable<wsnSupplierPayeesTopView>()
                               on payee.PayeeID equals npayee.ID
                           join supplier in reponsitory.ReadTable<wsnSuppliersTopView>()
                       on npayee.nSupplierID equals supplier.ID
                           where apply.ID == applicationId
                           select new PrePayApplyExtends
                           {
                               ID = apply.ID,
                               SupplierEnglishName = supplier.EnglishName,
                               SupplierAddress = supplier.RegAddress,
                               CreateDate = apply.CreateDate,
                               TotalPrice = apply.TotalPrice,
                               Currency = apply.Currency,
                               BankName = npayee.Bank,
                               BankAddress = npayee.BankAddress,
                               BankAccount = npayee.Account,
                               SwiftCode = npayee.SwiftCode
                           };
                return linq.FirstOrDefault();
            }
        }

    }
}
